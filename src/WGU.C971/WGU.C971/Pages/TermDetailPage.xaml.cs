using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using WGU.C971.Models;

namespace WGU.C971.Pages;

public partial class TermDetailPage : ContentPage
{
	private int _termId;
	private Term _term = new();

	public TermDetailPage(int termId)
	{
		InitializeComponent();
		_termId = termId;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadAsync();
    }

	private async Task LoadAsync()
	{
		_term = await App.Db.GetTermAsync(_termId) ?? new Term
		{
			StartDate = DateTime.Today,
			EndDate = DateTime.Today.AddMonths(6)
		};

		Title = $"Edit Term: {_term.Title}";
		TitleEntry.Text = _term.Title;
		StartPicker.Date = _term.StartDate == default ? DateTime.Today : _term.StartDate;
		EndPicker.Date = _term.EndDate == default ? DateTime.Today.AddMonths(6) : _term.EndDate;
    }

	private async void OnSave(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(TitleEntry.Text))
		{
			await DisplayAlert("Error", "Title cannot be empty.", "OK");
			return;
        }

		if (StartPicker.Date > EndPicker.Date)
		{
			await DisplayAlert("Error", "Start date cannot be after end date.", "OK");
			return;
        }

		_term.Title = TitleEntry.Text!.Trim();
		_term.StartDate = StartPicker.Date;
		_term.EndDate = EndPicker.Date;

		await App.Db.SaveTermAsync(_term);
		await DisplayAlert("Success", "Term saved successfully.", "OK");
		await Navigation.PopAsync();
    }

	private async void OnBack(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
    }
}