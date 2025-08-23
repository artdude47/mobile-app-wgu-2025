using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using WGU.C971.Models;
using WGU.C971.Services;

namespace WGU.C971.Pages;

public partial class AssessmentDetailPage : ContentPage
{
	private int _assessmentId;
	private Assessments _a = new();
	private bool _suspendToggleHandlers;

	public AssessmentDetailPage(int assessmentId)
	{
		InitializeComponent();
		_assessmentId = assessmentId;
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		_a = await App.Db.GetAssessmentAsync(_assessmentId) ?? new Assessments();
		BindToUi();
    }

	private void BindToUi()
	{
		TypeEntry.Text = _a.Type.ToString();
		TitleEntry.Text = _a.Title;

		StartPicker.Date = _a.StartDate == default ? DateTime.Today : _a.StartDate;
		EndPicker.Date = _a.EndDate == default ? DateTime.Today.AddDays(7) : _a.EndDate;

		_suspendToggleHandlers = true;
		StartAlertSwitch.IsToggled = _a.StartAlertEnabled;
		EndAlertSwitch.IsToggled = _a.EndAlertEnabled;
		_suspendToggleHandlers = false;
    }

	private async void OnSave(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(TitleEntry.Text))
		{
			await DisplayAlert("Validation error", "Title is required.", "OK");
			return;
        }

		if (StartPicker.Date > EndPicker.Date)
		{
			await DisplayAlert("Validation error", "Start date must be before or equal to end date.", "OK");
			return;
        }

		_a.Title = TitleEntry.Text?.Trim();
		_a.StartDate = StartPicker.Date;
		_a.EndDate = EndPicker.Date;

		await App.Db.SaveAssessmentAsync(_a);

		if (_a.StartAlertEnabled)
		{
			if (_a.StartAlertId.HasValue) NotificationService.Cancel(_a.StartAlertId.Value);
			_a.StartAlertId = await NotificationService.ScheduleAsync(
				$"Assessment Starts: {_a.Title}", "Good Luck!", _a.StartDate);
        }
		if (_a.EndAlertEnabled)
		{
			if (_a.EndAlertId.HasValue) NotificationService.Cancel(_a.EndAlertId.Value);
			_a.EndAlertId = await NotificationService.ScheduleAsync(
				$"Assessment Ends: {_a.Title}", "Don't forget!", _a.EndDate);
        }

		await App.Db.SaveAssessmentAsync(_a);
		await DisplayAlert("Success", "Assessment saved.", "OK");
		await Navigation.PopAsync();
    }

	private async void OnBack(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
    }

	private async void OnDelete(object sender, EventArgs e)
	{
		var ok = await DisplayAlert("Confirm", "Delete this assessment?", "Yes", "No");
		if (!ok) return;
		await App.Db.DeleteAssessmentAsync(_a);
		await Navigation.PopAsync();
    }

	private async void OnStartAlertToggled(object sender, ToggledEventArgs e)
	{
		if (_suspendToggleHandlers) return;

		if (e.Value)
		{
			_a.StartAlertId = await NotificationService.ScheduleAsync(
				$"Assessment Starts: {_a.Title}", "Good Luck!", _a.StartDate);
			_a.StartAlertEnabled = true;
        }
        else
        {
             if (_a.StartAlertId.HasValue)
			 {
				 NotificationService.Cancel(_a.StartAlertId.Value);
				 _a.StartAlertId = null;
				_a.StartAlertEnabled = false;
             }
        }
        await App.Db.SaveAssessmentAsync(_a);
    }

	private async void OnEndAlertToggled(object sender, ToggledEventArgs e)
	{
		if (_suspendToggleHandlers) return;

		if (e.Value)
		{
			_a.EndAlertId = await NotificationService.ScheduleAsync(
				$"Assessment ends: {_a.Title}", "Deadline day!", _a.EndDate);
			_a.EndAlertEnabled = _a.EndAlertId > 0;
		}
		else
		{
			if (_a.EndAlertId.HasValue) NotificationService.Cancel(_a.EndAlertId.Value);
			_a.EndAlertEnabled = false;
			_a.EndAlertId = null;
        }
		await App.Db.SaveAssessmentAsync(_a);
    }
}