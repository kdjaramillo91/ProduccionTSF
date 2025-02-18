If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_InvoiceExteriorCR'
	)
Begin
	Drop Procedure dbo.par_InvoiceExteriorCR
End
Go
Create Procedure dbo.par_InvoiceExteriorCR
(
@id int      
)
As      
 set nocount on       
 select [Invoice].[id],     
	[Invoice].[id_saleOrder],      
	   [Invoice].[id_buyer],      
	   [Invoice].[id_remissionGuide],      
	   [Invoice].[subtotalIVA] as Subtotal12,      
	   dbo.FUN_GetRoundTotal( 'S',@id,2) as SubTotal0,--[Invoice].[subTotalIVA0] as SubTotal0,      
	   [Invoice].[subTotalNoObjectIVA],      
	   [Invoice].[subTotalExentIVA],      
	   dbo.FUN_GetRoundTotal( 'S',@id,2) as subTotal, --//[Invoice].[subTotal],      
	   [Invoice].[totalDiscount] as Descuento,      
	   [Invoice].[valueICE],      
	   [Invoice].[valueIRBPNR], [Invoice].[IVA],      
	   [Invoice].[tip], [Invoice].[totalValue],      
	   dbo.FUN_GetRoundTotal( 'S',@id,2) as SubTotalSinImpuesto,  --[Invoice].[subtotalNoTaxes] as SubTotalSinImpuesto,      
	   [Invoice].[id_InvoiceType],      
	   [Invoice].[id_InvoiceMode],      
	   [InvoiceDetail].[id] as [InvoiceDetail_id],      
	   [InvoiceDetail].[id_invoice],      
	   [InvoiceDetail].[id_item],      
	   [InvoiceDetail].[description],      
	   [InvoiceDetail].[amount],      
	   [InvoiceDetail].[unitPrice] as Precio,      
	   [InvoiceDetail].[discount] as Dscto,      
	   [InvoiceDetail].[totalPriceWithoutTax],      
	   [InvoiceDetail].[iva],      
	   [InvoiceDetail].[iva0],      
	   [InvoiceDetail].[ivaNoObject],      
	   [InvoiceDetail].[ivaExented],      
	   [InvoiceDetail].[valueICE] as [InvoiceDetail_valueICE],      
	   [InvoiceDetail].[valueIRBPNR] as [InvoiceDetail_valueIRBPNR],      
	   [InvoiceDetail].[total] as PrecioTotal,      
	   [InvoiceDetail].[descriptionAuxCode],      
	   [InvoiceDetail].[masterCode],      
	   [InvoiceDetail].[numBoxes] as Cartones,      
	   [InvoiceDetail].[id_metricUnit],      
	   [InvoiceDetail].[id_metricUnitInvoiceDetail],      
	   [InvoiceDetail].[id_amountInvoice] as Cantidad,      
	   [InvoiceDetail].[dateUpdate],      
	   [InvoiceDetail].[codePresentation],      
	   [InvoiceDetail].[presentationMinimum],      
	   [InvoiceDetail].[presentationMaximum],      
	   [InvoiceExterior].[id] as [InvoiceExterior_id],      
	   [InvoiceExterior].[id_productionOrder],      
	   [InvoiceExterior].[id_releaseProduction],      
	   [InvoiceExterior].[id_termsNegotiation],      
	   [InvoiceExterior].[id_PaymentMethod],      
	   [InvoiceExterior].[id_PaymentTerm],      
	   [InvoiceExterior].[id_portShipment],      
	   [InvoiceExterior].[id_portDestination],      
	   [InvoiceExterior].[id_portDischarge],      
	   --[InvoiceExterior].[dateShipment] as FechaEmbarque,      
	   FechaEmbarque = Convert(Datetime,[InvoiceExterior].[dateShipment],102),       
	   [InvoiceExterior].[id_shippingAgency],      
	   [InvoiceExterior].[daeNumber4] as DAE,      
	[InvoiceExterior].[blNumber] as BL,      
	[InvoiceExterior].[numberRemissionGuide] as GuiaRemision,      
	   Buque2 = [InvoiceExterior].[shipName] + ' V/'+ convert(varchar(39),[InvoiceExterior].[shipNumberTrip]),    
	ISNULL([InvoiceExterior].[shipName],'') AS Buque,----------------------------------------EV    
	ISNULL([InvoiceExterior].[shipNumberTrip] ,'') as VIAJE,------------------------------------EV    
	   [InvoiceExterior].[totalBoxes] as TotalCM,      
	   [InvoiceExterior].[id_capacityContainer],      
	   [InvoiceExterior].[id_tariffHeading],      
	   -- [InvoiceExterior].[direccion] as direccion,      
	   --[InvoiceExterior].[email],      
	Person.address as direccion,    
	   [ForCustomer].[emailInterno] as email,      
	   SUBSTRING([Document].[number],1,3) as  [ptoEmision],      
	SUBSTRING([Document].[number],5,3) as  [establecimiento],      
	[Document].[sequential] as  [secuencial],      
		dbo.FUN_GetRoundTotal( 'F',@id,2) as  ValorTotalFob, --[InvoiceExterior].[valueTotalFOB] as ValorTotalFob,      
	   [InvoiceExterior].[valueInternationalFreight] as FleteInternacional,      
	   [InvoiceExterior].[valueInternationalInsurance] as SeguroInternacional,      
	   [InvoiceExterior].[valueCustomsExpenditures] as GastosAduaneros,      
	   [InvoiceExterior].[valueTransportationExpenses] as GastosTrasnporte,      
	   [InvoiceExterior].[id_metricUnitInvoice],      
	   null as [fileXML],      
	   null as [idStatusDocument],      
	   [InvoiceExterior].[id_userCreate] as [InvoiceExterior_id_userCreate],      
	   [InvoiceExterior].[dateCreate] as [InvoiceExterior_dateCreate],      
	   [InvoiceExterior].[id_userUpdate] as [InvoiceExterior_id_userUpdate],      
	   [InvoiceExterior].[dateUpdate] as [InvoiceExterior_dateUpdate],      
	   [InvoiceExterior].[id_consignee],      
	   [InvoiceExterior].[id_notifier],      
	   [InvoiceExterior].[purchaseOrder],      
	   [InvoiceExterior].[id_ShippingLine],      
	   [MetricUnit].[code] as Unidad,      
	   [Person].[fullname_businessName] as RazonSocial,      
	   [Person].[identification_number] as RucCedula,      
	   [Item].[masterCode] as CodigoAuxiliar,      
	   [Item].[auxCode] as CodigoPrincipal,       
	case when isnull([Item].[description2],'')  ='' then [Item].[description] else [Item].[description2] end as Descripcion,      
	   PuertoDescarga =  [Port].[nombre]+', '+UPPER(coun.[name2]),      
	   PuertoDeEmbaruqe = [Port_1].[nombre] +', '+ UPPER([City_1].[name]) ,      
	   PuertoDestino = [Port_2].[nombre] +', '+ UPPER (coun2.[Name2]) ,      
	 [EmissionPoint].[name] as [EmissionPoint_name],      
	   [Company].[businessName],      
	 [Company].[address] as DirMatiz,      
  [Document].[number] as Factura,      
  [Document].[emissionDate] as FEchaEmision,      
  [Document].[authorizationNumber] as NumeroAutorizacion,      
  case when [Document].[authorizationDate] is not null       
  then convert(varchar,convert(varchar,[Document].[authorizationDate],103) + ' ' +      
  convert(varchar(8),convert(time,[Document].[authorizationDate])))       
  else convert(varchar,'01/01/0001 0:00:00') end  as FechaHoraAutorizacion,      
  [Document].[authorizationDate],      
  (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 1 and iew.id_metricUnit = 1 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesonetokilos,      
  (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 1 and iew.id_metricUnit = 4 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesonetolibras,      
  (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 2 and iew.id_metricUnit = 1 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesobrutokilos,      
  (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 2 and iew.id_metricUnit = 4 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesobrutolibras,      
   (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 3 and iew.id_metricUnit = 1 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesoglaskilos,      
  (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 3 and iew.id_metricUnit = 4 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesoglaslibras,      
   (select TOP 1 value from Setting where code = 'FPFF' ) as FormaDePago,      
   (select TOP 1 value from Setting where code = 'PLFF' ) as Plazo,      
   (select TOP 1 value from Setting where code = 'PTFF' ) as Tiempo,      
   [City_2].[name]+', '+upper([Country_Destination].[Name2]) as PaisDestino,    
   --FCI.numberIdentification  AS RUCEXTERIOR,------------------------------------------------------------
   RUCEXTERIOR = (SELECT TOP 1 ISNULL(numberIdentification,'') FROM [ForeignCustomerIdentification] WHERE id_ForeignCustomer = [Person].id)  , 
   ---validar en reporte que si no hay dato del glaseo, se haga invisible las etiquetas      
	   [ShippingAgency].[name] as Naviera,      
	   [ShippingLine].[name] as Linea_Naviera,      
	   [ShippingAgency].[name] as Naviera,    
	case        
	when InvoiceExterior.tariffHeadingDescription is null then [TariffHeading].[code]    
	else InvoiceExterior.tariffHeadingDescription    
	end as Partida,    
	[TariffHeading].[code]  as Partida2,         
	   NoConten = 'NroConten '+ [CapacityContainer].[code] + ' : '+ rtrim(convert(varchar(30),[InvoiceExterior].[numeroContenedores])),      
	[BranchOffice].[address] as DirSucural,      
	[Division].[address] as [Division_Address],      
	[Company].ruc as RUC,      
	[Company].logo as logo,      
	[Company].trademark as NombreCia,      
	Archivo = [Document].[accessKey]+'.xml',      
	[Document].[accessKey] as ClaveAcceso,      
	'Total ' +[termsNegotiation].[code] + ':' as TerminoNegocia,      
	[companyElectronicFacturation].resolutionNumber as ContribuyenteEspecialNo,      
	[environmentType].[description] as Ambiente,      
	[emissionType].[description] as Emision,      
   case when [companyElectronicFacturation].[requireAccounting] = 1 then 'SI' else 'NO' END AS ObligadoLlevarContabilidad,      
	dbo.FUN_GetRoundTotal( 'T',@id,2) as Valor, -- [InvoiceExterior].[valuetotalCIF] as Valor      
	 docs.[code] as codigoEstado      
		 
  from (((((((((((((((((((((((((([dbo].[Invoice] [Invoice]      
  inner join [dbo].[Document] doc on [Invoice].[id] = doc.[id]      
  inner join [dbo].[DocumentState] docs on doc.[id_documentState] = docs.[id]      
  left outer join [dbo].[InvoiceDetail] [InvoiceDetail]      
	   on ([InvoiceDetail].[id_invoice] = [Invoice].[id])      
	and [InvoiceDetail].[isActive] = 1)    
  inner join [dbo].[InvoiceExterior] [InvoiceExterior]      
	   on ([InvoiceExterior].[id] = [Invoice].[id]))      
  inner join [dbo].[Item] [Item]      
	   on ([Item].[id] = [InvoiceDetail].[id_item]))      
  inner join [dbo].[MetricUnit] [MetricUnit]      
	   on ([MetricUnit].[id] = [InvoiceDetail].[id_metricUnitInvoiceDetail]))      
  left outer join [dbo].[Person] [Person]      
	   on ([Person].[id]= [InvoiceExterior].[id_consignee]))      
  left outer join [dbo].[ForeignCustomer] [ForCustomer]      
	   on ([ForCustomer].[id] = [InvoiceExterior].[id_consignee]))      
  inner join [dbo].[Document] [Document]      
	   on ([Document].[id] = [Invoice].[id]))      
  left outer join [dbo].[Port] [Port]      
	   on ([Port].[id] = [InvoiceExterior].[id_portDischarge]))      
  left outer join [dbo].[City] [City]      
	   on ([Port].[id_city] = [City].[id]))      
   left outer join [dbo].[Country] coun      
	on coun.[id] = [City].[id_country]         
  left outer join [dbo].[Port] [Port_1]      
	   on ([Port_1].[id] = [InvoiceExterior].[id_portShipment]))      
  left outer join [dbo].[City] [City_1]      
	   on ([Port_1].[id_city] = [City_1].[id]))      
  left outer join [dbo].[Port] [Port_2]      
	   on ([Port_2].[id] = [InvoiceExterior].[id_portDestination]))      
  left outer join [dbo].[EmissionPoint] [EmissionPoint]      
	   on ([EmissionPoint].[id] = [Document].[id_emissionPoint]))      
  left outer join [dbo].[Company] [Company]      
	   on ([Company].[id] = [EmissionPoint].[id_company]))      
  left outer join [dbo].[City] [City_2]      
	   on ([City_2].[id] = [Port_2].[id_city]))      
  left outer join [dbo].[Country] coun2      
	on coun2.[id] = [City_2].[id_country]      
  left outer join [dbo].[Country] [Country_Destination]      
	   on ([Country_Destination].[id] = [City_2].[id_country]))      
			 
	left outer join [dbo].[ShippingLine]      
	   [ShippingLine]      
	   on ([ShippingLine].[id] = [InvoiceExterior].[id_ShippingLine]))      
			 
			 
  left outer join [dbo].[ShippingAgency]      
	   [ShippingAgency]      
	   on ([ShippingAgency].[id] = [InvoiceExterior].[id_shippingAgency]))      
  left outer join [dbo].[TariffHeading] [TariffHeading]      
	   on ([TariffHeading].[id] = [InvoiceExterior].[id_tariffHeading]))      
  left outer join [dbo].[CapacityContainer]      
	   [CapacityContainer]      
	   on ([CapacityContainer].[id] = [InvoiceExterior].[id_capacityContainer]))      
  left outer join [dbo].[BranchOffice] [BranchOffice]      
	   on ([BranchOffice].[id] = [EmissionPoint].[id_branchOffice]))      
  left outer join [dbo].[Division] [Division]      
	   on ([Division].[id] = [EmissionPoint].[id_division]))       
  left outer join [dbo].[termsNegotiation] [termsNegotiation]      
	   on ([InvoiceExterior].[id_termsNegotiation] = [termsNegotiation].[id]))      
  inner join [dbo].[CompanyElectronicFacturation] [companyElectronicFacturation]      
	on ([Company].[id] = [companyElectronicFacturation].id_company))      
  left outer join [dbo].[EnvironmentType] [environmentType]      
	   on ([environmentType].[id] = [companyElectronicFacturation].[id_enviromentType]))      
  left outer join [dbo].[EmissionType] [emissionType]      
	   on ([emissionType].[id] = [companyElectronicFacturation].[id_emissionType]))    
	--INNER JOIN   ForeignCustomerIdentification FCI    
	--ON FCI.id_ForeignCustomer = [Person].ID --------------------------EV    
		where [Invoice].[id] = convert(int,@id)   
		order by Item.masterCode  
Go
