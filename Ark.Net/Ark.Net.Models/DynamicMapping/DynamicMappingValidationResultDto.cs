using System.Collections.Generic;

namespace Ark.Net.Models
{
    /// <summary>
    /// This DTO contains a validation report of the Dynamic Mapping Dto.
    /// </summary>
    public class DynamicMappingValidationResultDto
    {
        #region Properties (Public)

        /// <summary>
        /// List of error on a condition or an action of the dynamic mapping.
        /// </summary>
        public List<DynamicMappingErrorDto> ConditionErrors { get; set; }

        /// <summary>
        /// All properties with a bad syntax name.
        /// </summary>
        public List<string> PropertyErrors { get; set; }


        /// <summary>
        /// The format of the excel is valid.
        /// </summary>
        public bool IsExcelFileValid { get; set; }


        /// <summary>
        /// The dynamic mapping can compile without errors.
        /// </summary>
        public bool IsDynamicMappingCompiled { get; set; }

        #endregion Properties (Public)

        #region Properties (Computed)

        /// <summary>
        /// To know if the result of the validation is a success.
        /// </summary>
        public bool Success => ConditionErrors.Count == 0 && PropertyErrors.Count == 0 && IsExcelFileValid && IsDynamicMappingCompiled;

        #endregion Properties (Computed)
    }

    /// <summary>
    /// This Dto contains errors for a condition or an action of the dynamic mapping.
    /// </summary>

    public class DynamicMappingErrorDto
    {
        /// <summary>
        /// The position of the condition/action in the excel file which is mapped in th dynamic mapping Dto.
        /// </summary>
        public string ExcelCell { get; set; }

        /// <summary>
        /// All the errors of the condition/action. 
        /// </summary>
        public List<DynamicMappingErrorEnum> Errors { get; set; }
    }

}
