
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ark.AspNetCore
{
    /// <summary>
    /// Provides helper methods to register MediatR handlers.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class MediatRExtensions
    {
        ///// <summary>
        ///// Registers MediatR and scans the assembly containing <typeparamref name="T"/>.
        ///// </summary>
        ///// <param name="services">The service collection to configure.</param>
        //public static IServiceCollection AddMediatRHandlersFrom<T>(this IServiceCollection services)
        //{
        //    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<T>());
        //    return services;
        //}
    }
}
