using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class InscripcionEntregaDocumentacion
    {
        public int InscripcionEntregaDocumentacionId { get; set; }
        public int InscripcionId { get; set; }
        public int RequisitoId { get; set; }
        public DateTime FechaHora { get; set; }
    }
}