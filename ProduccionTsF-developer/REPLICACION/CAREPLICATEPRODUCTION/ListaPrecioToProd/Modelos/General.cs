using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAREPLICATEPRODUCTION.ListaPrecioToProd.Modelos
{
    public class Calendario
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int idTipoCalendarioLista { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public int idCompania { get; set; }
        public bool activo { get; set; }
    }

    public class GrupoPersonaRol
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int idCompania { get; set; }
        public int idRol { get; set; }
        public string nombreRol { get; set; }
        public bool activo { get; set; }
    }
    public class GrupoPersonaRolDetalle
    {
        public int idGrupo { get; set; }
        public string nombreGrupo { get; set; }
        public int idPersona { get; set; }
        public string identificacionPersona { get; set; }
    }

    public class PersonaListaDetalle
    {
        public int idPersona { get; set; }
        public string identificacionPersona { get; set; }
        public int idRol { get; set; }
        public string nombreRol { get; set; }
    }

    public class TipoCalendario
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int idCompania { get; set; }
        public bool activo { get; set; }
    }
    public class ListaPrecio
    {
        public int idLista { get; set; }
        public string nombreLista { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public bool esCompra { get; set; }
        public bool esVenta { get; set; }
        public bool esCotizacion { get; set; }
        public int idListaPrecioCalendario { get; set; }
        public int? idProveedor { get; set; }
        public int? idCliente { get; set; }
        public bool? esGrupo { get; set; }
        public int? idGrupoPersonaRol { get; set; }
        public string codCertificacion { get; set; }
        public int? idCertificacion { get; set; }
        public int? idListaBase { get; set; }
        public string nombreListaBase { get; set; }
        public int? id_company { get; set; }
        public int? idTipoProceso { get; set; }
        public string codigoTipoProceso { get; set; }
        public DateTime? commercialDate { get; set; }
        public int? idUsuarioResponsable { get; set; }
        public string nombreUsuarioResponsable { get; set; }
        public int? idUsuarioCreacion { get; set; }
        public string nombreUsuarioCreacion { get; set; }
        public DateTime? dtUsuarioCreacion { get; set; }
        public int? idUsuarioModificacion { get; set; }
        public string nombreUsuarioModificacion { get; set; }
        public DateTime? dtUsuarioModificacion { get; set; }
    }
    public class Documento
    {
        public int idDocument { get; set; }
        public string numero { get; set; }
        public int sequencial { get; set; }
        public DateTime fechaEmision { get; set; }
        public DateTime? fechaAutorizacion { get; set; }
        public string numeroAutorizacion { get; set; }
        public string claveAcceso{ get; set; }
        public string descripcion { get; set; }
        public string referencia { get; set; }
        public int idPuntoEmision { get; set; }
        public int idTipoDocumento { get; set; }
        public string codigoTipoDocumento { get; set; }
        public int idEstadoDocumento { get; set; }
        public string nombreEstadoDocumento { get; set; }
        public int? idBaseDocumento { get; set; }
    }

    public class DocumentoCambioEstado
    {
        public int idDocumentoOrigen { get; set; }
        public int idEstadoDocumentOldSource { get; set; }
        public string codigoEstadoDocumentoOldSource { get; set; }
        public string nombreEstadoDocumentoOldSource { get; set; }
        public int idEstadoDocumentNewSource { get; set; }
        public string codigoEstadoDocumentoNewSource { get; set; }
        public string nombreEstadoDocumentoNewSource { get; set; }
        public int idUsuarioSource { get; set; }
        public string nombreUsuarioSource { get; set; }
        public int idUsuarioGrupoSource { get; set; }
        public string nombreUsuarioGrupoSource { get; set; }
        public DateTime changeTimeSource { get; set; }
    }
    public class ListaPrecioDetalle
    {
        public int id { get; set; }
        public int idTalla { get; set; }
        public string codigoTalla { get; set; }
        public decimal valor { get; set; }
        public decimal comision { get; set; }
        public int idClaseCamaron { get; set; }
        public string codigoClaseCamaron { get; set; }
        public int idCalidad { get; set; }
        public string codigoCalidad { get; set; }

    }
    public class ClasePenalizacion
    {
        public int id { get; set; }
        public int idClaseCamaron { get; set; }
        public string codigoClaseCamaron { get; set; }
        public decimal valorClaseCamaron { get; set; }
    }
    public class ListaSourceDTO
    {
        public ListaPrecio Lista { get; set; }

        public List<ListaPrecioDetalle> Detalle { get; set; }

        public List<ClasePenalizacion> DetallePenalizacion { get; set; }

        public Calendario CalendarioLista { get; set; }

        public TipoCalendario TipoCalendarioLista { get; set; }

        public GrupoPersonaRol GrupoPersonaRolLista { get; set; }

        public List<GrupoPersonaRolDetalle> lstGrupoPersonaRolDetalle { get; set; }

        public Documento ListaDocumento { get; set; }

        public List<DocumentoCambioEstado> CambioEstadoDocumento { get; set; }

        public List<PersonaListaDetalle> lstPersonaDetalle { get; set; }
    }
}
