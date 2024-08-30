using Microsoft.EntityFrameworkCore;

namespace TicketProject.DAL;

public partial class ReadTicketDbContext : WriteTicketDbContext
{
    public ReadTicketDbContext()
    {
    }

    // 透過WriteTicketDbContext_Partial的建構式來存取DbContext類
    public ReadTicketDbContext(DbContextOptions options)
    : base(options)
    {
    }
}
