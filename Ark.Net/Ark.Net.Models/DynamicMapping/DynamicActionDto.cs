namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO contains an action to execute when some conditions have been verified.
    /// </summary>
    public class DynamicActionDto
    {
        #region Properties (Public)

        /// <summary>
        /// The type of the action to execute.
        /// </summary>
        public DynamicActionTypeEnum Type { get; set; }

        /// <summary>
        /// The name of the property to set the value if the action is SetValue.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The expression used to compute the value to set for SetValue type or the mapping to execute for ExecuteMapping type.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Position of the cell in the excel for the user.
        /// A dynamic action can be seen as a cell of the excel.
        /// </summary>
        public string ExcelCell { get; set; }


        #endregion Properties (Public)
    }
}