using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AgileObjects.ReadableExpressions;
using Ark.Net.Models;
using Microsoft.Extensions.Logging;

// ReSharper disable InvalidXmlDocComment

namespace Ark.Net.CrossCutting
{
    /// <summary>
    /// This helper is used to compile a dynamic mapping from a DTO to a .NET compiled Action.
    /// It should be instanced at each call because it keeps some internal state.
    /// </summary>
    /// <typeparam name="TObject">The type of the parameter object which will be used in the compiled method.</typeparam>
    public class DynamicMappingCompiler<TObject>
    {
        #region Fields

        /// <summary>
        /// The mapping configuration.
        /// </summary>
        private readonly DynamicMappingDto _mapping;

        /// <summary>
        /// The object parameter expression to map.
        /// </summary>
        private ParameterExpression _obj;

        /// <summary>
        /// The return target is used as an exit spot to return from the main method block.
        /// </summary>
        private LabelTarget _returnTarget;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a <see cref="DynamicMappingCompiler{Tobject}"/> instance.
        /// </summary>
        /// <param name="mapping">The mapping configuration.</param>
        public DynamicMappingCompiler(DynamicMappingDto mapping)
        {
            _mapping = mapping;
        }

        #endregion Constructors

        #region Methods (Public)

        /// <summary>
        /// Compile a dynamic mapping method given the received configuration.
        /// </summary>
        /// <returns>
        /// Success : The compiled dynamic mapping method is returned.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<Action<object>>> Compile() => Task.Run(() =>
        {
            try
            {
                _obj = Expression.Parameter(typeof(TObject), "obj");
                var methodExpression = CreateSubConditionsOrActionsExpression(_mapping.RootCondition);
                var lambdaExpression = Expression.Lambda<Action<TObject>>(methodExpression, new List<ParameterExpression> { _obj });
                var compiledAction = lambdaExpression.Compile();

                // TODO : Only for DEV purpose, remove Nuget afterwards
                if (EnvironmentHelper.IsEnvironment(EnvironmentEnum.Debug))
                {
                    var code = lambdaExpression.ToReadableString();
                    CrossCuttingServices.Logger?.LogDebug($"Generated code for dynamic mapping : {Environment.NewLine}{code}");
                }

                return new Result<Action<object>>(obj => compiledAction((TObject)obj));
            }
            catch (Exception exception)
            {
                CrossCuttingServices.Logger?.LogError(exception, exception.Message);
                return new Result<Action<object>>(exception);
            }
        });

        #endregion Methods (Public)

        #region Methods (Helpers - Conditions)

        /// <summary>
        /// Given a parent condition, either check some sub conditions or execute some actions.
        /// </summary>
        /// <param name="parentCondition">The parent condition to check for sub conditions or actions.</param>
        /// <returns>An expression with either the sub conditions check or actions execution.</returns>
        private Expression CreateSubConditionsOrActionsExpression(DynamicConditionDto parentCondition)
        {
            if (parentCondition.Actions.HasAnElement())
                return CreateExecuteActionsExpression(parentCondition.Actions);

            if (parentCondition.SubConditions.HasNoElements())
                throw new Exception($"The condition for the property {parentCondition.PropertyName} ({parentCondition.Expression}) contains no sub conditions or actions for the dynamic mapping {_mapping.Name}");

            var blockExpression = CreateSubConditionsBlock(parentCondition);
            return blockExpression;
        }

        /// <summary>
        /// Creates a whole sub conditions blocks.
        /// First it gets the property value and it checks the value with the values in the conditions :
        /// <code>
        /// var value = obj.Value;
        /// if (value == 1)
        /// {
        ///   ... executes some actions or nest some additional sub conditions
        ///   return;
        /// }
        /// if (value > 2)
        /// {
        ///   ... executes some actions or nest some additional sub conditions
        ///   return;
        /// }
        /// ... executes some actions or nest some additional sub conditions
        /// </code>
        /// </summary>
        /// <param name="parentCondition">The condition that owns the sub conditions</param>
        /// <returns>The block expression containing the sub conditions with actions or nested conditions.</returns>
        private Expression CreateSubConditionsBlock(DynamicConditionDto parentCondition)
        {
            var propertyName = parentCondition.SubConditions.First().PropertyName;
            var assignPropertyValueToVar = CreateAssignPropertyValueToVarExpression(propertyName, out var varPropertyValue);

            Expression returnTargetExpression = null;
            if (_returnTarget == null)
            {
                _returnTarget = Expression.Label(typeof(void));
                returnTargetExpression = Expression.Label(_returnTarget, Expression.Constant(1));
            }

            var blockExpressions = new List<Expression> { assignPropertyValueToVar };
            blockExpressions.AddRange(parentCondition.SubConditions.Select(c => CreateConditionIfExpression(c, varPropertyValue)));
            if (returnTargetExpression != null)
                blockExpressions.Add(returnTargetExpression);

            var blockExpression = Expression.Block(new[] { varPropertyValue }, blockExpressions);
            return blockExpression;
        }

