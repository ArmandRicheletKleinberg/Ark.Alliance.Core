using System.Linq.Expressions;

namespace Ark
{
    /// <summary>
    /// This class extends the Expression class.
    /// </summary>
    public static class ExpressionExtensibility
    {
        #region Methods (Public)

        /// <summary>
        /// Gets the first member or method name of an expression.
        /// It could be used in property expression to find a name from code.
        /// </summary>
        /// <remarks>BEWARE ! semi recursive calls until finding a member or method expression.</remarks>
        /// <param name="expression">The expression to search for member or method name.</param>
        /// <returns>The member or method name if any found, null otherwise.</returns>
        public static string GetFirstMemberOrMethodName(this Expression expression)
        {
            while (true)
            {
                switch (expression)
                {
                    case MemberExpression memberExpression: return memberExpression.Member.Name;
                    case MethodCallExpression methodCallExpression: return methodCallExpression.Method.Name;
                    case LambdaExpression lambdaExpression: expression = lambdaExpression.Body; continue;
                    case UnaryExpression unaryExpression: expression = unaryExpression.Operand; continue;
                    case BinaryExpression binaryExpression:
                        var firstMemberOrMethodName = binaryExpression.Left.GetFirstMemberOrMethodName();
                        if (firstMemberOrMethodName != null)
                            return firstMemberOrMethodName;
                        expression = binaryExpression.Right;
                        continue;
                }

                return null;
            }
        }

        #endregion Methods (Public)
    }
}