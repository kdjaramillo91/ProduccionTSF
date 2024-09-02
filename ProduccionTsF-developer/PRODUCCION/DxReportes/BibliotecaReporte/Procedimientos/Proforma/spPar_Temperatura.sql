--exec "PanaceaProduccionPC0808"."dbo"."par_ProformasTemperatura";1 958035
GO
/****** Object:  StoredProcedure [dbo].[par_ProformasTemperatura]    Script Date: 20/12/2022 8:11:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO



CREATE procedure  [dbo].[spPar_Temperatura]  

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

 SET @Total = (SELECT sum(gastodistribuido) FROM #distribucionadicionales WHERE id=@id)
 SET @Cents = ((SELECT sum(gastodistribuido) FROM #distribucionadicionales WHERE id=@id) - round((SELECT sum(gastodistribuido) FROM #distribucionadicionales WHERE id=@id),0,1))*100

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
						   and wt.code = 'GLS')

 select @NetoLibras = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Lbs'
						   and wt.code = 'GLS')

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
		[Document].[emissiONDate] AS FechaEmision,
		[Document].[description] As Descripcion,
		[Document].[reference] As Referencia,
		[termsNegotiation].[Code] AS [TerminoNegociacion],-- = (SELECT [Code] FROM TermsNegotiatiON Tn WHERE Tn.[id]=[salesQuotationExterior].[id_termsNegotiatiON]),
		[salesQuotationExterior].[purchaseOrder] AS OrdenPedido,
		-- StakeHolder Sold to 
		[RazonSocialSoldTo]= (SELECT [foreignCustomer].[fullname_businessName] FROM Person [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		[USCISoldTo] = (SELECT ITP.code + ' - ' + FCI.numberIdentification FROM ForeignCustomer [foreignCustomer] 
						INNER JOIN ForeignCustomerIdentificatiON FCI ON FCI.id_ForeignCustomer = [foreignCustomer].[id]
						INNER JOIN Country_IdentificationType CIT ON CIT.id = FCI.id_Country_IdentificationType
						INNER JOIN IdentificationType ITP ON ITP.id = CIT.id_identificationType 
						WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		[AddressSoldTo1] = (SELECT address FROM PersON [persON]WHERE [persON].[id] = [invoice].[id_buyer]),
		[AddressSoldTo2] = (SELECT address FROM PersON [persON]WHERE [persON].[id] = [invoice].[id_buyer]),
		--[CountrySoldTo] =  (SELECT [c].[name] FROM Country c INNER JOIN ForeignCustomer [fc] ON [fc].[id_country]= [c].[id] WHERE [fc].[id] = [invoice].[id_buyer]),
		--[CitySoldTo] = (SELECT [c].[name] FROM City c 
		--			   INNER JOIN ForeignCustomer [fc] 
		--			   ON [fc].[id_city]= [c].[id] WHERE [fc].[id]=[invoice].[id_buyer]),
		[Telefono1SoldTo] = (SELECT case when @Formato = 'SI' Then FORMAT(CONVERT(float,phONe1FC),'(000)-###-###-####') else phONe1FC end FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		[Telefono2SoldTo] = (SELECT case when @Formato = 'SI' Then FORMAT(CONVERT(float,fax1FC),'(000)-###-###-####') else fax1FC end FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		--[EmailSoldTo] = (SELECT emailInternoCC FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		[EmailSoldTo] = (SELECT email FROM PersON [persON]WHERE [persON].[id] = [salesQuotationExterior].[id_personContact]),
		---- StakeHolder Ship to
		[RazonSocialShipTo] = (SELECT [foreignCustomer].[fullname_businessName] FROM Person [foreignCustomer] WHERE [foreignCustomer].[id] = [salesQuotationExterior].[id_consignee]),
		[USCIShipTo] = (SELECT ITP.code + ' - ' + FCI.numberIdentification FROM ForeignCustomer [foreignCustomer] 
						INNER JOIN ForeignCustomerIdentificatiON FCI ON FCI.id_ForeignCustomer = [foreignCustomer].[id]
						INNER JOIN Country_IdentificationType CIT ON CIT.id = FCI.id_Country_IdentificationType
						INNER JOIN IdentificationType ITP ON ITP.id = CIT.id_identificationType 
						WHERE [foreignCustomer].[id] = [salesQuotationExterior].[id_consignee]),
		[AddressShipTo1] = (SELECT address FROM PersON [persON] WHERE [persON].[id] = [salesQuotationExterior].[id_consignee]),
		[AddressShipTo2] = (SELECT address FROM PersON [persON] WHERE [persON].[id] = [salesQuotationExterior].[id_consignee]),
		--[CountryShipTo] = (SELECT [c].[name] FROM Country c INNER JOIN ForeignCustomer fc ON [fc].[id_country]= [c].[id] WHERE [fc].[id] = [salesQuotationExterior].[id_consignee]),
		--[CityShipTo] = (SELECT [c].[name] FROM City c 
		--				INNER JOIN ForeignCustomer [fc] 
		--				ON [fc].[id_city]= [c].[id] WHERE [fc].[id]=[salesQuotationExterior].[id_consignee]),
		[Telefono1ShipTo] = (SELECT case when @Formato = 'SI' Then FORMAT(CONVERT(float,phONe1FC),'(000)-###-###-####') else phONe1FC end FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [salesQuotationExterior].[id_consignee]),
		[Telefono2ShipTo] = (SELECT case when @Formato = 'SI' Then FORMAT(CONVERT(float,fax1FC),'(000)-###-###-####') else fax1FC end FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [salesQuotationExterior].[id_consignee]),
		--[EmailShipTo] = (SELECT emailInternoCC FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [salesQuotationExterior].[id_consignee]),
		[EmailShipTo] = (SELECT email FROM PersON [persON]WHERE [persON].[id] = [salesQuotationExterior].[id_personContact]),
		---- Datos del Producto
		[PuertoDestino] = [Port_2].[nombre] +', '+ UPPER ([coun2].[Name2]), 
		
		[Company].[trademark] AS Shipper,
		
		[PuertoDeEmbaruqe] = UPPER([City_1].[name]) , 
		[FechaEmbarque] = [mesFechaEmbarque].[mesIngles] + ' '+convert(char(2),DAY([salesQuotationExterior].[dateShipment]))+', '+convert(char(4),YEAR([salesQuotationExterior].[dateShipment]) ),
		[salesQuotationExterior].[purchaseOrder] AS PurchaseOrder,
		---- Detalles
		[InvoiceDetail].[numBoxes] AS Cartones ,
		[InvoiceDetail].[id_amountInvoice] AS Cantidad,
		[InvoiceDetail].[unitPrice] AS Precio,
		[itemSize].[name] AS size,
		--[Presentation].[name] + ' = ' + REPLACE(RTRIM(LTRIM(REPLACE(Convert(Varchar,Round(([Presentation].[minimum] * [Presentation].[maximum]),2),100),'0',' '))),' ','0') AS Casepack ,
		[ItemTrademarkModel].[name] + '(' + Convert(Varchar,[Presentation].[maximum]) + ' x ' + Convert(Varchar,Convert(decimal(10,3),Round(([InvoiceDetail].proformaWeight / [Presentation].[maximum]),3))) + ' ' + [MetricUnit].code  + ')' + ' = ' +  Convert(Varchar,Convert(decimal(10,3),Round([Presentation].[maximum] * ([InvoiceDetail].proformaWeight / [Presentation].[maximum]),3))) AS Casepack,
		[itemTrademark].[name] AS Marca,
		[Item].[foreignName] AS descripcion,
		-- Total en Letra
		Total = (SELECT TOP 1 [resultado] FROM @TEMP) + ' Dollars and ' + (SELECT TOP 1 [resultado] FROM @TEMPCents) + ' Cents',
		docs.[name] AS 'Estado',
		AD.gastodistribuido AS gastoDistribuido,
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
		doc.dateUpdate AS FechaActualizacion,
		GETDATE() AS FechaActual,
		Usuario = Case When docs.code = '03' Then Us.username else '' end,
		ISNULL([Company].plantCode,'') AS CodigoPlanta,
		ISNULL([Company].registryFDA,'') AS FDA,
		[salesQuotationExterior].numeroContenedores AS NumeroContenedores,
		[salesQuotationExterior].[valueSubscribed]AS ValorAbonado,-------------------------------------
		BUYER.fullname_businessName AS CLIENTE_EXTERIOR,--------------------------------------
		PuertoDescarga3 =  [Port].[nombre] + ', ' + coun.name,--------------------------------------
		CatD.code  AS TipoTemperatura,-----------------------------------------------------
		[salesQuotationExterior].temperatureInstruction as TEMPERATURAINSTRUCCION---------------
		FROM [dbo].[Invoice] [invoice]      
  INNER JOIN [dbo].[Document] doc ON [invoice].[id] = [doc].[id]      
  INNER JOIN [dbo].[DocumentState] docs ON doc.[id_documentState] = docs.[id]   
  INNER JOIN [dbo].[SalesQuotationExterior][salesQuotationExterior] ON [salesQuotationExterior].[id] = [invoice].[id]
		AND [salesQuotationExterior].[id] = [doc].[id]
  INNER JOIN [dbo].[Document] prof ON prof.[id] = [salesQuotationExterior].[id]
  INNER JOIN [dbo].[InvoiceDetail] [InvoiceDetail] ON [InvoiceDetail].[id_invoice] = [Invoice].[id]      
    and [InvoiceDetail].[isActive] = 1      
  INNER JOIN [dbo].[Item] [item] ON [Item].[id] = [InvoiceDetail].[id_item]      
  INNER JOIN [dbo].[MetricUnit] [MetricUnit] ON [MetricUnit].[id] = [InvoiceDetail].[id_metricUnitInvoiceDetail]  
  INNER JOIN [ItemGeneral] [itemGeneral] ON [invoiceDetail].[id_item] = [itemGeneral].[id_item]
  INNER JOIN [ItemSize] [itemSize] ON [itemSize].[id] = [itemGeneral].[id_size]
  INNER JOIN [ItemTrademark] [itemTrademark] ON [itemTrademark].[id] = [itemGeneral].[id_Trademark]
  INNER JOIN [dbo].[Document] [Document]  ON [Document].[id] = [Invoice].[id]      
  INNER JOIN #distribucionadicionales AD ON AD.id = [invoice].id AND AD.id_item = [InvoiceDetail].id_item
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
  LEFT OUTER JOIN  ForeignCustomerIdentificatiON FCI ON FCI.id_ForeignCustomer = [PersON].ID 
  LEFT OUTER JOIN [dbo].[User] Us ON Us.id = [doc].id_userUpdate
  LEFT OUTER JOIN PERSON BUYER ON BUYER.ID =  INVOICE.id_buyer----------------------
  left join 	tbsysCatalogueDetail CATD on CATD.id = [SalesQuotationExterior].idTipoTemperatura--------------------------------------
  WHERE [Invoice].[id] = CONVERT(INT,@id)
  ORDER BY Item.mASterCode  


 -- EXEC [dbo].[par_ProformasReport] 
