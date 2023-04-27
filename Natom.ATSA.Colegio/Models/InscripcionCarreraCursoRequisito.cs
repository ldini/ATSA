using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class InscripcionCarreraCursoRequisito
    {
        public int InscripcionCarreraCursoRequisitoId { get; set; }

        public int InscripcionId { get; set; }
        public Inscripcion Inscripcion { get; set; }

        public int CarreraCursoRequisitoId { get; set; }
        public CarreraCursoRequisito CarreraCursoRequisito { get; set; }

        public DateTime? FechaHoraEntregado { get; set; }
    }
}