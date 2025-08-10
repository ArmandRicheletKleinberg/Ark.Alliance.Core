namespace Ark
{
    /// <summary>
    /// A timed object is a group time/object.
    /// Useful for short life object.
    /// </summary>
    public class TimedObject<TObj>
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="TimedObject{TObj}"/> instance.
        /// </summary>
        /// <param name="obj">The object to keep along with the time.</param>
        /// <param name="time">The time to store along with the object. If null then DateTime.UtcNow is used.</param>
        public TimedObject(TObj obj, DateTime? time = null)
        {
            Object = obj;
            Time = time ?? DateTime.UtcNow;
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The time to store along with the object.
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// The object to keep along with the time.
        /// </summary>
        public TObj Object { get; }

        #endregion Properties (Public)
    }
}