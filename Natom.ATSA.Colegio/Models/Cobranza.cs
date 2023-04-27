using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class Cobranza
    {
        public int CobranzaId { get; set; }

        public int? CobranzaPagoAdelantadoId { get; set; }
        public CobranzaPagoAdelantado CobranzaPagoAdelantado { get; set; }

        public int InscripcionId { get; set; }
        public Inscripcion Inscripcion { get; set; }

        public DateTime FechaHora { get; set; }

        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Monto { get; set; }
        public string Observaciones { get; set; }
        public bool Anulado { get; set; }

        public bool Automatica { get; set; }

        public bool Efectivo { get; set; }
    }
}