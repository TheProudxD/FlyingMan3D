using System;

namespace _Project.Scripts.Tools.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime EpochTime = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimestamp(this DateTime dateTime) => (long)dateTime.Subtract(EpochTime).TotalSeconds;

        public static DateTime FromUnixTimestamp(this long unixTimestamp) => EpochTime.AddSeconds(unixTimestamp);

        public static bool Between(this TimeSpan target, TimeSpan min, TimeSpan max) => target >= min && target < max;

        public static bool Between(this DateTime target, DateTime min, DateTime max) => target >= min && target < max;

        public static DateTime WithDate(this DateTime dt, int? year = null, int? month = null, int? day = null)
        {
            int newYear = year ?? dt.Year;
            int newMonth = month ?? dt.Month;
            int newDay = day ?? dt.Day;

            // Ensure the new date is valid by clamping day if necessary
            int daysInMonth = DateTime.DaysInMonth(newYear, newMonth);
            newDay = Math.Min(newDay, daysInMonth);

            return new DateTime(newYear, newMonth, newDay, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }
    }
}