using Natom.ATSA.Colegio.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Natom.ATSA.Colegio.Models;

namespace Natom.ATSA.Colegio.Managers
{
    public class CobranzaManager
    {
        public const int DIAS_COBRANZA_ANTERIORIDAD_POR_DEFAULT = 7;

        private DbColegioContext db;

        public CobranzaManager()
        {
            db = new DbColegioContext();
        }

        public void CargarTablaMesAnio()
        {
            db.Database.ExecuteSqlCommand("DELETE FROM MesAnio WHERE Id > 0;");
            List<MesAnio> arr = new List<MesAnio>();
            for (int anio = 2020; anio <= DateTime.Now.Year + 5; anio++)
                for (int mes = 1; mes <= 12; mes++)
                    arr.Add(new MesAnio() { Mes = mes, Anio = anio });
            db.MesAnio.AddRange(arr);
            db.SaveChanges();
        }

        public Cobranza ObtenerCobranza(int id)
        {
            return this.db.Cobranzas
                            .Include(c => c.Inscripcion)
                            .Include(c => c.Inscripcion.CarreraCurso)
                            .FirstOrDefault(c => c.CobranzaId == id);
        }

        public CobranzaPagoAdelantado ObtenerPagoAdelantado(int pagoAdelantadoId)
        {
            return this.db.CobranzasPagoAdelantado
                            .Include(i => i.Inscripcion)
                            .Include(i => i.Cobranzas)
                            .Where(i => i.CobranzaPagoAdelantadoId == pagoAdelantadoId)
                            .FirstOrDefault();
        }

        public void RealizarCobranzasAutomaticasSiCorresponde(int? inscripcionId)
        {
            var cobranzas = ListarCobranzas(null, null, null, null, DateTime.Now.AddDays(DIAS_COBRANZA_ANTERIORIDAD_POR_DEFAULT), inscripcionId);
            var cobranzasDeAfiliados = cobranzas.Where(c => !c.CobranzaId.HasValue && !string.IsNullOrEmpty(c.Afiliado) && !c.Afiliado.Equals("NO")).ToList();
            foreach (var aCobrar in cobranzasDeAfiliados)
            {
                this.db.Cobranzas.Add(new Cobranza()
                {
                    Anio = aCobrar.Anio,
                    Automatica = true,
                    FechaHora = DateTime.Now,
                    InscripcionId = aCobrar.InscripcionId,
                    Mes = aCobrar.Mes,
                    Monto = 0,
                    Observaciones = "BONIFICADO POR SER AFILIADO."
                });
            }
            this.db.SaveChanges();
        }

        public void AnularCobranza(int inscripcionId, int mes, int anio)
        {
            var cobranza = this.db.Cobranzas.FirstOrDefault(c => !c.Anulado && c.InscripcionId == inscripcionId && c.Mes == mes && c.Anio == anio);
            if (cobranza != null)
            {
                this.db.Entry(cobranza).State = System.Data.Entity.EntityState.Modified;
                cobranza.Anulado = true;
                this.db.SaveChanges();
            }
        }

        public Cobranza GrabarCobranza(int inscripcionId, int mes, int anio, decimal monto, string observaciones, bool efectivo)
        {
            if (this.db.Cobranzas.Any(c => !c.Anulado && c.InscripcionId == inscripcionId && c.Mes == mes && c.Anio == anio))
            {
                throw new Exception("El período ya se encuentra cobrado para el alumno en cuestión.");
            }

            if (!string.IsNullOrEmpty(observaciones))
            {
                if (this.db.Cobranzas.Any(c => !c.Anulado && c.Observaciones.Equals(observaciones))
                    || this.db.CobranzasPagoAdelantado.Any(c => !c.Anulado && c.Observaciones.Equals(observaciones)))
                {
                    throw new Exception($"El recibo {observaciones} ya se encuentra usado.");
                }
            }

            Cobranza cobranza = new Cobranza()
            {
                Anio = anio,
                Automatica = false,
                Anulado = false,
                FechaHora = DateTime.Now,
                InscripcionId = inscripcionId,
                Mes = mes,
                Monto = monto,
                Observaciones = observaciones,
                Efectivo = efectivo
            };

            this.db.Cobranzas.Add(cobranza);
            this.db.SaveChanges();

            return cobranza;
        }