        /// <summary>
        /// Creates an expression that assigns the value of the object property given its name and casts it to the correct type into a variable :
        /// <code>var value = (decimal?)obj.Value;</code>
        /// This is done for performance purpose to avoid to cast get the value of the property at each condition comparison.
        /// </summary>
        /// <remarks>
        /// Beware ! Due to a strange comparison behavior in Expression when a string property is not interned, it is force interned when string :
        /// <code>var value = string.Intern((string)obj.Value);</code>
        /// </remarks>
        /// <param name="propertyName">The name of the property to get the value from.</param>
        /// <param name="varPropertyValue">The parameter expression that defines the variable that holds the property value.</param>
        /// <returns>The expression that assign the property value to a variable.</returns>
        private Expression CreateAssignPropertyValueToVarExpression(string propertyName, out ParameterExpression varPropertyValue)
        {
            var propertyInfo = typeof(TObject).GetProperty(propertyName);
            if (propertyInfo?.GetMethod == null)
                throw new Exception($"The condition with the property {propertyName} does not find any real property on type {typeof(TObject)} for the dynamic mapping {_mapping.Name} or the Get method of the property is missing");

            var getObjPropertyValue = Expression.Call(_obj, propertyInfo.GetMethod);
            varPropertyValue = Expression.Parameter(propertyInfo.PropertyType, propertyName.FirstLetterToLower());

            if (propertyInfo.PropertyType == typeof(string))
            {
                var internMethod = typeof(string).GetMethod("Intern", BindingFlags.Static | BindingFlags.Public);
                // ReSharper disable once AssignNullToNotNullAttribute
                var intern = Expression.Call(null, internMethod, getObjPropertyValue);
                return Expression.Assign(varPropertyValue, intern);
            }

            return Expression.Assign(varPropertyValue, getObjPropertyValue);
        }

        /// <summary>
        /// Create the property value comparison depending on the condition within an IF expression.
        /// The comparison can be one of the following types depending on the comparison string received :
        /// <code><![CDATA[
        /// Value or "Value" or =Value or ="Value" ==> propertyValue == "Value";
        /// !=Value or !="Value" or <>Value or <>"Value" ==> property != "Value";
        /// 123 or 123.45 or =123 or =123.45 ==> propertyValue == 123 or propertyValue == 123.45;
        /// !=123 or !=123.45 or <>123 or <>123.45 ==> property != 123 or propertyValue != 123.45;
        /// >123.45 or >=123.45 or <123 or <=123 ==> property >(or >=) 123.45 or property <(or <=) 123;
        /// @123;456 or @123.45;678.90 ==> (propertyValue >= 123) && (propertyValue <= 456) or (propertyValue >= 123.45) && (propertyValue <= 678.90);
        /// ]]></code>
        /// Then the comparison is included in this IF expression :
        /// <code><![CDATA[
        /// if (...comparison...)
        /// {
        ///   ... executes some actions or nest some additional sub conditions
        ///   return;
        /// }
        /// ]]></code>
        /// </summary>
        /// <param name="condition">The condition to create the IF comparison with.</param>
        /// <param name="varPropertyValue">The variable containing the property value.</param>
        /// <returns>The IF expression with the correct comparison and nested actions/sub conditions.</returns>
        private Expression CreateConditionIfExpression(DynamicConditionDto condition, ParameterExpression varPropertyValue)
        {
            var expression = condition.Expression.Trim();
            if (expression.IsNullOrEmpty() || expression == "-")
                return CreateSubConditionsOrActionsExpression(condition);

            var conditionalChars = new HashSet<char> { '@', '<', '>', '=', '!' };
            var prefix = new string(expression.TakeWhile(c => conditionalChars.Contains(c)).ToArray());
            var value = expression.Contains('"')
                ? (object)expression.Trim(' ', '"')
                : expression.Substring(prefix.Length).Trim().ToDecimal();

            BinaryExpression conditionExpression;
            switch (prefix)
            {
                case "@":
                    var from = expression.SubstringUntil(';').TrimStart('@').ToDecimal();
                    var to = expression.SubstringFrom(';').ToDecimal();

                    var greaterThanFrom = Expression.GreaterThanOrEqual(Expression.Convert(varPropertyValue, typeof(decimal)), Expression.Constant(from));
                    var lessThanTo = Expression.LessThanOrEqual(Expression.Convert(varPropertyValue, typeof(decimal)), Expression.Constant(to));
                    conditionExpression = Expression.And(greaterThanFrom, lessThanTo);
                    break;

                case "<=": conditionExpression = Expression.LessThanOrEqual(Expression.Convert(varPropertyValue, typeof(decimal)), Expression.Constant(value)); break;
                case ">=": conditionExpression = Expression.GreaterThanOrEqual(Expression.Convert(varPropertyValue, typeof(decimal)), Expression.Constant(value)); break;
                case "<>":
                case "!=":
                    conditionExpression = value is string
                    ? Expression.NotEqual(Expression.Convert(varPropertyValue, typeof(decimal)), Expression.Constant(value))
                    : Expression.NotEqual(varPropertyValue, Expression.Constant(value));
                    break;
                case "<": conditionExpression = Expression.LessThan(Expression.Convert(varPropertyValue, typeof(decimal)), Expression.Constant(value)); break;
                case ">": conditionExpression = Expression.GreaterThan(Expression.Convert(varPropertyValue, typeof(decimal)), Expression.Constant(value)); break;
                default:
                    conditionExpression = Expression.Equal(varPropertyValue, Expression.Constant(value));
                    break;
            }

            var ifThen = Expression.IfThen(conditionExpression, CreateSubConditionsOrActionsExpression(condition));
            return ifThen;
        }

