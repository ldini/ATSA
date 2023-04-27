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
    public class CiclosLectivosController : BaseController
    {
        CiclosLectivosManager manager = new CiclosLectivosManager();

        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);
            return View();
        }

        public ActionResult ObtenerListadoIndex(JQueryDataTableParamModel param, string filtro = "")
        {
            DataTableParams dtParams = new DataTableParams(Request);
            int cantidadRegistros = this.manager.ObtenerCantidadCiclosLectivos();

            IEnumerable<CicloLectivo> cargasFiltradas = this.manager.ObtenerCiclosLectivos(dtParams.Search);

            Func<CicloLectivo, string> orderingFunction =
                (c => dtParams.SortByColumnIndex == 0 ? c.Descripcion :
                dtParams.SortByColumnIndex == 1 ? c.FechaInicio.ToString() :
                dtParams.SortByColumnIndex == 2 ? c.FechaFin.ToString() :
                dtParams.SortByColumnIndex == 3 ? c.Estado.ToString() :
                "");

            if (dtParams.SortingDirection == eSortingDirection.ASC)
            {
                cargasFiltradas = cargasFiltradas.OrderBy(orderingFunction);
            }
            else
            {
                cargasFiltradas = cargasFiltradas.OrderByDescending(orderingFunction);
            }

            List<CicloLectivo> displayedCargas = cargasFiltradas
                .Skip(param.start).Take(param.length).ToList();


            var result = from c in displayedCargas
                         select new object[] {
                                        c.Descripcion,
                                        c.FechaInicio.ToString("dd/MM/yyyy"),
                                        c.FechaFin.ToString("dd/MM/yyyy"),
                                        c.Estado,
                                        c.CicloLectivoId
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

        public ActionResult EliminarCicloLectivo(int CicloLectivoId)
        {
            this.manager.EliminarCicloLectivo(CicloLectivoId);

            return RedirectToAction("Index", "CiclosLectivos");
        }

        public ActionResult Crear_EditarCicloLectivo(CicloLectivo model)
        {
            try
            {
                if (model.CicloLectivoId > 0)
                {

                    manager.EditarCicloLectivo(model);
                }
                else
                {
                    CicloLectivo cl = new CicloLectivo()
                    {
                        CicloLectivoId = model.CicloLectivoId,
                        Descripcion = model.Descripcion,
                        FechaInicio = model.FechaInicio,
                        FechaFin = model.FechaFin,
                        CobranzaInicioMes = model.CobranzaInicioMes,
                        CobranzaInicioAnio = model.CobranzaInicioAnio,
                        CobranzaFinMes = model.CobranzaFinMes,
                        CobranzaFinAnio = model.CobranzaFinAnio,
                        Cerrado = model.Cerrado,
                        InscripcionHabilitadaDesde = model.InscripcionHabilitadaDesde,
                        InscripcionHabilitadaHasta = model.InscripcionHabilitadaHasta,
                        ImporteDefault = model.ImporteDefault,
                        Anulado = false
                    };

                    manager.CrearCicloLectivo(cl);
                }

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }
        }

        public ActionResult AbrirModal(int? CicloLectivoId)
        {
            CicloLectivo c = new CicloLectivo();

            if (CicloLectivoId == 0)
            {
                c.Descripcion = "";
            }
            else
            {
                c = manager.GetCicloLectivo((int)CicloLectivoId);
            }

            return PartialView("_ModalCicloLectivo", c);
        }

    }
}