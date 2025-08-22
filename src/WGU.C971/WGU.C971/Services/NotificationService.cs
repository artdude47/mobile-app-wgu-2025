using Plugin.LocalNotification;

namespace WGU.C971.Services
{
    public static class NotificationService
    {
        private static int _nextId = 1;

        public static async Task ScheduleAsync(string title, string body, DateTime when)
        {
            var request = new NotificationRequest
            {
                NotificationId = _nextId++,
                Title = title,
                Description = body,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = when,
                    NotifyRepeatInterval = null
                }
            };
            await LocalNotificationCenter.Current.Show(request);
        }
    }
}
