using Microsoft.EntityFrameworkCore;

namespace TicketProject.DAL
{
    public partial class WriteTicketDbContext : DbContext
    {
        /*
        需要再Write加入這個建構式 讓繼承自Write的Read能夠使用這個建構是存取到DbContextOptions
        且因DbContext是由EfCore下指令生成 所以使用部分類撰寫這個建構式
        而這個options用於在注入時設定DbContext的各個屬性 例如連線字串
        */
        protected WriteTicketDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
