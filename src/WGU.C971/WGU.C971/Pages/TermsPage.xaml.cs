using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using WGU.C971.Models;

namespace WGU.C971.Pages;

public partial class TermsPage : ContentPage
{
	private bool _seeded = false;

	public TermsPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadAsync();

		if (!_seeded)
		{
			_seeded = true;
			await SeedIfEmptyAsync();
		}
	}

	private async Task SeedIfEmptyAsync()
	{
		try
		{
			var terms = await App.Db.GetTermsAsync();
			if (terms.Count > 0) return;

			var term = new Term
			{
				Title = "Term 1",
				StartDate = DateTime.Today,
				EndDate = DateTime.Today.AddMonths(6)
			};
			await App.Db.SaveTermAsync(term);

			var course = new Course
			{
				TermId = term.Id,
				Title = "Mobile Development",
				StartDate = term.StartDate,
				EndDate = term.EndDate,
				DueDate = term.EndDate,
				Status = CourseStatus.InProgress,
				InstructorName = "Anika Patel",
				InstructorPhone = "555-123-4567",
				InstructorEmail = "anika.patel@strimeuniversity.edu",
				Notes = "Seeded for evaluator."
			};
			await App.Db.SaveCourseAsync(course);

			var oa = new Assessments
			{
				CourseId = course.Id,
				Title = "Objective Assessment",
				Type = AssessmentType.Objective,
				StartDate = course.StartDate.AddDays(7),
				EndDate = course.StartDate.AddDays(14)
			};
			await App.Db.SaveAssessmentAsync(oa);

			var pa = new Assessments
			{
				CourseId = course.Id,
				Title = "Performance Assessment",
				Type = AssessmentType.Performance,
				StartDate = course.StartDate.AddDays(21),
				EndDate = course.StartDate.AddDays(28)
			};
			await App.Db.SaveAssessmentAsync(pa);
        }
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[SEED] Error seeding data: {ex}");
        }

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