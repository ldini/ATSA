using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class CarreraCurso
    {
        public CarreraCurso()
        {
            Requisitos = new List<CarreraCursoRequisito>();
        }
        public int CarreraCursoId { get; set; }

        public int CarreraCursoTipoId { get; set; }
        public CarreraCursoTipo CarreraCursoTipo { get; set; }

        public int Duracion { get; set; }

        public int TipoDuracionId { get; set; }
        public TipoDuracion TipoDuracion { get; set; }

        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Horarios { get; set; }

        public bool Anulado { get; set; }

        public List<CarreraCursoRequisito> Requisitos { get; set; }
    }
}