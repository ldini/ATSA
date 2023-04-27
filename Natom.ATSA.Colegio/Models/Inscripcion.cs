using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class Inscripcion
    {
        public int InscripcionId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public int? Edad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string LugarNacimiento { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }

        public string Afiliado { get; set; }

        public string Recibo { get; set; }

        public int CarreraCursoId { get; set; }
        public CarreraCurso CarreraCurso { get; set; }

        public int? CicloLectivoId { get; set; }
        public CicloLectivo CicloLectivo { get; set; }

        public DateTime? AltaFecha { get; set; }

        public DateTime? BajaCarreraFecha { get; set; }
        public string BajaMotivo { get; set; }

        public string Sexo { get; set; }
        public string EstadoCivil { get; set; }
        public string TrabajoCargoFuncion { get; set; }
        public string TrabajoAntiguedad { get; set; }
        public string TrabajoRegionSanitaria { get; set; }
        public string TrabajoDependencia { get; set; }
        public string TrabajoDireccionLaboral { get; set; }
        public string TrabajoLocalidad { get; set; }
        public string TrabajoTelefono { get; set; }
        public string TrabajoEmail { get; set; }

        public List<InscripcionEntregaDocumentacion> RequisitosEntregados { get; set; }
    }
}