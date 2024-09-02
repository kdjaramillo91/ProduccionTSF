/****** Object:  StoredProcedure [dbo].[OrdenProduccionReport_Materiales]    Script Date: 23/02/2023 02:17:38 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- SP 7 Query Original: OrdenProduccionReport_Materiales
CREATE OR ALTER PROCEDURE [dbo].[sp_ParOrdenProduccionMaterial] 
	@id INT
AS
	SET NOCOUNT ON
		DECLARE @Producto TABLE(
		id INT,id1 INT,
		descripcion VARCHAR(100),
		descripcionCod VARCHAR(30)
		)
		INSERT INTO @Producto
		SELECT distinct sod.id_salesOrder,it.id 'item' ,it.name,it.masterCode
		FROM item it
		INNER JOIN SalesOrderDetail SOD on sod.id_item=it.id
		inner join SalesOrder SO on SOD.id_salesOrder=so.id

		DECLARE @Proformas TABLE(
		id INT,
		numeroProforma VARCHAR(30)
		)
		INSERT INTO @Proformas
		SELECT DISTINCT sod.id_salesOrder,d.number 
		FROM SalesOrderDetailSalesQuotationExterior se
		INNER JOIN Document d ON d.id = se.id_salesQuotationExterior
		INNER JOIN SalesOrderDetail sod	ON sod.id = se.id_salesOrderDetail
		

		SELECT pr.descripcionCod 'codigo',pr.descripcion 'Nombre del Producto',dsso.name 'Estado',IL.name 'Linea de Inventario',IT.name 'Tipo de Producto'
		,IC.name 'Categoría',i.mastercode 'Codigo', i.name 'Ingrediente',sod.quantityRequiredForFormulation 'Cantformulada'	,sod.quantity 'Cantidad',mu.code 'UM'
		,dtso.name 'TipodeDocuemnto', ISNULL(p.numeroProforma,'') 'NumerodeProforma'
		,dso.emissionDate 'FechaEmision',pSoCliente.fullname_businessName 'Cliente'
		,pDestination.nombre 'Destino',so.dateShipment 'FechaEmbarque',pmth.name 'FormadePago',dso.number 'NumeroDocumento'
		,dso.description 'descripcion',pSoEmployeApplicant.fullname_businessName 'Solicitante', c.logo 'logo'
		,Isnull(so.numeroLote,'') numeroLote, Isnull(ore.name,'') motivo
		FROM  SalesOrder SO 
		inner join SalesOrderDetail SOR on SOR.id_salesOrder=so.id
		inner join SalesOrderMPMaterialDetail SOD on SOD.id_salesOrder=so.id
		inner join SalesOrderDetailMPMaterialDetail SODD on SODD.id_salesOrderMPMaterialDetail=sod.id and sodd.id_salesOrderDetail=SOR.id
		inner join item i on SOD.id_item=i.id 
		inner join item i2 on sor.id_item=i2.id
		inner join InventoryLine IL on il.id=i.id_inventoryLine
		inner join ItemType IT on it.id=i.id_itemType and it.id_inventoryLine=il.id
		inner join ItemTypeCategory IC on ic.id=i.id_itemTypeCategory
		inner join MetricUnit MU on mu.id=sod.id_metricUnit
		inner join @Producto PR on PR.id=sod.id_salesOrder and PR.id1=SOr.id_item

		inner join Document DSO on dso.id = so.id
		inner join DocumentState DSSO on dsso.id = dso.id_documentState
		inner join Person pSoCliente on pSoCliente.id = so.id_customer
		inner join DocumentType dtso on dtso.id = dso.id_documentType
		INNER JOIN Person pSoEmployeApplicant ON pSoEmployeApplicant.id = so.id_employeeApplicant
		LEFT JOIN PaymentMethod pmth ON pmth.id = so.id_PaymentMethod
		INNER JOIN Port pDestination ON pDestination.id = so.id_portDestination
		LEFT JOIN OrderReason ore ON ore.id = so.id_orderReason

		LEFT JOIN @Proformas p ON p.id = so.id
		INNER JOIN EmissionPoint ep ON ep.id = dso.id_emissionPoint
		INNER JOIN Company c ON c.id = ep.id_company

		WHERE so.id = @id
		order by pr.descripcionCod asc
/*
	EXEC sp_ParOrdenProduccionMaterial 0
*/
GO