        public CobranzaPagoAdelantado GrabarCobranzaPagoAdelantado(int inscripcionId, List<int> meses, int anio, decimal monto, string observaciones, bool efectivo)
        {
            if (!string.IsNullOrEmpty(observaciones))
            {
                if (this.db.Cobranzas.Any(c => !c.Anulado && c.Observaciones.Equals(observaciones))
                    || this.db.CobranzasPagoAdelantado.Any(c => !c.Anulado && c.Observaciones.Equals(observaciones)))
                {
                    throw new Exception($"El recibo {observaciones} ya se encuentra usado.");
                }
            }

            CobranzaPagoAdelantado pagoAdelantado = new CobranzaPagoAdelantado();
            pagoAdelantado.Monto = monto;
            pagoAdelantado.Observaciones = observaciones;
            pagoAdelantado.InscripcionId = inscripcionId;
            pagoAdelantado.Cobranzas = new List<Cobranza>();
            foreach (var mes in meses)
            {
                pagoAdelantado.Cobranzas.Add(new Cobranza()
                {
                    Anio = anio,
                    Automatica = false,
                    Anulado = false,
                    FechaHora = DateTime.Now,
                    InscripcionId = inscripcionId,
                    Mes = mes,
                    Monto = monto / (decimal)meses.Count,
                    Observaciones = "PAGO ADELANTADO // RBO. " + observaciones,
                    Efectivo = efectivo
                });
            }
            this.db.CobranzasPagoAdelantado.Add(pagoAdelantado);
            this.db.SaveChanges();

            return pagoAdelantado;
        }

        public IEnumerable<ListarCobranzasResult> ListarCobranzas(string search, string filtroCarrera, string filtroEstado, string filtroCiclo, DateTime? fechaLimite, int? filtroInscripcionId = null)
        {
            IEnumerable<ListarCobranzasResult> query = this.db.Database.SqlQuery<ListarCobranzasResult>("CALL ListarCobranzas({0})", filtroInscripcionId);
            if (!string.IsNullOrEmpty(search))
            {
                int n;
                if (int.TryParse(search, out n))
                {
                    query = query.Where(q => q.DNI.Equals(search)
                                                || q.Recibo.Contains(search));
                }
                else
                {
                    search = search.ToLower();
                    query = query.Where(q => q.Nombre.ToLower().Contains(search)
                                                || q.Apellido.ToLower().Contains(search)
                                                || q.CarreraCurso.ToLower().Contains(search)
                                                || q.Descripcion.ToLower().Contains(search)
                                                || q.Periodo.ToLower().Contains(search)
                                                || q.Estado.ToLower().Contains(search));
                }
            }
            if (!string.IsNullOrEmpty(filtroCarrera))
            {
                query = query.Where(s => s.CarreraCurso.ToLower().Equals(filtroCarrera));
            }
            if (!string.IsNullOrEmpty(filtroEstado))
            {
                query = query.Where(s => s.Estado.ToLower().Equals(filtroEstado));
            }
            if (!string.IsNullOrEmpty(filtroCiclo))
            {
                query = query.Where(s => s.Descripcion.ToLower().Equals(filtroCiclo));
            }
            if (fechaLimite.HasValue)
            {
                int mes = fechaLimite.Value.Month;
                int anio = fechaLimite.Value.Year;
                query = query.Where(q => (q.Mes <= mes && q.Anio <= anio)
                                            || q.Estado.ToUpper().Equals("ABONADO"));
            }
            return query;
        }

        public List<int> ObtenerMesesPagosCobranzaInscripcion(int inscripcionId, int year)
        {
            return this.db.Cobranzas.Where(c => c.InscripcionId == inscripcionId && !c.Anulado && c.Anio == year)
                                    .Select(c => c.Mes)
                                    .ToList();
        }

        public IEnumerable<Inscripcion> ObtenerInscripcionesTypeAhead(string inscripciones)
        {
            return this.db.Inscripciones
                            .Include(i => i.CarreraCurso)
                            .Where(i => ((i.Nombre + " " + i.Apellido).ToLower().Contains(inscripciones.ToLower())
                                        || (i.Apellido + " " + i.Nombre).ToLower().Contains(inscripciones.ToLower()))
                                        && !i.BajaCarreraFecha.HasValue
                                        && !i.CarreraCurso.Anulado
                                        )
                            .OrderBy(i => i.Apellido).ThenBy(i => i.Nombre);

        }

        public bool CobradoAutomaticamenteAlInscribir(Inscripcion inscripcion)
        {
            return this.db.Cobranzas.Any(c => c.InscripcionId == inscripcion.InscripcionId
                                                && !c.Anulado
                                                && c.Automatica);
        }
    }
}