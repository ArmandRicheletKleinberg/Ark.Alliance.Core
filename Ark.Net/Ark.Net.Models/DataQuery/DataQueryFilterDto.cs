using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ark.Net.Models
{
    /// <summary>
    /// This class is used to specify a filter on a property to query some filtered data.
    /// </summary>
    public class DataQueryFilterDto
    {
        #region Properties (Public)

        /// <summary>
        /// The name of the property on which apply this filter.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The comparison type between the property value and another value.
        /// Depends on the property type.
        /// </summary>
        public DataQueryFilterComparisonEnum Comparison { get; set; }

        /// <summary>
        /// The value to compare with the property value.
        /// </summary>
        [JsonConverter(typeof(PrimitiveObjectJsonConverter))]
        public object Value { get; set; }

        /// <summary>
        /// The link to apply with the other filters linked.
        /// If none, the other filters are not linked, otherwise a logical AND or OR will be applied.
        /// </summary>
        public DataQueryFilterLinkEnum Link { get; set; }

        /// <summary>
        /// The filters linked to this filter in order to create logical AND or OR group.
        /// The Link property must be set to anything else then None.
        /// </summary>
        public List<DataQueryFilterDto> LinkedFilters { get; set; }

        #endregion Properties (Public)
    }
}