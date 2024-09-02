/*
	Script para el Registro de Todos los Reportes del Sistema
*/

Declare	@pathReportProduction Table
(
	code		Char(6) Primary Key,
	path		VarChar(1000),
	nameReport	VarChar(100)
)

Insert	Into @pathReportProduction (
		code,
		path,
		nameReport)
	Values
		( 'APCAP ', 'C:\Aplicativo\Produccion\AnticipoProveedor\Reportes\', 'RptAnticipoCompraCamaron.rpt' ),
		( 'ACCIP ', 'C:\Aplicativo\Produccion\AnticipoProveedor\Reportes\', 'RptAnticipoCompraCamaronIndividual.rpt' ),
		( 'ROCP  ', 'C:\Aplicativo\Produccion\Compras\Reportes\', 'RptOrdenCompraCamaron.rpt' ),
		( 'FECEXC', 'C:\Aplicativo\Produccion\Factura\Reportes\', 'RptFacturaCommercialExcel.rpt' ),
		( 'FEXPAR', 'C:\Aplicativo\Produccion\Factura\Reportes\', 'RptFacturaExteriorPartial.rpt' ),
		( 'RFCLV1', 'C:\Aplicativo\Produccion\Factura\Reportes\', 'RptFacturasComerciales.rpt' ),
		( 'IEFE1 ', 'C:\Aplicativo\Produccion\FacturaExterior\Reportes\', 'RptFactFacturaElectronica.rpt' ),
		( 'RFFLV1', 'C:\Aplicativo\Produccion\FacturaExterior\Reportes\', 'RptFacturasFiscales.rpt' ),
		( 'ADLP  ', 'C:\Aplicativo\Produccion\Grupos\Reportes\', 'RptGruposPersonas.rpt' ),
		( 'RINTRE', 'C:\Aplicativo\Produccion\Integracion\Reportes\', 'RptIntegrationProcess.rpt' ),
		( 'RINTGP', 'C:\Aplicativo\Produccion\Integracion\Reportes\', 'RptIntegrationProcessGroup.rpt' ),
		( 'IKSPV1', 'C:\Aplicativo\Produccion\Inventario\Reportes\', 'RptKardex.rpt' ),
		( 'IDGV1 ', 'C:\Aplicativo\Produccion\Inventario\Reportes\', 'RptMovimientoInventario.rpt' ),
		( 'IMIPV1', 'C:\Aplicativo\Produccion\Inventario\Reportes\', 'RptMovimientos.rpt' ),
		( 'RMIRFE', 'C:\Aplicativo\Produccion\Inventario\Reportes\', 'RptRequerimientoInventario.rpt' ),
		( 'ISPV1 ', 'C:\Aplicativo\Produccion\Inventario\Reportes\', 'RptSaldos.rpt' ),
		( 'LFRTR ', 'C:\Aplicativo\Produccion\Liquidacion\Reportes\', 'RptRotacionTrans.rpt' ),
		( 'LFRTRF', 'C:\Aplicativo\Produccion\Liquidacion\Reportes\', 'RptRotacionTransFluvial.rpt' ),
		( 'CDLP  ', 'C:\Aplicativo\Produccion\ListasPrecios\Reportes\', 'RptListaCalendario.rpt' ),
		( 'LPV1FC', 'C:\Aplicativo\Produccion\ListasPrecios\Reportes\', 'RptListaPrecios.rpt' ),
		( 'SHTGR ', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptCompraHieloTercero.rpt' ),
		( 'LGRRP1', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptGuiaRemisionPers.rpt' ),
		( 'RGRGRF', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptGuiaRemisionPersFluvial.rpt' ),
		( 'RGRVD1', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptGuiaRemisionPersFluvialVD1.rpt' ),
		( 'LGRVD1', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptGuiaRemisionPersVD1.rpt' ),
		( 'GRDM1 ', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptRequisicionBodega.rpt' ),
		( 'D1GRDM', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptRequisicionBodegaVD1.rpt' ),
		( 'GRVCR ', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptViaticoCajaChica.rpt' ),
		( 'GRVCRF', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptViaticoCajaChicaFluvial.rpt' ),
		( 'F1GRVC', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptViaticoCajaChicaFluvialVD1.rpt' ),
		( 'D1GRVC', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptViaticoCajaChicaVD1.rpt' ),
		( 'GRVSR ', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptViaticosSupervisorPesca.rpt' ),
		( 'GRVSRF', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptViaticosSupervisorPescaFluvial.rpt' ),
		( 'F1GRVS', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptViaticosSupervisorPescaFluvialVD1.rpt' ),
		( 'D1GRVS', 'C:\Aplicativo\Produccion\Logistica\GuiaRemision\Reportes\', 'RptViaticosSupervisorPescaVD1.rpt' ),
		( 'LRFF1 ', 'C:\Aplicativo\Produccion\Logistica\Reportes\', 'RptFleteFluvial.rpt' ),
		( 'LRFT1 ', 'C:\Aplicativo\Produccion\Logistica\Reportes\', 'RptFleteTerrestre.rpt' ),
		( 'LGRVF ', 'C:\Aplicativo\Produccion\Logistica\Reportes\', 'RptGuiaRemisionViaticoFluvial.rpt' ),
		( 'LGRVT ', 'C:\Aplicativo\Produccion\Logistica\Reportes\', 'RptGuiaRemisionViaticoTerrestre.rpt' ),
		( 'LMIPG ', 'C:\Aplicativo\Produccion\Logistica\Reportes\', 'RptLiquidacionMateriales.rpt' ),
		( 'LMGPG ', 'C:\Aplicativo\Produccion\Logistica\Reportes\', 'RptLiquidacionMaterialesGeneral.rpt' ),
		( 'GRLTF ', 'C:\Aplicativo\Produccion\Logistica\Reportes\', 'RptLogListaGuias.rpt' ),
		( 'RAGRFL', 'C:\Aplicativo\Produccion\Logistica\Reportes\', 'RptPagosAnticiposFluvial.rpt' ),
		( 'RAGRTL', 'C:\Aplicativo\Produccion\Logistica\Reportes\', 'RptPagosAnticiposTerrestre.rpt' ),
		( 'RCLC  ', 'C:\Aplicativo\Produccion\Produccion\Reportes\', 'RptCierreLiquidacion.rpt' ),
		( 'RPCUP ', 'C:\Aplicativo\Produccion\Produccion\Reportes\', 'RptComprobanteUnicoPago.rpt' ),
		( 'LCXCP ', 'C:\Aplicativo\Produccion\Produccion\Reportes\', 'RptLiqCarroPorCarro.rpt' ),
		( 'LPPPL ', 'C:\Aplicativo\Produccion\Produccion\Reportes\', 'RptLiquidacionCompraCamaron.rpt' ),
		( 'RPEV1 ', 'C:\Aplicativo\Produccion\Produccion\Reportes\', 'RptPruebaEscurrido.rpt' ),
		( 'RMPV  ', 'C:\Aplicativo\Produccion\Produccion\Reportes\', 'RptVitacoraProduccion.rpt' ),
		( 'LCCPL ', 'C:\Aplicativo\Produccion\Recepcion\Reportes\', 'RptConsultaLiquidacionCamaron.rpt' ),
		( 'RICS1 ', 'C:\Aplicativo\Produccion\Recepcion\Reportes\', 'RptIngresoCamaronStatus.rpt' ),
		( 'RLLQ1 ', 'C:\Aplicativo\Produccion\Recepcion\Reportes\', 'RptLibrasLiquidadas.rpt' ),
		( 'RRCP1 ', 'C:\Aplicativo\Produccion\Recepcion\Reportes\', 'RptRecepcion.rpt' ),
		( 'LRPPL ', 'C:\Aplicativo\Produccion\Recepcion\Reportes\', 'RptRecepMatPrimaPendientes.rpt' );

-- Actualizar los existentes...
Update	F
Set		path = T.path, nameReport = T.nameReport
From	dbo.tbsysPathReportProduction F, @pathReportProduction T
Where	F.code = T.code
And		(F.path != T.path Or F.nameReport != T.nameReport)

-- Agregar los faltantes...
Insert	Into tbsysPathReportProduction (
		code,
		path,
		nameReport,
		isCrystalReport,
		verCrystalReport,
		extFile,
		isCustomized)
Select	code, path, nameReport, 1, '8.5', 'rpt', 0
From	@pathReportProduction
Where	code Not In (
			Select	code
			From	dbo.tbsysPathReportProduction
		)
