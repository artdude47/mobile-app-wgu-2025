using Plugin.LocalNotification;
using Microsoft.Maui.Devices;
using Microsoft.Maui.ApplicationModel;

namespace WGU.C971.Services
{
    public static class NotificationService
    {
        private static int _nextId = 1;

        private static bool IsAndroid => DeviceInfo.Platform == DevicePlatform.Android;

        public static void Cancel(int id)
        {
            LocalNotificationCenter.Current.Cancel(id);
        }

        public static async Task<int> ScheduleAsync(string title, string body, DateTime when)
        {
            var id = _nextId++;

            if (!IsAndroid)
            {
#if WINDOWS
                await MainThread.InvokeOnMainThreadAsync(async () =>
                      await Application.Current!.MainPage!.DisplayAlert(
                      "Not Supported",
                      "Local notifications run on Android. Use the emulator.",
                      "OK"));
#endif
                return id;
            }

#if ANDROID
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                if (status != PermissionStatus.Granted)  
                {
                    return id;
                }
            }
#endif

            var notifyTime = when < DateTime.Now.AddSeconds(2) ? DateTime.Now.AddSeconds(2) : when;

            var request = new NotificationRequest
            {
                NotificationId = id,
                Title = title,
                Description = body,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime,
                    NotifyRepeatInterval = null
                },
            };
            await LocalNotificationCenter.Current.Show(request);
            return id;
        }
    }
}
