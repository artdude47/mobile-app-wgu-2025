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

        _course.Status = Enum.Parse<CourseStatus>((string)StatusPicker.SelectedItem);

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

    private async void OnNotifyStart(object sender, EventArgs e)
        => await NotificationService.ScheduleAsync($"Course starts: {_course.Title}", "Good luck!", _course.StartDate);

    private async void OnNotifyEnd(object sender, EventArgs e)
        => await NotificationService.ScheduleAsync($"Course ends: {_course.Title}", "Deadline day!", _course.EndDate);

    private async void OnDelete(object sender, EventArgs e)
    {
        var ok = await DisplayAlert("Delete course?", $"Delete '{_course.Title}'?", "Delete", "Cancel");
        if (!ok) return;
        await App.Db.DeleteCourseAsync(_course);
        await Navigation.PopAsync();
    }
}