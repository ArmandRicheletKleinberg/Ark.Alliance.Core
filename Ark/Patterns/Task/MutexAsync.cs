// ReSharper disable UnusedType.Global

namespace Ark
{
    /// <summary>
    /// Async-compatible mutex built on top of <see cref="SemaphoreSlimEx"/> with a maximum count of one.
    /// <para>+ Guarantees exclusive access to a critical section across await points.</para>
    /// <para>- Requires careful disposal to avoid deadlocks.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.threading.semaphoreSlim"/>.</para>
    /// </summary>
    public class MutexAsync : SemaphoreSlimEx
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="MutexAsync"/> with an initial and maximum count of one.
        /// <para>+ Ready-to-use mutual exclusion without additional configuration.</para>
        /// <para>- Concurrency level cannot be changed after creation.</para>
        /// </summary>
        public MutexAsync() : base(1, 1)
        {
        }

        #endregion Constructors
    }
}
