using Natom.ATSA.Colegio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Natom.ATSA.Colegio.Models.Result;

namespace Natom.ATSA.Colegio.Managers
{
    public class InscripcionesManager
    {
        DbColegioContext db = new DbColegioContext();

        public IEnumerable<InscripcionesQueDebenDocumentacionResult> ObtenerInscripcionesQueDebenDocumentacion()
        {
            return this.db.Database.SqlQuery<InscripcionesQueDebenDocumentacionResult>("SELECT * FROM vwInscripcionesQueDebenDocumentacion");
        }

        public IEnumerable<Inscripcion> ObtenerInscripciones(string search, int? filtrocarreracurso, int? filtrociclolectivo)
        {
            IEnumerable<Inscripcion> query = this.db.Inscripciones
                                                    .Include("CarreraCurso")
                                                    .Include("CarreraCurso.CarreraCursoTipo")
                                                    .Include("CarreraCurso.TipoDuracion")
                                                    .Include("CicloLectivo");
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(q => q.Nombre.ToLower().Contains(search)
                                    || q.Apellido.ToLower().Contains(search)
                                    || (q.Afiliado != null && q.Afiliado.ToLower().Contains(search))
                                    || q.DNI.ToLower().Contains(search)
                                    || q.CarreraCurso.Titulo.ToLower().Contains(search)
                                    //|| q.CicloLectivo.Descripcion.ToLower().Contains(search) //Comment
                                    );
            }

            if (filtrocarreracurso > 0)
            {
                query = query.Where(x => x.CarreraCursoId == filtrocarreracurso);
            }

            if (filtrociclolectivo != null)                
            {
                Func<string, int> ingreso = s => {
                    int i;
                    return int.TryParse(s, out i) ? i : 0;
                };
                if (filtrociclolectivo > 0)
                    query = query.Where(i =>
                                                i.CarreraCurso.TipoDuracionId == 2
                                                    ? (DateTime.Now.Year - i.AltaFecha.Value.Year + ingreso(i.CicloIngreso)) + 1 == filtrociclolectivo
                                                    : filtrociclolectivo == 1 && new DateTime(i.AltaFecha.Value.Year, 2, 1).AddMonths(i.CarreraCurso.Duracion + 1).AddDays(-1) >= DateTime.Now.Date
                                        );
                else
                    query = query.Where(i => i.CarreraCurso.TipoDuracionId == 2
                                                    ? (DateTime.Now.Year - i.AltaFecha.Value.Year + ingreso(i.CicloIngreso)) + 1 > i.CarreraCurso.Duracion
                                                    : new DateTime(i.AltaFecha.Value.Year, 2, 1).AddMonths(i.CarreraCurso.Duracion + 1).AddDays(-1) < DateTime.Now.Date
                                        );
            }

            return query;
        }

        public Inscripcion ObtenerInscripcion(int inscripcionId)
        {
            return this.db.Inscripciones
                            .Include(i => i.CarreraCurso)
                            .Include(i => i.CicloLectivo)
                            .FirstOrDefault(i => i.InscripcionId == inscripcionId);
        }

        public int ObtenerCantidadInscripciones()
        {
            return db.Inscripciones.Count();
        }

