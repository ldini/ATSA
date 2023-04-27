using Natom.ATSA.Colegio.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
namespace Natom.ATSA.Colegio.Managers
{
    public class MovimientosManager
    {
        private DbColegioContext db = new DbColegioContext();

        public IEnumerable<ListarMovimientosResult> ListarMovimientos(string search, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            IEnumerable<ListarMovimientosResult> query = this.db.Database.SqlQuery<ListarMovimientosResult>("CALL ListarMovimientos({0}, {1})", fechaDesde, fechaHasta);
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(q => q.Descripcion.ToLower().Contains(search));
            }
            return query;

        }
    }
}