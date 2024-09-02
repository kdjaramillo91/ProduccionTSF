/****** Object:  StoredProcedure [dbo].[spPar_ProformasReportContract]    Script Date: 19/06/2023 01:38:11 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER   PROCEDURE [dbo].[spPar_ProformasReportContract]
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

 SET @Total = (SELECT valuetotalCIF FROM SalesQuotationExterior WHERE id=@id)
 SET @Cents = ((SELECT valuetotalCIF FROM SalesQuotationExterior WHERE id=@id) - round((SELECT valuetotalCIF FROM SalesQuotationExterior WHERE id=@id),0,1))*100 

 --if @Total > 0
 INSERT INTO @TEMP EXEC dbo.NumerosEnLetrASIngles @Total

  --if @Cents > 0
 INSERT INTO @TEMPCents EXEC dbo.NumerosEnLetrASIngles @Cents

 /* Cálculo del valor Terminos de pago en Texto */
DECLARE @TermPagoD INT
DECLARE @TermPagoC INT
DECLARE @AuxTermPago DECIMAL(13, 6)
DECLARE @TxtTermPagoD VARCHAR(MAX)
DECLARE @TxtTermPagoC VARCHAR(MAX)

SELECT @AuxTermPago = ISNULL(valueSubscribed, 0)*ISNULL([numeroContenedores], 0)
FROM SalesQuotationExterior WHERE id = @id

SET @TermPagoD = @AuxTermPago
SET @TermPagoC = (@AuxTermPago-ROUND(@AuxTermPago, 0, 1))*100

SET @TxtTermPagoD = [dbo].[FUN_CantidadConLetra](@TermPagoD)
SET @TxtTermPagoC = [dbo].[FUN_CantidadConLetra](@TermPagoC)

/*Fin cálculo de valor */

/* Cálculo de valor TOTALCFR - Términos de Pago*/
DECLARE @CFRTotal DECIMAL(13, 2)
DECLARE @Balance DECIMAL(13, 2)
DECLARE @BalanceD INT
DECLARE @BalanceC INT
DECLARE @TxtBalanceD VARCHAR(MAX)
DECLARE @TxtBalanceC VARCHAR(MAX)

SET @CFRTotal = CAST(@Total AS DECIMAL(13,2)) + (@Cents/100)
SET @Balance = @CFRTotal - @AuxTermPago

SET @BalanceD = @Balance
SET @BalanceC= (@Balance-ROUND(@Balance, 0, 1))*100
SET @TxtBalanceD = [dbo].[FUN_CantidadConLetra](@BalanceD)
SET @TxtBalanceC = [dbo].[FUN_CantidadConLetra](@BalanceC)

