using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace DataAccess.Context
{
    public partial class MiltronDbContext : DbContext
    {

        protected readonly IConfiguration Configuration;

        //public MiraDbContext(DbContextOptions<MiraDbContext> options) : base(options)
        //{
        //}
        public MiltronDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {            
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<User> Users { get; set; }        

    }
}
