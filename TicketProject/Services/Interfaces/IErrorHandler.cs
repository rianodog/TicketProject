namespace TicketProject.Services.Interfaces
{
    /// <summary>
    /// 定義錯誤處理介面。
    /// </summary>
    /// <typeparam name="T">處理錯誤的類型。</typeparam>
    public interface IErrorHandler<T>
    {
        void Debug(string message);

        /// <summary>
        /// 處理錯誤的方法。
        /// </summary>
        /// <param name="e">要處理的例外狀況。</param>
        void HandleError(Exception e);
    }
}
