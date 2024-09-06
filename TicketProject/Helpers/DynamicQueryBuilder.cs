using System.Linq.Expressions;
using System.Reflection;

namespace TicketProject.Helpers
{
    /// <summary>
    /// 用於建立動態查詢過濾器。
    /// </summary>
    /// <typeparam name="T">實體的類型。</typeparam>
    public static class DynamicQueryBuilder<T> where T : class
    {
        /// <summary>
        /// 使用指定的條件，將兩個過濾表達式結合起來建立過濾器表達式。
        /// </summary>
        /// <param name="filter">原始的過濾表達式。</param>
        /// <param name="newFilter">要與原始過濾器結合的新過濾表達式。</param>
        /// <param name="condition">用於結合過濾表達式的條件，預設為 "And"，可替換為"Or"</param>
        /// <returns>結合後的過濾器表達式。</returns>
        public static Expression<Func<T, bool>> BuildFilter(Expression<Func<T, bool>> filter, Expression<Func<T, bool>> newFilter, string condition = "And")
        {
            // 取得原始過濾表達式的參數
            var parameter = filter.Parameters[0];

            // LINQ無法解析閉包變量，因此需要將其轉換為常量表達式
            // 替換原始過濾表達式的閉包變量為常量，和過濾表達式中的參數用於避免表達式中的參數不同的防呆措施
            // 理論上左側不需進行替換，但若預設指派x=>true 會導致ParameterReplacer類的VisitMember和VisitParameter方法跳過導致類型不同的錯誤
            var left = ReplaceParameter(filter.Body, filter.Parameters[0], parameter);
            var right = ReplaceParameter(newFilter.Body, newFilter.Parameters[0], parameter);

            // 根據指定的條件結合兩個過濾表達式
            Expression combinedFilter;
            if (condition == "Or")
                combinedFilter = Expression.OrElse(left, right);
            else
                combinedFilter = Expression.AndAlso(left, right);

            // 返回結合後的過濾器表達式
            return Expression.Lambda<Func<T, bool>>(combinedFilter, filter.Parameters);
        }

        /// <summary>
        /// 替換表達式中的參數。
        /// </summary>
        /// <param name="expression">要替換參數的表達式。</param>
        /// <param name="oldParameter">舊的參數。</param>
        /// <param name="newParameter">新的參數。</param>
        /// <returns>替換參數後的表達式。</returns>
        private static Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
        }

        /// <summary>
        /// 用於替換表達式中的參數的訪問器。
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            /// <summary>
            /// 初始化 <see cref="ParameterReplacer"/> 類的新實例。
            /// </summary>
            /// <param name="oldParameter">舊的參數。</param>
            /// <param name="newParameter">新的參數。</param>
            public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            /// <summary>
            /// 訪問參數節點，並將舊參數替換為新參數。
            /// </summary>
            /// <param name="node">參數節點。</param>
            /// <returns>替換後的參數節點。</returns>
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }

            /// <summary>
            /// 訪問成員節點，並將閉包變量轉換為常量表達式。
            /// </summary>
            /// <param name="node">成員節點。</param>
            /// <returns>轉換後的成員節點。</returns>
            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression is ConstantExpression constantExpression)
                {
                    var value = ((FieldInfo)node.Member).GetValue(constantExpression.Value);
                    return Expression.Constant(value, node.Type);
                }
                return base.VisitMember(node);
            }
        }
    }
}
