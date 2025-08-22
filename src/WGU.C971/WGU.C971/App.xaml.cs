using WGU.C971.Services;

namespace WGU.C971
{
    public partial class App : Application
    {
        public static DatabaseService Db { get; } = null;


        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await Db.InitAsync();
            await SeedData.EnsureAsync();
        }
    }
}