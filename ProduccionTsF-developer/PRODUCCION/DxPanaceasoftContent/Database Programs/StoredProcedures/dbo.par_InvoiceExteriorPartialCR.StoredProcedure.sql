If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_InvoiceExteriorPartialCR'
	)
Begin
	Drop Procedure dbo.par_InvoiceExteriorPartialCR
End
Go
Create Procedure dbo.par_InvoiceExteriorPartialCR
(
@id int,    
@callIdentity varchar(50)    
)
As    
 set nocount on     
 select 
 [Invoice].[id], [Invoice].[id_saleOrder],    
	   [Invoice].[id_buyer],    
	   [Invoice].[id_remissionGuide],    
	   [Invoice].[subtotalIVA] as Subtotal12,    
	   -- [Invoice].[subTotalIVA0] as SubTotal0,    
	   z1.subtTotal0 as SubTotal0,    
	[Invoice].[subTotalNoObjectIVA],    
	   [Invoice].[subTotalExentIVA],    
	   [Invoice].[subTotal],    
	   [Invoice].[totalDiscount] as Descuento,    
	   [Invoice].[valueICE],    
	   [Invoice].[valueIRBPNR], [Invoice].[IVA],    
	   [Invoice].[tip], [Invoice].[totalValue],    
	--[Invoice].[subtotalNoTaxes] as SubTotalSinImpuesto,    
	   z1.subtTotalWithOutTax as SubTotalSinImpuesto,        
	   [Invoice].[id_InvoiceType],    
	   [Invoice].[id_InvoiceMode],    
	   [InvoiceDetail].[id] as [InvoiceDetail_id],    
	   [InvoiceDetail].[id_invoice],    
	   [InvoiceDetail].[id_item],    
	   [InvoiceDetail].[description],    
	   [InvoiceDetail].[amount],    
	   [InvoiceDetail].[unitPrice] as Precio,    
	   --- [InvoiceDetail].[discount] as Dscto,    
	z2.discount as Dscto,    
	   [InvoiceDetail].[totalPriceWithoutTax],    
	   [InvoiceDetail].[iva],    
	   [InvoiceDetail].[iva0],    
	   [InvoiceDetail].[ivaNoObject],    
	   [InvoiceDetail].[ivaExented],    
	   [InvoiceDetail].[valueICE] as [InvoiceDetail_valueICE],    
	   [InvoiceDetail].[valueIRBPNR] as [InvoiceDetail_valueIRBPNR],    
	   -- [InvoiceDetail].[total] as PrecioTotal,    
	z2.total as PrecioTotal,    
	   [InvoiceDetail].[descriptionAuxCode],    
	   [InvoiceDetail].[masterCode],    
	   -- [InvoiceDetail].[numBoxes] as Cartones,    
	z2.numBoxes as Cartones,    
	   [InvoiceDetail].[id_metricUnit],    
	   [InvoiceDetail].[id_metricUnitInvoiceDetail],    
	   -- [InvoiceDetail].[id_amountInvoice] as Cantidad,    
	z2.amount as Cantidad,    
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
	   convert (datetime, [InvoiceExterior].[dateShipment]) as FechaEmbarque, ---------------------------------------EV  
	   [InvoiceExterior].[id_shippingAgency],    
	   [InvoiceExterior].[daeNumber4] as DAE,    
	[InvoiceExterior].[blNumber] as BL,    
	[InvoiceExterior].[numberRemissionGuide] as GuiaRemision,    
