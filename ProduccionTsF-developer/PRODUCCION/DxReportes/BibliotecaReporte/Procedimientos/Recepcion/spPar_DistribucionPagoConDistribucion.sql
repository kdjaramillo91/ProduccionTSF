/****** Object:  StoredProcedure [dbo].[spPar_DistribucionPagoConDistribucion]    Script Date: 01/06/2023 10:56:03 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Query - SP Original: par_ProductionLotPaymentConDistributedDetail
CREATE OR ALTER PROC [dbo].[spPar_DistribucionPagoConDistribucion]
@id int
as 
select 
		[PLP].[id],
	    Item.name As NombreProducto,
		Itc.name AS Clase, 
		Iz.description As Talla,
		Itp.name As ProcesoDetalle,
		PLPD.performance as RendimientoTotal,
		Mu.code As UM,
		PLPD.priceLP as PrecioUnitario,
        PLPD.totalPayLP as ValorTotal
  from [dbo].[ProductionLotPayment]   PLP
  Inner join [dbo].[ProductionLotPaymentDistributed] [PLPD] On [PLPD].[id_ProductionLotPayment] = [PLP].[id]
  Inner Join Item Item On Item.[id] = [PLPD].[id_item]
  inner Join ItemType Itp On Item.[id_itemType] = Itp.[id]
  Inner Join Itemtypecategory Itc On Itc.[id] = Item.id_itemTypeCategory
  Inner Join Presentation Pt On Pt.[id] = Item.id_presentation
  Inner Join ItemGeneral Ig On Ig.id_item = Item.id
  Inner Join MetricUnit Mu On Mu.id = Pt.id_metricUnit
  Left Join ItemSize Iz On Iz.id = Ig.id_size
 
  Where [PLP].id_productionLot = @id 
  Order by Item.auxCode asc, Iz.[orderSize]

/*
	EXEC spPar_DistribucionPagoConDistribucion @id=982718
*/

GO