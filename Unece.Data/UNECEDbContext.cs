using Microsoft.EntityFrameworkCore;
using Unece.Data.Models;

namespace Unece.Data
{
    public class UNECEDbContext : DbContext
    {
        public UNECEDbContext(DbContextOptions<UNECEDbContext> options) : base(options)
        {

        }

        public DbSet<USR_Users> USR_Users { get; set; }
        public DbSet<DepartmentMaster> DepartmentMaster { get; set; }
        public DbSet<SiteMaster> SiteMaster { get; set; }
        public DbSet<ApplicationMaster> ApplicationMaster { get; set; }
        public DbSet<FieldTypeMaster> FieldTypeMaster { get; set; }
        public DbSet<FieldDetails> FieldDetails { get; set; }
        public DbSet<ModuleOne> Tbl_ModuleOne { get; set; }
        public DbSet<ModuleTwo> Tbl_ModuleTwo { get; set; }

    }
}
