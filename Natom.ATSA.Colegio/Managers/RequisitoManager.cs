using Natom.ATSA.Colegio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Managers
{
    public class RequisitoManager
    {
        DbColegioContext db = new DbColegioContext();

        public IEnumerable<Requisito> ObtenerRequisitos(string search)
        {
            IEnumerable<Requisito> query = this.db.Requisitos.Where(x => x.Anulado == false);
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(q => q.Descripcion.ToLower().Contains(search));
            }
            return query;
        }

        public int ObtenerCantidadRegistros()
        {
            return db.Requisitos.Where(x => x.Anulado == false).Count();
        }

        public void EliminarRequisito(int requisitoid)
        {
            var r = db.Requisitos.FirstOrDefault(x => x.RequisitoId == requisitoid);
            r.Anulado = true;

            db.Entry<Requisito>(r).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public void EditarRequisito(Requisito requisito)
        {
            var r = db.Requisitos.FirstOrDefault(x => x.RequisitoId == requisito.RequisitoId);
            r.Descripcion = requisito.Descripcion;
            r.Anulado = false;

            db.Entry<Requisito>(r).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public Requisito CrearRequisito(Requisito requisito)
        {
            db.Requisitos.Add(requisito);
            db.SaveChanges();

            return requisito;
        }

        public Requisito GetRequisito(int requisitoid)
        {
            return db.Requisitos.FirstOrDefault(x => x.RequisitoId == requisitoid);
        }

        public List<Requisito> GetRequisitos()
        {
            return db.Requisitos.Where(x => x.Anulado == false).ToList();
        }
    }
}