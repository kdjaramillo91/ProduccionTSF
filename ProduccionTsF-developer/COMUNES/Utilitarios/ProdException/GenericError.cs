using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios.ProdException
{
    public static class GenericError
    {
        public const string ErrorGeneral = "Ha ocurrido un error no controlado al procesar la acción";
        public const string ErrorGeneralEgreso = "Ha ocurrido un error al procesar el Egreso";
        public const string ErrorGeneralEgresoTransferencia = "Ha ocurrido un error al procesar el Egreso por Transferencia";
        public const string ErrorGeneralEgresoTransferenciaVirtual = "Ha ocurrido un error al procesar el Egreso por Transferencia, Bodega Virtual";
        public const string ErrorGeneralIngresoOrdenCompra = "Ha ocurrido un error al procesar el Ingreso por Orden de Compra";
        public const string ErrorGeneralEgresoBodegaVirtual = "Ha ocurrido un error al procesar el Egreso De Bodega Virtual";
        public const string ErrorGeneralEgresoTransferenciaAuto = "Ha ocurrido un error al procesar el Egreso por Transferencia Automática";
        public const string ErrorGeneralIngresoTransferencia = "Ha ocurrido un error al procesar el Ingreso por Transferencia";
        public const string ErrorGeneralIngresoRawRecepcion = "Ha ocurrido un error al procesar el Ingreso de Materia Prima";
        public const string ErrorGeneralSaveRequerimientoInventario = "Ha ocurrido un error al Actualizar el Requerimiento de Inventario";
        public const string ErrorGeneralAproveeRequerimientoInventario = "Ha ocurrido un error al Aprobar el Requerimiento de Inventario";
        public const string ErrorGeneralTransferDispatchMaterials = "Ha ocurrido un error al procesar la Transferencia de Materiales de Despacho";
        public const string ErrorGeneralOpeningClosingPlateLying = "Ha ocurrido un error al procesar la Tumbada de Placa";
        public const string ErrorGeneralMastered = "Ha ocurrido un error al procesar el Masterizado";
        public const string ErrorGeneralInventoryMovePlantTransfer = "Ha ocurrido un error al procesar la Transferencia a Túneles";
        public const string ErrorGeneralLiquidationMaterial = "Ha ocurrido un error al procesar la Liquidación de Inveentario";
        public const string ErrorGeneralRecepcionMaterialesDespacho = "Ha ocurrido un error al procesar la Recepción de Materiales de Despacho ";

    }
}
