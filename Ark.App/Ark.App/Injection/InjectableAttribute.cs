using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace Ark.App
{
    /// <summary>
    /// Marks a class so that it is automatically registered in the dependency
    /// injection container by <see cref="IHostBuilderExtensions.UseInjectables"/>.
    ///
    /// <para>Example with this attribute:</para>
    /// <code>
    /// [Injectable(ServiceLifetimeEnum.Singleton)]
    /// public class MyService { }
    /// </code>
    /// The service is discovered via reflection and registered at startup.
    ///
    /// <para>Example without this attribute:</para>
    /// <code>
    /// public class MyService { }
    /// // Registration must be manual:
    /// services.AddSingleton&lt;MyService&gt;();
    /// </code>
    ///
    /// <para>Advantages:</para>
    /// <list type="bullet">
    /// <item>Reduces boilerplate code when many services need the same lifetime.</item>
    /// <item>Centralises registration logic.</item>
    /// </list>
    /// <para>Disadvantages:</para>
    /// <list type="bullet">
    /// <item>Requires reflection during startup (a few milliseconds overhead).</item>
    /// <item>Less explicit than manual <c>AddSingleton</c>, <c>AddScoped</c> or <c>AddTransient</c>.</item>
    /// </list>
    ///
    /// <para>Performance:</para>
    /// Using this attribute adds minimal startup time compared to manual
    /// registrations. The running application performance is identical because
    /// the lifetime is enforced by the DI container. When startup time is
    /// critical and only a handful of services need registration, explicit calls
    /// to <c>IServiceCollection</c> can be slightly faster.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class InjectableAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="InjectableAttribute"/> instance.
        /// </summary>
        /// <param name="serviceLifetime">The service lifetime for the service injection. Default to transient.</param>
        public InjectableAttribute(ServiceLifetimeEnum serviceLifetime = ServiceLifetimeEnum.Transient)
            => ServiceLifetime = serviceLifetime;

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// The service lifetime for the service injection.
        /// Default to transient.
        /// </summary>
        public ServiceLifetimeEnum ServiceLifetime { get; }

        #endregion Properties (Public)
    }
}