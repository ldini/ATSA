using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class CicloLectivo
    {
        public int CicloLectivoId { get; set; }
        public string Descripcion { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public DateTime InscripcionHabilitadaDesde { get; set; }
        public DateTime InscripcionHabilitadaHasta { get; set; }

        public int CobranzaInicioMes { get; set; }
        public int CobranzaInicioAnio { get; set; }
        public int CobranzaFinMes { get; set; }
        public int CobranzaFinAnio { get; set; }

        public decimal? ImporteDefault { get; set; }

        public bool Cerrado { get; set; }
        public bool Anulado { get; set; }

        [NotMapped]
        public bool InscripcionHabilitada
        {
            get
            {
                return DateTime.Now >= this.InscripcionHabilitadaDesde && DateTime.Now <= this.InscripcionHabilitadaHasta;
            }
        }

        [NotMapped]
        public bool CursadaHabilitada
        {
            get
            {
                return DateTime.Now >= this.FechaInicio && DateTime.Now <= this.FechaFin;
            }
        }

        [NotMapped]
        public string Estado
        {
            get
            {
                string estado = "";
                if (DateTime.Now < this.FechaInicio && DateTime.Now < this.InscripcionHabilitadaDesde)
                {
                    estado = "PROGRAMADO";
                }
                else if (DateTime.Now > this.FechaFin && DateTime.Now > this.InscripcionHabilitadaHasta)
                {
                    estado = "CUMPLIDO";
                }
                else
                {
                    if (this.InscripcionHabilitada) estado = "INSCRIPCIÓN";
                    if (this.CursadaHabilitada) estado = "EN CURSO";
                }
                return estado;
            }
        }
    }
}