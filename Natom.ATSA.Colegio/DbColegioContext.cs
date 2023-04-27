using Natom.ATSA.Colegio.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio
{
    public class DbColegioContext : DbContext
    {
        public DbSet<MesAnio> MesAnio { get; set; }
        public DbSet<CarreraCurso> CarrerasCursos { get; set; }
        public DbSet<CarreraCursoRequisito> CarrerasCursosRequisitos { get; set; }
        public DbSet<CarreraCursoTipo> CarrerasCursosTipos { get; set; }
        public DbSet<CicloLectivo> CiclosLectivos { get; set; }
        public DbSet<Cobranza> Cobranzas { get; set; }
        public DbSet<CobranzaPagoAdelantado> CobranzasPagoAdelantado { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<InscripcionEntregaDocumentacion> InscripcionEntregasDocumentacion { get; set; }
        public DbSet<InscripcionCarreraCursoRequisito> InscripcionesCarreraCursoRequisito { get; set; }
        public DbSet<Requisito> Requisitos { get; set; }
        public DbSet<TipoDuracion> TiposDuracion { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public DbColegioContext()
            : base("name=DbColegioContext")
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