using Natom.ATSA.Colegio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Colegio.Managers
{
    public class AfiliadoManager
    {
        private DbAtsaContext db;

        public AfiliadoManager()
        {
            this.db = new DbAtsaContext();
        }

        public Persona ObtenerAfiliado(string numeroAfiliado)
        {
            return this.db.Personas.FirstOrDefault(p => p.Numero_Afiliado.Equals(numeroAfiliado) && p.Estado_Id == 2); //2: AFILIADO || 3: DESAFILIADO
        }

        public List<Persona> ObtenerFamiliares(long AfiliadoId)
        {
            return this.db.Database.SqlQuery<Persona>("SELECT F.* FROM persona P INNER JOIN Familiar R ON R.AFILIADO_ID = P.ID INNER JOIN persona F ON F.ID = R.FAMILIAR_ID WHERE P.ESTADO_ID = 2 AND P.ID = {0}", AfiliadoId).ToList();
        }
    }
}