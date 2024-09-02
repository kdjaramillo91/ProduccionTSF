
GO
/****** Object:  StoredProcedure [dbo].[spPar_KardexLote]    Script Date: 07/03/2023 10:36:23 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[spPar_SaldoSinLote]
@ParametrosBusquedaKardexSaldo		nVarChar(Max),
	@printDebug		Bit = 0
as 
Begin

	Set NoCount On
select [idDetalleInventario] ,
	[idCabeceraInventario] ,
	[fechaInicio] ,
	[fechaFin] ,
	[numeroDocumentoInventario] ,
	[idBodega] ,
	[nombreBodega],
	nombreProducto,
	[idUbicacion],
	[nombreUbicacion] ,
	[idProducto] ,
	[nombreProducto] ,
	[fechaEmison] ,
	[idMotivoInventario] ,
	[nombreMotivoInventario] ,
	[idUnidadMedida],
	[nombreUnidadMedida] ,
	[montoEntrada] ,
	[montoSalida] ,
	[balance] ,
	[previousBalance] ,
	[idEstado] ,
	[nombreEstado] ,
	[idCompania] ,
	[nameCompania] ,
	[nameDivision] ,
	[nameBranchOffice] ,
	[numberRemissionGuide] ,
	[numberLot] ,
	[Provider_name] ,
	[isCopacking],
	[nameProviderShrimp] ,
	[productionUnitProviderPool],
	[itemSize] ,
	[itemType] ,
	[ItemMetricUnit] ,
	[ItemPresentationValue] ,
	[amount] ,
	[amountCostUnit] ,
	[amountCostTotal] ,
	[previousPound] ,
	[previousCostPound] ,
	[previousTotalCostPound] ,
	[entradaPound],
	[entradaCostPound] ,
	[entradaTotalCostPound] ,
	[salidaPound] ,
	[salidaCostPound] ,
	[salidaTotalCostPound] ,
	[finalPound] ,
	[finalCostPound] ,
	[finalTotalCostPound] ,
	[itemPresentationDescrip] ,
	[oneItemPound] from RepoKardexCosto 
	END