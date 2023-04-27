using Natom.ATSA.Colegio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace Natom.ATSA.Colegio.Managers
{
    public class CarrerasCursosManager
    {
        DbColegioContext db = new DbColegioContext();

        public int ObtenerCantidadRegistros()
        {
            return db.CarrerasCursos.Count();
        }

        public IEnumerable<CarreraCurso> ObtenerCarreraCurso(string search, int? filtrotipo, int? filtrocursada)
        {
            IEnumerable<CarreraCurso> query = this.db.CarrerasCursos.Include(ca => ca.TipoDuracion).Where(x => x.Anulado == false);
            if (!string.IsNullOrEmpty(search))
            {
                int n;
                if (int.TryParse(search, out n))
                {
                    query = query.Where(q => q.Duracion == n);
                }
                else
                {
                    search = search.ToLower();
                    query = query.Where(q => q.Descripcion.ToLower().Contains(search)
                                        || q.Titulo.ToLower().Contains(search)
                                        || q.Horarios.ToLower().Contains(search));
                }

            }

            if (filtrotipo > 0)
            {
                query = query.Where(x => x.CarreraCursoTipoId == filtrotipo);
            }

            if (filtrocursada > 0)
            {
                query = query.Where(x => x.TipoDuracionId == filtrocursada);
            }

            return query;
        }

        public CarreraCurso GetCarreraCurso(int CarreraCursoId)
        {
            return db.CarrerasCursos.Include(x => x.Requisitos).FirstOrDefault(x => x.CarreraCursoId == CarreraCursoId);
        }

        public List<CarreraCursoRequisito> GetCarreraCursoRequisito(int CarreraCursoId)
        {
            return db.CarrerasCursosRequisitos.Where(x => x.CarreraCursoId == CarreraCursoId).ToList();
        }

        public CarreraCurso GrabarCarreraCurso(CarreraCurso carreracurso)
        {
            if (carreracurso.CarreraCursoId == 0)
            {
                if (this.db.CarrerasCursos.Any(ca => !ca.Anulado && ca.Titulo.ToUpper().Equals(carreracurso.Titulo.ToUpper())))
                {
                    throw new Exception("Ya existe una Carrera / Curso con el mismo título.");
                }
                db.CarrerasCursos.Add(carreracurso);
            }
            else
            {
                if (this.db.CarrerasCursos.Any(ca => !ca.CarreraCursoId.Equals(carreracurso.CarreraCursoId) && !ca.Anulado && ca.Titulo.ToUpper().Equals(carreracurso.Titulo.ToUpper())))
                {
                    throw new Exception("Ya existe una Carrera / Curso con el mismo título.");
                }

                List<CarreraCursoRequisito> requisitosdb = db.CarrerasCursosRequisitos.Where(x => x.CarreraCursoId == carreracurso.CarreraCursoId).ToList();
                foreach (var r in requisitosdb)
                {
                    db.CarrerasCursosRequisitos.Remove(r);
                }
                CarreraCurso carreracursodb = this.db.CarrerasCursos.Find(carreracurso.CarreraCursoId);
                this.db.Entry(carreracursodb).State = EntityState.Modified;
                carreracursodb.CarreraCursoTipoId = carreracurso.CarreraCursoTipoId;
                carreracursodb.Descripcion = carreracurso.Descripcion;
                carreracursodb.Duracion = carreracurso.Duracion;
                carreracursodb.Horarios = carreracurso.Horarios;
                carreracursodb.TipoDuracionId = carreracurso.TipoDuracionId;
                carreracursodb.Titulo = carreracurso.Titulo;

                foreach (var r in carreracurso.Requisitos)
                    this.db.CarrerasCursosRequisitos.Add(r);
            }

            db.SaveChanges();

            return carreracurso;
        }

        public void EliminarCarreraCurso(int CarreraCursoId)
        {
            var e = db.CarrerasCursos.FirstOrDefault(x => x.CarreraCursoId == CarreraCursoId);
            e.Anulado = true;
            db.Entry<CarreraCurso>(e).State = EntityState.Modified;
            db.SaveChanges();
        }

        public List<CarreraCursoTipo> GetCarreraCursoTipos()
        {
            return db.CarrerasCursosTipos.ToList();
        }

        public List<TipoDuracion> GetTipoDuracion()
        {
            return db.TiposDuracion.ToList();
        }

        public List<CarreraCurso> GetCarreraCursos()
        {
            return db.CarrerasCursos.Where(x=>x.Anulado == false).ToList();
        }

    }
}