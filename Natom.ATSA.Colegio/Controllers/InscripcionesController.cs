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
    public class InscripcionesController : BaseController
    {
        InscripcionesManager manager = new InscripcionesManager();

        CarrerasCursosManager carreracursomanager = new CarrerasCursosManager();
        CiclosLectivosManager ciclolectivomanager = new CiclosLectivosManager();

        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);

            ViewBag.CarrerasCursos = carreracursomanager.GetCarreraCursos();
            ViewBag.CiclosLectivos = ciclolectivomanager.GetCiclosLectivos();
            return View();
        }

        public ActionResult ObtenerEdadDesdeFecha(string fecha)
        {
            try
            {
                DateTime dt = DateTime.Parse(fecha);
                return Json(new { success = true, edad = Convert.ToInt32((DateTime.Now.Date - dt.Date).TotalDays / 365) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult ObtenerListadoIndex(JQueryDataTableParamModel param, int? filtrocarreracurso, int? filtrociclolectivo)
        {
            DataTableParams dtParams = new DataTableParams(Request);
            int cantidadRegistros = this.manager.ObtenerCantidadInscripciones();

            IEnumerable<Inscripcion> cargasFiltradas = this.manager.ObtenerInscripciones(dtParams.Search, filtrocarreracurso, filtrociclolectivo);
            
            Func<Inscripcion, string> orderingFunction =
                (c => dtParams.SortByColumnIndex == 0 ? (c.Afiliado ?? "NO") :
                dtParams.SortByColumnIndex == 1 ? c.Nombre + " " + c.Apellido :
                dtParams.SortByColumnIndex == 2 ? c.DNI :
                dtParams.SortByColumnIndex == 3 ? c.CarreraCurso.Titulo :
                "");

            if (dtParams.SortingDirection == eSortingDirection.ASC)
            {
                cargasFiltradas = cargasFiltradas.OrderBy(orderingFunction);
            }
            else
            {
                cargasFiltradas = cargasFiltradas.OrderByDescending(orderingFunction);
            }

            List<Inscripcion> displayedCargas = cargasFiltradas
                .Skip(param.start).Take(param.length).ToList();

            var inscripcionesQueDebenDocs = this.manager.ObtenerInscripcionesQueDebenDocumentacion()
                                                            .Select(s => s.InscripcionId)
                                                            .ToList();

            var cicloLectivoManager = new CiclosLectivosManager();
            var result = from c in displayedCargas
                         select new object[] {
                                (c.Afiliado ?? "NO"),
                                c.Nombre + " " + c.Apellido,
                                c.DNI,
                                c.CarreraCurso.Titulo,
                                cicloLectivoManager.CalcularCicloLectivoActual(c),
                                c.InscripcionId.ToString(),
                                c.BajaCarreraFecha?.ToString(),
                                c.BajaMotivo,
                                inscripcionesQueDebenDocs.Contains(c.InscripcionId) /*DEBE DOC*/
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

        public ActionResult DarBajaInscripcion(Inscripcion inscripcion)
        {
            this.manager.DarBajaInscripcion(inscripcion);

            return RedirectToAction("Index", "Inscripciones");
        }
        [HttpPost]
        public ActionResult GetAfiliado(string nroAfiliado)
        {
            try
            {
                List<Persona> datos = new List<Persona>();
                var afiliadoManager = new AfiliadoManager();
                var afiliado = afiliadoManager.ObtenerAfiliado(nroAfiliado);
                var familiares = afiliado != null ? afiliadoManager.ObtenerFamiliares(afiliado.ID) : new List<Persona>();
                if (afiliado != null) datos.Add(afiliado);
                if (familiares != null) datos.AddRange(familiares);

                return Json(new
                {
                    success = true,
                    encontrado = afiliado != null && afiliado?.Activo == true,
                    datos = from d in datos
                            select new {
                                id = d.ID,
                                numeroAfiliado = d.Numero_Afiliado,
                                nombre = d.Nombres,
                                apellido = d.Apellidos,
                                codpostal = d.Codigo_Postal,
                                dni = d.Documento,
                                cuil = d.CUIL,
                                localidad = d.Localidad,
                                profesion = d.Profesion,
                                sexo = d.Sexo,
                                telefono = d.Telefono,
                                domicilio = d.Domicilio,
                                edad = Convert.ToInt32((DateTime.Now - d.Fecha_Nacimiento).TotalDays / 365),
                                fechaNacimiento = d.Fecha_Nacimiento.ToString("dd/MM/yyyy")
                            }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetRequisitosPorCarreraCurso(int carreraCursoId)
        {
            try
            {
                var requisitos = this.manager.ObtenerRequisitosPorCarreraCurso(carreraCursoId);
                return Json(new
                {
                    success = true,
                    requisitos = from r in requisitos
                                 select new
                                 {
                                     id = r.RequisitoId,
                                     value = r.Descripcion
                                 }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Crear_EditarInscripcion(Inscripcion model)
        {
            try
            {
                bool esNuevo = false;
                int cobranzaMes = 0, cobranzaAnio = 0;
                bool cobradoAutomaticamente = false;
                Inscripcion inscripcion;
                if (model.InscripcionId > 0)
                {
                    manager.EditarInscripcion(model);
                    inscripcion = model;
                }
                else
                {
                    esNuevo = true;
                    inscripcion = manager.CrearInscripcion(model);
                    var cobranzaMgr = new CobranzaManager();
                    cobranzaMgr.RealizarCobranzasAutomaticasSiCorresponde(inscripcion.InscripcionId);
                    cobradoAutomaticamente = cobranzaMgr.CobradoAutomaticamenteAlInscribir(inscripcion);
                    if (!cobradoAutomaticamente)
                    {
                        cobranzaMes = 1;  //MATRICULA
                        cobranzaAnio = DateTime.Now.Year;
                    }
                }
                return Json(new
                {
                    success = true,
                    nuevo = esNuevo,
                    afiliado = !string.IsNullOrEmpty(inscripcion.Afiliado),
                    cobrado = cobradoAutomaticamente,
                    id = inscripcion.InscripcionId,
                    cobranza = new
                    {
                        mes = cobranzaMes,
                        anio = cobranzaAnio
                    }
                });
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }
        }

        public ActionResult AbrirModal(int? inscripcionid, bool reinscripcion = false)
        {
            Inscripcion i = new Inscripcion();

            ViewBag.Reinscripcion = reinscripcion;

            if (inscripcionid > 0)
            {
                i = manager.GetInscripcion((int)inscripcionid);
                i.FechaNacimiento = i.FechaNacimiento == null ? DateTime.Parse("1/1/0001") : i.FechaNacimiento;
                if (reinscripcion)
                {
                    i.InscripcionId = 0;
                }
            }
            else
            {
                i.FechaNacimiento = DateTime.Parse("1/1/0001");
                i.Sexo = "";
                i.EstadoCivil = "";
            }
            ViewBag.CarrerasCursos = carreracursomanager.GetCarreraCursos();
            ViewBag.CiclosLectivos = ciclolectivomanager.GetCiclosLectivos(); //ciclolectivomanager.GetCiclosLectivosVigentes();

            return PartialView("_ModalInscripcion", i);
        }

        [HttpPost]
        public ActionResult ObtenerDocumentacionFaltantePorInscripcion(int inscripcionId)
        {
            try
            {
                var data = this.manager.ObtenerRequisitosPendientesDeEntregar(inscripcionId).ToList();
                return Json(new
                {
                    success = true,
                    pendientes = from d in data
                                 select new
                                 {
                                     id = d.RequisitoId,
                                     value = d.Requisito
                                 }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult RegistrarEntregaDeDocumentacion(int inscripcionId, List<int> requisitosId)
        {
            try
            {
                this.manager.RegistrarEntregaDeDocumentacion(inscripcionId, requisitosId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        public ActionResult ModalDarBaja(int InscripcionId)
        {
            var e = manager.GetInscripcion(InscripcionId);

            return PartialView("_ModalDarBaja", e);
        }

        [HttpPost]
        public ActionResult ModalDarBaja(Inscripcion model)
        {
            manager.DarBajaInscripcion(model);

            return RedirectToAction("Index", "Inscripciones");
        }
    }
}