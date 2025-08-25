using Microsoft.Maui.Controls;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using WGU.C971.Models;
using WGU.C971.Services;

namespace WGU.C971.Pages;

public partial class CourseDetailPage : ContentPage
{
	private int _courseId;
	private Course _course = new();
    private bool _suppressToggleHandlers;

    public CourseDetailPage(int courseId)
	{
		InitializeComponent();
		_courseId = courseId;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		_course = await App.Db.GetCourseAsync(_courseId) ?? new Course();
		BindToUi();
        await LoadAssessmentsAsync();
    }

    private async Task LoadAssessmentsAsync()
    {
        var list = await App.Db.GetAssessmentsForCourseAsync(_course.Id);
        AssessList.ItemsSource = list;

        AssessCountLabel.Text = $"{list.Count} / 2";

        bool hasObj = list.Any(a => a.Type == AssessmentType.Objective);
        bool hasPerf = list.Any(a => a.Type == AssessmentType.Performance);

        AddObjBtn.IsEnabled = !hasObj;
        AddPerfBtn.IsEnabled = !hasPerf;
    }

    private async void OnAddObjective(object sender, EventArgs e)
    {
        var a = new Assessments
        {
            CourseId = _course.Id,
            Title = "New Objective Assessment",
            Type = AssessmentType.Objective,
            StartDate = _course.StartDate,
            EndDate = _course.EndDate
        };
        await App.Db.SaveAssessmentAsync(a);
        await Navigation.PushAsync(new AssessmentDetailPage(a.Id));
    }

    private async void OnAddPerformance(object sender, EventArgs e)
    {
        var a = new Assessments
        {
            CourseId = _course.Id,
            Title = "New Performance Assessment",
            Type = AssessmentType.Performance,
            StartDate = _course.StartDate,
            EndDate = _course.EndDate
        };
        await App.Db.SaveAssessmentAsync(a);
        await Navigation.PushAsync(new AssessmentDetailPage(a.Id));
    }

    private async void OnAssessmentSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Assessments a)
        {
            await Navigation.PushAsync(new AssessmentDetailPage(a.Id));
            AssessList.SelectedItem = null;
        }
    }

    private async void OnDeleteAssessment(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is Assessments a)
        {
            var ok = await DisplayAlert("Delete assessment?", $"Delete '{a.Title}'?", "Delete", "Cancel");
            if (!ok) return;
            await App.Db.DeleteAssessmentAsync(a);
            await LoadAssessmentsAsync();
        }
    }

    private void BindToUi()
    {
        TitleEntry.Text = _course.Title;
        StartPicker.Date = _course.StartDate == default ? DateTime.Today : _course.StartDate;
        EndPicker.Date = _course.EndDate == default ? DateTime.Today.AddMonths(3) : _course.EndDate;
        DuePicker.Date = _course.DueDate == default ? EndPicker.Date : _course.DueDate;

        StatusPicker.SelectedItem = _course.Status.ToString();

        InstrName.Text = _course.InstructorName;
        InstrPhone.Text = _course.InstructorPhone;
        InstrEmail.Text = _course.InstructorEmail;

        NotesEditor.Text = _course.Notes;

        _suppressToggleHandlers = true;
        StartAlertSwitch.IsToggled = _course.StartAlertEnabled;
        EndAlertSwitch.IsToggled = _course.EndAlertEnabled;
        _suppressToggleHandlers = false;
    }

    private bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try { _ = new MailAddress(email); return true; }
        catch { return false; }
    }

    private async void OnSave(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        { await DisplayAlert("Validation", "Course title is required.", "OK"); return; }

        if (StartPicker.Date > EndPicker.Date)
        { await DisplayAlert("Validation", "Start must be before end.", "OK"); return; }

        if (string.IsNullOrWhiteSpace(InstrName.Text)
            || string.IsNullOrWhiteSpace(InstrPhone.Text)
            || !IsValidEmail(InstrEmail.Text))
        {
            await DisplayAlert("Validation", "Valid instructor name, phone, and email are required.", "OK");
            return;
        }

        _course.Title = TitleEntry.Text!.Trim();
        _course.StartDate = StartPicker.Date;
        _course.EndDate = EndPicker.Date;
        _course.DueDate = DuePicker.Date;

        var statusText = (string?)StatusPicker.SelectedItem ?? CourseStatus.PlanToTake.ToString();
        _course.Status = Enum.Parse<CourseStatus>(statusText);

        _course.InstructorName = InstrName.Text!.Trim();
        _course.InstructorPhone = InstrPhone.Text!.Trim();
        _course.InstructorEmail = InstrEmail.Text!.Trim();

        _course.Notes = NotesEditor.Text;

        await App.Db.SaveCourseAsync(_course);
        await DisplayAlert("Saved", "Course saved.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnShareNotes(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_course.Notes))
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = _course.Notes,
                Title = $"Notes for {_course.Title}"
            });
        }
        else
        {
            await DisplayAlert("Info", "No notes to share.", "OK");
        }
    }

    private async void OnStartAlertToggled(object sender, ToggledEventArgs e)
    {
        if (_suppressToggleHandlers) return;
        if (e.Value)
        {
            _course.StartAlertId = await NotificationService.ScheduleAsync(
                $"Course Start: {_course.Title}", "Good Luck!", _course.StartDate);
            _course.StartAlertEnabled = true;
        }
        else
        {
            if (_course.StartAlertId.HasValue)
            {
                NotificationService.Cancel(_course.StartAlertId.Value);
            }
            _course.StartAlertEnabled = false;
            _course.StartAlertId = null;
        }
        await App.Db.SaveCourseAsync(_course);
    }

    private async void OnEndAlertToggled(object sender, ToggledEventArgs e)
    {
        if (_suppressToggleHandlers) return;
        if (e.Value)
        {
            _course.EndAlertId = await NotificationService.ScheduleAsync(
                $"Course End: {_course.Title}",
                "Deadline is today!",
                _course.EndDate);

            _course.EndAlertEnabled = true;
        }
        else
        {
            if (_course.EndAlertId.HasValue)
            {
                NotificationService.Cancel(_course.EndAlertId.Value);
            }
            _course.EndAlertEnabled = false;
            _course.EndAlertId = null;
        }

        await App.Db.SaveCourseAsync(_course);
    }

    private async void OnDelete(object sender, EventArgs e)
    {
        var ok = await DisplayAlert("Delete course?", $"Delete '{_course.Title}'?", "Delete", "Cancel");
        if (!ok) return;
        await App.Db.DeleteCourseAsync(_course);
        await Navigation.PopAsync();
    }
}