        #endregion Methods (Helpers - Conditions)

        #region Methods (Helpers - Actions)

        /// <summary>
        /// Execute some actions sequentially and return to exit the method execution.
        /// </summary>
        /// <param name="actions">The actions to execute sequentially.</param>
        /// <returns>A block expression with all the actions execution and the return.</returns>
        private Expression CreateExecuteActionsExpression(IEnumerable<DynamicActionDto> actions)
        {
            var actionsExpressions = actions.Select(CreateActionExpression).IfNotNull().ToList();
            actionsExpressions.Add(Expression.Return(_returnTarget));
            return Expression.Block(actionsExpressions);
        }

        /// <summary>
        /// Factory.
        /// Depending on the action type, execute the action.
        /// </summary>
        /// <param name="action">The actions to execute.</param>
        /// <returns>The expression with the action execution code.</returns>
        private Expression CreateActionExpression(DynamicActionDto action)
        {
            switch (action.Type)
            {
                case DynamicActionTypeEnum.SetValue: return CreateActionSetValueExpression(action);
                case DynamicActionTypeEnum.ExecuteMapping: return CreateActionExecuteMappingExpression(action);
                default:
                    CrossCuttingServices.Logger?.LogError($"The action type {action.Type} has not been found for dynamic mapping {_mapping.Name}");
                    return Expression.Empty();
            }
        }

        /// <summary>
        /// The action used to set a value expression to a property of the given object.
        /// </summary>
        /// <param name="action">The action to execute with the expression to set in the property of the object.</param>
        /// <returns>The expression with the property setter.</returns>
        private Expression CreateActionSetValueExpression(DynamicActionDto action)
        {
            var propertyInfo = typeof(TObject).GetProperty(action.PropertyName);
            if (propertyInfo?.SetMethod == null)
                throw new Exception($"The property {action.PropertyName} has not been found for dynamic mapping {_mapping.Name}, object type : {typeof(TObject)} or has no Set method defined.");

            var valueExpression = ComputeValueExpression(action.Expression);
            if (valueExpression == null)
                return null;

            var setObjectPropertyExpression = Expression.Call(_obj, propertyInfo.SetMethod, valueExpression);
            return setObjectPropertyExpression;
        }

        /// <summary>
        /// The action used to execute another dynamic mapping in sequence just after this one.
        /// </summary>
        /// <param name="action">The action to execute with the name of the dynamic mapping to execute in the expression.</param>
        /// <returns>The expression with the method call.</returns>
        private Expression CreateActionExecuteMappingExpression(DynamicActionDto action)
        {
            // TODO

            return Expression.Empty();
        }

        /// <summary>
        /// Compute a value given a logical expression.
        /// The syntax to use allows :
        /// - property name to be literally set
        /// - operators : + - * / ^ %
        /// - brackets () to group operations
        /// - constants
        /// </summary>
        /// <param name="expression">The expression to compute in the syntax.</param>
        /// <returns>The computing expression.</returns>
        private Expression ComputeValueExpression(string expression)
        {
            expression = expression.Trim().TrimStart('=');
            if (expression.IsNullOrWhiteSpace())
                return null;

            if (expression.StartsWith('"'))
                return Expression.Constant(expression.Trim('"'));

            if (decimal.TryParse(expression, NumberStyles.Any, CultureInfo.InvariantCulture, out var valueInDecimal))
                return Expression.Constant(valueInDecimal);

            var propertyInfo = typeof(TObject).GetProperty(expression);
            if (propertyInfo?.GetMethod != null)
                return Expression.Call(_obj, propertyInfo.GetMethod);

            throw new Exception($"The function {expression} is not valid.");
        }

        #endregion Methods (Helpers - Actions)
    }
}