using Microsoft.EntityFrameworkCore;

namespace MyTestApp.Model
{
    public class BankAccountContext:DbContext
    {
        public BankAccountContext(DbContextOptions<BankAccountContext> options) : base(options)
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; } = null!;
    }
}
