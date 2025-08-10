using System.ComponentModel;

namespace Ark.Net.Models
{
    /// <summary>
    /// This enumeration holds errors for a condition or an action of a dynamic mapping
    /// </summary>
    public enum DynamicMappingErrorEnum
    {
        /// <summary>
        /// No error occurred.
        /// </summary>
        None = 0,

        /// <summary>
        /// The condition has no action and no sub-condition.
        /// </summary>
        [Description("La condition n'a pas d'action ou de sous-condition")]
        NoActionNoCondition = 10,

        /// <summary>
        /// The condition has a sub-condition and an action.
        /// </summary>
        [Description("La condition a une sous-condition et une action")]
        HasActionAndCondition = 20,

        /// <summary>
        /// The expression has a bad format
        /// Example : The expression contains 'ù' which is forbidden.
        /// </summary>
        [Description("L'expression a un mauvais format")]
        BadExpressionFormat = 30,

        /// <summary>
        /// The expression has a bad type.
        /// Example : A string is awaited and the expression is a numeric.
        /// </summary>
        [Description("L'expression a un mauvais type")]
        BadExpressionType = 40,

        /// <summary>
        /// The expression of the condition is not well formated.
        /// Example => The ; of a range is missing
        /// </summary>
        [Description("L'expression de la condition n'est pas correctement formatée")]
        BadExpressionCondition = 50,

        /// <summary>
        /// The expression of the action is not well formated.
        /// For example : the formula is badly formated : (a + ) * 5
        /// </summary>
        [Description("L'expression de l'action n'est pas correctement formatée")]
        BadExpressionAction = 60
    }
}