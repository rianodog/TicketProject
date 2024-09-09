using System.Linq.Expressions;

namespace TicketProject.Services.Interfaces
{
    public interface IDynamicQueryBuilderService<T> where T : class
    {
        /// <summary>
        /// 使用指定的條件，將兩個過濾表達式結合起來建立過濾器表達式。
        /// </summary>
        /// <param name="filter">原始的過濾表達式。</param>
        /// <param name="newFilter">要與原始過濾器結合的新過濾表達式。</param>
        /// <param name="condition">用於結合過濾表達式的條件，預設為 "And"，可替換為"Or"</param>
        /// <returns>結合後的過濾器表達式。</returns>
        Expression<Func<T, bool>> BuildFilter(Expression<Func<T, bool>> filter, Expression<Func<T, bool>> newFilter, string condition = "And");
    }
}
