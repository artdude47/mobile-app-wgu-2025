using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using WGU.C971.Models;

namespace WGU.C971.Pages;

public partial class TermsPage : ContentPage
{
	public TermsPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadAsync();
	}

	private async Task LoadAsync()
	{
		var terms = await App.Db.GetTermsAsync();
		TermsList.ItemsSource = terms.OrderBy(t => t.StartDate);
    }

	private async void OnAdd(object sender, EventArgs e)
	{
		var today = DateTime.Today;
		var term = new Term
		{
			Title = "New Term",
			StartDate = today,
			EndDate = today.AddMonths(6)
		};

		await App.Db.SaveTermAsync(term);
        await Navigation.PushAsync(new TermDetailPage(term.Id));
    }

	private async void OnSelected(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is Term term)
		{
			await Navigation.PushAsync(new TermDetailPage(term.Id));
			((CollectionView)sender).SelectedItem = null;
		}
    }

	private async void OnDelete(object sender, EventArgs e)
	{
		if ((sender as Button)?.CommandParameter is Term term)
		{
			var ok = await DisplayAlert("Delete Term?",
				$"Delete '{term.Title}'?",
				"Delete", "Cancel");
			if (!ok) return;

			await App.Db.DeleteTermAsync(term);
			await LoadAsync();
        }
	}
}