using BibliotecaReporte.Auxiliares;
using BibliotecaReporte.Auxiliares.Compras;
using BibliotecaReporte.Auxiliares.Configuracion;
using BibliotecaReporte.Auxiliares.ControlDeCalidad;
using BibliotecaReporte.Auxiliares.FacturaComercial;
using BibliotecaReporte.Auxiliares.FacturasExterior;
using BibliotecaReporte.Auxiliares.Inventario;
using BibliotecaReporte.Auxiliares.Logistica;
using BibliotecaReporte.Auxiliares.OrdenesDeProduccion;
//using BibliotecaReporte.Auxiliares.Logo;
using BibliotecaReporte.Auxiliares.Proceso;
using BibliotecaReporte.Auxiliares.Produccion;
using BibliotecaReporte.Auxiliares.Proforma;
using BibliotecaReporte.Auxiliares.ProformasAuxiliar;
using BibliotecaReporte.Auxiliares.Recepcion;
using BibliotecaReporte.Auxiliares.SeguridadGarita;
using BibliotecaReporte.Model;
using System.Data.SqlClient;
using System.IO;
namespace BibliotecaReporte
{
    public static class Biblioteca
    {
        public static Stream GetReport(SqlConnection sql, string code, Parametro[] parametros)
        {
            switch (code)
            {
                case ReportAuxiliar.m_rptPilotoCode: return RptPilotoAuxiliar.GetReport(sql, parametros);
                #region Compras
                case ReportAuxiliar.m_rptOrdenCompraCamaron: return OrdenCompraCamaronAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptRequerimientoCompra: return RequerimientoCompraAuxiliar.GetReport(sql, parametros);
                #endregion
                #region PruebaLogo
                //case ReportAuxiliar.m_rptLogoPrueba: return LogoAuxiliar.GetReport(sql, parametros);
                #endregion
                #region Inventario
                case ReportAuxiliar.m_rptIngreso: return IngresoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptTransferEgreso: return TransferEgresoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptTransferIngreso: return TransferIngresoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMovimientoInventarioEgresoTransferenciaA: return MovimientoInventarioEgresoTransferenciaAAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMovimientoInventarioIngresoTransferenciaA: return MovimientoInventarioIngresoTransferenciaAAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMovimientoInventario: return MovimientoInventarioAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMovimientoInventarioMotivo: return MovimientoInventarioMotivoAuxiliar.GetReport(sql, parametros);
                //case ReportAuxiliar.m_rptKardex: return KardexAuxiliar.GetReport(sql, parametros);
                //case ReportAuxiliar.m_rptKardexLote: return KardexLoteAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptKardexCosto: return KardexCostoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptSaldos: return SaldoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptSaldosLote: return SaldoLoteAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMovimientos: return MovimientoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMovimientosLote: return MovimientoLoteAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptKardexProveedor: return KardexProveedorAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptKardexProveedorSaldo: return KardexProveedorSaldoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptSaldosSinLote: return SaldosSinLoteAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptSaldosSinLoteTodos: return SaldosSinLoteTodoAuxiliar.GetReport(sql, parametros);                    
                #endregion
                #region Logística
                case ReportAuxiliar.m_rptGuiaRemisionPers: return GuiaRemisionPersAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLogisticaGuias: return LogisticaGuiaAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptRotacionTrans: return RotacionTransAuxiliares.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptRotacionTransFluvial: return RotacionTransFluvialAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptGuiaRide: return GuiaRideAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptRequisionBodega:return RemisionBodegaAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptGuiaRemisionPersFluvial: return GuiaRemisionPersFluvialAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptFleteTerrestre: return FleteTerrestreAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptFleteFluvial: return FleteFluvialAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptGuiaRemisionViaticoFluvial: return GuiaRemisionViaticoFluvialAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLiquidacionMaterialesGeneral: return LiquidacionMaterialesGeneralAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLiquidacionMateriales: return LiquidacionMaterialesAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMovimientoInventarioTodos: return MovimientoInventarioTodosAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLogisticaGuiasExcel: return LogisticaGuiaExcelAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptRequerimientoInventario: return RequerimientoInventarioAuxiliar.GetReport(sql, parametros);
                    
                #endregion
                #region Procesos
                case ReportAuxiliar.m_rptProcesoInternoResumen: return ProcesoInternoResumenAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptProcesoInternoDetallado: return ProcesoInternoDetalladoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptResumenDeReproceso: return ResumenReprocesoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptDetalleDeReproceso: return DetalleDeReprocesoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMovimientoInventarioIngreso: return MovimientoInventarioIngresoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMovimientoInventarioEgreso: return MovimientoInventarioEgresoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptProcesoInterno: return ProcesoInternoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMatrizProcesoInterno: return MatrizProcesoInterno2Auxiliar.GetReport(sql, parametros);                    
                #endregion
                #region Producción
                case ReportAuxiliar.m_rptPruebaEscurrido: return PruebaDeEscurridoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMasterizadoPers: return MasterizadoPersAuxiliar.GetReport(sql, parametros);
                // case ReportAuxiliar.m_rptMovimientoInventarioMasterizadoIngreso: return MovimientoMasterizadoInventarioIngresoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptCierreMaquina: return CierreMaquinaAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptCierreturno: return CierreTurnoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptCierreturnoTemporal: return CierreTurnoTemporalAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMasterizado: return MasterizadoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMasterizadoIngreso: return MovimientoIngresoMasterizadoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMasterizadoEgreso: return MovimientoEgresoMasterizadoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptTmbadoPlacaIngresoTransferencia: return TumbadoPlacaIngresoTransferenciaAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptDescabezado: return DescabezadoAuxiliares.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptTransferenciaPlantaFCam001:return TransferenciaPlantaFCam001Auxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.AnticipoOrdenCamaron: return AnticipoComprarCamaronAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.AnticipoAProveedores:return AnticipoAProveedoresAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.LiquidacionCarroXCarro: return LiquidacionCarroXCarroAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.HorasNoProductivas: return HorasNoProductivasAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.ConsultaLiquidacionCamaron: return ConsultaLiquidacionCamaronAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.CierrePlacaTunelesFrescoFcam002: return CierrePlacaTunelesFrescoFcam002Auxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.CierrePlacaTunelesFrescoFcam003: return CierrePlacaTunelesFrescoFcam003Auxiliar.GetReport(sql, parametros);
                #endregion
                #region Proforma
                case ReportAuxiliar.m_rptProforma: return ProformaAuxiliar.GetReport(sql, parametros);
                //case ReportAuxiliar.m_rptProformaPC: return ProformaPCAuxiliar.GetReport(sql, parametros);
                //case ReportAuxiliar.m_rptProformaET: return ProformaETAuxiliar.GetReport(sql, parametros);
                //case ReportAuxiliar.m_rptProformaTS: return ProformaTSAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptTemperatura: return TemperaturaAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptProformaContrato:return ProformaContratoAuxiliar.GetReport(sql, parametros);
                #endregion
                #region Factura Exterior
                case ReportAuxiliar.m_rptFacturaElectronica: return FacturaElectronicaAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptPackinglist: return FacturaElectronicaPackinListAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptFacturaFiscal: return FacturaFiscalAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptISF: return ISFAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptNooWood: return NoomWoodAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptFacturaElectronicaPropia: return FacturaElectronicaPropiaAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptFacturaElectronicaPropiaExcel: return FacturaElectronicaPropiaExcelAuxiliar.GetReport(sql, parametros);                    
                case ReportAuxiliar.m_rptFactExteriorBL: return FactExteriorBLModelAuxiliar.GetReport(sql, parametros);
                //case ReportAuxiliar.m_rptFacturaComercialExcel: return ComercialExcelAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptTemperaturaF: return TemperaturafAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptFactPesos: return FactPesosAuxiliar.GetReport(sql, parametros);
                #endregion
                #region OrdenesDeProducción
                case ReportAuxiliar.m_rptOrdenesProduccion: return OrdenesProduccionAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptOrdenesProduccionVentas: return OrdenesProduccionVentasAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptOrdenesProduccionMaterial: return OrdenProduccionMaterialAuxiliar.GetReport(sql, parametros);                    
                #endregion
                #region AnalisisCalidad
                case ReportAuxiliar.m_rptAnalisisCalidad: return AnalisisDeCalidadAuxiliar.GetReport(sql, parametros);
               #endregion
                #region FacturaComercial
                case ReportAuxiliar.m_rptFacturaComercial: return FacturaComercialAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptFacturaComercialExcel: return FacturacionComercialExcelAuxiliar.GetReport(sql, parametros);
                #endregion
                #region Recepción
                case ReportAuxiliar.m_rptLibrasLiquidadasPendientesPagoAprobadoProveedor: return LibrasLiquidadasPendientePagoAprobadoProveedorAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptIngresoCamaronStatus:return IngresoCamaronStatusAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptRecepMateriaPrimaPendiente: return MateriaPrimaPendienteAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptVitacoraProduccion: return VitacoraProduccionAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptMargenPorTallas: return MargenPorTallasAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptResumenLiquidacionProveedor: return ResumenLiquidacionProveedorAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptResumenComprasPorPeriodoGlaseo: return ResumenComprasPorPeriodoGlaseoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptRecepcion: return RecepcionAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLibrasLiquidadas: return LibrasLiquidadasAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLibrasLiquidadasPendPago: return LibrasLiquidadasPendientePagoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptComprobanteUnicoPago:return ComprobanteUnicoPagoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLibrasLiquidadasPendientesPagoPAprobacion: return LibrasLiquidadasPendientesPagoPAprobacionAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLiquidacionCamaron: return LiquidacionCamaronAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptCierreLiquidacion: return CierreLiquidacionAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptCierreLiquidacionRendimiento: return CierreLiquidacionRendimientoAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLibrasLiquidadasSequencial:return LibrasLiquidadasSequencialAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLibrasLiquidadasPendientesPagoPAprobadoProveedor: return LibrasLiquidadasPendientesPagoPAprobadoProveedorAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptLiiquidacionCompraCamaron: return LiquidacionCompraCamaronAuxiliar.GetReport(sql, parametros);                    
                case ReportAuxiliar.m_rptDistribucionPago: return DistribucionPagoAuxiliar.GetReport(sql, parametros);                    
                case ReportAuxiliar.m_rptComprobanteUnicoPagoEquivalente: return ComprobanteUnicoPagoEquivalenteAuxiliar.GetReport(sql, parametros);                    
                #endregion
                #region SeguridadGarita
                case ReportAuxiliar.m_rptPagosAnticipadosTerrestre: return PagosAnticiposTerrestreAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptGuiaRemisionViaticoTerrestre: return GuiaRemisionViaticoTerrestreAuxiliar.GetReport(sql, parametros);
                case ReportAuxiliar.m_rptPagosAnticiposFluvial: return PagoAnticiposFluvialAuxiliar.GetReport(sql, parametros);                    
                #endregion
                #region CONFIGURACION
                case ReportAuxiliar.m_rptIntegrationProcess: return IntegrationProcessAuxiliar.GetReport(sql, parametros);                    
                #endregion
                default: return null;
            }
        }
    }
}
