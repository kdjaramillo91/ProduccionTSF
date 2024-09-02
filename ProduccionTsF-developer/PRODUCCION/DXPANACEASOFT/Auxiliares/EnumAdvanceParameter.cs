using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Auxiliares
{
    public class EnumAdvanceParameter
    {

        public static class Codes
        {
            public const string InventoryMovePackagingMaterials = "IPXM";
            public const string IntegrationProcessInvoiceCommercialParams = "INFCP";


        }

        public static class InventoryMovePackagingMaterialsCodes
        {

            public const string InventoryReason = "INRS";
            public const string DocumentType = "DTIM";
        }

        public static class IntegrationProcessInvoiceCommercialParamsCodes
        {
            public const string CodigoCompania = "FCCIA";
            public const string CodigoDivision = "FCDIV";
            public const string CodigoSucursal = "FCSUC";
            public const string SistemaDestino = "SIDES";
        }

    }
}