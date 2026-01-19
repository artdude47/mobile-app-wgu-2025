using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WGU.C971.Services;

namespace WGU.C971.Models;
public partial class MainViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;

    [ObservableProperty]
    string statusText = "Status: Ready";

    [ObservableProperty]
    bool isBusy;

    public MainViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    [RelayCommand]
    private async Task PingDb()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        statusText = "Connecting...";

        try
        {
            StatusText = "Connecting..."; 

            await _dbService.InitAsync(); 

            StatusText = "Database Connected!";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

