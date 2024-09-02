using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Auxiliares
{
    public class EnumIntegrationProcess
    {

        public static class States
        {
            public const string Create = "01";
            public const string Approved = "02";
            public const string Transmitted = "03";
            public const string Process = "04";
            public const string PartialProcess = "05";
            public const string Reject = "06";

            public const string Added = "07";
            public const string Deleted = "08";

        }

        public static class ParametersIntegration
        {
            public const string SourceReference = "INTG";
            public const string SourceMessage = "INMSG";
            
        }

        public static class SourceReference
        {
            public const string LoteIntegracion = "LOINT";
            public const string DetalleLoteIntegracion= "LOIDT"; 
            public const string RegistroSalidaIntegracion= "LOOUT";   
            public const string DetalleSalidaIntegracion= "LODUT"; 
        }
        public static class SourceMessage
        {
            public const string CreacionRegistrosSalida = "LMRSL";
            public const string ValidacionTransferencia = "LMTDC";
            public const string RetornoErroresIntegracion = "LMEIN";
        }

        public static class CodeIntegrationProcess
        {
            public const string AnticipoProveedorCamaron    = "APC";
            public const string AnticipoViaticoTransporte   = "VAT";
            public const string AnticipoTransportista       = "ANT";
            public const string ProvisionTransportista      = "PPT";
            public const string ProvisionProveedorCamaron = "PPC";

            public const string FacturaFiscal = "FAF";
            public const string FacturaComercial = "FAC";
            // Facturacion Materiales e Insumos
            public const string FacturaMaterialesInsumos = "FMA";
        }

        public static class ForeingTables
        {
            public const string FormasPago = "MPAGO";
            public const string PlazosPago = "PPAGO";
            public const string Vendedores = "VENDOR";
            public const string Bancos = "BANCOS";

            
        }
    }


}