
GO
/****** Object:  StoredProcedure [dbo].[par_InvoiceExteriorPropioCR]    Script Date: 24/02/2023 12:30:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  procedure  [dbo].[spPar_FactPesos]
@id INT      
    AS      
 SET nocount ON    
 
 --CREATE TABLE #DatosProducto(
	--id INT,
	--id_Invoice INT,
	--cASepack VARCHAR(50),
 --)

 Declare @Total INT
 Declare @Cents INT
 DECLARE @TEMP TABLE(resultado VARCHAR(MAX))
 DECLARE @TEMPCents TABLE(resultado VARCHAR(MAX))
 DECLARE @NetoLibras Decimal(15,6)
 DECLARE @NetoKilos Decimal(15,6)
 DECLARE @BrutoLibras Decimal(15,6)
 DECLARE @BrutoKilos Decimal(15,6)
 DECLARE @Proforma varchar(5)
 DECLARE @TotalFactProf Decimal(15,6)
 DECLARE @CentFactProf Decimal(15,6)
 Declare @Formato Varchar(2)
  Declare @GlaseoKilos Decimal(15,6)
 Declare @GlaseoLibras Decimal(15,6)
 Set @Formato = 'SI'
 select @Formato = value from Setting where code = 'FORMATOTEL'

 Select @proforma = id_documentOrigen from Document where id = @id
 --If @proforma <> ''
 --Begin

	--SELECT @TotalFactProf = Sum(Prof.id_amountInvoice * Prof.unitPrice)
	--FROM [InvoiceDetail] Fact
	--Inner Join (Select * from [InvoiceDetail] where id_Invoice = @proforma and isActive = 1) prof
	--   On prof.id_item = Fact.id_item
	--WHERE Fact.id_invoice = @id and Fact.isActive= 1
	

	--SELECT @CentFactProf = Round(Sum(Prof.id_amountInvoice * Prof.unitPrice),2)
	--FROM [InvoiceDetail] Fact
	--Inner Join (Select * from [InvoiceDetail] where id_Invoice = @proforma and isActive = 1) prof
	--   On prof.id_item = Fact.id_item
	--WHERE Fact.id_invoice = @id and Fact.isActive= 1

	--Set @Total = @TotalFactProf
	--Set @Cents = (@CentFactProf - @Total) * 100

 --End
 --Else
 --Begin
	--SET @Total = (SELECT sum([total]) FROM [InvoiceDetail] WHERE id_invoice=@id and isActive= 1)
	--SET @Cents = Round(((SELECT sum([total]) FROM [InvoiceDetail] WHERE id_invoice=@id and isActive= 1) - round((SELECT sum([total]) FROM [InvoiceDetail] WHERE id_invoice=@id and isActive= 1),0,1))*100,1)

	SET @Total = (SELECT valuetotalCIF FROM [InvoiceExterior] WHERE id=@id)
	SET @Cents = Round(((SELECT valuetotalCIF FROM [InvoiceExterior] WHERE id=@id) - round((SELECT valuetotalCIF FROM [InvoiceExterior] WHERE id=@id),0,1))*100,1)

 --End

 INSERT INTO @TEMP EXEC dbo.NumerosEnLetrASIngles @Total
 INSERT INTO @TEMPCents EXEC dbo.NumerosEnLetrASIngles @Cents


 --INSERT INTO #DatosProducto SELECT id_item,id_Invoice, RIGHT(descriptiON,13)AS cASepack  FROM [InvoiceDetail] idt   
 --UPDATE #DatosProducto SET cASepack=REPLACE(cASepack,'(','           ')
 --UPDATE #DatosProducto SET cASepack=REPLACE(cASepack,')','')
 --UPDATE #DatosProducto SET cASepack= RIGHT(cASepack,13)

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
		---- Cabecera 
		[Company].[trademark] AS NombreCia,
		[Company].[businessName] AS NombreCiaLargo,
		[Company].[phONeNumber] AS TelefONoCia,
		[Company].[email] AS email,
		[Company].[PlantCode] AS PlantCode, 
		[Company].[registryFDA] AS FDA,
		[Company].[websiteCompany] AS Web,
		[BranchOffice].[address] AS DirSucural,   
		[Company].[ruc] AS RucCia,
		[Company].[logo] AS Logo,
		ISNULL([Company].plantCode,'') AS CodigoPlanta,
		ISNULL([Company].registryFDA,'') AS FDA,
		---- Invoice
		[Document].[number] AS Factura,
		[Document].[emissiONDate] AS FechaEmisiON,
		[Document].[description] As Observacion,
		[TerminONegociación] = (SELECT top 1 [Code] FROM TermsNegotiatiON Tn WHERE Tn.[id]=[InvoiceExterior].[id_termsNegotiatiON]),------------------------------
		convert(varchar,[InvoiceExterior].[purchASeORDER]) AS OrdenPedido,
		---- StakeHolder Sold to 
		[RazONSocialSoldTo] = (SELECT top 1 [foreignCustomer].[fullname_businessName] FROM Person [foreignCustomer] inner join [ForeignCustomerIdentification] fci on fci.id_ForeignCustomer = [foreignCustomer].id WHERE [foreignCustomer].[id] = invoice.id_buyer),


		--[RazONSocialSoldTo]= (SELECT [Name] FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoice].[id_buyer]),
		
		-- select *from foreigncustomeridentification
--		select *from invoice
		[Fax1]= fci.fax1fc,
		--[Fax1]= (SELECT f.fax1FC from  foreigncustomeridentification f where f.id_ForeignCustomer = [invoice].[id_buyer] ),

		[USCISoldTo] = (SELECT top 1 ITP.code + ' - ' + FCI.numberIdentification FROM ForeignCustomer [foreignCustomer] 
						INNER JOIN ForeignCustomerIdentificatiON FCI ON FCI.id_ForeignCustomer = [foreignCustomer].[id]
						INNER JOIN Country_IdentificationType CIT ON CIT.id = FCI.id_Country_IdentificationType
						INNER JOIN IdentificationType ITP ON ITP.id = CIT.id_identificationType 
						WHERE fci.[id_ForeignCustomer] = [invoice].[id_buyer]),


						--select *from foreigncustomeridentification where id_foreigncustomer = 347320

		  --[AddressSoldTo1]= UPPER ((SELECT ADDRESSFOREIGN FROM  foreigncustomeridentification WHERE ID_FOREIGNCUSTOMER = [invoice].[id_buyer])),
		  --[AddressSoldTo2]= UPPER ((SELECT ADDRESSFOREIGN FROM  foreigncustomeridentification WHERE ID_FOREIGNCUSTOMER = [invoice].[id_buyer])),
		[AddressSoldTo1] = fci.addressForeign,
		[AddressSoldTo2] = fci.addressForeign,
		
		--[CountrySoldTo] =UPPER ((SELECT C.NAME FROM COUNTRY C INNER JOIN foreigncustomeridentification F ON F.ID_COUNTRY = C.ID WHERE F.ID_FOREIGNCUSTOMER = [invoice].[id_buyer])),
		[CitySoldTo] = UPPER ((SELECT top 1 C.NAME FROM CITY C INNER JOIN foreigncustomeridentification F ON F.ID_CITY = C.ID WHERE F.ID_FOREIGNCUSTOMER = [invoice].[id_buyer])),-------------------------------
		 [CountrySoldTo] = UPPER((SELECT top 1 [c].[name] FROM Country c INNER JOIN ForeignCustomerIdentification [fc] ON [fc].[id_country]= [c].[id] WHERE [fc].[id_ForeignCustomer] = [invoice].[id_buyer])) ,
			--[CountrySoldTo] = counfc.name,
		
		--[CitySoldTo] = UPPER((SELECT top 1 [c].[name] FROM City c 
		--			   INNER JOIN ForeignCustomerIdentification [fc] 
		--			   ON [fc].[id_city]= [c].[id] WHERE [fc].[id_ForeignCustomer]=[invoice].[id_buyer] and [fc].[id] = [InvoiceExterior].[id_addressCustomer])),
		--[CitySoldTo] = cityfc.name,

		[TelefONo1SoldTo] =(select top 1 a.phone1fc from ForeignCustomerIdentification a where a.id_ForeignCustomer = invoice.id_buyer), 
		
		[TelefONo2SoldTo] = (SELECT top 1 case when @Formato = 'SI' Then FORMAT(CONVERT(float,fax1FC),'(000)-###-###-####') else fax1FC end FROM ForeignCustomerIdentification [fc] WHERE [fc].[id_ForeignCustomer] = [invoice].[id_buyer] and [fc].[id] = [InvoiceExterior].[id_addressCustomer]),
		
		[EmailSoldTo] = (SELECT top 1 email FROM PersON [persON]WHERE [persON].[id] = [salesQuotationExterior].[id_personContact]),---------------------

		---- StakeHolder Ship to
		--[RazONSocialShipTo] = (SELECT [Name] FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoiceExterior].[id_cONsignee]),
		[RazONSocialShipTo] = (SELECT top 1 fullname_businessName FROM PersON [persON]  inner join ForeignCustomerIdentification f on f.id_ForeignCustomer = persON.id WHERE f.id_ForeignCustomer = InvoiceExterior.[id_cONsignee]),
		[USCIShipTo] = (SELECT top 1 ITP.code + ' - ' + FCI.numberIdentification FROM ForeignCustomer [foreignCustomer] 
						INNER JOIN ForeignCustomerIdentificatiON FCI ON FCI.id_ForeignCustomer = [foreignCustomer].[id]
						INNER JOIN Country_IdentificationType CIT ON CIT.id = FCI.id_Country_IdentificationType
						INNER JOIN IdentificationType ITP ON ITP.id = CIT.id_identificationType 
						WHERE [foreignCustomer].[id] = [invoiceExterior].[id_cONsignee]),
		
		[AddressShipTo1]= fci.addressForeign,
		--[AddressShipTo1] = upper((select top 1 p.addressforeign from ForeignCustomerIdentification p where p.id_ForeignCustomer = InvoiceExterior.id_consignee )),
		--[AddressShipTo1] = UPPER((SELECT address FROM PersON [persON] WHERE [persON].[id] = [invoiceExterior].[id_cONsignee])),
		[AddressShipTo2] = UPPER((SELECT top 1 address FROM PersON [persON] WHERE [persON].[id] = [invoiceExterior].[id_cONsignee])),
		
		--[CountryShipTo] = counfc2.name,
		--[CityShipTo] = cityfc2.name,
		[CountryShipTo] = upper((SELECT top 1 [c].[name] FROM Country c INNER JOIN ForeignCustomerIdentification fc ON [fc].[id_country]= [c].[id] WHERE [fc].[id_ForeignCustomer] = [InvoiceExterior].[id_cONsignee])),------------------
		[CityShipTo] = upper((SELECT top 1 [c].[name] FROM City c 
						INNER JOIN ForeignCustomerIdentification [fc] 
						ON [fc].[id_city]= [c].[id] WHERE [fc].[id_ForeignCustomer]=[InvoiceExterior].[id_cONsignee])),----------------------------------

						PAYM.NAME AS METODOPAGO,
		
		[TelefONo1ShipTo] = (select top 1 phone1FC from ForeignCustomerIdentification where id_ForeignCustomer = InvoiceExterior.id_consignee  ),
		
		[TelefONo2ShipTo] = (select top 1 fax1FC from ForeignCustomerIdentification where id_ForeignCustomer = InvoiceExterior.id_consignee),
		
		
		[EmailShipTo] = (SELECT email FROM PersON [persON]WHERE [persON].[id] = [salesQuotationExterior].[id_personContact]),
		[PersonC].fullname_businessName AS Contacto,
		---- Datos del Producto
		[Product]= ' ',
			--Ini Pais de Origen 
		[CountryOfOrigin] = (SELECT [country].[name] FROM [Port] [port]
								 INNER JOIN [city] [city]
							 ON [city].[id] = [port].[id_city]
								 INNER JOIN [Country] [country]
							 ON [country].[id] = [city].[id_country]
							 WHERE [port].[id]=[InvoiceExterior].[id_portShipment]),
			--Fin Pais de Origen 
		[PuertoDestino] = [Port_2].[nombre] +', '+ UPPER ([coun2].[Name2]), 
		[PuertoDeEmbarque1] =UPPER([City_1].[name]) +', '+UPPER(coun3.name) , 
		[PuertoDeEmbaruqe] = UPPER([City_1].[name]) , 
		[Company].[trademark] AS Shipper,
		
		[FechaEmbarque] = [mesFechaEmbarque].[mesIngles] + ' '+convert(char(2),DAY([InvoiceExterior].[dateShipment]))+', '+convert(char(4),YEAR([InvoiceExterior].[dateShipment]) ),
			---- Buque
			ISNULL([InvoiceExterior].[shipName],'') AS Buque,
			ISNULL([InvoiceExterior].[shipNumberTrip] ,'') AS VIAJE,
			---- /Buque
		[ShippingAgency].[name] AS Naviera,
		[InvoiceExterior].[blNumber] AS BL,
		CASE WHEN [salesQuotationExterior] .[purchASeORDER]= '' THEN 'N/A' ELSE [salesQuotationExterior].[purchASeORDER] END  AS PurchASeORDER,---------------------------------
		---- Detalles
		[InvoiceDetail].[numBoxes] AS CartONes ,
		[InvProf].[id_amountInvoice] AS Cantidad,
		[InvoiceDetail].[unitPrice] AS Precio,
		[InvProf].[unitPrice] AS PrecioProforma,
		invoicedetail.amount * invoicedetail.unitPriceProforma as TotalPriceWithoutTax,
		[itemSize].[name] AS size,
		--[DatosProducto].[cASepack] AS cASepack,
		[itemTrademark].[name] AS Marca,
		[Item].[foreignName] AS descripciON,
		CASE WHEN ISNULL([DocProf].[number], '') <> ''
			 THEN ISNULL([InvoiceDetail].descriptionCustomer, '') 
			 ELSE ' Frozen Head Less Shell On Raw Vannamei Shrimp' END AS [descripcionCliente],
		---- Total en Letra
		Total = (SELECT TOP 1 [resultado] FROM @TEMP) + ' Dollars and ' + (SELECT TOP 1 [resultado] FROM @TEMPCents) + ' Cents' ,
		--[Presentation].[name] + ' = ' + REPLACE(RTRIM(LTRIM(REPLACE(Convert(Varchar,Round(([Presentation].[minimum] * [Presentation].[maximum]),2),100),'0',' '))),' ','0') AS Casepack ,
		[ItemTrademarkModelPf].[name] + '(' + Convert(Varchar,[PresentationPf].[maximum]) + ' x ' + Convert(Varchar,Convert(decimal(10,2),Round(([InvProf].proformaWeight / [PresentationPf].[maximum]),3))) + ' ' + [MetricUnitPf].code  + ')' + ' = ' +  Convert(Varchar,Convert(decimal(10,2),Round([PresentationPf].[maximum] * ([InvProf].proformaWeight / [PresentationPf].[maximum]),3))) AS Casepack,
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
		Convert(Varchar,UPPER([InvoiceExterior].[numeroContenedores]),10) + ' CONTAINER ' + UPPER([CapacityCONtainer].[name]) + ' FEET' AS NumeroContenedores,
		ISNULL([InvoiceExterior].[containers],'') AS Contenedores,
		ISNULL([mesFechaProforma].[mesIngles] + ' '+convert(char(2),DAY([DocProf].[emissionDate]))+', '+convert(char(4),YEAR([DocProf].[emissionDate]) ),'')  As FechaProforma,
		ISNULL([DocProf].[number], '') AS Proforma,
		[InvoiceExterior].[daeNumber]  + '-'  + [InvoiceExterior].[daeNumber2] + '-'  + [InvoiceExterior].[daeNumber3] + '-'  + [InvoiceExterior].[daeNumber4]  AS Dae,
		[PaymentTerm].[descriptionEnglish] AS PlazoPago,
		[mesDae].[mesIngles] + ' '+convert(char(2),DAY([InvoiceExterior].[daeDate]))+', '+convert(char(4),YEAR([InvoiceExterior].[daeDate]) )  As FechaDae,
		[mesFechaCarga].[mesIngles] + ' '+convert(char(2),DAY([InvoiceExterior].[loadingDate]))+', '+convert(char(4),YEAR([InvoiceExterior].[loadingDate]) )  As FechaCarga,
		@NetoKilos AS NetoKilos,
		@NetoLibras AS NetoLibras,
		@BrutoKilos AS BrutoKilos,
		@BrutoLibras AS BrutoLibras,
		@GlaseoKilos As GlaseoKilos,
		@GlaseoLibras As GlaseoLibras,
		[InvoiceExterior].valueSubscribed AS ValorAbonado,
		[InvoiceExterior].balance AS Saldo,
		presentation.maximum as maximum,
		presentation.minimum as minimum,
		netweight2 = round(presentation.maximum,2) * round(Presentation.minimum,2)*[InvoiceDetail].[numBoxes],
		InvoiceExterior.seals as SELLO,
		CONTACTOCONS.fullname_businessName as CONTACTO_2,
		--[RazONSocialShipToid] = (SELECT person.id FROM PersON [persON]  inner join ForeignCustomerIdentification f on f.id_ForeignCustomer = persON.id WHERE f.id_ForeignCustomer = salesQuotationExterior.[id_cONsignee]),
		--pbuyerid =convert(varchar,(select  fc.id from ForeignCustomerIdentification fci inner join person fc on fc.id = fci.id_ForeignCustomer  where  fci.id_foreigncustomer =salesQuotationExterior.id_consignee ),(250)),
		pbuyer=(SELECT top 1 fullname_businessName FROM PersON [persON]  inner join ForeignCustomerIdentification f on f.id_ForeignCustomer = persON.id WHERE f.id_ForeignCustomer = salesQuotationExterior.[id_cONsignee]),
		--pbuyer =(select convert (varchar, fc.fullname_businessName,(250)) from ForeignCustomerIdentification fci inner join person fc on fc.id = fci.id_ForeignCustomer  where  fci.id_foreigncustomer =salesQuotationExterior.id_consignee ),
		--pbuyer.fullname_businessName as pbuyer,
		pbdireccion = fci.addressForeign,

		--upper((SELECT f.addressForeign FROM PersON [persON]  inner join ForeignCustomerIdentification f on f.id_ForeignCustomer = persON.id WHERE f.id_ForeignCustomer = salesQuotationExterior.[id_cONsignee])) as pbdireccion,
		--upper((select  fci.addressForeign from ForeignCustomerIdentification fci inner join ForeignCustomer fc on fc.id = fci.id_ForeignCustomer  where  fci.id_foreigncustomer  = InvoiceExterior.id_notifier)) as pbdireccion,
		--upper((select p.address from person p inner join foreigncustomer f on p.id = f.id  where f.id = InvoiceExterior.id_notifier)) as pbdireccion,
		pbuyer.identification_number as pbruc,
		pbuyercontact.fullname_businessName as contactopb,
		pbuyer.cellPhoneNumberPerson as numeropb,
		invoiceexterior.transport as freight,
		InvoiceExterior.PO as PO
		,invoicedetail.totalPriceWithoutTax as VFOB
		,InvoiceExterior.valueInternationalInsurance as TSeguro
		,InvoiceExterior.valueInternationalFreight as TFlete
		,InvoiceExterior.valueTotalFOB as TFOB
		,InvoiceExterior.valuetotalCIF as Tvalortotal
		,KGBRUTOSD = (itw.itemWeightGrossWeight*conversionToKilos*InvoiceDetail.numBoxes) 
		,kgnetos = (itw.itemWeightNetWeight*conversionToKilos*InvoiceDetail.numBoxes)
		,kgglaseoneto=(itw.weightWithGlaze*conversionToKilos*InvoiceDetail.numBoxes)
		,Certificacion = Case when cer.code = 'ASC' then 'ASC' else 'Conventional' end
		,itd.name as nproducto
		,Document.accessKey as AccessKey,
		-- Nuevos campos requeridos, JINM 09-02-2023
		ISNULL([invoiceExterior].[etaDate], '') AS [fechaETA],
		ISNULL([invoiceExterior].[seals], '') AS [sellos],
		ISNULL([invoiceExterior].[containers], '') AS [contenedores],
		ISNULL([invoiceExterior].[noContrato], '') AS [noContrato],
		ISNULL([invoiceExterior].[valueSubscribed], 0) AS [valorAbonado]
		FROM (((((((((((((((((((((((((([dbo].[Invoice] [Invoice]      
  INNER JOIN [dbo].[Document] doc ON [Invoice].[id] = [doc].[id]      
  INNER JOIN [dbo].[DocumentState] docs ON doc.[id_documentState] = docs.[id]      
  LEFT OUTER JOIN [dbo].[InvoiceDetail] [InvoiceDetail]      
       ON ([InvoiceDetail].[id_invoice] = [Invoice].[id])      
    and [InvoiceDetail].[isActive] = 1)    
  INNER JOIN [dbo].[InvoiceExterior] [InvoiceExterior]      
       ON ([InvoiceExterior].[id] = [Invoice].[id]))      
  INNER JOIN [dbo].[Item] [item]      
       ON ([Item].[id] = [InvoiceDetail].[id_item]))      
  INNER JOIN [dbo].[MetricUnit] [MetricUnit]      
       ON ([MetricUnit].[id] = [InvoiceDetail].[id_metricUnitInvoiceDetail]))   
	   left join person pbuyer on pbuyer.id = invoice.id_buyer
  LEFT OUTER JOIN [dbo].[PersON] [PersON]      
       ON ([PersON].[id]= [InvoiceExterior].[id_cONsignee]))      
  LEFT OUTER JOIN [dbo].[ForeignCustomer] [ForCustomer]      
       ON ([ForCustomer].[id] = [InvoiceExterior].[id_cONsignee]))      
  INNER JOIN [dbo].[Document] [Document]      
       ON ([Document].[id] = [Invoice].[id]))      
  LEFT OUTER JOIN [dbo].[Port] [Port]      
       ON ([Port].[id] = [InvoiceExterior].[id_portDischarge]))      
  LEFT OUTER JOIN [dbo].[City] [City]      
       ON ([Port].[id_city] = [City].[id]))      
   LEFT OUTER JOIN [dbo].[Country] coun      
    ON coun.[id] = [City].[id_country]         
  LEFT OUTER JOIN [dbo].[Port] [Port_1]      
       ON ([Port_1].[id] = [InvoiceExterior].[id_portShipment]))      
  LEFT OUTER JOIN [dbo].[City] [City_1]      
       ON ([Port_1].[id_city] = [City_1].[id]))      
  LEFT OUTER JOIN [dbo].[Port] [Port_2]      
       ON ([Port_2].[id] = [InvoiceExterior].[id_portDestinatiON]))      
  LEFT OUTER JOIN [dbo].[EmissiONPoINT] [EmissiONPoINT]      
       ON ([EmissiONPoINT].[id] = [Document].[id_emissiONPoINT]))      
  LEFT OUTER JOIN [dbo].[Company] [Company]      
       ON ([Company].[id] = [EmissiONPoINT].[id_company]))      
  LEFT OUTER JOIN [dbo].[City] [City_2]      
       ON ([City_2].[id] = [Port_2].[id_city]))      
  LEFT OUTER JOIN [dbo].[Country] coun2      
    ON coun2.[id] = [City_2].[id_country]      
  LEFT OUTER JOIN [dbo].[Country] [Country_DestinatiON]      
       ON ([Country_DestinatiON].[id] = [City_2].[id_country]))           
  LEFT OUTER JOIN [dbo].[ShippingLine]      
       [ShippingLine]      
       ON ([ShippingLine].[id] = [InvoiceExterior].[id_ShippingLine]))            
  LEFT OUTER JOIN [dbo].[ShippingAgency]      
       [ShippingAgency]      
       ON ([ShippingAgency].[id] = [InvoiceExterior].[id_shippingAgency]))      
  LEFT OUTER JOIN [dbo].[TariffHeading] [TariffHeading]      
       ON ([TariffHeading].[id] = [InvoiceExterior].[id_tariffHeading]))      
  LEFT OUTER JOIN [dbo].[CapacityCONtainer]  [CapacityCONtainer]      
       ON ([CapacityCONtainer].[id] = [InvoiceExterior].[id_capacityCONtainer]))      
  LEFT OUTER JOIN [dbo].[BranchOffice] [BranchOffice]      
       ON ([BranchOffice].[id] = [EmissiONPoINT].[id_branchOffice]))      
  LEFT OUTER JOIN [dbo].[DivisiON] [DivisiON]      
       ON ([DivisiON].[id] = [EmissiONPoINT].[id_divisiON]))       
  LEFT OUTER JOIN [dbo].[termsNegotiatiON] [termsNegotiatiON]      
       ON ([InvoiceExterior].[id_termsNegotiatiON] = [termsNegotiatiON].[id]))      
  INNER JOIN [dbo].[CompanyElectrONicFacturatiON] [companyElectrONicFacturatiON]      
		ON ([Company].[id] = [companyElectrONicFacturatiON].id_company))      
  LEFT OUTER JOIN [dbo].[EnvirONmentType] [envirONmentType]      
		  ON ([envirONmentType].[id] = [companyElectrONicFacturatiON].[id_enviromentType]))      
  LEFT OUTER JOIN [dbo].[EmissiONType] [emissiONType]      
	    ON ([emissiONType].[id] = [companyElectrONicFacturatiON].[id_emissiONType]))  
  --INNER JOIN [dbo].[#DatosProducto] [DatosProducto]
		--ON  [DatosProducto].[id] = [InvoiceDetail].[id_item]
	 --  and [DatosProducto].[id_Invoice] = [InvoiceDetail].[id_Invoice]
  INNER JOIN [ItemGeneral] [itemGeneral]
		ON [invoiceDetail].[Id_item] = [itemGeneral].[Id_item]
  INNER JOIN [ItemSize] [itemSize]
		ON [itemSize].[id] = [itemGeneral].[id_size]
  INNER JOIN [ItemTrademark] [itemTrademark]
		ON [itemTrademark].[id] = [itemGeneral].[id_Trademark]
  LEFT OUTER JOIN (select ctd.code, ctd.description mesEspanol, ctd.fldvarchar1 mesIngles
				from tbsysCatalogueDetail ctd 
				inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue
				where ct.code ='MONTHT') mesDae 
		ON mesDae.code =  cast(month([InvoiceExterior].[daeDate]) as varchar)
  LEFT OUTER JOIN (select ctd.code, ctd.description mesEspanol, ctd.fldvarchar1 mesIngles
				from tbsysCatalogueDetail ctd 
				inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue
				where ct.code ='MONTHT') mesFechaCarga
		ON mesFechaCarga.code =  cast(month([InvoiceExterior].[loadingDate]) as varchar)
  LEFT OUTER JOIN (select ctd.code, ctd.description mesEspanol, ctd.fldvarchar1 mesIngles
				from tbsysCatalogueDetail ctd 
				inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue
				where ct.code ='MONTHT') mesFechaEmbarque
		ON mesFechaEmbarque.code =  cast(month([InvoiceExterior].[dateShipment]) as varchar)
  LEFT OUTER JOIN [dbo].[Presentation] [Presentation]
        ON [Presentation].[id] = [item].[id_presentation] 
  LEFT OUTER JOIN [dbo].[BoxCardAndBank] [BoxCardAndBank]
        ON [BoxCardAndBank].[id] = [InvoiceExterior] .[id_bank] 
  LEFT OUTER JOIN [dbo].[Document] [DocProf]
        ON [DocProf].[id] = [doc].[id_documentOrigen]
  LEFT OUTER JOIN [dbo].[SalesQuotationExterior][salesQuotationExterior] ON [salesQuotationExterior].[id] = [DocProf].[id]
  LEFT OUTER JOIN [dbo].[InvoiceDetail] [InvProf] ON [InvProf].[id_invoice] = [DocProf].[id]
	   AND [InvProf].[id_item] =  [InvoiceDetail].id_item
	   AND [InvProf].[isActive] = 1
  LEFT OUTER JOIN [dbo].[Person] [PersonC] ON [PersonC].[id]= [salesQuotationExterior].[id_personContact]   	     
  LEFT OUTER JOIN [dbo].[Item] [itemPf] ON [itemPf].[id] = [InvProf].[id_item]
  LEFT OUTER JOIN [dbo].[ItemGeneral] [itemGeneralPf] ON [InvProf].[id_item] = [itemGeneralPf].[id_item]
  LEFT OUTER JOIN [dbo].[Presentation] [PresentationPf] ON [PresentationPf].[id] = [itemPf].[id_presentation] 
  LEFT OUTER JOIN [dbo].[ItemTrademarkModel] [ItemTrademarkModelPf] ON [ItemTrademarkModelPf].[id] = [itemGeneralPf].[id_trademarkModel] 
  LEFT OUTER JOIN [dbo].[MetricUnit] [MetricUnitPf] ON [MetricUnitPf].[id] = [InvProf].[id_metricUnitInvoiceDetail]  
  LEFT OUTER JOIN (select ctd.code, ctd.description mesEspanol, ctd.fldvarchar1 mesIngles
				from tbsysCatalogueDetail ctd 
				inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue
				where ct.code ='MONTHT') mesFechaProforma
		ON mesFechaProforma.code =  cast(month([DocProf].[emissionDate]) as varchar)
  LEFT OUTER JOIN [dbo].[PaymentTerm] [PaymentTerm]
        ON [PaymentTerm].[id] = [InvoiceExterior].[id_PaymentTerm]
left outer join person CONTACTOCONS ON CONTACTOCONS.id = [salesQuotationExterior].id_personcontactConsignatario
left join person pbuyercontact on pbuyercontact.id = salesQuotationExterior.id_personContact
left join country coun3 on coun3.id = [City_1].id_country
left join ForeignCustomerIdentification fci on fci.id = salesQuotationExterior.id_addressCustomer
LEFT OUTER JOIN PaymentMethod PAYM ON PAYM.ID = [InvoiceExterior].id_PaymentMethod
 left outer join item itd on itd.id = InvoiceDetail.id_item
left join [ItemWeightConversionFreezen] itw on itw.id_item = InvoiceDetail.id_item
LEFT OUTER JOIN certification cer ON cer.id = [itemGeneral].id_certification       
left join country counfc on counfc.id = fci.id_country
left join city cityfc on cityfc.id = fci.id_city
left join ForeignCustomerIdentification fci2 on fci2.id = salesQuotationExterior.id_addressCustomer
left join country counfc2 on counfc2.id = fci2.id_country
left join city cityfc2 on cityfc2.id = fci2.id_city
WHERE [Invoice].[id] = CONVERT(INT,@id)   
ORDER BY Item.mASterCode  

/*
	EXEC par_InvoiceExteriorPropioCR 3872
	EXEC par_InvoiceExteriorPropioCR 1116
	EXEC par_InvoiceExteriorPropioCR 3909
*/