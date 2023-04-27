using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models.Result
{
    public class ObtenerRequisitosPendientesDeEntregarResult
    {
		public int InscripcionId { get; set; }
		public string Nombre { get; set; }
		public string Apellido { get; set; }
		public string DNI { get; set; }
		public int CarreraCursoId { get; set; }
		public string Carrera { get; set; }
		public int RequisitoId { get; set; }
		public string Requisito { get; set; }
	}
}