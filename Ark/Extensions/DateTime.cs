using System.Globalization;
using System.Resources;

namespace Ark
{
    /// <summary>
    /// + Provides convenience methods to manipulate <see cref="DateTime"/> values.
    /// - Does not handle time zone conversions.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.datetime"/>
    /// </summary>
    public static class DateTimeExtensibility
    {
        #region Enums

        /// <summary>
        /// Defines the levels to truncate a date to.
        /// </summary>
        public enum DateTruncate
        {
            /// <summary>
            /// + Truncate to the first day of the year.
            /// - Ignores month and day components.
            /// </summary>
            Year,

            /// <summary>
            /// + Truncate to the first day of the month.
            /// - Ignores day component.
            /// </summary>
            Month,

            /// <summary>
            /// + Truncate to midnight of the given day.
            /// - Time components reset to zero.
            /// </summary>
            Day,

            /// <summary>
            /// + Truncate to the start of the hour.
            /// - Minutes and seconds reset to zero.
            /// </summary>
            Hour,

            /// <summary>
            /// + Truncate to the start of the minute.
            /// - Seconds reset to zero.
            /// </summary>
            Minute,

            /// <summary>
            /// + Truncate to the start of the second.
            /// - Milliseconds reset to zero.
            /// </summary>
            Second,

            /// <summary>
            /// + Truncate to remove sub-millisecond precision.
            /// - Least precise option available.
            /// </summary>
            Millisecond,
        }

        #endregion Enums

        #region Set Date and Time

        /// <summary>
        /// Sets only the date year.
        /// </summary>
        /// <param name="dateTime">The date to replace the year.</param>
        /// <param name="year">The year to set.</param>
        /// <returns>The date with the new year set.</returns>
        public static DateTime WithYear(this DateTime dateTime, int year)
            => dateTime.AddYears(year - dateTime.Year);

        /// <summary>
        /// Sets only the date month.
        /// </summary>
        /// <param name="dateTime">The date to replace the month.</param>
        /// <param name="month">The month to set.</param>
        /// <returns>The date with the new month set.</returns>
        public static DateTime WithMonth(this DateTime dateTime, int month)
            => dateTime.AddMonths(month - dateTime.Month);

        /// <summary>
        /// Sets only the date day.
        /// </summary>
        /// <param name="dateTime">The date to replace the day.</param>
        /// <param name="day">The day to set.</param>
        /// <returns>The date with the new month set.</returns>
        public static DateTime WithDay(this DateTime dateTime, int day)
            => dateTime.AddDays(day - dateTime.Day);

        /// <summary>
        /// Sets only the time hours.
        /// </summary>
        /// <param name="dateTime">The time to replace the hours.</param>
        /// <param name="hour">The hours to set.</param>
        /// <returns>The time with the new hours set.</returns>
        public static DateTime WithHour(this DateTime dateTime, int hour)
            => dateTime.AddHours(hour - dateTime.Hour);

        /// <summary>
        /// Sets only the time minutes.
        /// </summary>
        /// <param name="dateTime">The time to replace the minutes.</param>
        /// <param name="minute">The minutes to set.</param>
        /// <returns>The time with the new minutes set.</returns>
        public static DateTime WithMinute(this DateTime dateTime, int minute)
            => dateTime.AddMinutes(minute - dateTime.Minute);

        /// <summary>
        /// Sets only the time seconds.
        /// </summary>
        /// <param name="dateTime">The time to replace the seconds.</param>
        /// <param name="second">The seconds to set.</param>
        /// <returns>The time with the new seconds set.</returns>
        public static DateTime WithSecond(this DateTime dateTime, int second)
            => dateTime.AddSeconds(second - dateTime.Second);

        #endregion Set Date and Time

        #region Time Differences

