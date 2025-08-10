using System;
using System.Linq;
using System.Reflection;
using Ark.Data.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedTypeParameter

namespace Ark.AspNetCore.Search
{
    /// <summary>
    /// This class extend the <see cref="IWebHostBuilder"/> class.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IWebHostBuilderExtensions
    {
        #region Methods (UseSearch)

        /// <summary>
        /// Enables the search feature to this application.
        /// It will search in any defined assemblies the <see cref="SearchItemType"/> type to add these types to the application search.
        /// A type defines how the data will be searched in the database accessed by the context given.
        /// </summary>
        /// <typeparam name="TContext">The type of the database context to search for search items depending on the types.</typeparam>
        /// <param name="builder">The web host builder to add the search feature to the application.</param>
        /// <param name="assembliesToSearchForSearchItemTypes">The assemblies into which search for <see cref="SearchItemType"/> types to include in the application search.</param>
        /// <returns>The same web host builder to chain.</returns>
        public static IWebHostBuilder UseSearch<TContext>(this IWebHostBuilder builder, params Assembly[] assembliesToSearchForSearchItemTypes)
            where TContext : DbContextEx, new()
            => builder.ConfigureServices((context, services) =>
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("SearchPolicy", policyBuilder =>
                    {
                        policyBuilder.AllowAnyOrigin();
                    });
                });

                var types = assembliesToSearchForSearchItemTypes
                    .SelectMany(assembly => assembly.DefinedTypes)
                    .Where(type => type.IsAssignableTo(typeof(SearchItemType)) && !type.IsAbstract)
                    .Select(type => type.New<SearchItemType>())
                    .ToArray();

                var errors = types.Select(t => t.Validate()).IfNotNull().ToArray();
                if (errors.HasAnElement())
                    throw new Exception($"Validation error for the search types:{Environment.NewLine}{Environment.NewLine}{string.Join($"{Environment.NewLine}{Environment.NewLine}", errors)}");

                SearchController.TypesByCode = types.ToDictionary(t => t.Code);
                SearchController.CreateDbContextFunc = () => new TContext();
            });

        #endregion Methods (UseSearch)
    }
}