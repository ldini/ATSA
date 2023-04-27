using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class Gasto
    {
        public int GastoId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
        public int Tipo { get; set; }
        public bool Anulado { get; set; }
    }
}