        /// <summary>
        /// Gets the laps in ms between this time and now (UTC).
        /// </summary>
        public static int GetLapsFromUtcNow(this DateTime dateTime)
        {
            return Convert.ToInt32(Math.Min(DateTime.UtcNow.Subtract(dateTime).TotalMilliseconds, int.MaxValue));
        }

        /// <summary>
        /// Gets the laps in ms between this time and now (local).
        /// </summary>
        public static int GetLapsFromNow(this DateTime dateTime)
        {
            return Convert.ToInt32(Math.Min(DateTime.Now.Subtract(dateTime).TotalMilliseconds, int.MaxValue));
        }

        /// <summary>
        /// Checks whether this date/time is in a given laps from now (in UTC or not, depending on the given time, default to UTC).
        /// </summary>
        /// <param name="dateTime">The date/time to check.</param>
        /// <param name="lapsToCheck">The laps to check.</param>
        /// <returns>True if the date/time is in the interval between the current time and the given laps, false otherwise.</returns>
        public static bool CheckLapsFromNow(this DateTime dateTime, TimeSpan lapsToCheck)
            => (dateTime.Kind == DateTimeKind.Local ? DateTime.Now : DateTime.UtcNow).Subtract(dateTime) <= lapsToCheck;

        /// <summary>
        /// Checks whether this date/time is in a given laps from now (in UTC or not, depending on the given time, default to UTC).
        /// </summary>
        /// <param name="dateTime">The date/time to check.</param>
        /// <param name="lapsToCheck">The laps to check.</param>
        /// <returns>True if the date/time is in the interval between the current time and the given laps, false otherwise.</returns>
        public static bool CheckLapsFromNow(this DateTime? dateTime, TimeSpan lapsToCheck)
            => dateTime.HasValue && CheckLapsFromNow(dateTime.Value, lapsToCheck);

        #endregion Time Differences

        #region Kind Conversion

        /// <summary>
        /// Converts the datetime into another time kind.
        /// </summary>
        /// <param name="dateTime">The datetime to convert.</param>
        /// <param name="kind">The kind to convert the date time.</param>
        /// <returns>The converted date time.</returns>
        public static DateTime ToKind(this DateTime dateTime, DateTimeKind kind)
        {
            switch (kind)
            {
                case DateTimeKind.Local: return dateTime.ToLocalTime();
                case DateTimeKind.Utc: return dateTime.ToUniversalTime();
                default:
                    return DateTime.SpecifyKind(dateTime, kind);
            }
        }

        /// <summary>
        /// Specify the kind of a datetime without converting it.
        /// </summary>
        /// <param name="dateTime">The datetime to specify the king.</param>
        /// <param name="kind">The kind to specify for the date time.</param>
        /// <returns>The kind-specified date time.</returns>
        public static DateTime? SpecifyKind(this DateTime? dateTime, DateTimeKind kind)
            => dateTime.HasValue ? DateTime.SpecifyKind(dateTime.Value, kind) : (DateTime?)null;

        /// <summary>
        /// Specify the kind of a datetime without converting it.
        /// </summary>
        /// <param name="dateTime">The datetime to specify the king.</param>
        /// <param name="kind">The kind to specify for the date time.</param>
        /// <returns>The kind-specified date time.</returns>
        public static DateTime SpecifyKind(this DateTime dateTime, DateTimeKind kind)
            => DateTime.SpecifyKind(dateTime, kind);

        #endregion Kind Conversion

        #region Formatting

