﻿using System.Security.Cryptography;
using System.Text;

namespace TicketProject.Helpers
{
    /// <summary>  
    /// 提供雜湊功能的輔助類別。  
    /// </summary>  
    public static class HashHelper
    {
        /// <summary>  
        /// 使用 SHA256 演算法對密碼進行雜湊。  
        /// </summary>  
        /// <param name="password">要雜湊的密碼。</param>  
        /// <returns>雜湊後的密碼字串。</returns>  
        /// <exception cref="Exception">當雜湊過程中發生錯誤時拋出。</exception>  
        public static async Task<string> HashPassword(string password)
        {
            try
            {
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(password));
                var bytes = await SHA256.HashDataAsync(stream);
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}