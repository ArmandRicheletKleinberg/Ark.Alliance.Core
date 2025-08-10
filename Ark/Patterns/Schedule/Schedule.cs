// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace Ark
{
    /// <summary>
    /// This class allows to specify a repetitive or single schedule.
    /// </summary>
    public class Schedule : ISchedule
    {
        #region Static Constructors

        /// <summary>
        /// The scheduled event should only occur once at a specified time.
        /// </summary>
        /// <param name="nextTime">The next time the scheduled event should occur.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule Single(DateTime nextTime)
            => new(nextTime, (TimeSpan?)null);

        /// <summary>
        /// The scheduled event should only occur once after a defined laps.
        /// </summary>
        /// <param name="nextLaps">The next laps to wait to execute the scheduled event.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule Single(TimeSpan nextLaps)
            => new(DateTime.UtcNow.Add(nextLaps), (TimeSpan?)null);

        /// <summary>
        /// The scheduled event should execute every timespan from now.
        /// </summary>
        /// <param name="nextLaps">The next event scheduled laps.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule Every(TimeSpan nextLaps)
            => new(DateTime.UtcNow.Add(nextLaps), nextLaps);

        /// <summary>
        /// The scheduled event should execute every timespan from a starting time.
        /// </summary>
        /// <param name="startTime">The starting time.</param>
        /// <param name="nextLaps">The next event scheduled laps.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule Every(DateTime startTime, TimeSpan nextLaps)
            => new(startTime, nextLaps);

        /// <summary>
        /// The scheduled event should execute every second from now.
        /// </summary>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EverySecondFromNow()
            => EverySecondsFromNow(1);

        /// <summary>
        /// The scheduled event should execute every a given number of seconds from now.
        /// </summary>
        /// <param name="seconds">The given number of seconds for the new scheduled event.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EverySecondsFromNow(int seconds)
            => new(DateTime.UtcNow.AddSeconds(seconds), new TimeSpan(0, 0, 0, seconds));

        /// <summary>
        /// The scheduled event should execute every second at a specified time.
        /// </summary>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EverySecond(int milliseconds = 0)
        {
            var now = DateTime.UtcNow;
            var nextTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddSeconds(1);
            return new Schedule(nextTime, new TimeSpan(0, 0, 0, 1));
        }

        /// <summary>
        /// The scheduled event should execute every minute from now.
        /// </summary>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryMinuteFromNow()
            => EveryMinutesFromNow(1);

        /// <summary>
        /// The scheduled event should execute every a given number of minutes from now.
        /// </summary>
        /// <param name="minutes">The given number of minutes for the new scheduled event.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryMinutesFromNow(int minutes)
            => new(DateTime.UtcNow.AddMinutes(minutes), new TimeSpan(0, 0, minutes, 0));

        /// <summary>
        /// The scheduled event should execute every minute at a specified time.
        /// </summary>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryMinute(int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.UtcNow;
            var nextTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddMinutes(1);
            return new Schedule(nextTime, new TimeSpan(0, 0, 1, 0));
        }

        /// <summary>
        /// The scheduled event should execute every hour from now.
        /// </summary>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryHourFromNow()
            => EveryHoursFromNow(1);

        /// <summary>
        /// The scheduled event should execute every a given number of hours from now.
        /// </summary>
        /// <param name="hours">The given number of hours for the new scheduled event.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryHoursFromNow(int hours)
            => new(DateTime.UtcNow.AddHours(hours), new TimeSpan(0, hours, 0, 0));

        /// <summary>
        /// The scheduled event should execute every hour at a specified time.
        /// </summary>
        /// <param name="minutes">The minutes when the execution is scheduled.</param>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryHour(int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.UtcNow;
            var nextTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, minutes, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddHours(1);
            return new Schedule(nextTime, new TimeSpan(0, 1, 0, 0));
        }

        /// <summary>
        /// The scheduled event should execute every day from now.
        /// </summary>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryDayFromNow()
            => EveryDaysFromNow(1);

        /// <summary>
        /// The scheduled event should execute every a given number of days from now.
        /// </summary>
        /// <param name="days">The given number of days for the new scheduled event.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryDaysFromNow(int days)
            => new(DateTime.UtcNow.AddDays(days), new TimeSpan(days, 0, 0, 0));

        /// <summary>
        /// The scheduled event should execute every day at a specified time.
        /// </summary>
        /// <param name="hours">The hours when the execution is scheduled.</param>
        /// <param name="minutes">The minutes when the execution is scheduled.</param>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryDay(int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.Now;
            var nextTime = new DateTime(now.Year, now.Month, now.Day, hours, minutes, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddDays(1);
            return new Schedule(nextTime, new TimeSpan(1, 0, 0, 0));
        }

        /// <summary>
        /// The scheduled event should execute every day at a specified time.
        /// </summary>
        /// <param name="hours">The hours when the execution is scheduled (in UTC).</param>
        /// <param name="minutes">The minutes when the execution is scheduled.</param>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryDayUtc(int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.UtcNow;
            var nextTime = new DateTime(now.Year, now.Month, now.Day, hours, minutes, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddDays(1);
            return new Schedule(nextTime, new TimeSpan(1, 0, 0, 0));
        }

        /// <summary>
        /// The scheduled event should execute every week from now.
        /// </summary>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryWeekFromNow()
            => EveryWeeksFromNow(1);

        /// <summary>
        /// The scheduled event should execute every a given number of days from now.
        /// </summary>
        /// <param name="weeks">The given number of weeks for the new scheduled event.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryWeeksFromNow(int weeks)
            => new(DateTime.UtcNow.AddDays(weeks * 7), new TimeSpan(weeks * 7, 0, 0, 0));

        /// <summary>
        /// The scheduled event should execute every week at a specified time.
        /// </summary>
        /// <param name="day">The days of the week when the execution is scheduled.</param>
        /// <param name="hours">The hours when the execution is scheduled.</param>
        /// <param name="minutes">The minutes when the execution is scheduled.</param>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryWeek(DayOfWeek day = DayOfWeek.Sunday, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.Now;
            var nextTime = new DateTime(now.Year, now.Month, now.Day + (int)day - (int)now.DayOfWeek, hours, minutes, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddDays(7);
            return new Schedule(nextTime, new TimeSpan(7, 0, 0, 0));
        }

        /// <summary>
        /// The scheduled event should execute every week at a specified time.
        /// </summary>
        /// <param name="day">The days of the week when the execution is scheduled.</param>
        /// <param name="hours">The hours when the execution is scheduled (in UTC).</param>
        /// <param name="minutes">The minutes when the execution is scheduled.</param>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryWeekUtc(DayOfWeek day = DayOfWeek.Sunday, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.UtcNow;
            var nextTime = new DateTime(now.Year, now.Month, now.Day + (int)day - (int)now.DayOfWeek, hours, minutes, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddDays(7);
            return new Schedule(nextTime, new TimeSpan(7, 0, 0, 0));
        }

        /// <summary>
        /// The scheduled event should execute every month from now.
        /// </summary>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryMonthFromNow()
            => EveryMonthFromNow(1);

        /// <summary>
        /// The scheduled event should execute every a given number of months from now.
        /// </summary>
        /// <param name="months">The given number of months for the new scheduled event.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryMonthFromNow(int months)
            => new(DateTime.UtcNow.AddMonths(months), months);

        /// <summary>
        /// The scheduled event should execute every month at a specified time.
        /// </summary>
        /// <param name="day">The days of the month when the execution is scheduled.</param>
        /// <param name="hours">The hours when the execution is scheduled.</param>
        /// <param name="minutes">The minutes when the execution is scheduled.</param>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryMonth(int day = 1, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.Now;
            var nextTime = new DateTime(now.Year, now.Month, day, hours, minutes, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddMonths(1);
            return new Schedule(nextTime, 1);
        }

        /// <summary>
        /// The scheduled event should execute every month at a specified time.
        /// </summary>
        /// <param name="day">The days of the week when the execution is scheduled.</param>
        /// <param name="hours">The hours when the execution is scheduled (in UTC).</param>
        /// <param name="minutes">The minutes when the execution is scheduled.</param>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryMonthUtc(int day = 1, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.UtcNow;
            var nextTime = new DateTime(now.Year, now.Month, day, hours, minutes, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddMonths(1);
            return new Schedule(nextTime, 1);
        }

        /// <summary>
        /// The scheduled event should execute every year from now.
        /// </summary>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryYearFromNow()
            => EveryMonthFromNow(12);

        /// <summary>
        /// The scheduled event should execute every a given number of years from now.
        /// </summary>
        /// <param name="years">The given number of years for the new scheduled event.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryYearsFromNow(int years)
            => new(DateTime.UtcNow.AddMonths(years * 12), years * 12);

        /// <summary>
        /// The scheduled event should execute every year at a specified time.
        /// </summary>
        /// <param name="month">The months in the year when the execution is scheduled.</param>
        /// <param name="day">The days of the month when the execution is scheduled.</param>
        /// <param name="hours">The hours when the execution is scheduled.</param>
        /// <param name="minutes">The minutes when the execution is scheduled.</param>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryYear(int month = 1, int day = 1, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.Now;
            var nextTime = new DateTime(now.Year, month, day, hours, minutes, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddYears(1);
            return new Schedule(nextTime, 12);
        }

        /// <summary>
        /// The scheduled event should execute every year at a specified time.
        /// </summary>
        /// <param name="month">The months in the year when the execution is scheduled.</param>
        /// <param name="day">The days of the week when the execution is scheduled.</param>
        /// <param name="hours">The hours when the execution is scheduled (in UTC).</param>
        /// <param name="minutes">The minutes when the execution is scheduled.</param>
        /// <param name="seconds">The seconds when the execution is scheduled.</param>
        /// <param name="milliseconds">The milliseconds when the execution is scheduled.</param>
        /// <returns>The created schedule instance.</returns>
        public static Schedule EveryYearUtc(int month = 1, int day = 1, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            var now = DateTime.UtcNow;
            var nextTime = new DateTime(now.Year, month, day, hours, minutes, seconds, milliseconds);
            nextTime = nextTime > now ? nextTime : nextTime.AddYears(1);
            return new Schedule(nextTime, 12);
        }

        #endregion Static Constructors

        #region Fields

        /// <summary>
        /// The next UTC time the scheduled event is planned, null if finished.
        /// </summary>
        private DateTime? _nextTime;

        /// <summary>
        /// The laps to wait between two scheduled events.
        /// </summary>
        private readonly TimeSpan? _recurringLaps;

        /// <summary>
        /// The number of months between 2 execution to wait between two scheduled events.
        /// This is done because month is variable unit of time.
        /// </summary>
        private readonly int? _recurringMonths;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Private constructor.
        /// Use static method to instance the schedule.
        /// </summary>
        /// <param name="nextTime">The next time the scheduled event should occur.</param>
        /// <param name="recurringLaps">The recurring laps between the schedule task.</param>
        private Schedule(DateTime nextTime, TimeSpan? recurringLaps = null)
        {
            _nextTime = nextTime.ToUniversalTime();
            _recurringLaps = recurringLaps;
        }

        /// <summary>
        /// Private constructor.
        /// Use static method to instance the schedule.
        /// </summary>
        /// <param name="nextTime">The next time the scheduled event should occur.</param>
        /// <param name="recurringMonths">The recurring months between the schedule task.</param>
        private Schedule(DateTime nextTime, int? recurringMonths = null)
        {
            _nextTime = nextTime.ToUniversalTime();
            _recurringMonths = recurringMonths;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The number of occurence of the schedule.
        /// It is computed from the GetNextScheduleTime call time.
        /// Not relevant in strict mode.
        /// </summary>
        public int CurrentOccurenceNumber { get; set; }

        /// <summary>
        /// The maximum number of occurence to limit (finished afterwards).
        /// </summary>
        public int? MaxOccurenceNumber { get; set; }

        /// <summary>
        /// In strict mode, the schedule will bypass the next time if already past. Doing so will guarantee the schedule to trigger at the specified time.
        /// If not strict, then the next execution is delayed and the next time will return the current time. Doing so will guarantee the number of schedules are correct.
        /// </summary>
        public bool StrictMode { get; set; }

        #endregion Properties (Public)

        #region Methods (Public)

        /// <summary>
        /// Sets the max occurence number limit for this schedule.
        /// </summary>
        /// <param name="maxOccurenceNumber">The max occurence number.</param>
        /// <returns>This schedule to chain.</returns>
        public Schedule SetMaxOccurenceNumber(int maxOccurenceNumber)
        {
            MaxOccurenceNumber = maxOccurenceNumber;
            return this;
        }

        /// <summary>
        /// Sets the strict mode for the schedule.
        /// In strict mode, the schedule will bypass the next time if already past. Doing so will guarantee the schedule to trigger at the specified time.
        /// If not strict, then the next execution is delayed and the next time will return the current time. Doing so will guarantee the number of schedules are correct.
        /// </summary>
        /// <returns>This schedule to chain.</returns>
        public Schedule SetStrictMode()
        {
            StrictMode = true;
            return this;
        }

        #endregion Methods (Public)

        #region ISchedule

        /// <summary>
        /// Gets the next schedule time in a laps of time or null if the schedule is finished.
        /// </summary>
        /// <returns>The laps to the next execution time if any or null if finished.</returns>
        public TimeSpan? GetNextScheduleLaps()
        {
            var utcNow = DateTime.UtcNow;
            return GetNextScheduleTimeUtc(utcNow)?.Subtract(utcNow);
        }

        /// <summary>
        /// Gets the next schedule time in local time or null if the schedule is finished.
        /// </summary>
        /// <returns>The next execution time in local time if any or null if finished.</returns>
        public DateTime? GetNextScheduleTime()
            => GetNextScheduleTimeUtc()?.ToLocalTime();

        /// <summary>
        /// Gets the next schedule time in UTC time or null if the schedule is finished.
        /// </summary>
        /// <returns>The next execution time in UTC time if any or null if finished.</returns>
        public DateTime? GetNextScheduleTimeUtc()
            => GetNextScheduleTimeUtc(DateTime.UtcNow);

        /// <summary>
        /// Gets the next schedule time in local time or null if the schedule is finished.
        /// </summary>
        /// <param name="scheduleLimit">^The limit of schedules to return.</param>
        /// <returns>The next execution time in local time if any or null if finished.</returns>
        public DateTime[] GetNextScheduleTimes(int scheduleLimit)
        {
            // Gets the start time and returns an empty array if already finished
            var startTime = GetNextScheduleTimeUtc(DateTime.Now);
            if (!startTime.HasValue)
                return new DateTime[0];

            // Fills the array
            var times = new DateTime[scheduleLimit];
            times[0] = startTime.Value;
            for (var counter = 1; counter < scheduleLimit; counter++)
                times[counter] = ComputeNextTime(times[counter - 1], _recurringLaps, _recurringMonths);

            return times;
        }

        /// <summary>
        /// Gets the next schedule time in UTC time or null if the schedule is finished.
        /// </summary>
        /// <returns>The next execution time in UTC time if any or null if finished.</returns>
        public DateTime[] GetNextScheduleTimesUtc(int scheduleLimit)
        {
            // Gets the start time and returns an empty array if already finished
            var startTime = GetNextScheduleTimeUtc(DateTime.UtcNow);
            if (!startTime.HasValue)
                return new DateTime[0];

            // Fills the array
            var times = new DateTime[scheduleLimit];
            times[0] = startTime.Value;
            for (var counter = 1; counter < scheduleLimit; counter++)
                times[counter] = ComputeNextTime(times[counter - 1], _recurringLaps, _recurringMonths);

            return times;
        }

        #endregion ISchedule

        #region Methods (Helpers)

        /// <summary>
        /// Gets the next schedule time in UTC time or null if the schedule is finished.
        /// </summary>
        /// <param name="utcNow">The current time in UTC.</param>
        /// <returns>The next execution time in UTC time if any or null if finished.</returns>
        private DateTime? GetNextScheduleTimeUtc(DateTime utcNow)
        {
            // Checks the max occurence limit
            if (MaxOccurenceNumber.HasValue && MaxOccurenceNumber.Value > CurrentOccurenceNumber)
            {
                _nextTime = null;
                return null;
            }

            // Returns the next time if finished or not yet reached
            if (_nextTime == null || _nextTime >= utcNow)
            {
                CurrentOccurenceNumber = CurrentOccurenceNumber > 0 ? CurrentOccurenceNumber : 1;
                return _nextTime;
            }

            // if the recurring laps is over then do not start a next time
            if (_recurringLaps == null && _recurringMonths == null)
            {
                _nextTime = null;
                return null;
            }

            // Computes the next time
            CurrentOccurenceNumber++;
            _nextTime = ComputeNextTime(_nextTime.Value, _recurringLaps, _recurringMonths);
            while (StrictMode && _nextTime.Value < utcNow)
                _nextTime = ComputeNextTime(_nextTime.Value, _recurringLaps, _recurringMonths);
            _nextTime = _nextTime < utcNow ? utcNow : _nextTime;

            return _nextTime;
        }

        /// <summary>
        /// Computes the next time given a starting time and a laps + number of month.
        /// </summary>
        /// <param name="startTime">The starting time.</param>
        /// <param name="laps">The laps to add.</param>
        /// <param name="month">The number of month to add.</param>
        /// <returns>The next time.</returns>
        private DateTime ComputeNextTime(DateTime startTime, TimeSpan? laps, int? month)
            => startTime.Add(laps ?? TimeSpan.Zero).AddMonths(month ?? 0);

        #endregion Methods (Helpers)
    }
}