        public void DarBajaInscripcion(Inscripcion Inscripcion)
        {
            var r = db.Inscripciones.FirstOrDefault(x => x.InscripcionId == Inscripcion.InscripcionId);
            r.BajaMotivo = Inscripcion.BajaMotivo;
            r.BajaCarreraFecha = DateTime.Now;

            db.Entry<Inscripcion>(r).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public void EditarInscripcion(Inscripcion Inscripcion)
        {
            var r = db.Inscripciones.Include(i => i.RequisitosEntregados).FirstOrDefault(x => x.InscripcionId == Inscripcion.InscripcionId);

            this.db.InscripcionEntregasDocumentacion.RemoveRange(r.RequisitosEntregados);

            r.Nombre = Inscripcion.Nombre;
            r.CarreraCursoId = Inscripcion.CarreraCursoId;
            r.CicloLectivoId = Inscripcion.CicloLectivoId; //Comment
            //r.CicloLectivoId = (int?)null; // Inscripcion.CicloLectivoId; //Comment
            r.Apellido = Inscripcion.Apellido;
            r.DNI = Inscripcion.DNI;
            r.Telefono = Inscripcion.Telefono;
            r.Email = Inscripcion.Email;
            r.Edad = Inscripcion.Edad;
            r.FechaNacimiento = Inscripcion.FechaNacimiento;
            r.LugarNacimiento = Inscripcion.LugarNacimiento;
            r.Direccion = Inscripcion.Direccion;
            r.Localidad = Inscripcion.Localidad;
            r.Afiliado = Inscripcion.Afiliado;
            r.Recibo = Inscripcion.Recibo;
            r.Sexo = Inscripcion.Sexo;
            r.EstadoCivil = Inscripcion.EstadoCivil;
            r.TrabajoAntiguedad = Inscripcion.TrabajoAntiguedad;
            r.TrabajoCargoFuncion = Inscripcion.TrabajoCargoFuncion;
            r.TrabajoDependencia = Inscripcion.TrabajoDependencia;
            r.TrabajoDireccionLaboral = Inscripcion.TrabajoDireccionLaboral;
            r.TrabajoEmail = Inscripcion.TrabajoEmail;
            r.TrabajoLocalidad = Inscripcion.TrabajoLocalidad;
            r.TrabajoRegionSanitaria = Inscripcion.TrabajoRegionSanitaria;
            r.TrabajoTelefono = Inscripcion.TrabajoTelefono;
            r.CicloIngreso = Inscripcion.CicloIngreso;

            db.Entry<Inscripcion>(r).State = System.Data.Entity.EntityState.Modified;

            if (Inscripcion.RequisitosEntregados != null)
                foreach (var entregado in Inscripcion.RequisitosEntregados)
                {
                    r.RequisitosEntregados.Add(new InscripcionEntregaDocumentacion()
                    {
                        RequisitoId = entregado.RequisitoId
                    });
                }

            db.SaveChanges();
        }

        public Inscripcion CrearInscripcion(Inscripcion Inscripcion)
        {
            if (this.db.Inscripciones.Any(i => !i.BajaCarreraFecha.HasValue
                                                && i.DNI.Equals(Inscripcion.DNI)
                                                && (i.AltaFecha.HasValue 
                                                && i.AltaFecha.Value.Year <= (i.CarreraCurso.TipoDuracionId == 2 ? (i.AltaFecha.Value.Year + i.CarreraCurso.Duracion - 1) : i.AltaFecha.Value.Year)) 
                                                //&& i.CicloLectivoId.Equals(Inscripcion.CicloLectivoId) 
                                                && i.CicloIngreso.Equals(Inscripcion.CicloIngreso) //Comment
                                                && i.CarreraCursoId.Equals(Inscripcion.CarreraCursoId)))
            {
                throw new Exception("Ya existe una inscripción vigente de la misma persona para la misma carrera / curso.");
            }
            
            Inscripcion.AltaFecha = DateTime.Now;

            db.Inscripciones.Add(Inscripcion);
            db.SaveChanges();

            return Inscripcion;
        }

        public List<Requisito> ObtenerRequisitosPorCarreraCurso(int carreraCursoId)
        {
            return this.db.CarrerasCursosRequisitos
                            .Include(c => c.Requisito)
                            .Where(c => c.CarreraCursoId == carreraCursoId)
                            .Select(c => c.Requisito)
                            .Where(c => !c.Anulado)
                            .ToList();
        }

        public CicloLectivo ObtenerCicloLectivoDeInscripcion(int inscripcionId)
        {
            return this.db.Inscripciones
                            .Include(i => i.CicloLectivo)
                            .Where(i => i.InscripcionId == inscripcionId)
                            .Select(i => i.CicloLectivo)
                            .FirstOrDefault();
        }

        public IEnumerable<ObtenerRequisitosPendientesDeEntregarResult> ObtenerRequisitosPendientesDeEntregar(int? inscripcionId)
        {
            return this.db.Database.SqlQuery<ObtenerRequisitosPendientesDeEntregarResult>("CALL ObtenerRequisitosPendientesDeEntregar({0})", inscripcionId);
        }

        public Inscripcion GetInscripcion(int InscripcionId)
        {
            return db.Inscripciones
                .Include("CarreraCurso")
                .Include("CarreraCurso.CarreraCursoTipo")
                .Include("CarreraCurso.TipoDuracion")
                .Include("CicloLectivo")
                .Include("RequisitosEntregados")
                .FirstOrDefault(x => x.InscripcionId == InscripcionId);
        }

        public List<Inscripcion> GetInscripciones()
        {
            return db.Inscripciones
                .Include("CarreraCurso")
                .Include("CarreraCurso.TipoDuracion")
                .Include("CicloLectivo")
                .ToList();
        }

        public void RegistrarEntregaDeDocumentacion(int inscripcionId, List<int> requisitosId)
        {
            foreach (var id in requisitosId)
            {
                this.db.InscripcionEntregasDocumentacion.Add(new InscripcionEntregaDocumentacion()
                {
                    FechaHora = DateTime.Now,
                    InscripcionId = inscripcionId,
                    RequisitoId = id
                });
            }
            this.db.SaveChanges();
        }
    }
}