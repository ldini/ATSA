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
    public class RequisitosController : BaseController
    {

        RequisitoManager manager = new RequisitoManager();

        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);
            return View();
        }

        public ActionResult ObtenerListadoIndex(JQueryDataTableParamModel param, string filtro = "")
        {
            DataTableParams dtParams = new DataTableParams(Request);
            int cantidadRegistros = this.manager.ObtenerCantidadRegistros();

            IEnumerable<Requisito> cargasFiltradas = this.manager.ObtenerRequisitos(dtParams.Search);

            Func<Requisito, string> orderingFunction =
                (c => dtParams.SortByColumnIndex == 0 ? c.Descripcion :
                dtParams.SortByColumnIndex == 1 ? c.RequisitoId.ToString() :
                "");

            if (dtParams.SortingDirection == eSortingDirection.ASC)
            {
                cargasFiltradas = cargasFiltradas.OrderBy(orderingFunction);
            }
            else
            {
                cargasFiltradas = cargasFiltradas.OrderByDescending(orderingFunction);
            }

            List<Requisito> displayedCargas = cargasFiltradas
                .Skip(param.start).Take(param.length).ToList();


            var result = from c in displayedCargas
                         select new object[] {
                                c.Descripcion,
                                c.RequisitoId
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

        public ActionResult EliminarRequisito(int requisitoid)
        {
            this.manager.EliminarRequisito(requisitoid);

            return RedirectToAction("Index", "Requisitos");
        }

        public ActionResult Crear_EditarRequisito(Requisito model)
        {
            try
            {
                if (model.RequisitoId > 0)
                {
                    manager.EditarRequisito(model);
                }
                else
                {
                    Requisito requisito = new Requisito
                    {
                        Descripcion = model.Descripcion,
                        Anulado = false
                    };

                    manager.CrearRequisito(requisito);
                }

                return RedirectToAction("Index", "Requisitos");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult AbrirModal(int? requisitoid)
        {
            Requisito e = new Requisito();

            if (requisitoid == 0)
            {
                e.RequisitoId = 0;
                e.Descripcion = "";
            }
            else
            {
                e = manager.GetRequisito((int)requisitoid);
            }

            return PartialView("_ModalRequisitos", e);
        }
    }
}