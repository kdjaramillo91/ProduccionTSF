/****** Object:  StoredProcedure [dbo].[OrdenProduccionReport]    Script Date: 16/06/2023 03:20:53 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

-- Query / SP Original : OrdenProduccionReport
CREATE OR ALTER PROCEDURE [dbo].[spPar_OrdenProduccionVenta]
	@id INT
AS
	SET NOCOUNT ON

	DECLARE @conversiones TABLE(
		idUnidadOrigen INT,
		codigoUnidadOrigen VARCHAR(100),
		idUnidadDestino INT,
		codigoUnidadDestino VARCHAR(100),
		factor DECIMAL(20,6)
	)

	DECLARE @detalles TABLE(
		id INT,
		id_item INT,
		codigo VARCHAR(50),
		descripcion VARCHAR(250),
		talla VARCHAR(50),
		marca VARCHAR(100),
		empaque VARCHAR(100),
		cartones DECIMAL(20,6),
		kilos DECIMAL(20,6),
		libras DECIMAL(20,6),
		ordenTalla INT,
		lineaInventario VARCHAR(100),
		tipoProducto VARCHAR(100)
	)


	DECLARE @Proformas TABLE(
		id INT,
		id_item INT,
		numeroProforma VARCHAR(30),
		descripcionMarcada VARCHAR(250),
		tallaMarcada VARCHAR(50)
	)

	INSERT INTO @Proformas
	SELECT DISTINCT
		 sod.id_salesOrder
		,sod.id_item
		,d.number
		,ISNULL(it.name,'')
		,ISNULL(iz.name,'')
	FROM SalesOrderDetailSalesQuotationExterior se
	INNER JOIN Document d
		ON d.id = se.id_salesQuotationExterior
	INNER JOIN SalesOrderDetail sod
		ON sod.id = se.id_salesOrderDetail
	INNER JOIN InvoiceDetail dt 
	    ON dt.id = se.id_invoiceDetail
	   AND dt.isActive = 1
	INNER JOIN Item it
	    ON it.id = dt.id_itemMarked
	LEFT JOIN ItemGeneral ig
	    ON ig.id_item = it.id
    LEFT JOIN ItemSize iz 
	    ON iz.id = ig.id_size


	INSERT INTO @conversiones
	SELECT
		muO.id 'idMetricOrigen',
		muO.code 'MetricOrigen',
		mud.id 'idMetricDesttino',
		mud.code 'MetricDestino',
		muc.factor 'Factor'
	FROM MetricUnitConversion muc
	INNER JOIN MetricUnit muO
	ON muo.id = muc.id_metricOrigin
	INNER JOIN MetricUnit mud
	ON mud.id = muc.id_metricDestiny
	WHERE mud.code IN ('Kg','Lbs')

	INSERT INTO @detalles
	SELECT 
		 sod.id_salesOrder
		,sod.id_item
		,i.masterCode 'Codigo'
		,i.name 'Descripcion'
		,isz.name 'Talla'
		,it.name 'Marca'
		,p.name 'Empaque'
		,sod.quantityTypeUMSale 'Cartones'
		,sod.quantityTypeUMSale * ISNULL(cKg.factor,1) * ISNULL(p.minimum,1) * ISNULL(p.maximum,1) 'kilos'
		,sod.quantityTypeUMSale * ISNULL(cLbs.factor,1) * ISNULL(p.minimum,1) * ISNULL(p.maximum,1) 'libras'
		,isz.orderSize 'ordenTalla'
		,il.name 'lineaInventario'
		,itp.name 'tipoProducto'

	FROM SalesOrderDetail sod

	INNER JOIN Item i
		ON i.id = sod.id_item
	LEFT JOIN Presentation p
		ON i.id_presentation = p.id
	INNER JOIN InventoryLine il
		ON il.id = i.id_inventoryLine
	LEFT JOIN ItemType itp
		ON itp.id = i.id_itemType
		AND itp.id_inventoryLine = il.id
	LEFT JOIN ItemGeneral ig
		ON ig.id_item = i.id
	LEFT JOIN ItemSize isz
		ON isz.id = ig.id_size
	LEFT JOIN ItemTrademark it
		ON it.id = ig.id_trademark
	LEFT JOIN ItemTrademarkModel itm
		ON itm.id = ig.id_trademarkModel
	LEFT join @conversiones cKg
		ON cKg.idUnidadOrigen = sod.id_metricUnitTypeUMPresentation
		AND cKg.codigoUnidadDestino = 'Kg'
	LEFT join @conversiones cLbs
		ON cLbs.idUnidadOrigen = sod.id_metricUnitTypeUMPresentation
		AND cLbs.codigoUnidadDestino = 'Lbs'
	ORDER BY it.name,il.name,itp.name,isz.name


	SELECT 
		--=====================================
		--			Cabecera 1
		--=====================================
		 dtso.name 'TipoDocumento'
		,dso.number 'NumeroDocumento'
		,dso.emissionDate 'FechaEmision'
		,dsso.name 'Estado'
		,pSoEmployeApplicant.fullname_businessName 'Solicitante'
		,c.logo 'Logo'
		--=====================================
		--			Cabecera 2
		--=====================================
		,pSoCliente.fullname_businessName 'Cliente'
		,ISNULL(p.numeroProforma,'') 'numeroProforma'
		,pmth.name 'FormadePago'
		,so.dateShipment 'FechaEmbarque'
		,pDestination.nombre 'Destino'

		--=====================================
		--			Cabecera 3
		--=====================================
		,dso.description 'descripcion'
		--=====================================
		--			Detalles
		--=====================================
		,sod.codigo 'codigo'
		,sod.descripcion 'descripcionDetalle'
		,sod.marca 'marca'
		,sod.talla 'talla'
		,sod.empaque 'empaque'
		,sod.cartones 'cartones'
		,sod.kilos 'kilos'
		,sod.libras 'libras'
		,p.descripcionMarcada 'descripcionMarcada'
		,p.tallaMarcada 'tallaMarcada'
        ,per.fullname_businessName as VentaUsuario
		,case when doc.id_documentState <> 1 then 
		per2.fullname_businessName 
		else '' end
		as DirectorVenta
		,Isnull(so.numeroLote,'') numeroLote
		,Isnull(ore.name,'') motivo
		,Isnull(so.additionalInstructions,'') instrucciones
		,Isnull(per3.identification_Number,'') RucProveedor
		,Isnull(per3.fullname_businessName,'') NombreProveedor
		,Isnull(per3.cellPhoneNumberPerson,'') TelefonoProveedor
		,Isnull(per3.address,'') DireccionProveedor
		,Isnull(prd.plantCode,'') CodigoPlanta
		,Isnull(prd.fda,'') FDA
	FROM SalesOrder so
		inner join document doc on so.id = doc.id 
	left join [user] us on us.id = doc.id_userCreate
	left join person per on per.id = us.id_employee

	left join [user] us2 on us2.id = doc.id_userUpdate
	left join person per2 on per2.id = us2.id_employee
	left join person per3 on per3.id = so.id_provider
	left join ProviderGeneralData prd on prd.id_provider = per3.id
	LEFT JOIN @detalles sod
		ON so.id = sod.id
	INNER JOIN Document dso
		ON dso.id = so.id
	INNER JOIN DocumentState dsso
		ON dsso.id = dso.id_documentState
	INNER JOIN Person pSoCliente
		ON pSoCliente.id = so.id_customer
	INNER JOIN DocumentType dtso
		ON dtso.id = dso.id_documentType
	INNER JOIN Person pSoEmployeApplicant
		ON pSoEmployeApplicant.id = so.id_employeeApplicant
	LEFT JOIN PaymentMethod pmth
		ON pmth.id = so.id_PaymentMethod
	INNER JOIN Port pDestination
		ON pDestination.id = so.id_portDestination
	LEFT JOIN @Proformas p
		ON p.id = so.id
       AND p.id_item = sod.id_item
	INNER JOIN EmissionPoint ep
		ON ep.id = dso.id_emissionPoint
	INNER JOIN Company c
		ON c.id = ep.id_company
	LEFT JOIN OrderReason ore ON ore.id = so.id_orderReason
	WHERE so.id = @id
	ORDER BY sod.marca,sod.lineaInventario, sod.tipoProducto, sod.ordenTalla

/*
	EXEC spPar_OrdenProduccionVenta 0
*/
GO