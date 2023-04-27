using Natom.ATSA.Colegio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Managers
{
    public class GastosManager
    {
        DbColegioContext db = new DbColegioContext();

        public IEnumerable<Gasto> ObtenerGastos(string search)
        {
            IEnumerable<Gasto> query = this.db.Gastos.Where(x => x.Anulado == false);
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(q => q.Concepto.ToLower().Contains(search));
            }
            return query;
        }

        public int ObtenerCantidadGastos()
        {
            return db.Gastos.Where(x => x.Anulado == false).Count();
        }

        public void EliminarGasto(int gastoid)
        {
            var r = db.Gastos.FirstOrDefault(x => x.GastoId == gastoid);
            r.Anulado = true;

            db.Entry<Gasto>(r).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public void EditarGasto(Gasto gasto)
        {
            var r = db.Gastos.FirstOrDefault(x => x.GastoId == gasto.GastoId);
            r.Concepto = gasto.Concepto;
            r.Monto = gasto.Monto;
            r.Anulado = false;

            db.Entry<Gasto>(r).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public Gasto CrearGasto(Gasto gasto)
        {
            db.Gastos.Add(gasto);
            db.SaveChanges();

            return gasto;
        }

        public Gasto GetGasto(int gastoid)
        {
            return db.Gastos.FirstOrDefault(x => x.GastoId == gastoid);
        }

        public List<Gasto> GetGastos()
        {
            return db.Gastos.Where(x => x.Anulado == false).ToList();
        }
    }
}