        /// <summary>
        /// Convert a date in a more literary form for displaying purposes.
        /// </summary>
        /// <param name="datetime">The date to convert.</param>
        /// <param name="displayMonth">How we should display the date. (Exemple: If displayMonth == true: 31 January 2000, else: 01/31/2000)</param>
        /// <param name="displaySeconds">Whether to display the seconds along with the time.</param>
        /// <returns></returns>
        public static string ConvertToSmartFormat(this DateTime datetime, bool displayMonth = false, bool displaySeconds = false)
        {
            var resourceManager = new ResourceManager("Ark.Resources.Localization.Resource", typeof(DateTimeExtensibility).Assembly);

            datetime = datetime.ToLocalTime();
            var diffWithNow = (int)DateTime.Now.Subtract(datetime).TotalMinutes;

            if (diffWithNow < 60)
                return string.Format(resourceManager.GetString("MinutesAgo"), diffWithNow, diffWithNow == 1 || diffWithNow == 0 ? "" : "s");
            if (DateTime.Today.Date == datetime.Date)
                return string.Format(resourceManager.GetString("TodayAt"), datetime.ToString($"HH:mm{(displaySeconds ? ":ss" : "")}"));
            if (DateTime.Today.AddDays(-1).Date == datetime.Date)
                return string.Format(resourceManager.GetString("YesterdayAt"), datetime.ToString($"HH:mm{(displaySeconds ? ":ss" : "")}"));
            if (SecondsSinceStartOfWeek(DateTime.Now) - SecondsSinceStartOfWeek(datetime) >= 0 && diffWithNow < 7 * 24 * 60)
                return string.Format(resourceManager.GetString("DateDisplay3"), datetime.ToString("dddd").FirstLetterToUpper(), datetime.ToString($"HH:mm{(displaySeconds ? ":ss" : "")}"));
            if (displayMonth)
                return string.Format(resourceManager.GetString("DateDisplay2"), datetime.Day,
                    datetime.ToString("MMMM").FirstLetterToUpper(), datetime.Year, datetime.ToString($"HH:mm{(displaySeconds ? ":ss" : "")}"));

            return string.Format(resourceManager.GetString("DateDisplay"), datetime.ToString("dd/MM/yyyy"), datetime.ToString($"HH:mm{(displaySeconds ? ":ss" : "")}"));
        }

        /// <summary>
        /// Returns the date as an ISO formatted string.
        /// </summary>
        /// <param name="datetime">The date to format.</param>
        /// <returns>The ISO formatted date string.</returns>
        public static string ToIsoString(this DateTime datetime)
            => datetime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        /// <summary>
        /// Truncates the date to the specified level.
        /// </summary>
        /// <param name="datetime">The date to truncate.</param>
        /// <param name="truncateTo">The level to truncate to.</param>
        /// <returns>The truncated date.</returns>
        public static DateTime TruncateTo(this DateTime datetime, DateTruncate truncateTo)
            => truncateTo switch
            {
                DateTruncate.Year => new DateTime(datetime.Year, 1, 1, 0, 0, 0, datetime.Kind),
                DateTruncate.Month => new DateTime(datetime.Year, datetime.Month, 1, 0, 0, 0, datetime.Kind),
                DateTruncate.Day => new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0, datetime.Kind),
                DateTruncate.Hour => new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, 0, 0, datetime.Kind),
                DateTruncate.Minute => new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0, datetime.Kind),
                DateTruncate.Second => new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Kind),
                DateTruncate.Millisecond => new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind),
                _ => throw new ArgumentOutOfRangeException(nameof(truncateTo), truncateTo, "Invalid truncation level"),
            };

        /// <summary>
        /// Converts a <see cref="DateTime"/> to Unix timestamp in seconds.
        /// </summary>
        /// <param name="datetime">The date to convert.</param>
        /// <returns>The Unix timestamp representation.</returns>
        public static long ToUnixTimestamp(this DateTime datetime)
            => new DateTimeOffset(datetime).ToUnixTimeSeconds();

        #endregion Formatting

        private static int SecondsSinceStartOfWeek(DateTime datetime)
        {
            if (datetime.DayOfWeek == DayOfWeek.Sunday)
                return 7 * 86400 + datetime.Hour * 3600 + datetime.Minute * 60 + datetime.Second;

            return ((int)datetime.DayOfWeek - 1) * 86400 + datetime.Hour * 3600 + datetime.Minute * 60 + datetime.Second;
        }


    }
}
