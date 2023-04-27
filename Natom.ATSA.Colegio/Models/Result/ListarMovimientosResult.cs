using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models.Result
{
    public class ListarMovimientosResult
    {
        public DateTime FechaHora { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public int Signo { get; set; }
    }
}