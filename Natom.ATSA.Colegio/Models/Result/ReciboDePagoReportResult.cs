using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models.Result
{
    public class ReciboDePagoReportResult
    {
        public string FechaDia { get; internal set; }
        public string FechaMes { get; internal set; }
        public string FechaAnio { get; internal set; }
        public string Senior { get; internal set; }
        public string Domicilio { get; internal set; }
        public string Concepto { get; internal set; }
        public string ConceptoImporte { get; internal set; }
        public string Efectivo { get; internal set; }
        public string Total { get; internal set; }
        public string DNI { get; internal set; }
    }
}