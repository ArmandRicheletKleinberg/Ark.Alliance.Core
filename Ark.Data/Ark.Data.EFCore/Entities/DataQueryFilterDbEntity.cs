using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ark.Net.Models;

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Represents a predicate applied to an entity property.
    /// <para>+ Supports nested groups via <see cref="LinkedFilters"/>.</para>
    /// <para>- Incorrect type mappings can lead to runtime errors.</para>
    /// Ref: <see href="https://learn.microsoft.com/ef/core/querying/complex-query-scenarios"/>
    /// </summary>
    public class DataQueryFilterDbEntity
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="DataQueryFilterDbEntity"/> instance.
        /// </summary>
        public DataQueryFilterDbEntity()
        { }

        /// <summary>
        /// Creates a new <see cref="DataQueryFilterDbEntity"/> instance given a DTO.
        /// Replaces possibly a string enumeration value to int if the property type is specified and is Enum.
        /// </summary>
        /// <param name="filter">The DTO containing the filter parameters.</param>
        /// <param name="dtoToEntityNamesMapping">The dictionary of the corresponding DTO/entity names if different.</param>
        /// <param name="properties">The properties of the entity ordered by property name needed to replace the string value of an enumeration to the enumeration itself.</param>
        public DataQueryFilterDbEntity(DataQueryFilterDto filter, IDictionary<string, string> dtoToEntityNamesMapping = null, Dictionary<string, PropertyInfo> properties = null)
        {
            string GetEntityName(string dtoName) => dtoToEntityNamesMapping?.GetValue(dtoName) ?? dtoName;

            PropertyName = GetEntityName(filter.PropertyName);
            Comparison = filter.Comparison;

            var propertyType = properties.GetValue(PropertyName)?.PropertyType;
            if (propertyType != null && propertyType.IsEnum && Enum.TryParse(propertyType, filter.Value as string, out var enumValue))
                Value = enumValue;
            else
                Value = filter.Value;

            Link = filter.Link;
            LinkedFilters = filter.LinkedFilters?.Select(f => new DataQueryFilterDbEntity(f, dtoToEntityNamesMapping, properties)).ToList();
        }

        #endregion Constructors

        #region Properties (Public)

        /// <summary>
        /// Name of the property on which to apply the filter.
        /// <para>+ Aligns with column names for server-side evaluation.</para>
        /// <para>- Typos surface as runtime exceptions.</para>
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Comparison operator used for evaluation.
        /// <para>+ Mirrors <see cref="DataQueryFilterComparisonEnum"/> semantics.</para>
        /// <para>- Unsupported operators may not translate to SQL.</para>
        /// </summary>
        public DataQueryFilterComparisonEnum Comparison { get; set; }

        /// <summary>
        /// Value to compare against the property.
        /// <para>+ Accepts primitives or enum names serialized as strings.</para>
        /// <para>- Mismatched types result in ignored filters.</para>
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Logical operator linking this filter to others.
        /// <para>+ Enables AND/OR group composition.</para>
        /// <para>- <see cref="DataQueryFilterLinkEnum.None"/> leaves filters ungrouped.</para>
        /// </summary>
        public DataQueryFilterLinkEnum Link { get; set; }

        /// <summary>
        /// Filters nested under this predicate.
        /// <para>+ Allows recursive query trees.</para>
        /// <para>- Deep recursion may impact performance.</para>
        /// </summary>
        public List<DataQueryFilterDbEntity> LinkedFilters { get; set; }

        #endregion Properties (Public)
    }
}