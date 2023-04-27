using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Models
{
    public class Requisito
    {
        public int RequisitoId { get; set; }
        public string Descripcion { get; set; }
        public bool Anulado { get; set; }
    }
}