using Natom.ATSA.Colegio.Managers;
using Natom.ATSA.Colegio.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Natom.ATSA.Colegio
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<DbColegioContext>(null);
            Database.SetInitializer<DbAtsaContext>(null);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var cobranzaMgr = new CobranzaManager();
            cobranzaMgr.CargarTablaMesAnio();
            cobranzaMgr.RealizarCobranzasAutomaticasSiCorresponde(inscripcionId: null);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(HttpContext.Current.Request.CurrentExecutionFilePathExtension))
            {
                HttpContextBase context = new HttpContextWrapper(HttpContext.Current);
                string controllerName = "";
                string actionName = "";
                RouteData rd = null;
                try
                {
                    rd = RouteTable.Routes.GetRouteData(context);
                    controllerName = rd.GetRequiredString("controller");
                    actionName = rd.GetRequiredString("action");
                }
                catch
                {
                }
                finally
                {
                    if (rd != null && !string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
                    {
                        if (!(controllerName.ToLower().Equals("home") && actionName.ToLower().Equals("login")) && !(controllerName.ToLower().Equals("usuarios") && actionName.ToLower().Equals("recuperodeclave")) && !(controllerName.ToLower().Equals("usuarios") && actionName.ToLower().Equals("enviarmailrecupero")))
                        {
                            HttpCookie cookie = Request.Cookies["ATSAColgWebApp"];
                            if (cookie == null)
                            {
                                Response.Redirect("/colegiotest/home/Login"); //Response.Redirect("/liquidacionestest/colegiotest/home/Login");
                                Response.End();
                            }
                        }
                        
                    }
                }
            }
        }
    }
}