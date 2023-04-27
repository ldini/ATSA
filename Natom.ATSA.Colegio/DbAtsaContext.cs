using Natom.ATSA.Colegio.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio
{
    public class DbAtsaContext : DbContext
    {
        public DbSet<Persona> Personas { get; set; }
        
        public DbAtsaContext()
            : base("name=DbAtsaContext")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}