using Natom.ATSA.Colegio.Models;
using Natom.ATSA.Colegio.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Natom.ATSA.Colegio.Managers
{
    public class CargaManager
    {
        private DbColegioContext db = new DbColegioContext();

        //public ResultadoImportacion ImportarRendicion(string importData)
        //{
        //    var result = new ResultadoImportacion();
        //    RendicionCabecera rendicion = null;

        //    try
        //    {
        //        int numLine = 0;
        //        string[] lines = importData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        //        foreach (var line in lines)
        //        {
        //            numLine++;
        //            if (numLine == 1)
        //            {
        //                rendicion = MapCabecera(line);
        //                rendicion.Detalle = new List<RendicionDetalle>();
        //                rendicion.UploadedBy = "admin";
        //                rendicion.UploadedDateTime = DateTime.Now;
        //                ValidarSiLaRendicionYaFueCargada(rendicion);
        //            }
        //            else
        //            {
        //                rendicion.Detalle.Add(MapDetalle(line));
        //            }
        //        }

        //        db.Rendiciones.Add(rendicion);
        //        db.SaveChanges();

        //        result.SinErrores = true;
        //        result.Mensaje = "Se han procesado " + rendicion.Detalle.Count + " registros correctamente.";
        //    }
        //    catch (Exception ex)
        //    {
        //        result.SinErrores = false;
        //        result.Mensaje = ex.Message;
        //    }

        //    return result;
        //}

        //public IEnumerable<ListarCargasResult> ObtenerCargasConFiltros(string search, int? clinicaId = null)
        //{
        //    IEnumerable<ListarCargasResult> query = this.db.Database.SqlQuery<ListarCargasResult>("CALL ListarCargas({0})", clinicaId);
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        int n;
        //        if (int.TryParse(search, out n))
        //        {
        //            query = query.Where(q => q.Numero == n);
        //        }
        //        else
        //        {
        //            search = search.ToLower();
        //            query = query.Where(q => (q.Mes + "/" + q.Anio).Equals(search)
        //                                        || q.Clinica.ToLower().Contains(search)
        //                                        || q.Estado.ToLower().Contains(search));
        //        }
        //    }
        //    return query;
        //}

        //public int ObtenerCantidadCargas()
        //{
        //    try
        //    {
        //        return this.db.Cargas.Count();
        //    }
        //    catch (Exception e)
        //    {

        //        throw e;
        //    }

        //}

        //private DateTime ObtenerFechaDeFormatoAAAAMMDD(string date)
        //{
        //    int año = Convert.ToInt32(date.Substring(0, 4));
        //    int mes = Convert.ToInt32(date.Substring(4, 2));
        //    int dia = Convert.ToInt32(date.Substring(6, 2));
        //    return new DateTime(año, mes, dia);
        //}

        //public Carga ObtenerCarga(int cargaId)
        //{
        //    return this.db.Cargas.FirstOrDefault(c => c.CargaId == cargaId);
        //}

        //private decimal ObtenerDecimalDeFormato8E2D(string dato)
        //{
        //    decimal retorno = Convert.ToDecimal(dato);
        //    retorno /= 100;
        //    return retorno;
        //}

        //public IEnumerable<Clinica> ObtenerClinicas(string clinica)
        //{
        //    return db.Clinicas.Where(x => x.Descripcion.ToLower().Contains(clinica.ToLower()));
        //}

        //public Carga ImportarExcel(Stream stream, int usuarioId, int mes, int anio, string clinica, int? clinicaId)
        //{
        //    Carga carga = new Carga();
        //    carga.Facturas = this.ObtenerFacturasDesdeExcel(stream);
        //    carga.Numero = (this.db.Cargas.Max(c => (int?)c.Numero) ?? 0) + 1;
        //    carga.CargadoFechaHora = DateTime.Now;
        //    carga.CargadoPorUsuarioId = usuarioId;
        //    carga.Mes = mes;
        //    carga.Anio = anio;

        //    Clinica dbClinica = this.db.Clinicas.FirstOrDefault(c => c.ClinicaId == clinicaId);
        //    if (dbClinica == null)
        //    {
        //        dbClinica = this.db.Clinicas.FirstOrDefault(c => c.Descripcion.ToLower().Trim().Equals(clinica.ToLower().Trim()));
        //    }
        //    if (dbClinica == null)
        //    {
        //        carga.Clinica = new Clinica();
        //        carga.Clinica.Descripcion = clinica;
        //    }
        //    else
        //    {
        //        carga.ClinicaId = dbClinica.ClinicaId;
        //    }
        //    this.db.Cargas.Add(carga);
        //    this.db.SaveChanges();

        //    return carga;
        //}

        public void EliminarCarga(int cargaId, int usuarioId, string motivo)
        {
            //Carga c = this.db.Cargas.Find(cargaId);
            //this.db.Entry(c).State = System.Data.Entity.EntityState.Modified;
            //c.AnuladoFechaHora = DateTime.Now;
            //c.AnuladoMotivo = motivo;
            //c.AnuladoPorUsuarioId = usuarioId;
            //this.db.SaveChanges();
        }

        //public void ValidarPeriodo(int mes, int anio, string clinica, int clinicaId)
        //{
        //    Clinica dbClinica = this.db.Clinicas.FirstOrDefault(c => c.ClinicaId == clinicaId);
        //    if (dbClinica == null)
        //    {
        //        dbClinica = this.db.Clinicas.FirstOrDefault(c => c.Descripcion.ToLower().Trim().Equals(clinica.ToLower().Trim()));
        //    }
        //    if (dbClinica == null)
        //    {
        //        throw new Exception("Debe seleccionar una clinica.");
        //    }
        //    else
        //    {
        //        if (this.db.Cargas.Any(c => !c.AnuladoFechaHora.HasValue
        //                                        && c.Mes == mes && c.Anio == anio
        //                                        && c.ClinicaId == dbClinica.ClinicaId))
        //        {
        //            throw new Exception("Ya existe una carga para la clínica y período seleccionado.");
        //        }
        //    }
        //}


    }
}