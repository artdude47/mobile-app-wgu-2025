using Plugin.LocalNotification;
using Microsoft.Maui.Devices;
using Microsoft.Maui.ApplicationModel;

namespace WGU.C971.Services
{
    public static class NotificationService
    {
        private static int _nextId = 1;

        private static bool IsAndroid => DeviceInfo.Platform == DevicePlatform.Android;

        public static async Task ScheduleAsync(string title, string body, DateTime when)
        {
            if (!IsAndroid)
            {
#if WINDOWS
                await MainThread.InvokeOnMainThreadAsync(async () =>
                      await Application.Current!.MainPage!.DisplayAlert(
                      "Not Supported",
                      "Local notifications run on Android. Use the emulator.",
                      "OK"));
#endif
                return;
            }

#if ANDROID
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                if (status != PermissionStatus.Granted)  
                {
                    return;
                }
            }
#endif

            var notifyTime = when < DateTime.Now.AddSeconds(2) ? DateTime.Now.AddSeconds(2) : when;

            var request = new NotificationRequest
            {
                NotificationId = _nextId++,
                Title = title,
                Description = body,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime,
                    NotifyRepeatInterval = null
                },
            };
            await LocalNotificationCenter.Current.Show(request);
        }
    }
}
