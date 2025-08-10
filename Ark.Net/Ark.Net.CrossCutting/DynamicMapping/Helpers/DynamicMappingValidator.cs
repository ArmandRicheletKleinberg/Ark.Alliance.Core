using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Ark.Net.Models;

namespace Ark.Net.CrossCutting
{
    /// <summary>
    /// Helper to see if the dynamic mapping condition tree has no error. 
    /// </summary>
    public class DynamicMappingValidator
    {

        #region Properties (Public)

        /// <summary>
        /// The Dto to send to the client to see where are the errors on the dynamic mapping sent.
        /// </summary>
        public DynamicMappingValidationResultDto DynamicMappingValidationResultDto { get; set; }

        #endregion Properties (Public)

        #region Properties (Helpers)

        /// <summary>
        /// Regular expression which accepts only characters in an action.
        /// </summary>
        private readonly Regex _charsAcceptedInAction = new Regex("^[a-zA-Z0-9\"/+*%^(),._-]*$");

        /// <summary>
        /// Regular expression which accepts only characters in a condition.
        /// </summary>
        private readonly Regex _charsAcceptedInCondition = new Regex("^[a-zA-Z0-9\"<=>,._@;-]*$");

        /// <summary>
        /// Regular expression which accepts only characters in a property name.
        /// </summary>
        private readonly Regex _charsAcceptedInProperty = new Regex("^[a-zA-Z][a-zA-Z0-9_-]*$");

        /// <summary>
        /// Regular expression which accepts only characters in a variable of a formula.
        /// </summary>
        private readonly Regex _charsAcceptedInVariable = new Regex("^[a-zA-Z0-9_]*$");

        /// <summary>
        /// Regular expression which accepts only the mathematical operator for a formula in an action.
        /// </summary>
        private readonly Regex _operators = new Regex("[+-/%^*]");

        #endregion Properties (Helpers)


        #region Methods (Public)

        /// <summary>
        /// Recursive method for the validation of a tree conditions
        /// </summary>
        /// <param name="condition">Root condition To validate</param>
        /// <param name="isRoot">Indicate if it's a root or not a rood condition</param>
        public void ValidateTreeConditions(DynamicConditionDto condition, bool isRoot = true)
        {
            condition = JsonSerializer.Deserialize<DynamicConditionDto>(JsonSerializer.Serialize(condition)); // Clone the condition to avoid changing its values
            ValidateCondition(condition, isRoot);
            condition.SubConditions?.ForEach(c => ValidateTreeConditions(c, false));
            condition.Actions?.ForEach(ValidateAction);
        }

        /// <summary>
        /// Validation of an action of a dynamic mapping.
        /// </summary>
        /// <param name="action">The action to check</param>
        private void ValidateAction(DynamicActionDto action)
        {
            if (action.Expression.Trim().IsNullOrEmpty())
                return;

            CheckPropertyName(action.PropertyName);

            var conditionError = new DynamicMappingErrorDto { ExcelCell = action.ExcelCell, Errors = new List<DynamicMappingErrorEnum>() };
            action.Expression = CleanExpression(action.Expression);

            if (!_charsAcceptedInAction.IsMatch(action.Expression))
            {
                conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionFormat);
                DynamicMappingValidationResultDto.ConditionErrors.Add(conditionError);
                return;
            }

            if (IsFormulaExpression(action.Expression))
                ValidateFormula(action.Expression, conditionError);
            else
                ValidateEqualValue(action.Expression, conditionError);

            if (conditionError.Errors.Count > 0)
                DynamicMappingValidationResultDto.ConditionErrors.Add(conditionError);
        }

        /// <summary>
        /// Validation of a condition of a dynamic mapping
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="isRoot">The condition is the root of the dynamic mapping ?</param>
        private void ValidateCondition(DynamicConditionDto condition, bool isRoot = false)
        {
            var conditionError = new DynamicMappingErrorDto { ExcelCell = condition.ExcelCell, Errors = new List<DynamicMappingErrorEnum>() };

            if ((condition.Actions == null || condition.Actions.Count == 0) && (condition.SubConditions == null || condition.SubConditions.Count == 0))
                conditionError.Errors.Add(DynamicMappingErrorEnum.NoActionNoCondition);
            else if (condition.Actions != null && condition.Actions.Count > 0 && condition.SubConditions != null && condition.SubConditions.Count > 0)
                conditionError.Errors.Add(DynamicMappingErrorEnum.HasActionAndCondition);

            if (isRoot) return;

            CheckExpressionConditionFormat(condition.Expression, conditionError);
            CheckPropertyName(condition.PropertyName);
            CheckExpressionConditionContent(condition.Expression, conditionError);

            if (conditionError.Errors.Count > 0)
                DynamicMappingValidationResultDto.ConditionErrors.Add(conditionError);
        }

        #endregion Methods (Public)

        #region Methods (Helpers)

