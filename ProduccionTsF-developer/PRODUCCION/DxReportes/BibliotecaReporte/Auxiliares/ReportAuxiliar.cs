using BibliotecaReporte.Model;
using Dapper;

namespace BibliotecaReporte.Auxiliares
{
    internal class ReportAuxiliar
    {
        #region Compras

        internal const string m_rptOrdenCompraCamaron = "ROCP";
        internal const string m_rptRequerimientoCompra = "REQCMP";

        #endregion Compras

        #region PruebaLogo

        //internal const string m_rptLogoPrueba = "RPFM";

        #endregion PruebaLogo

        #region FacturasComercial

        internal const string m_rptFacturaComercial = "RFCLV1";
        internal const string m_rptFacturaComercialExcel = "FECEXC";
        #endregion FacturasComercial

        #region FacturaExterior
        internal const string m_rptFacturaElectronica = "IEFE1";
        internal const string m_rptPackinglist = "PAKINL";
        internal const string m_rptFacturaFiscal = "RFFLV1";
        internal const string m_rptISF = "FEISF";
        internal const string m_rptNooWood = "NONWOD";
        internal const string m_rptFacturaElectronicaPropia = "IEFEP";
        internal const string m_rptFacturaElectronicaPropiaExcel = "FACPE";
        internal const string m_rptFactExteriorBL = "FEBL";
        internal const string m_rptTemperaturaF = "FETEM";
        internal const string m_rptFactPesos = "FACPES";
        #endregion FacturaExterior

        #region OrdenesDeProducción
        internal const string m_rptOrdenesProduccion = "RPTOP";
        internal const string m_rptOrdenesProduccionVentas = "RVENOP";
        internal const string m_rptOrdenesProduccionMaterial = "RPTOPD";
        #endregion
        #region AnalisisCalidad
        internal const string m_rptAnalisisCalidad = "QUALIT";
        #endregion

        #region Inventario
        internal const string m_rptIngreso = "IDGV9";
        internal const string m_rptTransferEgreso = "IDMET";
        internal const string m_rptTransferIngreso = "IDMIT";
        internal const string m_rptMovimientoInventarioEgresoTransferenciaA = "TRAEG";
        internal const string m_rptMovimientoInventarioIngresoTransferenciaA = "TRAIN";
        internal const string m_rptMovimientoInventarioMotivo = "IDGVM";
        internal const string m_rptKardex= "IKSPV1";
        internal const string m_rptKardexLote ="IKSPVL";
        internal const string m_rptKardexCosto ="KEDST";
        internal const string m_rptSaldos = "ISPV1";
        internal const string m_rptSaldosLote = "ISPVL";
        internal const string m_rptMovimientos = "IMIPV1";
        internal const string m_rptMovimientosLote = "IMIPVL";
        internal const string m_rptKardexProveedor = "IKRPV";
        internal const string m_rptKardexProveedorSaldo = "IKRPS";
        internal const string m_rptSaldosSinLote = "ISPWVL";
        internal const string m_rptSaldosSinLoteTodos = "ISPWVT";
        #endregion Inventario

        #region Logistica
        internal const string m_rptGuiaRemisionPers = "LGRRP1";
        internal const string m_rptLogisticaGuias = "GRLTF";
        internal const string m_rptLogisticaGuiasExcel = "GRLTFE";
        internal const string m_rptRotacionTrans = "LFRTR";
        internal const string m_rptRotacionTransFluvial = "LFRTRF";
        internal const string m_rptGuiaRide = "RIDE";
        internal const string m_rptRequisionBodega = "GRDM1";
        internal const string m_rptGuiaRemisionPersFluvial = "RGRGRF";
        internal const string m_rptFleteTerrestre = "LRFT1";
        internal const string m_rptFleteFluvial = "LRFF1";
        internal const string m_rptGuiaRemisionViaticoFluvial = "LGRVF";
        internal const string m_rptLiquidacionMaterialesGeneral = "LMGPG";
        internal const string m_rptLiquidacionMateriales = "LMIPG";
        internal const string m_rptMovimientoInventarioTodos = "IDGV1T";
        internal const string m_rptMovimientoInventario = "IDGV1";
        internal const string m_rptRequerimientoInventario = "RMIRFE";

