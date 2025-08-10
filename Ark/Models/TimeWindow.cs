namespace Ark
{
    /// <summary>
    /// Represents a local time window used for queries.
    /// </summary>
    public class TimeWindow
    {
        /// <summary>Date part of the start.</summary>
        public DateTime FromDate { get; set; }

        /// <summary>Date part of the end.</summary>
        public DateTime ToDate { get; set; }

        /// <summary>Time of day for the start.</summary>
        public TimeSpan FromTime { get; set; }

        /// <summary>Time of day for the end.</summary>
        public TimeSpan ToTime { get; set; }

        /// <summary>Computed start in local time.</summary>
        public DateTime StartLocal => FromDate.Date + FromTime;

        /// <summary>Computed end in local time.</summary>
        public DateTime EndLocal => ToDate.Date + ToTime;

        /// <summary>Start of the window converted to UTC.</summary>
        public DateTime StartUtc => DateTime.SpecifyKind(StartLocal, DateTimeKind.Local).ToUniversalTime();

        /// <summary>End of the window converted to UTC.</summary>
        public DateTime EndUtc => DateTime.SpecifyKind(EndLocal, DateTimeKind.Local).ToUniversalTime();
    }

}
