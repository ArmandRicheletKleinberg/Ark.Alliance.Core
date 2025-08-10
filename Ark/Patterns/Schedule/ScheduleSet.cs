// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ark
{
    /// <summary>
    /// This class allows to specify a set of repetitive or single schedules.
    /// </summary>
    public class ScheduleSet : ISchedule
    {
        #region Static Constructors

        /// <summary>
        /// Creates a new schedule set given a schedule.
        /// </summary>
        /// <param name="schedule">the schedule to add in the set.</param>
        /// <returns>The created scheduled set.</returns>
        public static ScheduleSet Create(ISchedule schedule = null)
            => new(schedule);

        #endregion Static Constructors

        #region Properties (Public)

        /// <summary>
        /// The number of occurence of the schedule.
        /// It is computed from the GetNextScheduleTime call time.
        /// Not relevant in strict mode.
        /// </summary>
        public List<ISchedule> Schedules { get; }

        #endregion Properties (Public)

        #region Constructors

        /// <summary>
        /// Creates a <see cref="ScheduleSet"/> instance.
        /// </summary>
        /// <param name="schedule">A first schedule if any.</param>
        public ScheduleSet(ISchedule schedule = null)
        {
            Schedules = new List<ISchedule>();
            if (schedule != null)
                Schedules.Add(schedule);
        }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Adds a new schedule to the set.
        /// </summary>
        /// <param name="schedule">The schedule to add to the set.</param>
        public ScheduleSet AddSchedule(ISchedule schedule)
        {
            if (schedule != null)
                Schedules.Add(schedule);

            return this;
        }

        #endregion Methods (Public)

        #region ISchedule

        /// <summary>
        /// Gets the next schedule time in a laps of time or null if the schedule is finished.
        /// </summary>
        /// <returns>The laps to the next execution time if any or null if finished.</returns>
        public TimeSpan? GetNextScheduleLaps()
            => Schedules.Select(s => s.GetNextScheduleLaps()).IfNotNull().MinOrDefault();


        /// <summary>
        /// Gets the next schedule time in local time or null if the schedule is finished.
        /// </summary>
        /// <returns>The next execution time in local time if any or null if finished.</returns>
        public DateTime? GetNextScheduleTime()
            => Schedules.Select(s => s.GetNextScheduleTime()).IfNotNull().MinOrDefault();

        /// <summary>
        /// Gets the next schedule time in UTC time or null if the schedule is finished.
        /// </summary>
        /// <returns>The next execution time in UTC time if any or null if finished.</returns>
        public DateTime? GetNextScheduleTimeUtc()
            => Schedules.Select(s => s.GetNextScheduleTimeUtc()).IfNotNull().MinOrDefault();

        /// <summary>
        /// Gets the next schedule time in local time or null if the schedule is finished.
        /// </summary>
        /// <param name="scheduleLimit">^The limit of schedules to return.</param>
        /// <returns>The next execution time in local time if any or null if finished.</returns>
        public DateTime[] GetNextScheduleTimes(int scheduleLimit)
            => Schedules.SelectMany(s => s.GetNextScheduleTimes(scheduleLimit)).OrderBy(t => t).Take(scheduleLimit).ToArray();

        /// <summary>
        /// Gets the next schedule time in UTC time or null if the schedule is finished.
        /// </summary>
        /// <returns>The next execution time in UTC time if any or null if finished.</returns>
        public DateTime[] GetNextScheduleTimesUtc(int scheduleLimit)
            => Schedules.SelectMany(s => s.GetNextScheduleTimesUtc(scheduleLimit)).OrderBy(t => t).Take(scheduleLimit).ToArray();

        #endregion ISchedule
    }
}
