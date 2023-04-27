using Natom.ATSA.Colegio.Managers;
using Natom.ATSA.Colegio.Models;
using Natom.ATSA.Colegio.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Colegio.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);
            return View();
        }

        public ActionResult Login(string error = "")
        {
            ViewBag.ErrorMessage = error;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginView data)
        {
            try
            {
                SesionManager mgr = new SesionManager();
                int usuarioId;
                mgr.ValidarLogin(data.Email, data.Clave, out usuarioId);
                HttpCookie myCookie = new HttpCookie("ATSAColgWebApp");
                myCookie.Value = usuarioId.ToString();
                Response.Cookies.Add(myCookie);
                Response.Redirect("/colegiotest/home/Index"); //Response.Redirect("/liquidacionestest/colegiotest/home/Index");
                Response.End();
                return null;
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Home", new { @error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpCookie cookie = Request.Cookies["ATSAColgWebApp"];
            Request.Cookies.Remove("ATSAColgWebApp");
            Response.Redirect("/colegiotest/home/Login"); //Response.Redirect("/liquidacionestest/colegiotest/home/Login");
            Response.End();
            return RedirectToAction("Login", "Home", new { @error = "" });
        }
    }
}