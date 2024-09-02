
GO
/****** Object:  StoredProcedure [dbo].[par_InvoiceExteriorPakingListCR]    Script Date: 29/12/2022 01:56:55 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
    
CREATE procedure  [dbo].[spPar_FacturaElectronicaPackinList] --6559  

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
 Declare @GlaseoKilos Decimal(15,6)
 Declare @GlaseoLibras Decimal(15,6)

 SET @Total = (SELECT sum([total]) FROM [InvoiceDetail] WHERE id_invoice=@id)
 SET @Cents = ((SELECT sum([total]) FROM [InvoiceDetail] WHERE id_invoice=@id) - round((SELECT sum([total]) FROM [InvoiceDetail] WHERE id_invoice=@id),0,1))*100

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
		[Company].[phONeNumber] AS TelefONoCia,
		[Company].[email] AS email,
		[Company].[PlantCode] AS PlantCode, 
		[Company].[registryFDA] AS FDA,
		[Company].[websiteCompany] AS Web,
		[BranchOffice].[address] AS DirSucural,   
		[Company].[ruc] AS RucCia,
		[Company].[logo] AS Logo,
		---- Invoice
		[Document].[number] AS Factura,
		[Document].[emissiONDate] AS FechaEmisiON,
		[TerminONegociacion] = (SELECT [Code] FROM TermsNegotiatiON Tn WHERE Tn.[id]=[InvoiceExterior].[id_termsNegotiatiON]),
		[InvoiceExterior].[purchASeORDER] AS OrdenPedido,
		---- StakeHolder Sold to 
		[RazONSocialSoldTo]= (SELECT [Name] FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoiceExterior].[id_cONsignee]),
		[USCISoldTo] = (SELECT numberIdentification FROM ForeignCustomerIdentification [FCI] WHERE [FCI].[id_ForeignCustomer] = [invoiceExterior].[id_consignee] and Fci.id = InvoiceExterior.id_addressCustomer),
		[AddressSoldTo1] = (SELECT fci.addressForeign FROM ForeignCustomerIdentification fci WHERE fci.[id_ForeignCustomer] = [invoiceExterior].[id_cONsignee] and Fci.id = InvoiceExterior.id_addressCustomer ),
		[AddressSoldTo2] = (SELECT fci.addressForeign FROM ForeignCustomerIdentification fci WHERE fci.[id_ForeignCustomer] = [invoiceExterior].[id_cONsignee] and Fci.id = InvoiceExterior.id_addressCustomer),
		[CountrySoldTo] =  (SELECT [c].[name] FROM Country c INNER JOIN ForeignCustomerIdentification [fc] ON [fc].[id_country]= [c].[id] WHERE [fc].[id_ForeignCustomer] = [invoiceExterior].[id_cONsignee] and Fc.id = InvoiceExterior.id_addressCustomer),
		[CitySoldTo] = (SELECT [c].[name] FROM City c 
					   INNER JOIN ForeignCustomerIdentification [fc] 
					   ON [fc].[id_city]= [c].[id] WHERE [fc].[id_ForeignCustomer]=[invoiceExterior].[id_cONsignee] and Fc.id = InvoiceExterior.id_addressCustomer),
		[TelefONo1SoldTo] = (SELECT  [phONe1FC] FROM ForeignCustomerIdentification [foreignCustomer] WHERE [foreignCustomer].[id_ForeignCustomer] = [invoiceExterior].[id_cONsignee] and [foreignCustomer].id = InvoiceExterior.id_addressCustomer),
		[TelefONo2SoldTo] = (SELECT  [fax1FC] FROM ForeignCustomerIdentification [foreignCustomer] WHERE [foreignCustomer].[id_ForeignCustomer] = [invoiceExterior].[id_cONsignee] and [foreignCustomer].id = InvoiceExterior.id_addressCustomer),
		---- StakeHolder Ship to
		[RazONSocialShipTo] = (SELECT [Name] FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoiceExterior].[id_notifier]),
		[USCIShipTo] = (SELECT numberIdentification FROM ForeignCustomerIdentification [FCI] WHERE [FCI].[id_ForeignCustomer] = [invoiceExterior].[id_notifier] and fci.id = InvoiceExterior.id_addressCustomer),
		[AddressShipTo1] = (SELECT  address FROM PersON [persON] WHERE [persON].[id] = [invoiceExterior].[id_notifier]),
		[AddressShipTo2] = (SELECT address FROM PersON [persON] WHERE [persON].[id] = [invoiceExterior].[id_notifier]),
		[CountryShipTo] = (SELECT [c].[name] FROM Country c INNER JOIN ForeignCustomerIdentification fc ON [fc].[id_country]= [c].[id] WHERE [fc].[id_ForeignCustomer] = [invoiceExterior].[id_notifier] and fc.id = InvoiceExterior.id_addressCustomer),
		[CityShipTo] = (SELECT [c].[name] FROM City c 
						INNER JOIN ForeignCustomerIdentification [fc] 
						ON [fc].[id_city]= [c].[id] WHERE [fc].[id_ForeignCustomer]=[invoiceExterior].[id_notifier] and fc.id = InvoiceExterior.id_addressCustomer),
		[TelefONo1ShipTo] = (SELECT [phONe1FC] FROM ForeignCustomerIdentification [foreignCustomer] WHERE [foreignCustomer].[id_ForeignCustomer] = [invoiceExterior].[id_notifier] and ForeignCustomer.id = InvoiceExterior.id_addressCustomer),
		[TelefONo2ShipTo] = (SELECT  [fax1FC] FROM ForeignCustomerIdentification [foreignCustomer] WHERE [foreignCustomer].[id_ForeignCustomer] = [invoiceExterior].[id_notifier] and ForeignCustomer.id = InvoiceExterior.id_addressCustomer),
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
		[PuertoDeEmbaruqe] = [Port_1].[nombre] +', '+ UPPER([City_1].[name]) , 
		[Company].[trademark] AS Shipper,
		
		--[FechaEmbarque] =[InvoiceExterior].[dateShipment],
			---- Buque
			ISNULL([InvoiceExterior].[shipName],'') AS Buque,
			ISNULL([InvoiceExterior].[shipNumberTrip] ,'') AS VIAJE,
			---- /Buque
		[ShippingAgency].[name] AS Naviera,
		[InvoiceExterior].[blNumber] AS BL,
		[InvoiceExterior].[purchASeORDER] AS PurchASeORDER,
		convert(varchar,[InvoiceExterior].[dateShipment],105) AS FechaEmbarque,
		---- Detalles
		[InvoiceDetail].[numBoxes] AS CartONes ,
		[InvoiceDetail].[id_amountInvoice] AS Cantidad,
		[InvoiceDetail].[unitPrice] AS Precio,
		[itemSize].[name] AS Size,
		--[DatosProducto].[cASepack] AS cASepack,
		Certificacion = Case when cer.code = 'ASC' then 'ASC' else 'Conventional' end,
		(([InvoiceDetail].[numBoxes] * icf.itemWeightNetWeight) * icf.conversionToKilos) AS [PesoKG],
		([InvoiceDetail].[numBoxes] * icf.itemWeightNetWeight) * icf.conversionToPounds AS [PesoLB],
		(([InvoiceDetail].[numBoxes] * icf.itemWeightGrossWeight) * icf.conversionToKilos)  AS [PesoGlaseoKG],-----------
		([InvoiceDetail].[numBoxes] * icf.itemWeightGrossWeight) * icf.conversionToPounds AS [PesoGlaseoLB],
		([InvoiceDetail].[numBoxes] * icf.weightWithGlaze) * icf.conversionToPounds AS [GlaseoLIBRA],
		([InvoiceDetail].[numBoxes] * icf.weightWithGlaze) * icf.conversionToKilos AS [GlaseoKG],

		[itemTrademark].[name] AS Marca,
		[Item].[descriptiON] AS DescripciON2,
		[Item].foreignName as DescripciON,
		---- Total en Letra
		Total = (SELECT TOP 1 [resultado] FROM @TEMP) + ' Dollars and ' + (SELECT TOP 1 [resultado] FROM @TEMPCents) + ' Cents' ,
		--[Presentation].[name] + ' = ' + Convert(Varchar,cast(Round(([Presentation].[minimum] * [Presentation].[maximum]),2) as decimal(19,2)),50) AS Casepack ,
		[ItemTrademarkModelPf].[name] + '(' + Convert(Varchar,[PresentationPf].[maximum]) + ' x ' + Convert(Varchar,Convert(decimal(10,3),Round(([InvProf].proformaWeight / [PresentationPf].[maximum]),3))) + ' ' + [MetricUnitPf].code  + ')' + ' = ' +  Convert(Varchar,Convert(decimal(10,3),Round([PresentationPf].[maximum] * ([InvProf].proformaWeight / [PresentationPf].[maximum]),3))) AS Casepack,
		Convert(Varchar,[InvoiceExterior].[numeroContenedores],10) + ' Container ' + [CapacityCONtainer].[name] + ' m3' AS NumeroContenedores,
		ISNULL([InvoiceExterior].[containers],'') AS Contenedores,
		[mesFechaProforma].[mesIngles] + ' '+convert(char(2),DAY([DocProf].[emissionDate]))+', '+convert(char(4),YEAR([DocProf].[emissionDate]) )  As FechaProforma,
		[DocProf].[number] AS Proforma,
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
		convert(varchar,[InvoiceExterior].[etdDate],105) AS FechaETD
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
  LEFT OUTER JOIN certification cer
       ON cer.id = [itemGeneral].id_certification
  LEFT OUTER JOIN ItemWeightConversionFreezen icf
        ON icf.id_Item = [Item].[Id]
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
        ON [Presentation].[id] = [item] .[id_presentation] 
  LEFT OUTER JOIN [dbo].[BoxCardAndBank] [BoxCardAndBank]
        ON [BoxCardAndBank].[id] = [InvoiceExterior] .[id_bank] 
  LEFT OUTER JOIN [dbo].[Document] [DocProf]
        ON [DocProf].[id] = [doc].[id_documentOrigen]
  LEFT OUTER JOIN (select ctd.code, ctd.description mesEspanol, ctd.fldvarchar1 mesIngles
				from tbsysCatalogueDetail ctd 
				inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue
				where ct.code ='MONTHT') mesFechaProforma
		ON mesFechaProforma.code =  cast(month([DocProf].[emissionDate]) as varchar)
  LEFT OUTER JOIN [dbo].[PaymentTerm] [PaymentTerm]
        ON [PaymentTerm].[id] = [InvoiceExterior].[id_PaymentTerm]

-------------------PROFORMA--------------
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

  ---------------------------------------------------

    --INNER JOIN   ForeignCustomerIdentificatiON FCI    
    --ON FCI.id_ForeignCustomer = [PersON].ID --------------------------EV    
        WHERE [Invoice].[id] = CONVERT(INT,@id)   
		ORDER BY Item.mASterCode 
		
		--select * from Invoice



