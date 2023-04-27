using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class Persona
    {
        public long ID { get; set; }
        public bool Activo { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Codigo_Postal { get; set; }
        public string CUIL { get; set; }
        public string Localidad { get; set; }
        public string Profesion { get; set; }
        public string Sexo { get; set; }
        public string Documento { get; set; }
        public string Telefono { get; set; }
        public string Domicilio { get; set; }
        public string Numero_Afiliado { get; set; }

        public int? Estado_Id { get; set; }

        public DateTime? Fecha_Afiliacion { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
    }
}