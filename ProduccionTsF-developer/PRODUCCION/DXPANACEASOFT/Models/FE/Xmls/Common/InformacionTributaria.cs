using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.Common
{
    public class InformacionTributaria
    {
        ///<ambiente>1</ambiente>
        [XmlElement(ElementName = "ambiente", Order = 1, IsNullable = false)]
        public int Ambiente { get; set; }

        ///<tipoEmision>1</tipoEmision>
        [XmlElement(ElementName = "tipoEmision", Order = 2, IsNullable = false)]
        public int TipoEmision { get; set; }

        ///<razonSocial>Ax</razonSocial>
        [XmlElement(ElementName = "razonSocial", Order = 3, IsNullable = false)]
        public string RazonSocial { get; set; }

        ///<nombreComercial>Ax</nombreComercial>
        [XmlElement(ElementName = "nombreComercial", Order = 4, IsNullable = true)]
        public string NombreComercial { get; set; }

        ///<ruc>0000000000001</ruc>
        [XmlElement(ElementName = "ruc", Order = 5, IsNullable = false)]
        public string RUC { get; set; }

        ///<claveAcceso>0000000000000000000000000000000000000000000000000</claveAcceso>
        [XmlElement(ElementName = "claveAcceso", Order = 6, IsNullable = false)]
        public string ClaveAcceso { get; set; }

        ///<codDoc>00</codDoc>
        [XmlElement(ElementName = "codDoc", Order = 7, IsNullable = false)]
        public string CodigoDocumento { get; set; }

        ///<estab>000</estab>
        [XmlElement(ElementName = "estab", Order = 8, IsNullable = false)]
        public string Establecimiento { get; set; }

        ///<ptoEmi>000</ptoEmi>
        [XmlElement(ElementName = "ptoEmi", Order = 9, IsNullable = false)]
        public string PuntoEmision { get; set; }

        ///<secuencial>000000000</secuencial>
        [XmlElement(ElementName = "secuencial", Order = 10)]
        public string Secuencial { get; set; }

        ///<dirMatriz>Ax</dirMatriz>
        [XmlElement(ElementName = "dirMatriz", Order = 11, IsNullable = false)]
        public string DireccionMatriz { get; set; }

        public InformacionTributaria()
        {
            Ambiente = 1;
            TipoEmision = 1;
            RazonSocial = string.Empty;
            NombreComercial = string.Empty;
            RUC = string.Empty;
            ClaveAcceso = string.Empty;
            CodigoDocumento = string.Empty;
            Establecimiento = string.Empty;
            PuntoEmision = string.Empty;
            Secuencial = string.Empty;
            DireccionMatriz = string.Empty;
        }

        public InformacionTributaria(int ambiente, int tipoEmision, string razonSocial, string nombreComercial, 
            string ruc, string codigoDocumento, string establecimiento, int puntoEmision, int secuencial,
            string direccionMatriz, string claveAcceso)
        {
            Ambiente = ambiente;
            TipoEmision = tipoEmision;
            RazonSocial = razonSocial;
            NombreComercial = nombreComercial;
            RUC = ruc;            
            CodigoDocumento = codigoDocumento;
            Establecimiento = establecimiento.PadLeft(3,'0');
            PuntoEmision = puntoEmision.ToString("D3");
            Secuencial = secuencial.ToString("D9");
            DireccionMatriz = direccionMatriz;
            ClaveAcceso = claveAcceso;
            
        }

    }
}
