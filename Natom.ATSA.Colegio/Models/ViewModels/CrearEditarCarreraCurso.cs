using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models.ViewModels
{
    public class CrearEditarCarreraCurso
    {
        public CrearEditarCarreraCurso()
        {
            Requisitos = new List<RequisitosAplicados>();
        }
        public int CarreraCursoId { get; set; }

        public int CarreraCursoTipoId { get; set; }

        public int Duracion { get; set; }

        public int TipoDuracionId { get; set; }

        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Horarios { get; set; }

        public bool Anulado { get; set; }

        public List<RequisitosAplicados> Requisitos { get; set; }
    }

    public class RequisitosAplicados
    {
        public RequisitosAplicados()
        {
            Elimina = true;
        }
        public int RequisitoId { get; set; }

        public bool? Elimina { get; set; }
    }
}