using System.Collections.Generic;

namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO contains a single expression condition in a dynamic mapping.
    /// </summary>
    public class DynamicConditionDto
    {
        #region Properties (Public)

        /// <summary>
        /// The name of the property to check the value from.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The boolean expression to check.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Position of the cell in the excel for the user.
        /// A dynamic condition can be seen as a cell of the excel.
        /// </summary>
        public string ExcelCell { get; set; }

        /// <summary>
        /// The sub conditions to check if this condition is verified.
        /// </summary>
        public List<DynamicConditionDto> SubConditions { get; set; }

        /// <summary>
        /// The actions to execute if this condition is verified.
        /// </summary>
        public List<DynamicActionDto> Actions { get; set; }

        #endregion Properties (Public)
    }
}