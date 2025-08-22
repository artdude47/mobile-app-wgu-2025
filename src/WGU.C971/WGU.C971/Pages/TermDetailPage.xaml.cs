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

		TitleEntry.Text = _term.Title;
		StartPicker.Date = _term.StartDate == default ? DateTime.Today : _term.StartDate;
		EndPicker.Date = _term.EndDate == default ? DateTime.Today.AddMonths(6) : _term.EndDate;

		await LoadCoursesAsync();
    }

	private async Task LoadCoursesAsync()
	{
		var list = await App.Db.GetCoursesForTermAsync(_term.Id);
		CoursesList.ItemsSource = list.OrderBy(c => c.StartDate).ToList();

		var count = list.Count;
		CourseCountLabel.Text = $"{count} / 6";
		AddCourseBtn.IsEnabled = count < 6;
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
		//await Navigation.PopAsync();
    }

	private async void OnBack(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
    }

	private async void OnAddCourse(object sender, EventArgs e)
	{
		var c = new Course
		{
			TermId = _term.Id,
			Title = "New Course",
			StartDate = _term.StartDate,
			EndDate = _term.EndDate,
			DueDate = _term.EndDate,
			Status = CourseStatus.PlanToTake,
			InstructorName = "",
			InstructorPhone = "",
			InstructorEmail = "",
		};

		await App.Db.SaveCourseAsync(c);
		await Navigation.PushAsync(new CourseDetailPage(c.Id));
    }

	private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is Course c)
		{
			await Navigation.PushAsync(new CourseDetailPage(c.Id));
			((CollectionView)sender).SelectedItem = null;
        }
    }
}