using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.GuiaRemision
{
    public class InformacionGuiaRemision
    {
        ///<dirEstablecimiento>Ax</dirEstablecimiento>
        [XmlElement(ElementName = "dirEstablecimiento", Order = 1, IsNullable = false)]
        public string DireccionEstablecimiento { get; set; }

        /// <dirPartida>Ax</dirPartida>
        [XmlElement(ElementName = "dirPartida", Order = 2, IsNullable = false)]
        public string DireccionPartida { get; set; }

        ///<razonSocialTransportista>Ax</razonSocialTransportista>
        [XmlElement(ElementName = "razonSocialTransportista", Order = 3, IsNullable = false)]
        public string RazonSocialTransportista { get; set; }

        ///<tipoIdentificacionTransportista>05</tipoIdentificacionTransportista>
        [XmlElement(ElementName = "tipoIdentificacionTransportista", Order = 4, IsNullable = false)]
        public string TipoIdentificacionTransportista { get; set; }

        ///<rucTransportista>Ax</rucTransportista>
        [XmlElement(ElementName = "rucTransportista", Order = 5, IsNullable = false)]
        public string RucTransportista { get; set; }

        ///<rise>String</rise>
        [XmlElement(ElementName = "rise", Order = 6, IsNullable = false)]
        public string Rise { get; set; }

        ///<obligadoContabilidad>SI</obligadoContabilidad>
        [XmlElement(ElementName = "obligadoContabilidad", Order = 7, IsNullable = false)]
        public string ObligadoContabilidad { get; set; }

        ///<contribuyenteEspecial>123</contribuyenteEspecial>
        [XmlElement(ElementName = "contribuyenteEspecial", Order = 8, IsNullable = false)]
        public string ContribuyenteEspecial { get; set; }

        ///<fechaIniTransporte>01/01/1000</fechaIniTransporte>
        [XmlElement(ElementName = "fechaIniTransporte", Order = 9, IsNullable = false)]
        public string FechaInicioTransporte { get; set; }

        ///<fechaFinTransporte>01/01/1000</fechaFinTransporte>
        [XmlElement(ElementName = "fechaFinTransporte", Order = 10, IsNullable = false)]
        public string FechaFinTransporte { get; set; }

        ///<placa>Ax</placa>
        [XmlElement(ElementName = "placa", Order = 11, IsNullable = false)]
        public string Placa { get; set; }

        public InformacionGuiaRemision()
        {
            DireccionEstablecimiento = string.Empty;
            DireccionPartida = string.Empty;
            RazonSocialTransportista = string.Empty;
            TipoIdentificacionTransportista = string.Empty;
            RucTransportista = string.Empty;
            Rise = string.Empty;
            ObligadoContabilidad = string.Empty;
            ContribuyenteEspecial = string.Empty;
            FechaInicioTransporte = string.Empty;
            FechaFinTransporte = string.Empty;
            Placa = string.Empty;
        }

        public InformacionGuiaRemision(string direccionEstablecimiento, string direccionPartida, string razonSocialTransportista,
            string tipoIdentificacionTransportista, string rucTransportista, string rise, string obligadoContabilidad,
            string contribuyenteEspecial, string fechaInicioTransporte, string fechaFinTransporte, string placa)
        {
            DireccionEstablecimiento = direccionEstablecimiento;
            DireccionPartida = direccionPartida;
            RazonSocialTransportista = razonSocialTransportista;
            TipoIdentificacionTransportista = tipoIdentificacionTransportista;
            RucTransportista = rucTransportista;
            Rise = rise;
            ObligadoContabilidad = obligadoContabilidad;
            ContribuyenteEspecial = contribuyenteEspecial;
            FechaInicioTransporte = fechaInicioTransporte;
            FechaFinTransporte = fechaFinTransporte;
            Placa = placa;
        }
    }
}