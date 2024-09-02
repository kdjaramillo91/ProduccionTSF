using System.Collections.Generic;
using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.GuiaRemision
{
    public class Destinatario
    {
        ///<identificacionDestinatario>Ax</identificacionDestinatario>
        [XmlElement(ElementName = "identificacionDestinatario", Order = 1, IsNullable = false)]
        public string IdentificacionDestinatario { get; set; }

        ///<razonSocialDestinatario>Ax</razonSocialDestinatario>
        [XmlElement(ElementName = "razonSocialDestinatario", Order = 2, IsNullable = false)]
        public string RazonSocialDestinatario { get; set; }

        ///<dirDestinatario>Ax</dirDestinatario>
        [XmlElement(ElementName = "dirDestinatario", Order = 3, IsNullable = false)]
        public string DireccionDestinatario { get; set; }

        ///<motivoTraslado>Ax</motivoTraslado>
        [XmlElement(ElementName = "motivoTraslado", Order = 4, IsNullable = false)]
        public string MotivoTraslado { get; set; }

        ///<docAduaneroUnico>Ax</docAduaneroUnico>
        [XmlElement(ElementName = "docAduaneroUnico", Order = 5, IsNullable = false)]
        public string DocumentoAduaneroUnico { get; set; }

        ///<codEstabDestino>000</codEstabDestino>
        [XmlElement(ElementName = "codEstabDestino", Order = 6, IsNullable = false)]
        public string CodigoEstablecimientoDestino { get; set; }

        ///<ruta>Ax</ruta>
        [XmlElement(ElementName = "ruta", Order = 7, IsNullable = false)]
        public string Ruta { get; set; }

        ///<codDocSustento>00</codDocSustento>
        [XmlElement(ElementName = "codDocSustento", Order = 8, IsNullable = false)]
        public string CodigoDocumentoSustento { get; set; }

        ///<numDocSustento>000-000-000000000</numDocSustento>
        [XmlElement(ElementName = "numDocSustento", Order = 9, IsNullable = false)]
        public string NumeroDocumentoSustento { get; set; }

        ///<numAutDocSustento>0000000000</numAutDocSustento>
        [XmlElement(ElementName = "numAutDocSustento", Order = 10, IsNullable = false)]
        public string NumeroAutorizacionDocumentoSustento { get; set; }

        ///<fechaEmisionDocSustento>01/01/1000</fechaEmisionDocSustento>
        [XmlElement(ElementName = "fechaEmisionDocSustento", Order = 11, IsNullable = false)]
        public string FechaEmisionDocumentoSustento { get; set; }

        [XmlArray(ElementName = "detalles", Order = 12, IsNullable = true)]
        [XmlArrayItem(ElementName = "detalle", IsNullable = true)]
        public List<Detalle> Detalles { get; set; }

        public Destinatario()
        {
            IdentificacionDestinatario = string.Empty;
            RazonSocialDestinatario = string.Empty;
            DireccionDestinatario = string.Empty;
            MotivoTraslado = string.Empty;
            DocumentoAduaneroUnico = string.Empty;
            CodigoEstablecimientoDestino = string.Empty;
            Ruta = string.Empty;
            CodigoDocumentoSustento = string.Empty;
            NumeroDocumentoSustento = string.Empty;
            NumeroAutorizacionDocumentoSustento = string.Empty;
            FechaEmisionDocumentoSustento = string.Empty;
            Detalles = new List<Detalle>();
        }

        public Destinatario(string identificacionDestinatario, string razonSocialDestinatario, string direccionDestinatario,
            string motivoTraslado, string documentoAduaneroUnico, string codigoEstablecimientoDestino, string ruta,
            string codigoDocumentoSustento, string numeroDocumentoSustento, string numeroAutorizacionDocumentoSustento,
            string fechaEmisionDocumentoSustento, List<Detalle> detalles)
        {
            IdentificacionDestinatario = identificacionDestinatario;
            RazonSocialDestinatario = razonSocialDestinatario;
            DireccionDestinatario = direccionDestinatario;
            MotivoTraslado = motivoTraslado;
            DocumentoAduaneroUnico = documentoAduaneroUnico;
            CodigoEstablecimientoDestino = codigoEstablecimientoDestino;
            Ruta = ruta;
            CodigoDocumentoSustento = codigoDocumentoSustento;
            NumeroDocumentoSustento = numeroDocumentoSustento;
            NumeroAutorizacionDocumentoSustento = numeroAutorizacionDocumentoSustento;
            FechaEmisionDocumentoSustento = fechaEmisionDocumentoSustento;
            Detalles = detalles;
        }

    }
}