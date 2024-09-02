/****** Object:  StoredProcedure [dbo].[spPar_FacturaElectronicaAuxiliar]    Script Date: 30/05/2023 09:43:14 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROC [dbo].[spPar_FacturaElectronicaAuxiliar]      
@id int      
    as      
 set nocount on       
 select  [Invoice].[id],     
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
       FechaEmbarque = ISNULL([InvoiceExterior].[dateShipment], ''),
       [InvoiceExterior].[id_shippingAgency],      
       [InvoiceExterior].[daeNumber] + '-'+ [InvoiceExterior].[daeNumber2] + '-'+ [InvoiceExterior].[daeNumber3]+ '-'+ [InvoiceExterior].[daeNumber4] as DAE,      
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
       [ForCustomerID].[emailInterno] as email,      
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
    case when isnull([Item].foreignName,'')  ='' then [Item].name else [Item].foreignName end as Descripcion,      
       PuertoDescarga =  [Port].[nombre],
	   TipoPuertoDescarga = [PortType].[code],      
       PuertoDeEmbaruqe = [Port_1].[nombre] +', '+ UPPER([City_1].[name]) ,  
	   TipoPuertoEmbarque = [PortType1].[code],      
       PuertoDestino = [Port_2].[nombre] +', '+ UPPER (coun2.[Name2]) ,   
	   TipoPuertoDestino = [PortType2].[code],     
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
   upper([Country_Destination].[Name2]) as PaisDestino,    
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
    [Company].ruc as RUCcia,
    [Company].trademark as NombreCia,      
    Archivo = [Document].[accessKey]+'.xml',      
    [Document].[accessKey] as ClaveAcceso,      
    'Total ' +[termsNegotiation].[code] + ':' as TerminoNegocia,      
    [companyElectronicFacturation].resolutionNumber as ContribuyenteEspecialNo,      
    [environmentType].[description] as Ambiente,      
    [emissionType].[description] as Emision,      
   case when [companyElectronicFacturation].[requireAccounting] = 1 then 'SI' else 'NO' END AS ObligadoLlevarContabilidad,      
    dbo.FUN_GetRoundTotal( 'T',@id,2) as Valor, -- [InvoiceExterior].[valuetotalCIF] as Valor      
    docs.[code] as codigoEstado,
	ISNULL([InvoiceExterior].[containers],'') as Contenedores,  
    [termsNegotiation].[code] as   TerminoNegociacion,  
	[InvoiceExterior].[numeroContenedores] as NumeroContenedores
	,sqd.number as Proforma
	,Orden =(select top 1 d.number from SalesOrderDetailSalesQuotationExterior a
	inner join SalesOrderDetail b on b.id = a.id_salesOrderDetail
	inner join SalesOrder c on c.id = b.id_salesOrder
	inner join document d on d.id = c.id
	inner join document e on e.id_documentOrigen = a.id_salesQuotationExterior
	where e.id = @id
	 )
	 
  from ((((((((((((((((((((((((((((([dbo].[Invoice] [Invoice]      
  inner join [dbo].[Document] doc on [Invoice].[id] = doc.[id]      
  inner join [dbo].[DocumentState] docs on doc.[id_documentState] = docs.[id]      
  left outer join [dbo].[InvoiceDetail] [InvoiceDetail]      
       on ([InvoiceDetail].[id_invoice] = [Invoice].[id])      
    and [InvoiceDetail].[isActive] = 1)    
  inner join [dbo].[InvoiceExterior] [InvoiceExterior]      
       on ([InvoiceExterior].[id] = [Invoice].[id]))      
  inner join [dbo].[Item] [Item]      
       on ([Item].[id] = [InvoiceDetail].[id_item] ))      
  inner join [dbo].[MetricUnit] [MetricUnit]      
       on ([MetricUnit].[id] = [InvoiceDetail].[id_metricUnitInvoiceDetail]))      
  left outer join [dbo].[Person] [Person]      
       on ([Person].[id]= [InvoiceExterior].[id_consignee]))      
  left outer join [dbo].[ForeignCustomer] [ForCustomer]      
       on ([ForCustomer].[id] = [InvoiceExterior].[id_consignee]))   
	    left outer join [dbo].[ForeignCustomerIdentification] [ForCustomerId]      
       on ([ForCustomerId].[id_ForeignCustomer] = [InvoiceExterior].[id_consignee]
	   And [ForCustomerId].[id] = [InvoiceExterior].[id_addressCustomer] ))
  inner join [dbo].[Document] [Document]      
       on ([Document].[id] = [Invoice].[id]))      
  left outer join [dbo].[Port] [Port]      
       on ([Port].[id] = [InvoiceExterior].[id_portDischarge])) 
  left outer join [dbo].[PortType] [PortType]      
       on ([PortType].[id] = [Port].[id_portType]))  	        
  left outer join [dbo].[City] [City]      
       on ([Port].[id_city] = [City].[id]))      
   left outer join [dbo].[Country] coun      
    on coun.[id] = [City].[id_country]         
  left outer join [dbo].[Port] [Port_1]      
       on ([Port_1].[id] = [InvoiceExterior].[id_portShipment])) 
  left outer join [dbo].[PortType] [PortType1]      
       on ([PortType1].[id] = [Port_1].[id_portType]))  	        
  left outer join [dbo].[City] [City_1]      
       on ([Port_1].[id_city] = [City_1].[id]))      
  left outer join [dbo].[Port] [Port_2]      
       on ([Port_2].[id] = [InvoiceExterior].[id_portDestination])) 
  left outer join [dbo].[PortType] [PortType2]      
       on ([PortType2].[id] = [Port_2].[id_portType]))  	        
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
       on ([emissionType].[id] = [companyElectronicFacturation].[id_emissionType])   

	   left outer join SalesQuotationExterior sqe on sqe.id = Document.id_documentOrigen
	   left outer join document sqd on sqd.id = sqe.id
	   
where [Invoice].[id] = convert(int,@id)   

/*
	EXEC spPar_FacturaElectronicaAuxiliar @id=985170
*/

GO