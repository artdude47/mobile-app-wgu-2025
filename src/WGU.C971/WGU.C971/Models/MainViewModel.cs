using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WGU.C971.Services;

namespace WGU.C971.Models;
public partial class MainViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;

    [ObservableProperty]
    string statusText = "Status: Ready";

    public MainViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    [RelayCommand]
    private async Task PingDb()
    {
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
    }
}

