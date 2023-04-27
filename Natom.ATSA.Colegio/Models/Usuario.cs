using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
	    public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Clave { get; set; }

        public DateTime FechaHoraAlta { get; set; }
        public DateTime? FechaHoraBaja { get; set; }
        public string Token { get; set; }
    }
}