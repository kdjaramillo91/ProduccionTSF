/****** Object:  StoredProcedure [dbo].[spPar_ProformaPC]    Script Date: 17/04/2023 9:47:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE OR ALTER PROC [dbo].[spPar_ProformaPC]
@id INT      
    AS      
 SET nocount ON    
 
 CREATE TABLE #DatosProducto(
	id INT,
	id_Invoice INT,
	cASepack VARCHAR(50),
 )

 
Create Table #Adicionales(
id	int,
id_item int,
totalAdicionales decimal(14,6),
porcentajeAdicionales decimal(14,6),
totalFob decimal(14,6)
)

Create table #distribucionadicionales(
id	int,
id_item int,
gastodistribuido decimal(14,6),
precioFob decimal(14,6))

 Declare @Formato Varchar(2)
 Declare @Total INT
 Declare @Cents INT
 DECLARE @TEMP TABLE(resultado VARCHAR(MAX))
 DECLARE @TEMPCents TABLE(resultado VARCHAR(MAX))
 DECLARE @DatosProducto TABLE(id Int, id_invoice Int, cASepack VARCHAR(MAX))
 DECLARE @NetoLibras Decimal(15,6)
 DECLARE @NetoKilos Decimal(15,6)
 DECLARE @BrutoLibras Decimal(15,6)
 DECLARE @BrutoKilos Decimal(15,6)
   Declare @GlaseoKilos Decimal(15,6)
 Declare @GlaseoLibras Decimal(15,6)
  Declare @ProformaKilos Decimal(15,6)
    Declare @ProformaLibras Decimal(15,6)
  
 Set @Formato = 'SI'
 select @Formato = value from Setting where code = 'FORMATOTEL'

 Insert Into #Adicionales
 select iv.id, id_item,
 TotalAdicionales =  valueInternationalFreight + valueInternationalInsurance + valueCustomsExpenditures + valueTransportationExpenses,
 porcentajeAdicionales = case when (valueInternationalFreight + valueInternationalInsurance + valueCustomsExpenditures + valueTransportationExpenses) > 0 then 
round(((totalPriceWithoutTax - discount) / sqe.valueTotalFOB) * 100,4) else 0 end,
 totalFob = totalPriceWithoutTax - discount
 from SalesQuotationExterior sqe
 Inner JOin Invoice iv
    On iv.id = sqe.id
 Inner Join InvoiceDetail ivd
    On ivd.id_invoice = iv.id
 where iv.id = CONVERT(INT,@id)
   And ivd.isActive = 1

 Insert into #distribucionadicionales
 Select iv.id, ivd.id_item,
 gastoDistribuido = Round(totalFob + (totalAdicionales * porcentajeAdicionales)/100,4),
 precioFob = Round((totalFob + (totalAdicionales * porcentajeAdicionales)/100) / amount,5)
 from #Adicionales ad
 Inner JOin Invoice iv
    On iv.id = ad.id
 Inner Join InvoiceDetail ivd
    On ivd.id_invoice = iv.id
   And ivd.id_item = ad.id_item
 Where ivd.isActive = 1


 --select * from #distribucionadicionales
 --[par_ProformasReport] 6375

 SET @Total = (SELECT valuetotalCIF FROM SalesQuotationExterior WHERE id=@id)
 SET @Cents = ((SELECT valuetotalCIF FROM SalesQuotationExterior WHERE id=@id) - round((SELECT valuetotalCIF FROM SalesQuotationExterior WHERE id=@id),0,1))*100

 --if @Total > 0
 INSERT INTO @TEMP EXEC dbo.NumerosEnLetrASIngles @Total

  --if @Cents > 0
 INSERT INTO @TEMPCents EXEC dbo.NumerosEnLetrASIngles @Cents

 
 --INSERT INTO @DatosProducto
 --SELECT id_item,id_Invoice, RIGHT(descriptiON,13)AS cASepack FROM [InvoiceDetail] idt WHERE [idt].[id_invoice] = CONVERT(INT,@id) and [idt].[isActive] = 1 

 --UPDATE @DatosProducto SET cASepack=REPLACE(cASepack,'(','           ')
 --UPDATE @DatosProducto SET cASepack=REPLACE(cASepack,')','')
 --UPDATE @DatosProducto SET cASepack= RIGHT(cASepack,13)

 select @NetoKilos = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Kg'
						   and wt.code = 'NET')

 select @NetoLibras = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Lbs'
						   and wt.code = 'NET')

 select @BrutoKilos = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Kg'
						   and wt.code = 'BRT')

 select @BrutoLibras = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Lbs'
						   and wt.code = 'BRT')

 select @GlaseoKilos = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Kg'
						   and wt.code = 'GLS')
 

 select @GlaseoLibras = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Lbs'
						   and wt.code = 'GLS')


 select @ProformaKilos = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Kg'
						   and wt.code = 'PSP')


 select @ProformaLibras = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Lbs'
						   and wt.code = 'PSP')


 SELECT 
		[Invoice].[id] AS id,
		-- Cabecera 
		[Company].[trademark] AS NombreCia,
		[Company].[phONeNumber] AS TelefonoCia,
		[Company].[email] AS email,
		[Company].[PlantCode] AS PlantCode, 
		[Company].[registryFDA] AS FDA,
		[Company].[websiteCompany] AS Web,
		[BranchOffice].[address] AS DirSucural,   
		[Company].[ruc] AS RucCia,
		[Company].[logo] AS Logo,
		---- Invoice
		[Document].[number] AS Factura,
		[Document].[emissiONDate] AS FechaEmisión,
		[Document].[description] As Descripción,
		[Document].[reference] As Referencia,
		[termsNegotiation].[Code] AS [TerminoNegociación],-- = (SELECT [Code] FROM TermsNegotiatiON Tn WHERE Tn.[id]=[salesQuotationExterior].[id_termsNegotiatiON]),
		[salesQuotationExterior].[purchaseOrder] AS OrdenPedido,
		-- StakeHolder Sold to 
		[RazonSocialSoldTo]= (SELECT  [foreignCustomer].[fullname_businessName] FROM Person [foreignCustomer] 
		inner join ForeignCustomerIdentification fci on fci.id_ForeignCustomer = [foreignCustomer].id 
		WHERE fci.id_ForeignCustomer = salesQuotationExterior.id_consignee and fci.id = salesQuotationExterior.id_addressCustomer),
--[USCISoldTo] =(SELECT top 1 [foreignCustomer].[identification_number] FROM Person [foreignCustomer] WHERE [foreignCustomer].[id] = salesQuotationExterior.[id_notifier]),
[USCISoldTo] = (select a.numberIdentification from ForeignCustomerIdentification a where a.id_ForeignCustomer = salesQuotationExterior.id_consignee and
a.id = salesQuotationExterior.id_addressCustomer),
--[AddressSoldTo1] = upper((select top 1 f.addressForeign from foreigncustomeridentification f where f.id_foreignCustomer =[id_buyer]  )),
--[AddressSoldTo2] = upper((select top 1 f.addressForeign from foreigncustomeridentification f where f.id_foreignCustomer =[id_buyer]  )),
--select *from foreigncustomeridentification
[AddressSoldTo1] = (SELECT [ForeignCustomerIdentification].addressForeign FROM ForeignCustomerIdentification [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].id_ForeignCustomer = salesQuotationExterior.id_consignee and
ForeignCustomerIdentification.id =salesQuotationExterior.id_addressCustomer),
[AddressSoldTo2] =  (SELECT [ForeignCustomerIdentification].addressForeign FROM ForeignCustomerIdentification [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].id_ForeignCustomer = salesQuotationExterior.id_consignee and
ForeignCustomerIdentification.id =salesQuotationExterior.id_addressCustomer),
		--[AddressSoldTo1] = UPPER((SELECT address FROM PersON [persON]WHERE [persON].[id] = [invoice].[id_buyer])),
		--[AddressSoldTo2] = UPPER((SELECT address FROM PersON [persON]WHERE [persON].[id] = [invoice].[id_buyer])),
		--[CountrySoldTo] =  UPPER((SELECT [c].[name] FROM Country c INNER JOIN ForeignCustomer [fc] ON [fc].[id_country]= [c].[id] WHERE [fc].[id] = [invoice].[id_buyer])),
		[CountrySoldTo] = UPPER((SELECT c.[name]  from country c inner join foreigncustomeridentification f on f.id_country = c.id where f.id_ForeignCustomer = salesQuotationExterior.id_consignee and f.id = salesQuotationExterior.id_addressCustomer )),
		[CitySoldTo] = UPPER ((SELECT C.[NAME] FROM city c inner join foreigncustomeridentification f on f.id_city = C.ID where f.id_ForeignCustomer = salesQuotationExterior.id_consignee and f.id = salesQuotationExterior.id_addressCustomer)),

		


		-- select *from foreigncustomeridentification
		[Telefono1SoldTo] = (SELECT [ForeignCustomerIdentification].phone1FC FROM [ForeignCustomerIdentification] [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].[id_ForeignCustomer] = salesQuotationExterior.id_consignee and 
		ForeignCustomerIdentification.id = salesQuotationExterior.id_addressCustomer),
		--[Telefono1SoldTo] = (select phone1FC from ForeignCustomerIdentification where id_ForeignCustomer = salesQuotationExterior.id_notifier),
		[Telefono2SoldTo] =(SELECT top 1 [ForeignCustomerIdentification].fax1fc FROM [ForeignCustomerIdentification] [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].[id_ForeignCustomer] = salesQuotationExterior.[id_notifier]),---------------------------------------------
		[EmailSoldTo] = (SELECT  [ForeignCustomerIdentification].emailinterno FROM [ForeignCustomerIdentification] [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].[id_ForeignCustomer] = salesQuotationExterior.id_consignee and
		salesQuotationExterior.id_addressCustomer = ForeignCustomerIdentification.id),


		----------------------------------
		
		
		----CONSIGNATARIO

		--[Telefono1SoldTo] = (SELECT case when @Formato = 'SI' Then FORMAT(CONVERT(float,phONe1FC),'(000)-###-###-####') else phONe1FC end FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		--[Telefono2SoldTo] = (SELECT case when @Formato = 'SI' Then FORMAT(CONVERT(float,fax1FC),'(000)-###-###-####') else fax1FC end FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		--[EmailSoldTo] = (SELECT emailInternoCC FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		--[EmailSoldTo] =(select email from person person where person.id = invoice.id_buyer),
		---- StakeHolder Ship to
		[RazonSocialShipTo] = (SELECT top 1 [foreignCustomer].[fullname_businessName] FROM Person [foreignCustomer] WHERE [foreignCustomer].[id] = [salesQuotationExterior].id_notifier),
		
		[USCIShipTo] = (select top 1 a.numberIdentification from ForeignCustomerIdentification a where a.id_ForeignCustomer = salesQuotationExterior.id_notifier),
		--[AddressShipTo1] = upper((select top 1 addressforeign from foreigncustomeridentification where id_ForeignCustomer = [salesQuotationExterior].[id_consignee] )),
		--[AddressShipTo2] = upper((select top 1 addressforeign from foreigncustomeridentification where id_ForeignCustomer = [salesQuotationExterior].[id_consignee] )),
		[packingdetail] = salesQuotationExterior.PackingDetails,
	[AddressShipTo1] =	(SELECT top 1 [ForeignCustomerIdentification].addressForeign FROM [ForeignCustomerIdentification] [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].[id_ForeignCustomer] = salesQuotationExterior.id_notifier 
	and AddressType = 31),
		--[AddressShipTo1] = fcaddress.addressforeign,
		--[AddressShipTo2] = fcaddress.addressforeign,
		--[AddressShipTo1] = UPPER((SELECT top 1  address FROM PersON [persON] WHERE [persON].[id] = [salesQuotationExterior].[id_consignee])),
		--[AddressShipTo2] = UPPER((SELECT top 1 address FROM PersON [persON] WHERE [persON].[id] = [salesQuotationExterior].[id_consignee])),
		[CountryShipTo] = UPPER((SELECT top 1 c.[name]  from country c inner join foreigncustomeridentification f on f.id_country = c.id where f.id_ForeignCustomer = salesQuotationExterior.id_notifier)),
		[CityShipTo]  = UPPER ((SELECT top 1 C.[NAME] FROM city c inner join foreigncustomeridentification f on f.id_city = C.ID where f.id_ForeignCustomer = salesQuotationExterior.id_notifier)),
		--[CountryShipTo] = (SELECT [c].[name] FROM Country c INNER JOIN ForeignCustomer fc ON [fc].[id_country]= [c].[id] WHERE [fc].[id] = [salesQuotationExterior].[id_consignee]),
		--[CityShipTo] = (SELECT [c].[name] FROM City c 
		--				INNER JOIN ForeignCustomer [fc] 
		--				ON [fc].[id_city]= [c].[id] WHERE [fc].[id]=[salesQuotationExterior].[id_consignee]),
		[Telefono1ShipTo] =(select phone1FC from foreigncustomeridentification where id_ForeignCustomer =[salesQuotationExterior].id_notifier and AddressType = 31),
		[Telefono2ShipTo] = (select top 1 fax1FC from foreigncustomeridentification where id_ForeignCustomer =[salesQuotationExterior].id_notifier) ,
		--select *from foreigncustomeridentification

		--[EmailShipTo] = (SELECT emailInternoCC FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [salesQuotationExterior].[id_consignee]),
		[EmailShipTo] =  (select emailInterno from foreigncustomeridentification where id_ForeignCustomer =[salesQuotationExterior].id_notifier and AddressType = 31),
		---- Datos del Producto
		[PuertoDestino] = [Port_2].[nombre] +', '+ UPPER ([coun2].[Name2]), 
		
		[Company].[trademark] AS Shipper,
		
		[PuertoDeEmbaruqe] = UPPER([City_1].[name]) , 
		[FechaEmbarque] = ISNULL([mesFechaEmbarque].[mesIngles],'') + ' '+ISNULL(convert(char(2),DAY([salesQuotationExterior].[dateShipment])),'')+', '+ISNULL(convert(char(4),YEAR([salesQuotationExterior].[dateShipment])),'' ),
		[salesQuotationExterior].[purchaseOrder] AS PurchaseOrder,
		---- Detalles
		[InvoiceDetail].[numBoxes] AS Cartones ,
		[InvoiceDetail].[id_amountInvoice] AS Cantidad,
		[InvoiceDetail].[unitPrice] AS Precio,
		[InvoiceDetail].[id_amountInvoice] * [InvoiceDetail].[unitPrice] as gastoDistribuido,
		[itemSize2].[name] AS size,
		--[Presentation].[name] + ' = ' + REPLACE(RTRIM(LTRIM(REPLACE(Convert(Varchar,Round(([Presentation].[minimum] * [Presentation].[maximum]),2),100),'0',' '))),' ','0') AS Casepack ,
		[ItemTrademarkModel].[name] + 
		'(' + Convert(Varchar,[Presentation].[maximum]) + ' x ' 
		+ Convert(Varchar,Convert(decimal(10,3),
		Round(([InvoiceDetail].proformaWeight / [Presentation].[maximum]),3))) + 
		' ' + [MetricUnit].code  + ')' + ' = ' + 
		Convert(Varchar,Convert(decimal(10,3),Round([Presentation].[maximum] * 
		([InvoiceDetail].proformaWeight / [Presentation].[maximum]),3))) as Casepack,
		Presentation.name  AS Casepack2,
		[itemTrademark].[name] AS Marca,
		[itemDesc].[foreignName] AS descripcion,
		--[Item].[foreignName] AS descripcion,
		-- Total en Letra
		Total = (SELECT TOP 1 [resultado] FROM @TEMP) + ' Dollars and ' + (SELECT TOP 1 [resultado] FROM @TEMPCents) + ' Cents',
		docs.[name] AS 'Estado',
		--AD.gastodistribuido AS gastoDistribuido,-------------------
		--AD.precioFob AS precioFob,
		[InvoiceDetail].unitPrice AS precioFob,
		[salesQuotationExterior].valuetotalCIF AS valuetotalCIF,
		[PersonV].tradeName AS Vendedor,
		[PersonC].fullname_businessName AS Contacto,
		[PaymentTerm].[descriptionEnglish] AS PlazoPago,
		ISNULL([BoxCardAndBank].[code],'') AS CodigoBanco,
		ISNULL([BoxCardAndBank].[name],'') AS NombreBanco,
		ISNULL([BoxCardAndBank].[country],'') AS PaisBanco,
		ISNULL([BoxCardAndBank].[adress],'') AS DireccionBanco,
		ISNULL([BoxCardAndBank].[currency],'') AS MonedaBanco,
		ISNULL([BoxCardAndBank].[routing],0) AS EnrutamientoBanco,
		ISNULL([BoxCardAndBank].[account],'') AS CuentaBanco,
		ISNULL([BoxCardAndBank].[companyName],'') AS NombreCompaniaBanco,
		ISNULL([BoxCardAndBank].[codeIntermediary],'') AS CodigoBancoIntermediario,
		ISNULL([BoxCardAndBank].[bankNameIntermediary],'') AS NombreBancoIntermediario,
		ISNULL([BoxCardAndBank].[accountIntermediary],'') AS CuentaBancoIntermediario,
		ISNULL([BoxCardAndBank].[currencyIntermediary],'') AS MonedaBancoIntermediario,
		ISNULL([BoxCardAndBank].[countryIntermediary],'') AS PaisBancoIntermediario,
		ISNULL([salesQuotationExterior].[Product],'') AS Product,
		ISNULL([salesQuotationExterior].[ColourGrade],'') AS ColourGrade,
		ISNULL([salesQuotationExterior].[PackingDetails],'') AS PackingDetails,
		ISNULL([salesQuotationExterior].[ContainerDetails],'') AS ContainerDetails,
		@NetoKilos AS NetoKilos,
		@NetoLibras AS NetoLibras,
		@BrutoKilos AS BrutoKilos,
		@BrutoLibras AS BrutoLibras,
		@GlaseoKilos As GlaseoKilos,
		@GlaseoLibras As GlaseoLibras,
		@ProformaKilos as ProformaKilos,
		@ProformaLibras as ProformaLibras,
		doc.dateUpdate AS FechaActualizacion,
		GETDATE() AS FechaActual,
		Usuario = Case When docs.code = '03' Then Us.username else '' end,
		ISNULL([Company].plantCode,'') AS CodigoPlanta,
		ISNULL([Company].registryFDA,'') AS FDA,
		--[salesQuotationExterior].numeroContenedores AS NumeroContenedores,
		Convert(Varchar,[salesQuotationExterior].[numeroContenedores],10) + ' CONTAINER ' + UPPER([ca].[name]) + ' FEET' AS NumeroContenedores,
		[salesQuotationExterior].[valueSubscribed]AS ValorAbonado,-------------------------------------
		CONTACTOCONS.fullname_businessName as contacto_2,
		--descripcion = [InvoiceDetail].descriptionCustomer,	
		descripcion = [InvoiceDetail].descriptionCustomer,	
		PAYM.name AS METODOPAGO,
		isnull(upper(salesQuotationExterior.transport),'')  as transport
		--[ForCustomer].fax1FC as Fax
	
		,MetricUnit.id as MU
		,ussoli.fullname_businessName as UsuarioVentas
		,case when 
		doc.id_documentState <> 1 then 
		direc.fullname_businessName else 
		' 'end as DirectorComercial,PaymentTerm.id,[PaymentTerm].[descriptionEnglish] AS PlazoPago2
	
		
		FROM [dbo].[Invoice] [invoice]      
  INNER JOIN [dbo].[Document] doc ON [invoice].[id] = [doc].[id]      
  INNER JOIN [dbo].[DocumentState] docs ON doc.[id_documentState] = docs.[id]   
  INNER JOIN [dbo].[SalesQuotationExterior][salesQuotationExterior] ON [salesQuotationExterior].[id] = [invoice].[id]
		AND [salesQuotationExterior].[id] = [doc].[id]
  INNER JOIN [dbo].[Document] prof ON prof.[id] = [salesQuotationExterior].[id]
  INNER JOIN [dbo].[InvoiceDetail] [InvoiceDetail] ON [InvoiceDetail].[id_invoice] = [Invoice].[id]      
    and [InvoiceDetail].[isActive] = 1      
  INNER JOIN [dbo].[Item] [item] ON [Item].[id] = [InvoiceDetail].[id_item]
  INNER JOIN [dbo].[Item] [itemDesc] ON [itemDesc].[id] = [InvoiceDetail].[id_itemMarked]
  INNER JOIN [dbo].[MetricUnit] [MetricUnit] ON [MetricUnit].[id] = [InvoiceDetail].[id_metricUnitInvoiceDetail]  
  INNER JOIN [ItemGeneral] [itemGeneral] ON [invoiceDetail].[id_item] = [itemGeneral].[id_item]

  INNER JOIN [ItemSize] [itemSize] ON [itemSize].[id] = [itemGeneral].[id_size]
  INNER JOIN [ItemTrademark] [itemTrademark] ON [itemTrademark].[id] = [itemGeneral].[id_Trademark]
  INNER JOIN [dbo].[Document] [Document]  ON [Document].[id] = [Invoice].[id]      
  --INNER JOIN #distribucionadicionales AD ON AD.id = [invoice].id AND AD.id_item = [InvoiceDetail].id_item-----------------------------------------------
  LEFT OUTER JOIN [ItemGeneral] [ItemGeneral2] on [itemDesc].id=[ItemGeneral2].id_item
  LEFT OUTER JOIN [ItemSize] [itemSize2] ON [itemSize2].[id] = [itemGeneral2].[id_size]
  LEFT OUTER JOIN [dbo].[Presentation] [Presentation] ON [Presentation].[id] = [item].[id_presentation] 	
  LEFT OUTER JOIN [dbo].[ItemTrademarkModel] [ItemTrademarkModel] ON [ItemTrademarkModel].[id] = [itemGeneral].[id_trademarkModel]       
  LEFT OUTER JOIN [dbo].[PersON] [PersON] ON [PersON].[id]= [salesQuotationExterior].[id_consignee]    
  LEFT OUTER JOIN [dbo].[Person] [PersonV] ON [PersonV].[id]= [salesQuotationExterior].[idVendor] 
  LEFT OUTER JOIN [dbo].[Person] [PersonC] ON [PersonC].[id]= [salesQuotationExterior].[id_personContact]   	     
  LEFT OUTER JOIN [dbo].[ForeignCustomer] [ForCustomer] ON [ForCustomer].[id] = [salesQuotationExterior].[id_consignee]      
  LEFT OUTER JOIN [dbo].[Port] [Port] ON [Port].[id] = [salesQuotationExterior].[id_portDischarge]      
  LEFT OUTER JOIN [dbo].[City] [City] ON [Port].[id_city] = [City].[id]      
  LEFT OUTER JOIN [dbo].[Country] coun ON coun.[id] = [City].[id_country]   
  LEFT OUTER JOIN [dbo].[Port] [Port_1] ON [Port_1].[id] = [salesQuotationExterior].[id_portShipment]     
  LEFT OUTER JOIN [dbo].[City] [City_1] ON [Port_1].[id_city] = [City_1].[id]
  LEFT OUTER JOIN [dbo].[Port] [Port_2] ON [Port_2].[id] = [salesQuotationExterior].[id_portDestination]      
  LEFT OUTER JOIN [dbo].[EmissionPoint] [emissionPoint] ON [emissionPoint].[id] = [Document].[id_emissionPoint]      
  LEFT OUTER JOIN [dbo].[Company] [Company] ON [Company].[id] = [emissionPoint].[id_company]   	      
  LEFT OUTER JOIN [dbo].[City] [City_2] ON [City_2].[id] = [Port_2].[id_city]      
  LEFT OUTER JOIN [dbo].[Country] [coun2] ON [coun2].[id] = [City_2].[id_country]      
  LEFT OUTER JOIN [dbo].[Country] [Country_DestinatiON] ON [Country_DestinatiON].[id] = [City_2].[id_country]           
  LEFT OUTER JOIN [dbo].[TariffHeading] [TariffHeading] ON [TariffHeading].[id] = [invoiceDetail].[id_tariffHeadingDetail]          
  LEFT OUTER JOIN [dbo].[BranchOffice] [BranchOffice] ON [BranchOffice].[id] = [emissionPoint].[id_branchOffice]      
  LEFT OUTER JOIN [dbo].[DivisiON] [DivisiON] ON [Division].[id] = [emissionPoint].[id_division]       
  LEFT OUTER JOIN [dbo].[TermsNegotiation] [termsNegotiation] ON [salesQuotationExterior].[id_termsNegotiation] = [termsNegotiation].[id]      
  LEFT OUTER JOIN [dbo].[CompanyElectronicFacturation] [companyElectronicFacturation] ON [Company].[id] = [companyElectronicFacturation].[id_company]  
  LEFT OUTER JOIN [dbo].[PaymentTerm] [PaymentTerm] ON [PaymentTerm].[id] = [salesQuotationExterior].[id_PaymentTerm]  
  LEFT OUTER JOIN [dbo].[EnvironmentType] [environmentType] ON [environmentType].[id] = [companyElectronicFacturation].[id_enviromentType]      
  LEFT OUTER JOIN [dbo].[EmissionType] [emissionType] ON [emissionType].[id] = [companyElectronicFacturation].[id_emissiONType]  
  LEFT OUTER JOIN (select ctd.code, ctd.description mesEspanol, ctd.fldvarchar1 mesIngles
				from tbsysCatalogueDetail ctd 
				inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue
				where ct.code ='MONTHT') mesFechaEmbarque
		ON mesFechaEmbarque.code =  cast(month([SalesQuotationExterior].[dateShipment]) as varchar)
  LEFT OUTER JOIN [dbo].[BoxCardAndBank] [BoxCardAndBank] ON [BoxCardAndBank].[id] = [SalesQuotationExterior] .[id_bank] 
  LEFT OUTER JOIN  ForeignCustomerIdentificatiON FCI ON FCI.id_ForeignCustomer = [PersON].ID and FCI.id = salesQuotationExterior.id_addressCustomer
  LEFT OUTER JOIN [dbo].[User] Usu ON Usu.id = [doc].id_userUpdate
  lEFT OUTER JOIN [dbo].[User] Us ON Us.id = [doc].id_userCreate
  LEFT OUTER JOIN PaymentMethod PAYM ON PAYM.ID = SalesQuotationExterior.id_PaymentMethod
  LEFT OUTER JOIN [dbo].[CapacityContainer] ca ON ca.id = [salesQuotationExterior].id_capacityContainer
  LEFT OUTER JOIN [dbo].[MetricUnit] Umc ON Umc.id = [ca].id_metricUnit
  left outer join person CONTACTOCONS ON CONTACTOCONS.id = [salesQuotationExterior].id_personcontactConsignatario
  left outer join ForeignCustomer phfc on phfc.id = personc.id
  left outer join ForeignCustomer phfc2 on phfc2.id = CONTACTOCONS.id 
  LEFT JOIN PERSON ussoli on ussoli.id = us.id_employee 
  left join person direc on direc.id = usu.id_employee 
  WHERE [Invoice].[id] = CONVERT(INT,@id)
  ORDER BY Item.mASterCode  

/*
	EXEC spPar_ProformaPC 0
*/

GO