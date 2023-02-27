using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;
using System.Configuration;

namespace PTMK
{
    public class TestTaskContext : DbContext
    {

        public DbSet<Human>? Humans { get; set; }
        
        public TestTaskContext() : base(ConfigurationManager.ConnectionStrings["SQLExpress"].ConnectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
