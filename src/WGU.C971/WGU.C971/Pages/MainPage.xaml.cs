using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace WGU.C971.Pages;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
        
		try
		{
			await App.Db.InitAsync();
			StatusLabel.Text = "Status: DB Initialized";
        }
		catch (Exception ex)
		{
			StatusLabel.Text = "Status: Db init failed";
			await DisplayAlert("DB init error", ex.Message, "OK");
		}
    }

	private async void OnPingDb(object sender, EventArgs e)
	{
		try
		{
			var terms = await App.Db.GetTermAsync();
			await DisplayAlert("DB Ping", $"Terms in DB: {terms.Count}", "OK");
        }
		catch (Exception ex)
		{
			await DisplayAlert("DB Ping Error", ex.Message, "OK");
        }

    }
}