/*Fin cálculo */

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


 SELECT 
		[Invoice].[id] AS id,
		-- Cabecera 
		[Company].[businessName] AS NombreCiaLargo,
		[Company].[trademark] AS NombreCia,
		[Company].[phONeNumber] AS TelefonoCia,
		[Company].[email] AS email,
		[Company].[PlantCode] AS PlantCode, 
		[Company].[registryFDA] AS FDA,
		[Company].[websiteCompany] AS Web,
		company.[address] AS DirSucural,   
		[Company].[ruc] AS RucCia,
		[Company].[logo] AS Logo,
		---- Invoice
		[Document].[number] AS Factura,
		[Document].[emissiONDate] AS FechaEmision,
		[Document].[description] As Descripción,
		[Document].[reference] As Referencia,
		[termsNegotiation].[Code] AS [TerminoNegociación],-- = (SELECT [Code] FROM TermsNegotiatiON Tn WHERE Tn.[id]=[salesQuotationExterior].[id_termsNegotiatiON]),
		[salesQuotationExterior].[purchaseOrder] AS OrdenPedido,
		-- StakeHolder Sold to 
		[RazonSocialSoldTo]= (SELECT top 1 [foreignCustomer].[fullname_businessName] FROM Person [foreignCustomer] WHERE [foreignCustomer].[id] = salesQuotationExterior.[id_notifier]),
		[USCISoldTo] =(SELECT top 1 [foreignCustomer].[identification_number] FROM Person [foreignCustomer] WHERE [foreignCustomer].[id] = salesQuotationExterior.[id_notifier]),
		[AddressSoldTo1] = (SELECT [ForeignCustomerIdentification].addressForeign FROM [ForeignCustomerIdentification] [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].[id_ForeignCustomer] = salesQuotationExterior.[id_notifier] and 
		[ForeignCustomerIdentification].id = salesQuotationExterior.id_addressCustomer),
		[AddressSoldTo2] =  (SELECT top 1 [foreignCustomer].address FROM Person [foreignCustomer] WHERE [foreignCustomer].[id] = salesQuotationExterior.[id_notifier]),
		[CountrySoldTo] = UPPER((SELECT top 1 c.[name]  from country c inner join foreigncustomeridentification f on f.id_country = c.id where f.id_ForeignCustomer = [invoice].[id_buyer] )),
		[CitySoldTo] = UPPER ((SELECT top 1 C.[NAME] FROM city c inner join foreigncustomeridentification f on f.id_city = C.ID where f.id_ForeignCustomer = [invoice].[id_buyer] and f.id = salesQuotationExterior.id_addressCustomer)),
		[StateCountrySoldTo] = ISNULL(UPPER((SELECT TOP 1 st.name FROM foreigncustomeridentification fc
											INNER JOIN Country Co ON Co.id = fc.id_country
											INNER JOIN City ct ON ct.id = fc.id_city
											 AND ct.id_country = fc.id_country
											INNER JOIN StateOfContry st ON st.id = ct.id_stateOfContry
											 AND st.id_country = fc.id_country
											WHERE fc.id_ForeignCustomer = [invoice].[id_buyer] )), ''),
		[Telefono1SoldTo] = (SELECT top 1 [ForeignCustomerIdentification].phone1FC FROM [ForeignCustomerIdentification] [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].[id_ForeignCustomer] = salesQuotationExterior.[id_notifier]),
		[Telefono2SoldTo] =(SELECT top 1 [ForeignCustomerIdentification].fax1fc FROM [ForeignCustomerIdentification] [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].[id_ForeignCustomer] = salesQuotationExterior.[id_notifier]),-------------


		[EmailSoldTo] = (SELECT top 1 [ForeignCustomerIdentification].emailinterno FROM [ForeignCustomerIdentification] [ForeignCustomerIdentification] WHERE [ForeignCustomerIdentification].[id_ForeignCustomer] = salesQuotationExterior.[id_notifier]),
		----------------------------------
		
		
		----CONSIGNATARIO

		--[Telefono1SoldTo] = (SELECT case when @Formato = 'SI' Then FORMAT(CONVERT(float,phONe1FC),'(000)-###-###-####') else phONe1FC end FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		--[Telefono2SoldTo] = (SELECT case when @Formato = 'SI' Then FORMAT(CONVERT(float,fax1FC),'(000)-###-###-####') else fax1FC end FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		--[EmailSoldTo] = (SELECT emailInternoCC FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		--[EmailSoldTo] =(select email from person person where person.id = invoice.id_buyer),
		---- StakeHolder Ship to
		[RazonSocialShipTo] = (SELECT top 1 [foreignCustomer].[fullname_businessName] FROM Person [foreignCustomer] WHERE [foreignCustomer].[id] = [salesQuotationExterior].[id_consignee]),
		
		[USCIShipTo] = (SELECT top 1 ITP.code + ' - ' + FCI.numberIdentification FROM ForeignCustomer [foreignCustomer] 
						INNER JOIN ForeignCustomerIdentificatiON FCI ON FCI.id_ForeignCustomer = [foreignCustomer].[id]
						INNER JOIN Country_IdentificationType CIT ON CIT.id = FCI.id_Country_IdentificationType
						INNER JOIN IdentificationType ITP ON ITP.id = CIT.id_identificationType 
						WHERE [foreignCustomer].[id] = [salesQuotationExterior].[id_consignee]),
		[packingdetail] = salesQuotationExterior.PackingDetails,
		[AddressShipTo1] = fcaddress.addressforeign,
		[AddressShipTo2] = fcaddress.addressforeign,
		[CountryShipTo] = UPPER((SELECT top 1 c.[name]  from country c inner join foreigncustomeridentification f on f.id_country = c.id where f.id_ForeignCustomer = salesQuotationExterior.id_consignee)),
		[CityShipTo]  = UPPER ((SELECT top 1 C.[NAME] FROM city c inner join foreigncustomeridentification f on f.id_city = C.ID where f.id_ForeignCustomer = salesQuotationExterior.id_consignee and f.id = salesQuotationExterior.id_addressCustomer)),
		[Telefono1ShipTo] =(select top 1 phone1FC from foreigncustomeridentification where id =[salesQuotationExterior].[id_addressCustomer]) ,
		[Telefono2ShipTo] = (select top 1 fax1FC from foreigncustomeridentification where id =[salesQuotationExterior].[id_addressCustomer]) ,
		[EmailShipTo] =  (select top 1 emailInterno from foreigncustomeridentification where id =[salesQuotationExterior].[id_addressCustomer]) ,
		---- Datos del Producto
		[PuertoDestino] = [Port_2].[nombre] +', '+ UPPER ([coun2].[Name2]), 		
		[Company].[trademark] AS Shipper,		
		[PuertoDeEmbaruqe] = UPPER([City_1].[name]), 
		ISNULL(UPPER(CoShipment.name), '') AS [PaisEmbarque],
		ISNULL(UPPER([City_1].[name]), '') + ', ' + ISNULL(UPPER(CoShipment.name), '') AS [PortLoading],
		[FechaEmbarque] = ISNULL([mesFechaEmbarque].[mesIngles],'') + ' '+ISNULL(convert(char(2),DAY([salesQuotationExterior].[dateShipment])),'')+', '+ISNULL(convert(char(4),YEAR([salesQuotationExterior].[dateShipment])),'' ),
		[FechaEmbarqueIngles] = ISNULL(ISNULL(CONVERT(CHAR(2), DAY([salesQuotationExterior].[dateShipment])), '') + '-' + ISNULL(CONVERT(CHAR(2), MONTH([salesQuotationExterior].[dateShipment])),'') + '-' + ISNULL(CONVERT(CHAR(4), YEAR([salesQuotationExterior].[dateShipment])), ''), ''),
		--[FechaEmbarqueEsp] = ISNULL(CONVERT(DATETIME, [salesQuotationExterior].[dateShipment]), ''),
		[salesQuotationExterior].[purchaseOrder] AS PurchaseOrder,
		---- Detalles
		[InvoiceDetail].[numBoxes] AS Cartones ,
		[InvoiceDetail].[id_amountInvoice] AS Cantidad,
		[InvoiceDetail].amount AS CantidadOrigen,
		[InvoiceDetail].[unitPrice] AS Precio,
		[InvoiceDetail].[id_amountInvoice] * [InvoiceDetail].[unitPrice] as gastoDistribuido,
		[itemSize].[name] AS size,
		--[ItemTrademarkModel].[name] + '(' + Convert(Varchar,[Presentation].[maximum]) + ' x ' + Convert(Varchar,Convert(decimal(10,2),Round(([InvoiceDetail].proformaWeight / [Presentation].[maximum]),2))) + ' ' + [MetricUnit].code  + ')' + ' = ' +  Convert(Varchar,Convert(decimal(10,2),Round([Presentation].[maximum] * ([InvoiceDetail].proformaWeight / [Presentation].[maximum]),2))) AS Casepack,
		CONVERT(VARCHAR(20), [Presentation].maximum) + ' X ' + CONVERT(VARCHAR, CONVERT(DECIMAL(6, 2), [Presentation].minimum))+ ' ' + CONVERT(VARCHAR, ISNULL(metU.code, 0)) AS [CasePackDimension],
		[itemTrademark].[name] AS Marca,
		[Item].[foreignName] AS descripcion,
		ISNULL([salesQuotationExterior].[Product], '') AS [descripcionGeneral],
		-- Total en Letra
		Total = (SELECT TOP 1 [resultado] FROM @TEMP) + ' Dollars and ' + (SELECT TOP 1 [resultado] FROM @TEMPCents) + ' Cents',
		docs.[name] AS 'Estado',
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
		doc.dateUpdate AS FechaActualizacion,
		GETDATE() AS FechaActual,
		Usuario = Case When docs.code = '03' Then Us.username else '' end,
		ISNULL([Company].plantCode,'') AS CodigoPlanta,
		ISNULL([Company].registryFDA,'') AS FDA,
		Convert(Varchar,[salesQuotationExterior].[numeroContenedores],10) + ' CONTAINER ' + UPPER([ca].[name]) + ' FEET' AS NumeroContenedores,
		CONVERT(VARCHAR, [salesQuotationExterior].[numeroContenedores],10) AS [CantidadContenedores],
		UPPER([ca].[name]) + ' FEET' AS [TamañoContenedor],
		[salesQuotationExterior].[valueSubscribed]AS ValorAbonado,-------------------------------------
		CONTACTOCONS.fullname_businessName as contacto_2,
		descripcionCliente = [InvoiceDetail].descriptionCustomer,			
		PAYM.name AS METODOPAGO,
		isnull(upper(salesQuotationExterior.transport),'')  as transport,
		MetricUnit.id as MU,
		NProforma = 'TSF - '+ convert(varchar,Document.sequential),
		-- Nuevos campos requeridos (17-01-2023)
		ISNULL(ideT.name, '') AS [IdenficationTypeName],
		ISNULL(City_2.name, '') AS [CityName], 
		ISNULL(soC.name, '') AS [StateofCountryName],
		ISNULL(iteGC.name, '') AS [Categoria2],
		ISNULL(iteV.name, '') AS [TallaMarcada],
		ISNULL([salesQuotationExterior].valueSubscribed, 0) AS [ValorAnticipo],
		CONVERT(DECIMAL(13, 2), ISNULL([salesQuotationExterior].valueSubscribed, 0)*ISNULL([salesQuotationExterior].[numeroContenedores], 0)) AS [ValorTotalTermPago],
		@TxtTermPagoD + ' DOLLAR(S) AND ' + @TxtTermPagoC + ' CENT(S)' AS [ValorTotalTermPagoString],
		@Balance AS [Balance],
		@TxtBalanceD + ' DOLLAR(S) AND ' + @TxtBalanceC + ' CENT(S)' AS [BalanceString]
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
  LEFT OUTER JOIN [dbo].[Presentation] [Presentation] ON [Presentation].[id] = [item].[id_presentation] 
  LEFT OUTER JOIN [dbo].[MetricUnit] [metU]	 ON metU.id = Presentation.id_metricUnit
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
  -- jinm
  LEFT OUTER JOIN [dbo].[Country] CoShipment ON CoShipment.id = [City_1].id_country
  -- fin jinm
  LEFT OUTER JOIN [dbo].[Port] [Port_2] ON [Port_2].[id] = [salesQuotationExterior].[id_portDestination]      
  LEFT OUTER JOIN [dbo].[EmissionPoint] [emissionPoint] ON [emissionPoint].[id] = [Document].[id_emissionPoint]      
  LEFT OUTER JOIN [dbo].[Company] [Company] ON [Company].[id] = [emissionPoint].[id_company]   	      
  LEFT OUTER JOIN [dbo].[City] [City_2] ON [City_2].[id] = [Port_2].[id_city]  
  LEFT OUTER JOIN [dbo].[Country] [coun2] ON [coun2].[id] = [City_2].[id_country]      
  LEFT OUTER JOIN [dbo].[StateOfContry] soC
   ON soC.id = [City_2].id_stateOfContry
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
  LEFT OUTER JOIN [dbo].[User] Us ON Us.id = [doc].id_userUpdate
  LEFT OUTER JOIN PaymentMethod PAYM ON PAYM.ID = SalesQuotationExterior.id_PaymentMethod
  LEFT OUTER JOIN [dbo].[CapacityContainer] ca ON ca.id = [salesQuotationExterior].id_capacityContainer
  LEFT OUTER JOIN [dbo].[MetricUnit] Umc ON Umc.id = [ca].id_metricUnit
  left outer join person CONTACTOCONS ON CONTACTOCONS.id = [salesQuotationExterior].id_personcontactConsignatario
  left outer join ForeignCustomer phfc on phfc.id = personc.id
  left outer join ForeignCustomer phfc2 on phfc2.id = CONTACTOCONS.id 
  -- jinm
  INNER JOIN IdentificationType ideT ON ideT.id = PersON.id_identificationType
  LEFT OUTER JOIN ItemGroupCategory iteGC ON iteGC.id = [itemGeneral].id_groupCategory
  LEFT OUTER JOIN (SELECT itee.id, iteSe.name FROM item itee
				   INNER JOIN itemGeneral iteGe ON iteGe.id_item = itee.id
				   INNER JOIN ItemSize iteSE ON iteSE.id = iteGe.id_size) AS iteV ON iteV.id  = InvoiceDetail.id_itemMarked
  -- fin jinm
  LEFT join foreigncustomeridentification fcaddress on fcaddress.id_ForeignCustomer = salesQuotationExterior.id_consignee and fcaddress.id = salesQuotationExterior.id_addressCustomer
 
  WHERE [Invoice].[id] = CONVERT(INT,@id)
  ORDER BY Item.mASterCode
  
/* 
	EXEC par_ProformasReportContract 4019
	--exec spPar_ProformasReportContract @id=985681
*/
GO