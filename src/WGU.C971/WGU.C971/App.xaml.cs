using WGU.C971.Services;
using WGU.C971.Pages;

namespace WGU.C971
{
    public partial class App : Application
    {
        public static DatabaseService Db { get; } = new();

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Pages.TermsPage());

            MainThread.BeginInvokeOnMainThread(async () => await Db.InitAsync());
        }
    }
}