--       Buque = [InvoiceExterior].[shipName] + ', '+ convert(varchar(39),[InvoiceExterior].[shipNumberTrip]),  
	   ISNULL([InvoiceExterior].[shipName],'') as Buque
	   ,ISNULL([InvoiceExterior].[shipNumberTrip],'') as Viaje  ,
	   [InvoiceExterior].[totalBoxes] as TotalCM,    
	   [InvoiceExterior].[id_capacityContainer],    
	   [InvoiceExterior].[id_tariffHeading],    
	   [InvoiceExterior].[direccion] as direccion,    
	   [InvoiceExterior].[email],    
	[ForCustomer].[emailInterno] as email,    
	   SUBSTRING([Document].[number],1,3 ) as [ptoEmision],    
	   SUBSTRING([Document].[number],4,3 ) as [establecimiento],    
	   [Document].[sequential],    
	   -- [InvoiceExterior].[valueTotalFOB] as ValorTotalFob,    
	z1.subtTotalFob as ValorTotalFob,    
	   [z1].[valueInternationalFreight] as FleteInternacional,    
	   [z1].[valueInternationalInsurance] as SeguroInternacional,    
	   [z1].[valueCustomsExpenditures] as GastosAduaneros,    
	   [z1].[valueTransportationExpenses] as GastosTrasnporte,    
	   [InvoiceExterior].[id_metricUnitInvoice],    
	   null [fileXML],    
	   [Document].id_documentState as [idStatusDocument],    
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
	case [Item].[description2] when null then [Item].[description] else [Item].[description2] end as Descripcion,    
	   PuertoDescarga =  [Port].[nombre]+', '+coun.[Name],    
	   PuertoDeEmbaruqe = [Port_1].[nombre] +', '+ [City_1].[name] ,    
	   PuertoDestino = [Port_2].[nombre] +', '+  coun2.[name] ,    
	 [EmissionPoint].[name] as [EmissionPoint_name],    
	   [Company].[businessName],    
	 [Company].[address] as DirMatiz,      
  z1.numberDocumentDivided as Factura,     
  [Document].[emissionDate] as FEchaEmision,    
  [Document].[authorizationNumber] as NumeroAutorizacion,    
  case when [Document].[authorizationDate] is not null     
  then convert(varchar,convert(varchar,[Document].[authorizationDate],103) + ' ' +    
  convert(varchar(8),convert(time,[Document].[authorizationDate])))     
  else convert(varchar,'01/01/0001 0:00:00') end  as FechaHoraAutorizacion,    
  [Document].[authorizationDate],    
  ( select TOP 1 iew_div.peso    
   from InvoiceExteriorWeightDivided iew_div              
   where   iew_div.id_InvoiceExteriorDivided= z1.id         
	 and iew_div.id_WeightType = 1 and iew_div.id_metricUnit = 1    
  ) as pesonetokilos,    
  ( select TOP 1 iew_div.peso     
   from InvoiceExteriorWeightDivided iew_div         
   where   iew_div.id_InvoiceExteriorDivided= z1.id    
	   and iew_div.id_WeightType = 1 and iew_div.id_metricUnit = 4    
  ) as pesonetolibras,    
  ( select TOP 1 iew_div.peso     
   from InvoiceExteriorWeightDivided iew_div    
   where   iew_div.id_InvoiceExteriorDivided= z1.id    
	 and iew_div.id_WeightType = 2 and iew_div.id_metricUnit = 1    
  ) as pesobrutokilos,    
  ( select TOP 1 iew_div.peso     
   from InvoiceExteriorWeightDivided iew_div    
   where   iew_div.id_InvoiceExteriorDivided= z1.id    
	 and iew_div.id_WeightType = 2 and iew_div.id_metricUnit = 4    
  ) as pesobrutolibras,    
  ( select TOP 1 iew_div.peso      
	from  InvoiceExteriorWeightDivided iew_div     
	where  iew_div.id_InvoiceExteriorDivided= z1.id    
	 and iew_div.id_WeightType = 3 and iew_div.id_metricUnit = 1    
  ) as pesoglaskilos,    
  ( select TOP 1 iew_div.peso      
	from  InvoiceExteriorWeightDivided iew_div     
	where  iew_div.id_InvoiceExteriorDivided= z1.id    
	 and iew_div.id_WeightType = 3 and iew_div.id_metricUnit = 4         
  ) as pesoglaslibras,    
  (select TOP 1 value from Setting where code = 'FPFF' ) as FormaDePago,    
  (select TOP 1 value from Setting where code = 'PLFF' ) as Plazo,    
  (select TOP 1 value from Setting where code = 'PTFF' ) as Tiempo,    
  [City_2].[name]+','+[Country_Destination].[name] as PaisDestino,    
   ---validar en reporte que si no hay dato del glaseo, se haga invisible las etiquetas    
	   [ShippingAgency].[name] as Naviera,    
	   --[TariffHeading].[code] as Partida,    
	   InvoiceExterior.tariffHeadingDescription as Partida,
	   NoConten = [CapacityContainer].[code] + ' : '+ rtrim(convert(varchar(30),[InvoiceExterior].[numeroContenedores])),    
	[BranchOffice].[address] as DirSucural,    
	[Division].[address] as [Division_Address],    
	[Company].ruc as RUC,    
	[Company].logo as logo,    
	[Company].trademark as NombreCia,    
	Archivo = [Document].[accessKey]+'.xml',    
	[Document].[accessKey] as ClaveAcceso,    
	[termsNegotiation].[code],    
	[companyElectronicFacturation].resolutionNumber as ContribuyenteEspecialNo,    
	[environmentType].[description] as Ambiente,    
	[emissionType].[description] as Emision,    
	case when [companyElectronicFacturation].[requireAccounting] = 1 then 'SI' else 'NO' END AS ObligadoLlevarContabilidad, 
	RUCEXTERIOR = (SELECT TOP 1 ISNULL(numberIdentification,'') FROM [ForeignCustomerIdentification] WHERE id_ForeignCustomer = [CONS].id)  ,    
	--- [InvoiceExterior].[valuetotalCIF] as Valor,    
	z1.subtTotalIncoTerm as Valor,     
 (select  top 1 letter from tbsysParceLetterNumber where number = z1.secuencie ) as SecuenceLetter,
 z1.secuencie  
  from (((((((((((((((((((((((((([dbo].[Invoice] [Invoice]    
  inner join [dbo].[InvoiceDetail] [InvoiceDetail]    
	   on ([InvoiceDetail].[id_invoice] = [Invoice].[id]))    
  inner join [dbo].[InvoiceExterior] [InvoiceExterior]    
	   on ([InvoiceExterior].[id] = [Invoice].[id]))    
  inner join [dbo].[Item] [Item]    
	   on ([Item].[id] = [InvoiceDetail].[id_item]))    
  inner join [dbo].[MetricUnit] [MetricUnit]    
	   on ([MetricUnit].[id] = [InvoiceDetail].[id_metricUnitInvoiceDetail]))    
  inner join [dbo].[Person] [Person]    
	   on ([Person].[id]= [Invoice].[id_buyer]))    
  inner join [dbo].[ForeignCustomer] [ForCustomer]    
	   on ([ForCustomer].[id] = [Invoice].[id_buyer]))    
  inner join [dbo].[Document] [Document]    
	   on ([Document].[id] = [Invoice].[id]))    
  /* Seccion   */    
  inner join InvoiceExteriorDivided z1    
	on z1.id_invoice = Invoice.id    
   inner join InvoiceExteriorDetailDivided z2    
   on z2.id_InvoiceExteriorDivided = z1.id       
   and z2.id_InvoiceDetail = [InvoiceDetail].id    
  /* End Seccion */    
  left outer join [dbo].[Port] [Port]    
	   on ([Port].[id] = [InvoiceExterior].[id_portDischarge]))    
  left outer join [dbo].[City] [City]    
	   on ([Port].[id_city] = [City].[id]))    
  left outer join [dbo].[Country] coun   
	   on coun.id = [City].id_country  
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
		 left outer join [dbo].[Person] [CONS]      
	   on ([CONS].[id]= [InvoiceExterior].[id_consignee]))  
	left outer join [dbo].[Country] coun2   
	   on coun2.id = [City_2].id_country   
  left outer join [dbo].[Country] [Country_Destination]    
	   on ([Country_Destination].[id] = [City_2].[id_country]))    
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
where [Invoice].[id] = convert(int,@id)  
--order by  [Item].[masterCode] asc  
   and z1.callIdentity = @callIdentity
order by z1.secuencie;    
Go
