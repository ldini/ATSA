using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class CarreraCursoRequisito
    {
        public int CarreraCursoRequisitoId { get; set; }

        public int CarreraCursoId { get; set; }
        public CarreraCurso CarreraCurso { get; set; }

        public int RequisitoId { get; set; }
        public Requisito Requisito { get; set; }

        public bool EsExcluyente { get; set; }
    }
}