
--Compras
EXEC sp_rename 'par_OrdendecompracamaronCR','spPar_OrdenCompraCamaron'
--FacturaComercial
EXEC sp_rename 'par_Factura_Comercial_Lista','spPar_FacturaComercial'
--FacturasExterior
EXEC sp_rename 'par_Factura_Fiscal_Lista','spPar_FacturaFiscal'
EXEC sp_rename 'par_InvoiceExterior_NonWoodCR','spPar_NomWoodCR'
EXEC sp_rename 'par_InvoiceExteriorPakingListCR','spPar_InvoiceExteriorPakingListCR'
EXEC sp_rename 'par_InvoiceExteriorPakingListCR','spPar_FacturaElectronicaAuxiliar'
EXEC sp_rename 'par_ISF','spPar_ISF'
--Inventario
EXEC sp_rename 'par_Movimiento_InventarioMotivo','spPar_TransferIngreso'
EXEC sp_rename 'par_Movimiento_InventarioMotivo','spPar_TransferEgreso'
EXEC sp_rename 'par_Movimiento_InventarioMotivoConversion','spPar_Ingreso'
--Logistica
EXEC sp_rename 'par_Guia_Remision_Personalizada','spPar_GuiaRemisionPers'
EXEC sp_rename 'par_Guia_Remision_TerrestreFluvial','spPar_LogisticaGuia'
EXEC sp_rename 'par_LiquidacionRotacionTransportista','spPar_RotacionTrans'
EXEC sp_rename 'par_LiquidacionRotacionTransportistaFluvial','spPar_RotacionTransFluvial'
--Procesos
EXEC sp_rename 'Par_ProcesosInternosDetalladoTipos','spPar_DetalleDeReproceso'
EXEC sp_rename 'Par_ProcesosInternosDetallado','spPar_ProcesosInternosDetallado'
EXEC sp_rename 'Par_ProcesosInternosDetalladopxp','spPar_ProcesosInternoResumen'
EXEC sp_rename 'Par_ProcesosInternosDetalladoTiposRES','spPar_ResumenReproceso'
--Producción
EXEC sp_rename 'par_Movimiento_InventarioMasterizado','spPar_MasterizadoPers'
EXEC sp_rename 'par_prueba_escurrido','spPar_PruebaDeEscurrido'
EXEC sp_rename 'RptCierreMaquina','spRptCierreMaquina'
EXEC sp_rename 'RptCierreTurno','spRptCierreTurno'
EXEC sp_rename 'RptCierreTurnoTemporal','spRptCierreTurnoTemporal'
--Proforma 
EXEC sp_rename 'par_ProformasReport','spPar_Proforma'
EXEC sp_rename 'par_ProformasTemperatura','spPar_Temperatura'
--Recepción
EXEC sp_rename 'SP_ResumenTallaCompras','SP_MargenPorTallas'
EXEC sp_rename 'par_ProductionShrimpIncomeCR','spPar_IngresoCamaronStatus'
EXEC sp_rename 'par_ProductionPoundsliquidationCR','spPar_LibrasLiquidadas'
EXEC sp_rename 'par_ProductionPoundsliquidationProvPayAproved','spPar_LibrasLiquidadaspendientePagoAprobadoProveedor'
EXEC sp_rename 'par_ProductionPoundsliquidationPayPendient','spPar_LibrasLiquidadasPendientePago'
EXEC sp_rename 'par_ProductionLiquidationEntryCR','spPar_Recepcion'
EXEC sp_rename 'par_ProductionShrimpIncome_PendientesAnuladosCR','spPar_MateriaPrimaPendiente'
EXEC sp_rename 'SP_ResumenComprasPeriodosG','spPar_ResumenComprasPorPeriodoGlaseo'
EXEC sp_rename 'SP_ResumenLiquidacionesProveedor','spPar_ResumenLiquidacionProveedor'
EXEC sp_rename 'par_Vitacora_Produccion','spPar_VitacoraProduccion'
--Seguridad Garita
EXEC sp_rename 'par_GuideRemisionViaticoTransportistaTerretre','spPar_PagosAnticiposTerrestre'













