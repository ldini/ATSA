using Natom.ATSA.Colegio.Managers;
using Natom.ATSA.Colegio.Models;
using Natom.ATSA.Colegio.Models.DataTable;
using Natom.ATSA.Colegio.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Colegio.Controllers
{
    public class CarrerasCursosController : BaseController
    {
        CarrerasCursosManager manager = new CarrerasCursosManager();
        RequisitoManager requisitomanager = new RequisitoManager();

        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);
            ViewBag.Tipo = manager.GetCarreraCursoTipos();
            ViewBag.Duracion = manager.GetTipoDuracion();

            return View();
        }

        public ActionResult ObtenerListadoIndex(JQueryDataTableParamModel param, int? filtrotipo, int? filtrocursada)
        {
            DataTableParams dtParams = new DataTableParams(Request);
            int cantidadRegistros = this.manager.ObtenerCantidadRegistros();

            IEnumerable<CarreraCurso> cargasFiltradas = this.manager.ObtenerCarreraCurso(dtParams.Search, filtrotipo, filtrocursada);

            Func<CarreraCurso, string> orderingFunction =
                (c => dtParams.SortByColumnIndex == 0 ? c.Titulo :
                dtParams.SortByColumnIndex == 1 ? c.Duracion.ToString().PadLeft(2, '0') + " " + c.TipoDuracion.Descripcion :
                dtParams.SortByColumnIndex == 2 ? c.Horarios.ToString() :
                dtParams.SortByColumnIndex == 3 ? c.CarreraCursoId.ToString() :
                "");

            if (dtParams.SortingDirection == eSortingDirection.ASC)
            {
                cargasFiltradas = cargasFiltradas.OrderBy(orderingFunction);
            }
            else
            {
                cargasFiltradas = cargasFiltradas.OrderByDescending(orderingFunction);
            }

            List<CarreraCurso> displayedCargas = cargasFiltradas
                .Skip(param.start).Take(param.length).ToList();


            var result = from c in displayedCargas
                         select new object[] {
                                c.Titulo,
                                c.Duracion.ToString() + " " + c.TipoDuracion.Descripcion,
                                c.Horarios,
                                c.CarreraCursoId
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

        public ActionResult EliminarCarreraCurso(int carreracursoid)
        {
            this.manager.EliminarCarreraCurso(carreracursoid);

            return RedirectToAction("Index", "CarrerasCursos");
        }

        [HttpPost]
        public ActionResult Crear_EditarCarreraCurso(CarreraCurso model)
        {
            try
            {
                manager.GrabarCarreraCurso(model);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }
        }

        public ActionResult AbrirModal(int? carreracursoid)
        {
            CarreraCurso cc = new CarreraCurso();

            if (carreracursoid > 0)
            {
                cc = manager.GetCarreraCurso((int)carreracursoid);
            }

            ViewBag.Tipo = manager.GetCarreraCursoTipos();
            ViewBag.Requisitos = requisitomanager.GetRequisitos();
            ViewBag.Duracion = manager.GetTipoDuracion();

            return PartialView("_ModalCarreraCurso", cc);
        }
    }
}