        #endregion Logistica
        #region Proceso
        internal const string m_rptProcesoInternoResumen = "PIRES";
        internal const string m_rptProcesoInternoDetallado = "PIDET";
        internal const string m_rptResumenDeReproceso = "PIRRP";
        internal const string m_rptDetalleDeReproceso = "PIDRP";
        internal const string m_rptMovimientoInventarioIngreso = "INEGIN";
        internal const string m_rptMovimientoInventarioEgreso = "INEGEG";
        internal const string m_rptProcesoInterno = "PROCIN";
        internal const string m_rptMatrizProcesoInterno = "MTPROI";
        #endregion Proceso
        #region Producción
        internal const string m_rptMasterizado = "MASGEN";
        internal const string m_rptPruebaEscurrido = "RPEV1";
        internal const string m_rptMasterizadoPers = "MASPER";
        internal const string m_rptMasterizadoIngreso ="MASING";
        internal const string m_rptMasterizadoEgreso = "MASEGR";
        internal const string m_rptCierreturnoTemporal = "RPLTT";
        internal const string m_rptCierreMaquina = "RPCTM";
        internal const string m_rptCierreturno = "RPLT";
        internal const string m_rptTmbadoPlacaIngresoTransferencia ="RPTIT";
        internal const string m_rptDescabezado = "DESCA";
        internal const string m_rptTransferenciaPlantaFCam001 = "FCAM01";
        internal const string m_rptTumbadoPlacaFcam005 = "FCAM05";
        internal const string AnticipoOrdenCamaron = "APCAP";
        internal const string AnticipoAProveedores = "ACCIP";
        internal const string LiquidacionCarroXCarro = "LCXCP";
        internal const string HorasNoProductivas = "RNPH";
        internal const string ConsultaLiquidacionCamaron = "LCCPL";
        internal const string CierrePlacaTunelesFrescoFcam002 = "FCAM02";
        internal const string CierrePlacaTunelesFrescoFcam003 = "FCAM03";
        #endregion Producción
        #region Proforma
        internal const string m_rptProforma = "RPFM";
        internal const string m_rptProformaPC = "RPFMPC";
        internal const string m_rptProformaET = "RPFMET"; 
        internal const string m_rptProformaTS = "RPFMTS";
        internal const string m_rptTemperatura = "PRTEM";
        internal const string m_rptProformaContrato = "PRSACO";
        #endregion Proforma

        #region Recepción
        internal const string m_rptLibrasLiquidadasPendientesPagoAprobadoProveedor = "RPVA";
        internal const string m_rptIngresoCamaronStatus = "RICS1";
        internal const string m_rptRecepMateriaPrimaPendiente = "LRPPL";
        internal const string m_rptVitacoraProduccion = "RMPV";
        internal const string m_rptMargenPorTallas = "RTCOM";
        internal const string m_rptResumenLiquidacionProveedor = "RPCOM";//subreporte
        internal const string m_rptResumenComprasPorPeriodoGlaseo = "RCOMPG";
        //internal const string m_rptKardex = "IKSPV1";
        //internal const string m_rptKardex = "IKRPS";
        internal const string m_rptRecepcion="RRCP1";
        internal const string m_rptLibrasLiquidadas="RLLQ1";
        internal const string m_rptLibrasLiquidadasPendPago="RMPP";
        internal const string m_rptComprobanteUnicoPago="RPCUP";
        internal const string m_rptLibrasLiquidadasPendientesPagoPAprobacion="RMPPA";
        internal const string m_rptLiquidacionCamaron = "LPPLL";
        internal const string m_rptCierreLiquidacion = "LCIERR";
        internal const string m_rptCierreLiquidacionRendimiento= "LCIERE";
        internal const string m_rptLibrasLiquidadasSequencial = "RMRL";
        internal const string m_rptLibrasLiquidadasPendientesPagoPAprobadoProveedor = "RPVPA";
        internal const string m_rptLiiquidacionCompraCamaron = "LPPPL";
        internal const string m_rptDistribucionPago = "RPDIS";
        internal const string m_rptComprobanteUnicoPagoEquivalente = "RICPE";
        #endregion Recepción

        #region Seguridad Garita
        internal const string m_rptPagosAnticipadosTerrestre = "RAGRTL";
        internal const string m_rptGuiaRemisionViaticoTerrestre = "LGRVT";
        internal const string m_rptPagosAnticiposFluvial = "RAGRFL";
        #endregion Seguridad Garita
        #region Configuracion

        internal const string m_rptIntegrationProcess = "RINTRE";
        #endregion

        #region Paciente cero

        internal const string m_rptPilotoCode = "PILOT";

        #endregion Paciente cero

        #region Métodos Adicionales

        internal static DynamicParameters ToDynamicParameters(Parametro[] parametros)
        {
            var queryParameters = new DynamicParameters();
            foreach (var parametro in parametros)
            {
                queryParameters.Add(parametro.Nombre, parametro.Valor);
            }
            return queryParameters;
        }

        #endregion Métodos Adicionales
    }
}