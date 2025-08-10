// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.App
{
    /// <summary>
    /// Defines the lifetime used when registering a service in the dependency
    /// injection container. This mirrors the lifetimes found in ASP.NET Core.
    ///
    /// <para>Example:</para>
    /// <code>
    /// [Injectable(ServiceLifetimeEnum.Scoped)]
    /// public class DataRepository { }
    /// </code>
    /// Without specifying a lifetime the default is
    /// <see cref="ServiceLifetimeEnum.Transient"/>.
    ///
    /// <para>Performance:</para>
    /// Choosing <c>Singleton</c> avoids repeated allocations but may consume
    /// more memory. <c>Transient</c> provides isolation at the cost of more
    /// allocations. <c>Scoped</c> is a compromise used per HTTP request.
    /// </summary>
    public enum ServiceLifetimeEnum
    {
        /// <summary>
        /// This service will be created at each time it is injected.
        /// </summary>
        Transient = 0,

        /// <summary>
        /// This service will be created only once for per client request (connection).
        /// For Web API, it will be created once by controller call.
        /// For Blazor, it will be created for a whole user circuit.
        /// </summary>
        Scoped = 1,

        /// <summary>
        /// This service is a single static instance only created once.
        /// </summary>
        Singleton = 2
    }
}