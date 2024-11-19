using TicketProject.Services.Interfaces;

namespace TicketProject.Services.Implement
{
    /// <summary>  
    /// 錯誤處理類別，用於處理和記錄錯誤。  
    /// </summary>  
    /// <typeparam name="T">處理錯誤的類型。</typeparam>  
    public class ErrorHandler<T> : IErrorHandler<T>
    {
        private readonly ILogger<T> _logger;

        /// <summary>  
        /// 初始化 ErrorHandler 類別的新執行個體。  
        /// </summary>  
        /// <param name="logger">日誌記錄器。</param>  
        public ErrorHandler(ILogger<T> logger)
        {
            _logger = logger;
        }

        /// <summary>  
        /// 處理錯誤的方法。  
        /// </summary>  
        /// <param name="e">要處理的例外狀況。</param>  
        public void HandleError(Exception e)
        {
            if (e.Data["ErrorSource"] == null)
            {
                e.Data["ErrorSource"] = typeof(T).FullName!;
                _logger.LogError(e.ToString());
            }
        }

        public void Debug(string message)
        {
            _logger.LogDebug(message);
        }
    }
}
