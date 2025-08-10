namespace Ark
{
    /// <summary>
    /// This interface is used to define the minimum functionality that composes a schedule (even a set).
    /// </summary>
    public interface ISchedule
    {
        /// <summary>
        /// Gets the next schedule time in a laps of time or null if the schedule is finished.
        /// </summary>
        /// <returns>The laps to the next execution time if any or null if finished.</returns>
        TimeSpan? GetNextScheduleLaps();

        /// <summary>
        /// Gets the next schedule time in local time or null if the schedule is finished.
        /// </summary>
        /// <returns>The next execution time in local time if any or null if finished.</returns>
        DateTime? GetNextScheduleTime();

        /// <summary>
        /// Gets the next schedule time in UTC time or null if the schedule is finished.
        /// </summary>
        /// <returns>The next execution time in UTC time if any or null if finished.</returns>
        DateTime? GetNextScheduleTimeUtc();

        /// <summary>
        /// Gets the next schedule time in local time or null if the schedule is finished.
        /// </summary>
        /// <param name="scheduleLimit">^The limit of schedules to return.</param>
        /// <returns>The next execution time in local time if any or null if finished.</returns>
        DateTime[] GetNextScheduleTimes(int scheduleLimit);

        /// <summary>
        /// Gets the next schedule time in UTC time or null if the schedule is finished.
        /// </summary>
        /// <returns>The next execution time in UTC time if any or null if finished.</returns>
        DateTime[] GetNextScheduleTimesUtc(int scheduleLimit);
    }
}
