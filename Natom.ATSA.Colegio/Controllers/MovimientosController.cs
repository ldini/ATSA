using Natom.ATSA.Colegio.Managers;
using Natom.ATSA.Colegio.Models.DataTable;
using Natom.ATSA.Colegio.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Colegio.Controllers
{
    public class MovimientosController : BaseController
    {
        private MovimientosManager manager;

        public MovimientosController()
        {
            this.manager = new MovimientosManager();
        }

        // GET: Cobranzas
        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);
            return View();
        }

        public ActionResult ObtenerListadoIndex(JQueryDataTableParamModel param, string desde = null, string hasta = null)
        {
            DateTime dt;
            DateTime? fechaDesde = null;
            DateTime? fechaHasta = null;

            if (DateTime.TryParse(desde, out dt)) fechaDesde = dt;
            if (DateTime.TryParse(hasta, out dt)) fechaHasta = dt;

            DataTableParams dtParams = new DataTableParams(Request);
            int cantidadRegistros = this.manager.ListarMovimientos(null, null, null).Count();

            IEnumerable<ListarMovimientosResult> cargasFiltradas = this.manager.ListarMovimientos(dtParams.Search, fechaDesde, fechaHasta);

            Func<ListarMovimientosResult, string> orderingFunction =
                (c => dtParams.SortByColumnIndex == 0 ? c.FechaHora.ToOADate().ToString().PadLeft(20, '0') :
                dtParams.SortByColumnIndex == 1 ? c.Descripcion :
                dtParams.SortByColumnIndex == 2 ? c.Monto.ToString().PadLeft(8, '0') :
                "");

            if (dtParams.SortingDirection == eSortingDirection.ASC)
            {
                cargasFiltradas = cargasFiltradas.OrderBy(orderingFunction);
            }
            else
            {
                cargasFiltradas = cargasFiltradas.OrderByDescending(orderingFunction);
            }

            var sumaTotal = cargasFiltradas.Sum(c => c.Monto * c.Signo);

            List<ListarMovimientosResult> displayedCargas = cargasFiltradas
                .Skip(param.start).Take(param.length).ToList();

            var result = from c in displayedCargas
                         select new object[] {
                                        c.FechaHora.ToString("dd/MM/yyyy HH:mm") + " hs",
                                        c.Descripcion,
                                        "$ " + c.Monto.ToString(),
                                        sumaTotal
                            };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = cantidadRegistros,
                iTotalDisplayRecords = cargasFiltradas.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);

        }
    }
}