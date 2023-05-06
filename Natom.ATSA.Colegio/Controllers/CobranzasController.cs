using Microsoft.Reporting.WebForms;
using Natom.ATSA.Colegio.Managers;
using Natom.ATSA.Colegio.Models;
using Natom.ATSA.Colegio.Models.DataTable;
using Natom.ATSA.Colegio.Models.ViewModels;
using Natom.ATSA.Colegio.Reporting;
using Natom.ATSA.Colegio.Models.Result;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Colegio.Controllers
{
    public class CobranzasController : BaseController
    {
        private CobranzaManager manager;

        public CobranzasController()
        {
            this.manager = new CobranzaManager();
        }

        // GET: Cobranzas
        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);
            return View();
        }

        public FileResult ImprimirReciboDePago(int id)
        {
            ReciboDePagoReportResult data;
            Cobranza cobranza = manager.ObtenerCobranza(id);
            if (cobranza.CobranzaPagoAdelantadoId.HasValue)
            {
                CobranzaPagoAdelantado pagoAdelantado = manager.ObtenerPagoAdelantado(cobranza.CobranzaPagoAdelantadoId.Value);
                data = new ReciboDePagoReportResult()
                {
                    FechaDia = pagoAdelantado.Cobranzas[0].FechaHora.Day.ToString().PadLeft(2, '0'),
                    FechaMes = pagoAdelantado.Cobranzas[0].FechaHora.Month.ToString().PadLeft(2, '0'),
                    FechaAnio = pagoAdelantado.Cobranzas[0].FechaHora.Year.ToString().Substring(2, 2),
                    Senior = String.Format("{0} {1}", pagoAdelantado.Inscripcion.Apellido, pagoAdelantado.Inscripcion.Nombre).ToUpper(),
                    DNI = String.Format("DNI: {0}", pagoAdelantado.Inscripcion.DNI),
                    Domicilio = String.Format("{0},  {1}", pagoAdelantado.Inscripcion.Direccion, pagoAdelantado.Inscripcion.Localidad).ToUpper(),
                    Concepto = "PAGO ADELANTADO\n" + "MESES DE " + GetMesesPagoAdelantado(pagoAdelantado) + "\n" + pagoAdelantado.Inscripcion.CarreraCurso.Titulo,
                    ConceptoImporte = pagoAdelantado.Monto.ToString("C2"),
                    Total = pagoAdelantado.Monto.ToString("C2"),
                    Efectivo = pagoAdelantado.Cobranzas[0].Efectivo ? pagoAdelantado.Monto.ToString("C2") : ""
                };
            }
            else
            {
                data = new ReciboDePagoReportResult()
                {
                    FechaDia = cobranza.FechaHora.Day.ToString().PadLeft(2, '0'),
                    FechaMes = cobranza.FechaHora.Month.ToString().PadLeft(2, '0'),
                    FechaAnio = cobranza.FechaHora.Year.ToString().Substring(2, 2),
                    Senior = String.Format("{0} {1}", cobranza.Inscripcion.Apellido, cobranza.Inscripcion.Nombre).ToUpper(),
                    DNI = String.Format("DNI: {0}", cobranza.Inscripcion.DNI),
                    Domicilio = String.Format("{0},  {1}", cobranza.Inscripcion.Direccion, cobranza.Inscripcion.Localidad).ToUpper(),
                    Concepto = cobranza.Inscripcion.CarreraCurso.Titulo,
                    ConceptoImporte = cobranza.Monto.ToString("C2"),
                    Total = cobranza.Monto.ToString("C2"),
                    Efectivo = cobranza.Efectivo ? cobranza.Monto.ToString("C2") : ""
                };
            }

            var reportDataSource = new List<ReciboDePagoReportResult>() { data };

            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = Server.MapPath("~/Reporting/ReciboDePagoReport.rdlc");
            viewer.LocalReport.EnableExternalImages = true;
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", reportDataSource));

            string filePath = System.IO.Path.GetTempFileName();
            ReportHelper.ExportToPDF(viewer, filePath);

            viewer.Dispose();
            return File(filePath, "application/pdf");
        }

        private string GetMesesPagoAdelantado(CobranzaPagoAdelantado pagoAdelantado)
        {
            List<string> mesesPagos = new List<string>();
            string[] meses = new string[] {"Matricula","Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            foreach (var cobranza in pagoAdelantado.Cobranzas)
            {
                mesesPagos.Add(meses[cobranza.Mes - 1]);
            }
            return String.Join(", ", mesesPagos);
        }

        [HttpPost]
        public ActionResult Grabar(int inscripcionId, int mes, int anio, decimal monto, string observaciones, bool efectivo)
        {
            try
            {
                var cobranza = this.manager.GrabarCobranza(inscripcionId, mes, anio, monto, observaciones, efectivo);
                return Json(new { success = true, id = cobranza.CobranzaId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GrabarPagoAdelantado(int inscripcionId, List<int> meses, int anio, decimal monto, string observaciones, bool efectivo)
        {
            try
            {
                var cobranza = this.manager.GrabarCobranzaPagoAdelantado(inscripcionId, meses, anio, monto, observaciones, efectivo);
                return Json(new { success = true, id = cobranza.CobranzaPagoAdelantadoId, cobranzasId = cobranza.Cobranzas.Select(s => s.CobranzaId) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult Anular(int inscripcionId, int mes, int anio)
        {
            try
            {
                this.manager.AnularCobranza(inscripcionId, mes, anio);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult AbrirModal(int InscripcionId, int Mes, int Anio)
        {
            Cobranza c = new Cobranza();
            c.InscripcionId = InscripcionId;
            c.Mes = Mes;
            c.Anio = Anio;

            var inscripcion = new InscripcionesManager().ObtenerInscripcion(InscripcionId);
            ViewBag.CarreraCurso = inscripcion.CarreraCurso.Titulo;
            //ViewBag.CicloLectivo = inscripcion.CicloLectivo.Descripcion; //Comment
            ViewBag.CiclosLectivos = new CiclosLectivosManager().CalcularCiclosLectivos(inscripcion);
            ViewBag.Periodo = Mes == 1 ? "Matricula" : String.Format("{0}-{1}", Mes.ToString().PadLeft(2, '0'), Anio);
            ViewBag.NombreYApellido = inscripcion.Apellido + " " + inscripcion.Nombre;
            ViewBag.DNI = inscripcion.DNI;

            string configValue = "";
            if (Mes == 1)
                configValue = ConfigurationManager.AppSettings["ATSA.Cobranza.MontoMatricula"];
            else
                configValue = ConfigurationManager.AppSettings["ATSA.Cobranza.MontoMensual"];
            decimal montoMensual = Convert.ToDecimal(configValue.Replace(".", ","));
            ViewBag.MontoMensual = montoMensual;



            return PartialView("_ModalCobranza", c);
        }

        public ActionResult AbrirModalPagoAdelantado()
        {
            CobranzaPagoAdelantado c = new CobranzaPagoAdelantado();
            c.InscripcionId = 0;
            
            string configValue = ConfigurationManager.AppSettings["ATSA.Cobranza.MontoMensual"];
            decimal montoMensual = Convert.ToDecimal(configValue.Replace(".", ","));
            ViewBag.MontoMensual = montoMensual;

            configValue = ConfigurationManager.AppSettings["ATSA.Cobranza.MontoMatricula"];
            decimal montoMatricula = Convert.ToDecimal(configValue.Replace(".", ","));
            ViewBag.MontoMatricula = montoMatricula;

            return PartialView("_ModalCobranzaPagoAdelantado", c);
        }

        [HttpPost]
        public JsonResult ObtenerCiclosLectivos(int inscripcionId)
        {
            try
            {
                var manager = new InscripcionesManager();
                var inscripcion = manager.ObtenerInscripcion(inscripcionId);
                var cicloLectivo = manager.ObtenerCicloLectivoDeInscripcion(inscripcionId);
                return Json(new
                {
                    success = true,
                    data = new CiclosLectivosManager().CalcularCiclosLectivos(inscripcion)
                                .Select(c => new
                                {
                                    anio = c.Anio,
                                    descripcion  = c.Descripcion,
                                    selected = c.Anio == DateTime.Now.Year
                                })
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult ObtenerDetalleCobranzaInscripcion(int inscripcionId, int anio)
        {
            List<int> meses = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            try
            {
                var cicloLectivo = new InscripcionesManager().ObtenerCicloLectivoDeInscripcion(inscripcionId); //Comment
                string configValue = ConfigurationManager.AppSettings["ATSA.Cobranza.MontoMensual"];
                decimal montoMensual = Convert.ToDecimal(configValue.Replace(".", ","));

                configValue = ConfigurationManager.AppSettings["ATSA.Cobranza.MontoMatricula"];
                decimal montoMatricula = Convert.ToDecimal(configValue.Replace(".", ","));

                return Json(new
                {
                    success = true,
                    mesesPagos = this.manager.ObtenerMesesPagosCobranzaInscripcion(inscripcionId, anio),
                    mesesNoCobrables = new List<int>(), //meses.Where(m => m < cicloLectivo.CobranzaInicioMes || m > cicloLectivo.CobranzaFinMes),
                    importeDefault = montoMensual,
                    importeMatricula = montoMatricula
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult ObtenerInscripcionesTypeAhead(string inscripciones)
        {
            try
            {
                return Json(new
                {
                    success = true,
                    datos = from l in this.manager.ObtenerInscripcionesTypeAhead(inscripciones).Take(20)
                            select new
                            {
                                id = l.InscripcionId,
                                label = l.Apellido + " " + l.Nombre + " /// " + l.CarreraCurso.Titulo,
                                apyn = l.Apellido + " " + l.Nombre,
                                dni = l.DNI,
                                carreraCurso = l.CarreraCurso.Titulo,
                                esAfiliado = !string.IsNullOrEmpty(l.Afiliado)
                            }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        public ActionResult ObtenerListadoIndex(JQueryDataTableParamModel param, string filtroCarrera = "", string filtroEstado = "", string filtroCiclo = "")
        {
            DateTime fechaLimite = DateTime.Now.AddDays(CobranzaManager.DIAS_COBRANZA_ANTERIORIDAD_POR_DEFAULT);
            DataTableParams dtParams = new DataTableParams(Request);
            int cantidadRegistros = this.manager.ListarCobranzas(null, filtroCarrera, filtroEstado, filtroCiclo, fechaLimite).Count();

            IEnumerable<ListarCobranzasResult> cargasFiltradas = this.manager.ListarCobranzas(dtParams.Search, filtroCarrera, filtroEstado, filtroCiclo, fechaLimite);

            Func<ListarCobranzasResult, string> orderingFunction =
                (c => dtParams.SortByColumnIndex == 0 ? c.Afiliado ?? "NO" : 
                dtParams.SortByColumnIndex == 1 ? c.Apellido + " " + c.Nombre :
                dtParams.SortByColumnIndex == 2 ? c.DNI.ToString() :
                dtParams.SortByColumnIndex == 3 ? c.CarreraCurso.ToString() :
                //.SortByColumnIndex == 4 ? c.Descripcion.ToString() :
                dtParams.SortByColumnIndex == 4 ? ((c.Anio * 12) + c.Mes).ToString() :
                dtParams.SortByColumnIndex == 5 ? c.Recibo :
                dtParams.SortByColumnIndex == 6 ? c.Estado.ToString() :
                "");

            if (dtParams.SortingDirection == eSortingDirection.ASC)
            {
                cargasFiltradas = cargasFiltradas.OrderBy(orderingFunction);
            }
            else
            {
                cargasFiltradas = cargasFiltradas.OrderByDescending(orderingFunction);
            }

            List<ListarCobranzasResult> displayedCargas = cargasFiltradas
                .Skip(param.start).Take(param.length).ToList();


            var cicloLectivoManager = new CiclosLectivosManager();
            var result = from c in displayedCargas
                         select new object[] {
                                        c.Afiliado ?? "NO",
                                        c.Apellido + " " + c.Nombre,
                                        c.DNI.ToString(),
                                        c.CarreraCurso.ToString(),
                                        cicloLectivoManager.CalcularCicloLectivo(c.InscripcionId, c.Anio), //c.Descripcion.ToString(),
                                        c.Periodo.ToString(),
                                        c.Recibo,
                                        c.Estado.ToString(),
                                        c.InscripcionId,
                                        c.CarreraCursoId,
                                        c.CobranzaId,
                                        c.Mes,
                                        c.Anio,
                                        c.DebeDocumentacion
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