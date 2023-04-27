using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Colegio.Controllers
{
    public class BaseController : Controller
    {
        public int? SesionUsuarioId
        {
            get
            {
                HttpCookie cookie = Request.Cookies["ATSAColgWebApp"];
                if (cookie == null)
                {
                    return null;
                }
                else
                {
                    return Convert.ToInt32(cookie.Value);
                }
            }
        }
    }
}