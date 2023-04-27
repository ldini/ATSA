using Natom.ATSA.Colegio.Managers;
using Natom.ATSA.Colegio.Models;
using Natom.ATSA.Colegio.Models.DataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Colegio.Controllers
{
    public class GastosController : BaseController
    {
        GastosManager manager = new GastosManager();

        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);

            return View();
        }

        public ActionResult ObtenerListadoIndex(JQueryDataTableParamModel param, string filtro = "")
        {
            DataTableParams dtParams = new DataTableParams(Request);
            int cantidadRegistros = this.manager.ObtenerCantidadGastos();

            IEnumerable<Gasto> cargasFiltradas = this.manager.ObtenerGastos(dtParams.Search);

            Func<Gasto, string> orderingFunction =
                (c => dtParams.SortByColumnIndex == 0 ? c.Concepto :
                dtParams.SortByColumnIndex == 1 ? c.Monto.ToString() :
                dtParams.SortByColumnIndex == 2 ? c.FechaHora.ToString() :
                dtParams.SortByColumnIndex == 3 ? c.GastoId.ToString().PadLeft(8, '0') :
                "");

            if (dtParams.SortingDirection == eSortingDirection.ASC)
            {
                cargasFiltradas = cargasFiltradas.OrderBy(orderingFunction);
            }
            else
            {
                cargasFiltradas = cargasFiltradas.OrderByDescending(orderingFunction);
            }

            List<Gasto> displayedCargas = cargasFiltradas
                .Skip(param.start).Take(param.length).ToList();


            var result = from c in displayedCargas
                         select new object[] {
                                c.Concepto,
                                c.Monto.ToString("C2"),
                                c.FechaHora.ToString("dd/MM/yyyy HH:mm") + " hs",
                                c.GastoId,
                                c.Tipo
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

        public ActionResult EliminarGasto(int gastoid)
        {
            this.manager.EliminarGasto(gastoid);

            return RedirectToAction("Index", "Gastos");
        }

        public ActionResult Crear_EditarGasto(Gasto model)
        {
            try
            {
                if (model.GastoId > 0)
                {
                    manager.EditarGasto(model);
                }
                else
                {
                    Gasto gasto = new Gasto()
                    {
                        Concepto = model.Concepto,
                        Monto = model.Monto,
                        FechaHora = DateTime.Now,
                        Tipo = model.Tipo,
                        Anulado = false
                    };

                    manager.CrearGasto(gasto);
                }

                return RedirectToAction("Index", "Gastos");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult AbrirModal(int? gastoid)
        {
            Gasto e = new Gasto();

            if (gastoid == 0)
            {
                e.GastoId = 0;
                e.Concepto = "";
                e.Monto = 0;
            }
            else
            {
                e = manager.GetGasto((int)gastoid);
            }

            return PartialView("_ModalGastos", e);
        }
    }
}