        /// <summary>
        /// Vérifie que le contenu de l'expression d'une condition contient uniquement les caractères autorisés. 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="conditionError"></param>
        private void CheckExpressionConditionFormat(string expression, DynamicMappingErrorDto conditionError)
        {
            if (!_charsAcceptedInCondition.IsMatch(expression))
                conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionFormat);
        }

        /// <summary>
        /// Vérifie que le contenu d'une expression est valide.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="conditionError"></param>
        private void CheckExpressionConditionContent(string expression, DynamicMappingErrorDto conditionError)
        {
            if (expression == "-") return;

            var properExpression = ChangeRangeExpression(expression);

            if (properExpression == null || expression.EndsWith(";"))
            {
                conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionCondition);
                return;
            }

            var betweenSemicolon = properExpression.Split(';');
            var type = properExpression.EndsWith("\"") ? typeof(string) : typeof(decimal);

            betweenSemicolon.ForEach(e => CheckExpressionBetweenSemicolon(e, conditionError, type));
        }

        /// <summary>
        /// Vérifie qu'une partie entre ; d'une expression est bien valide.
        /// </summary>
        /// <param name="semicolon"></param>
        /// <param name="conditionError"></param>
        /// <param name="type"></param>
        private void CheckExpressionBetweenSemicolon(string semicolon, DynamicMappingErrorDto conditionError, Type type)
        {
            var semicolonWithoutOperator = RemoveOperatorStarter(semicolon, conditionError);
            if (semicolonWithoutOperator == null) return;

            var numberOfColon = Regex.Matches(semicolonWithoutOperator, ":").Count;
            var numberOfQuote = Regex.Matches(semicolonWithoutOperator, "\"").Count;

            if (!CheckDashAndQuote(numberOfColon, numberOfQuote, semicolonWithoutOperator) && conditionError.Errors.Count(e => e == DynamicMappingErrorEnum.BadExpressionCondition) == 0)
                conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionCondition);

            if (!CheckTypeOfSemicolon(numberOfColon, numberOfQuote, semicolonWithoutOperator, type) && conditionError.Errors.Count(e => e == DynamicMappingErrorEnum.BadExpressionType) == 0)
                conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionType);
        }


        /// <summary>
        /// Vérifie que le type dans une partie entre ; est bien un décimal ou un string.
        /// </summary>
        /// <param name="numberOfColon">Nombre de : présents dans l'expression</param>
        /// <param name="numberOfQuote">Nombre de quotes présents dans l'expression</param>
        /// <param name="semicolonWithoutOperator">Expression entre ;</param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool CheckTypeOfSemicolon(int numberOfColon, int numberOfQuote, string semicolonWithoutOperator, Type type)
        {
            if (numberOfQuote == 0 && type == typeof(string) || ((numberOfQuote == 2 || numberOfQuote == 4) && type == typeof(decimal)))
                return false;

            if (numberOfQuote == 0 && numberOfColon == 0 && !decimal.TryParse(semicolonWithoutOperator, NumberStyles.Any, new CultureInfo("en-US"), out _))
                return false;

            if (numberOfQuote == 0 && numberOfColon == 1)
            {
                var ranges = semicolonWithoutOperator.Split(':');
                return ranges.All(r => decimal.TryParse(r, NumberStyles.Any, new CultureInfo("en-US"), out _));
            }

            return true;
        }

        /// <summary>
        /// Vérifie que l'expression entre ; contient le bon nombre de - et de ".
        /// Vérifie aussi que la présence des " sont bien à la fin et au début de chaque partie. 
        /// </summary>
        /// <param name="numberOfColon">Nombre de : présents dans l'expression</param>
        /// <param name="numberOfQuote">Nombre de quotes présents dans l'expression</param>
        /// <param name="semicolonWithoutOperator">Expression entre ;</param>
        /// <returns></returns>
        private static bool CheckDashAndQuote(int numberOfColon, int numberOfQuote, string semicolonWithoutOperator)
        {
            if (numberOfColon > 1)
                return false;

            if (numberOfQuote != 0 && numberOfQuote != 2 && numberOfQuote != 4)
                return false;

            if (numberOfColon == 1 && numberOfQuote == 2 || numberOfColon == 0 && numberOfQuote == 4)
                return false;

            if (numberOfQuote == 2 && (!semicolonWithoutOperator.StartsWith("\"") || !semicolonWithoutOperator.EndsWith("\"")))
                return false;

            if (numberOfQuote == 4)
            {
                var ranges = semicolonWithoutOperator.Split(':');
                return ranges.All(r => r.StartsWith("\"") && r.EndsWith(("\"")));
            }

            return true;
        }

        /// <summary>
        /// Removes leading comparison operators (e.g., &lt;, &gt;, &lt;&gt;, =, &lt;=, &gt;=).
        /// + Validates operator usage before stripping it.
        /// - Returns <c>null</c> when an invalid sequence is detected.
        /// </summary>
        /// <param name="semicolon">Expression segment to analyze.</param>
        /// <param name="conditionError">Accumulator for detected errors.</param>
        /// <returns>The segment without its leading operator, or <c>null</c> if invalid.</returns>
        private string RemoveOperatorStarter(string semicolon, DynamicMappingErrorDto conditionError)
        {
            var operators = new List<string> { "<", ">", "=" };//ORDRE A DE L'IMPORTANCE ! 

            foreach (var oneOperator in operators)
            {
                var numberOfStarter = Regex.Matches(semicolon, oneOperator).Count;

                if (numberOfStarter == 0 || numberOfStarter == 1 && semicolon.StartsWith(oneOperator))
                    semicolon = semicolon.Replace(oneOperator, string.Empty);
                else
                {
                    if (conditionError.Errors.Count(e => e == DynamicMappingErrorEnum.BadExpressionCondition) == 0)
                        conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionCondition);
                    return null;
                }
            }

            return semicolon;
        }


        /// <summary>
        /// Méthode qui transforme les ranges en Value1->Value2
        /// Range type 1 : @Value1;Value2 => Value1:Value2
        /// Range type 2 : Value1->Value2 => Value1:Value2
        /// </summary>
        /// <param name="expression">expression à transformer</param>
        /// <returns></returns>
        private string ChangeRangeExpression(string expression)
        {
            expression = expression.Replace("->", ":");

            while (expression.Contains('@'))
            {
                var indexArobase = expression.IndexOf('@');
                var indexNextSemiColon = expression.IndexOf(';', indexArobase);

                if (indexNextSemiColon < 0)
                    return null;

                expression = expression.Substring(0, indexArobase) +
                             expression.Substring(indexArobase + 1, indexNextSemiColon - indexArobase - 1) +
                             ":" +
                             expression.Substring(indexNextSemiColon + 1);
            }

            return expression;
        }

        /// <summary>
        /// Vérifie que le nom de propriété est bien en alphanumérique et commence par une lettre.
        /// Si ce n'est pas le cas, ajoute cette propriété à la liste des propriétés en erreur.
        /// </summary>
        /// <param name="property"></param>
        private void CheckPropertyName(string property)
        {
            if (!_charsAcceptedInProperty.IsMatch(property) && !DynamicMappingValidationResultDto.PropertyErrors.Contains(property))
                DynamicMappingValidationResultDto.PropertyErrors.Add(property);
        }


        /// <summary>
        /// Retire d'une chaine les espaces blancs et le = en début de chaine.
        /// </summary>
        /// <param name="expression">chaine à nettoyer</param>
        /// <returns></returns>
        private string CleanExpression(string expression)
        {
            expression = new string(expression.Where(e => !char.IsWhiteSpace(e)).ToArray());

            if (expression.StartsWith("="))
                expression = expression.Substring(1);

            return expression;
        }

        /// <summary>
        /// Indique si l'expression d'une action est une formule ou non.
        /// </summary>
        /// <param name="expression">Expression de l'action.</param>
        /// <returns></returns>
        private bool IsFormulaExpression(string expression)
        {
            if (expression.StartsWith("-"))
                return _operators.Matches(expression.Substring(1)).Count > 0;
            return _operators.Matches(expression).Count > 0;
        }

        /// <summary>
        /// Vérifie que la valeur de l'action est bien un nombre ou une chaine entre "".
        /// </summary>
        private void ValidateEqualValue(string value, DynamicMappingErrorDto conditionError)
        {

            var numberOfQuote = Regex.Matches(value, "\"").Count;

            if (numberOfQuote != 0 && numberOfQuote != 2)
            {
                conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionAction);
                return;
            }


            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                if (!_charsAcceptedInVariable.IsMatch(value.Replace("\"", "")))
                    conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionAction);
            }
            else
            {
                if (!decimal.TryParse(value, NumberStyles.Any, new CultureInfo("en-US"), out _))
                {
                    // TODO : check the property if the value is a property
                    //conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionAction);
                }
            }
        }


        /// <summary>
        /// Vérifie que le string est bien une formule mathématique simple
        /// N'accepte que +-*/^% et (). 
        /// </summary>
        private void ValidateFormula(string formula, DynamicMappingErrorDto conditionError)
        {
            var formulaSplits = Regex.Split(formula, "([+*-/()%^])").Where(x => !string.IsNullOrWhiteSpace(x));

            var parenthesis = 0;
            var fetchVariable = true;
            var fetchMinus = false;

            foreach (var part in formulaSplits)
            {
                if (fetchVariable)
                {
                    if (part == "(")
                    {
                        parenthesis++;
                        fetchMinus = false;
                    }
                    else if (part == "-")
                    {
                        fetchMinus = !fetchMinus;

                        if (fetchMinus) continue;
                        conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionAction);
                        return;

                    }
                    else
                    {
                        if (!decimal.TryParse(part, NumberStyles.Any, new CultureInfo("en-US"), out _) && !_charsAcceptedInProperty.IsMatch(part))
                        {
                            conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionAction);
                            return;
                        }

                        fetchVariable = false;
                        fetchMinus = false;
                    }
                }
                else
                {
                    if (part == ")")
                        parenthesis--;
                    else
                    {
                        if (!_operators.IsMatch(part))
                        {
                            conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionAction);
                            return;
                        }

                        if (part == "-")
                            fetchMinus = true;

                        fetchVariable = true;
                    }
                }

            }

            if (parenthesis != 0)
                conditionError.Errors.Add(DynamicMappingErrorEnum.BadExpressionAction);
        }

        #endregion Methods (Helpers)



    }
}
