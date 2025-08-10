using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Data.EFCore
{
    /// <summary>
    /// This class extends the <see cref="ModelBuilder"/> class.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Seeds some data into an entity table.
        /// It is used by the EF Core migration to insert/update/delete some initial data in the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to seed.</typeparam>
        /// <param name="modelBuilder">The model builder to add some initial data to the model.</param>
        /// <param name="dataToSeed">The data to seed, may be multiple entities.</param>
        public static void Seed<TEntity>(this ModelBuilder modelBuilder, params TEntity[] dataToSeed)
            where TEntity : DbEntity
            => modelBuilder.Entity<TEntity>().HasData(dataToSeed);

        #endregion Methods (Public)
    }
}