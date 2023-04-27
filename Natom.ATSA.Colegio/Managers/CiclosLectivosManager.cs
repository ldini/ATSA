using Natom.ATSA.Colegio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Managers
{
    public class CiclosLectivosManager
    {
        DbColegioContext db = new DbColegioContext();

        public List<CicloLectivoCalculado> CalcularCiclosLectivos(Inscripcion inscripcion)
        {
            var ciclos = new List<CicloLectivoCalculado>();
            var aniosDuracion = inscripcion.CarreraCurso.TipoDuracionId == 2 ? inscripcion.CarreraCurso.Duracion : 1;
            if (aniosDuracion == 1)
            {
                ciclos.Add(new CicloLectivoCalculado
                {
                    Descripcion = $"({inscripcion.AltaFecha.Value.Year}) Único año",
                    Anio = inscripcion.AltaFecha.Value.Year
                });
            }
            else
            {
                var ciclosStr = new List<string>() { "1er", "2do", "3er", "4to", "5to", "6to", "7mo", "8vo", "9no", "10mo", "", "", "", "", "" };
                var ciclo = 0;
                var anioDesde = inscripcion.AltaFecha.Value.Year;
                var anioHasta = inscripcion.AltaFecha.Value.Year + aniosDuracion;
                for (int anio = anioDesde; anio <= anioHasta && anio <= DateTime.Now.Year; anio++)
                {
                    ciclos.Add(new CicloLectivoCalculado
                    {
                        Descripcion = $"({anio}) {ciclosStr[ciclo]} año",
                        Anio = anio
                    });
                    ciclo++;
                }
            }
            return ciclos;
        }

        public string CalcularCicloLectivo(int inscripcionId, int anio)
        {
            var inscripcion = this.db.Inscripciones
                                        .Include("CarreraCurso")
                                        .FirstOrDefault(i => i.InscripcionId == inscripcionId);

            var aniosDuracion = inscripcion.CarreraCurso.TipoDuracionId == 2 ? inscripcion.CarreraCurso.Duracion : 1;
            if (aniosDuracion == 1)
            {
                if (inscripcion.AltaFecha.Value.Year < anio)
                    return "COMPLETADO";
                else
                    return $"({anio}) Único año";
            }
            else
            {
                var ciclosStr = new List<string>() { "1er", "2do", "3er", "4to", "5to", "6to", "7mo", "8vo", "9no", "10mo", "", "", "", "", "" };
                var anioDesde = inscripcion.AltaFecha.Value.Year;
                var anioHasta = inscripcion.AltaFecha.Value.Year + aniosDuracion;

                if (anioHasta < anio)
                    return "COMPLETADO";
                else
                {
                    var ciclo = anio - anioDesde;
                    return $"({anio}) {ciclosStr[ciclo]} año";
                }
            }
        }

        public string CalcularCicloLectivoActual(Inscripcion inscripcion)
        {
            var aniosDuracion = inscripcion.CarreraCurso.TipoDuracionId == 2 ? inscripcion.CarreraCurso.Duracion : 1;
            if (aniosDuracion == 1)
            {
                if (inscripcion.AltaFecha.Value.Year < DateTime.Now.Year)
                    return "COMPLETADO";
                else
                    return $"Cursando único año";
            }
            else
            {
                var ciclosStr = new List<string>() { "1er", "2do", "3er", "4to", "5to", "6to", "7mo", "8vo", "9no", "10mo", "", "", "", "", "" };
                var anioDesde = inscripcion.AltaFecha.Value.Year;
                var anioHasta = inscripcion.AltaFecha.Value.Year + aniosDuracion;

                if (anioHasta < DateTime.Now.Year)
                    return "COMPLETADO";
                else
                {
                    var ciclo = DateTime.Now.Year - anioDesde;
                    return $"Cursando {ciclosStr[ciclo]} año";
                }
            }
        }

        public IEnumerable<CicloLectivo> ObtenerCiclosLectivos(string search)
        {
            IEnumerable<CicloLectivo> query = this.db.CiclosLectivos.Where(x => x.Anulado == false);
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(q => q.Descripcion.ToLower().Contains(search));
            }
            return query;
        }

        public int ObtenerCantidadCiclosLectivos()
        {
            return db.CiclosLectivos.Where(x => x.Anulado == false).Count();
        }

        public void EliminarCicloLectivo(int CicloLectivoId)
        {
            var r = db.CiclosLectivos.FirstOrDefault(x => x.CicloLectivoId == CicloLectivoId);
            r.Anulado = true;

            db.Entry<CicloLectivo>(r).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public void EditarCicloLectivo(CicloLectivo CicloLectivo)
        {
            if (this.db.CiclosLectivos.Any(ca => !ca.Anulado && !ca.CicloLectivoId.Equals(CicloLectivo.CicloLectivoId) && ca.Descripcion.ToUpper().Equals(CicloLectivo.Descripcion.ToUpper())))
            {
                throw new Exception("Ya existe un ciclo lectivo con misma Descripción.");
            }

            ValidarDatosCicloLectivo(CicloLectivo);

            var r = db.CiclosLectivos.FirstOrDefault(x => x.CicloLectivoId == CicloLectivo.CicloLectivoId);
            r.Descripcion = CicloLectivo.Descripcion;
            r.FechaInicio = CicloLectivo.FechaInicio;
            r.FechaFin = CicloLectivo.FechaFin;
            r.CobranzaInicioMes = CicloLectivo.CobranzaInicioMes;
            r.CobranzaInicioAnio = CicloLectivo.CobranzaInicioAnio;
            r.CobranzaFinMes = CicloLectivo.CobranzaFinMes;
            r.CobranzaFinAnio = CicloLectivo.CobranzaFinAnio;
            r.Cerrado = CicloLectivo.Cerrado;
            r.InscripcionHabilitadaDesde = CicloLectivo.InscripcionHabilitadaDesde;
            r.InscripcionHabilitadaHasta = CicloLectivo.InscripcionHabilitadaHasta;
            r.ImporteDefault = CicloLectivo.ImporteDefault;
            r.Anulado = false;

            db.Entry<CicloLectivo>(r).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public CicloLectivo CrearCicloLectivo(CicloLectivo CicloLectivo)
        {
            if (this.db.CiclosLectivos.Any(ca => !ca.Anulado && ca.Descripcion.ToUpper().Equals(CicloLectivo.Descripcion.ToUpper())))
            {
                throw new Exception("Ya existe un ciclo lectivo con misma Descripción.");
            }

            ValidarDatosCicloLectivo(CicloLectivo);

            db.CiclosLectivos.Add(CicloLectivo);
            db.SaveChanges();

            return CicloLectivo;
        }

        private void ValidarDatosCicloLectivo(CicloLectivo CicloLectivo)
        {
            if (CicloLectivo.FechaInicio > CicloLectivo.FechaFin)
            {
                throw new Exception("Período de cursada: Fecha 'Desde' no puede ser superior a 'Hasta'.");
            }

            if (CicloLectivo.InscripcionHabilitadaDesde > CicloLectivo.InscripcionHabilitadaHasta)
            {
                throw new Exception("Período de inscripción: Fecha 'Desde' no puede ser superior a 'Hasta'.");
            }

            int cobranzaInicio = CicloLectivo.CobranzaInicioAnio * 12 + CicloLectivo.CobranzaInicioMes;
            int cobranzaFin = CicloLectivo.CobranzaFinAnio * 12 + CicloLectivo.CobranzaFinMes;
            if (cobranzaInicio > cobranzaFin)
            {
                throw new Exception("Período de cobranza: Mes y año 'Desde' son superiores a 'Hasta'.");
            }
        }

        public CicloLectivo GetCicloLectivo(int CicloLectivoId)
        {
            return db.CiclosLectivos.FirstOrDefault(x => x.CicloLectivoId == CicloLectivoId);
        }

        public List<CicloLectivo> GetCiclosLectivos()
        {
            return db.CiclosLectivos.Where(x => x.Anulado == false).ToList();
        }

        public List<CicloLectivo> GetCiclosLectivosVigentes()
        {
            DateTime hoy = DateTime.Now;
            return db.CiclosLectivos.Where(x => x.Anulado == false
                                                && x.InscripcionHabilitadaDesde <= hoy
                                                && x.InscripcionHabilitadaHasta >= hoy).ToList();
        }

        public CicloLectivo ObtenerCicloLectivo(int cicloLectivoId)
        {
            return this.db.CiclosLectivos.Find(cicloLectivoId);
        }
    }
}