using System.Collections.Generic;

namespace Natom.ATSA.Colegio.Models
{
    public class CobranzaPagoAdelantado
    {
        public int CobranzaPagoAdelantadoId { get; set; }
        
        public int InscripcionId { get; set; }
        public Inscripcion Inscripcion { get; set; }

        public decimal Monto { get; set; }
        public string Observaciones { get; set; }
        public bool Anulado { get; set; }

        public List<Cobranza> Cobranzas { get; set; }
    }
}