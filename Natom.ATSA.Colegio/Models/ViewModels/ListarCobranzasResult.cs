using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models.ViewModels
{
    public class ListarCobranzasResult
    {
        public int CarreraCursoId { get; set; }
        public int InscripcionId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Afiliado { get; set; }
        public DateTime? BajaCarreraFecha { get; set; }
        public string CarreraCurso { get; set; }
        public string Descripcion { get; set; }
        public string Periodo { get; set; }
        public string Recibo { get; set; }
        public int? CobranzaId { get; set; }
        public string Estado { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public bool DebeDocumentacion { get; set; }
    }
}