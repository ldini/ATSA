using Natom.ATSA.Colegio.Managers;
using Natom.ATSA.Colegio.Models;
using Natom.ATSA.Colegio.Models.DataTable;
using Natom.ATSA.Colegio.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Colegio.Controllers
{
    public class CargasController : BaseController
    {
        private CargaManager manager = new CargaManager();


        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);
            return View();
        }

        
        [HttpPost]
        public ActionResult EliminarCarga(int CargaId, string motivo)
        {
            try
            {
                int usuarioId = this.SesionUsuarioId.Value;
                this.manager.EliminarCarga(CargaId, usuarioId, motivo);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        
    }
}