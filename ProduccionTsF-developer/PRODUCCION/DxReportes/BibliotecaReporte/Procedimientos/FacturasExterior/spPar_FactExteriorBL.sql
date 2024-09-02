/****** Object:  StoredProcedure [dbo].[spPar_FactExteriorBL]    Script Date: 12/05/2023 02:00:15 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

-- Query - SP Original: par_InvoiceExteriorBL
CREATE OR ALTER PROC [dbo].[spPar_FactExteriorBL]
@id int      
    as      
 set nocount on       
 select [Invoice].[id],     
	   Isnull([Invoice].[id_saleOrder],'')AS id_saleOrder,      
       [Invoice].[id_buyer],      
       isnull([Invoice].[id_remissionGuide],'') as id_remissionGuide   ,
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
	   CBL = (select sum(ID.numboxes) from invoicedetail id
	   where id.id_invoice =@id and id.isActive = 1 group by id.id_invoice),
	   --sum([InvoiceDetail].[numBoxes]) as NCartonesBL, 
       [InvoiceDetail].[id_metricUnit],      
       [InvoiceDetail].[id_metricUnitInvoiceDetail],      
       [InvoiceDetail].[id_amountInvoice] as Cantidad,      
       [InvoiceDetail].[dateUpdate],      
       [InvoiceDetail].[codePresentation],      
       [InvoiceDetail].[presentationMinimum],      
       [InvoiceDetail].[presentationMaximum],      
       [InvoiceExterior].[id] as [InvoiceExterior_id],      
       ISNULL([InvoiceExterior].[id_productionOrder],'')as id_productionOrder,      
       ISNULL([InvoiceExterior].[id_releaseProduction],'') as id_releaseProduction,      
       [InvoiceExterior].[id_termsNegotiation],      
       [InvoiceExterior].[id_PaymentMethod],      
       [InvoiceExterior].[id_PaymentTerm],      
       ISNULL([InvoiceExterior].[id_portShipment],'') as id_portShipment,      
       ISNULL([InvoiceExterior].[id_portDestination],'')as id_portDestination,      
       ISNULL([InvoiceExterior].[id_portDischarge],'')as id_portDischarge,      
       --[InvoiceExterior].[dateShipment] as FechaEmbarque,      
       FechaEmbarque = Convert(Datetime,[InvoiceExterior].[dateShipment],102),       
       ISNULL([InvoiceExterior].[id_shippingAgency],''),      
       ISNULL([InvoiceExterior].[daeNumber],'') + '-'+ ISNULL([InvoiceExterior].[daeNumber2],'') + '-'+ ISNULL([InvoiceExterior].[daeNumber3],'')+ '-'+ ISNULL([InvoiceExterior].[daeNumber4],'') as DAE,      
     ISNULL([InvoiceExterior].[blNumber],'') as BL,      
     ISNULL([InvoiceExterior].[numberRemissionGuide],'') as GuiaRemision,      
       Buque2 =  ISNULL([InvoiceExterior].[shipName],'') + ' V/'+  ISNULL(convert(varchar(39),[InvoiceExterior].[shipNumberTrip]),''),    
    ISNULL([InvoiceExterior].[shipName],'') AS Buque,----------------------------------------EV    
    ISNULL([InvoiceExterior].[shipNumberTrip] ,'') as VIAJE,------------------------------------EV    
       [InvoiceExterior].[totalBoxes] as TotalCM,      
       [InvoiceExterior].[id_capacityContainer],      
       ISNULL([InvoiceExterior].[id_tariffHeading],'')as id_tariffHeading  ,
       -- [InvoiceExterior].[direccion] as direccion,      
       --[InvoiceExterior].[email],      
    Person.address as direccion,    
       --[ForCustomer].[emailInterno] as email,      
       SUBSTRING([Document].[number],1,3) as  [ptoEmision],      
    SUBSTRING([Document].[number],5,3) as  [establecimiento],      
    [Document].[sequential] as  [secuencial],      
        dbo.FUN_GetRoundTotal( 'F',@id,2) as  ValorTotalFob, --[InvoiceExterior].[valueTotalFOB] as ValorTotalFob,      
       [InvoiceExterior].[valueInternationalFreight] as FleteInternacional,      
       [InvoiceExterior].[valueInternationalInsurance] as SeguroInternacional,      
       [InvoiceExterior].[valueCustomsExpenditures] as GastosAduaneros,      
       [InvoiceExterior].[valueTransportationExpenses] as GastosTrasnporte,      
       ISNULL([InvoiceExterior].[id_metricUnitInvoice],'')as id_metricUnitInvoice,      
       null as [fileXML],																			
       null as [idStatusDocument],      
       [InvoiceExterior].[id_userCreate] as [InvoiceExterior_id_userCreate],      
       [InvoiceExterior].[dateCreate] as [InvoiceExterior_dateCreate],      
       [InvoiceExterior].[id_userUpdate] as [InvoiceExterior_id_userUpdate],      
       [InvoiceExterior].[dateUpdate] as [InvoiceExterior_dateUpdate],      
       [InvoiceExterior].[id_consignee],      
       [InvoiceExterior].[id_notifier],      
       [InvoiceExterior].[purchaseOrder],      
       ISNULL([InvoiceExterior].[id_ShippingLine],'')as id_ShippingLine,      
       [MetricUnit].[code] as Unidad,      
       [Person].[fullname_businessName] as RazonSocial,      
       [Person].[identification_number] as RucCedula,      
       [Item].[masterCode] as CodigoAuxiliar,      
       [Item].[auxCode] as CodigoPrincipal,       
    case when isnull([Item].[name],'')  ='' then [Item].[name] else [Item].[name] end as Descripcion,      
       PuertoDescarga =  ISNULL([Port].[nombre],''),
	   TipoPuertoDescarga = ISNULL([PortType].[code],''),      
       PuertoDeEmbaruqe = ISNULL([Port_1].[nombre],'') +', '+ ISNULL(UPPER([City_1].[name]),'') ,  
	   TipoPuertoEmbarque = ISNULL([PortType1].[code],''),      
       PuertoDestino = ISNULL([Port_2].[nombre],'') +', '+ ISNULL(UPPER(coun2.[Name2]),'') ,   
	   TipoPuertoDestino = ISNULL([PortType2].[code],''),     
     [EmissionPoint].[name] as [EmissionPoint_name],      
       [Company].[businessName],      
     [Company].[address] as DirMatiz,      
  [Document].[number] as Factura,      
  [Document].[emissionDate] as FEchaEmision,      
  ISNULL([Document].[authorizationNumber],'') as NumeroAutorizacion,      
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
   ISNULL(upper([Country_Destination].[Name2]),'') as PaisDestino,    
   --FCI.numberIdentification  AS RUCEXTERIOR,------------------------------------------------------------
   RUCEXTERIOR = (SELECT TOP 1 ISNULL(numberIdentification,'') FROM [ForeignCustomerIdentification] WHERE id_ForeignCustomer = [Person].id)  , 
   ---validar en reporte que si no hay dato del glaseo, se haga invisible las etiquetas      
      ISNULL( [ShippingAgency].[name],'') as Naviera,      
       isnull([ShippingLine].[name],'') as Linea_Naviera,      
       isnull([ShippingAgency].[name],'') as Naviera,    
    case        
    when InvoiceExterior.tariffHeadingDescription is null then [TariffHeading].[code]    
    else substring(InvoiceExterior.tariffHeadingDescription,1,6)    
    end as Partida,    
    ISNULL([TariffHeading].[code],'')  as Partida2,         
       NoConten = 'NroConten '+ [CapacityContainer].[code] + ' : '+ rtrim(convert(varchar(30),[InvoiceExterior].[numeroContenedores])),      
    [BranchOffice].[address] as DirSucural,      
    [Division].[address] as [Division_Address],      
    [Company].ruc as RUC,      
    [Company].logo as logo,      
    [Company].trademark as NombreCia,
	company.phoneNumber as TelefonoCompania,
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
	[InvoiceExterior].[numeroContenedores] as numeroContenedores,
	-------------------------------------------------------------BL-----------------------------------------------------------
	UPPER([Person].fullname_businessName) AS CONSIGNATARIO,
	[Person].identification_number AS CONSIGNATARIORUC,
	UPPER((select addressForeign from ForeignCustomerIdentification where  id_ForeignCustomer=InvoiceExterior.id_consignee and id=InvoiceExterior.id_addressCustomer)) as CONSIGNATARIODIRECCION,
	UPPER((select emailInterno from ForeignCustomerIdentification where  id_ForeignCustomer=InvoiceExterior.id_consignee and id=InvoiceExterior.id_addressCustomer)) as CONSIGNATARIOEMAIL,
	--upper(isnull([Person].email,'')) as CONSIGNATARIOEMAIL,
	ISNULL(fci.phone1FC,'') AS CONSIGNATARIOTLFNO,
	CASE WHEN LEN(ISNULL(iden.name, '')) > 15 
		THEN SUBSTRING(ISNULL(iden.name, ''), 0, 15)
		ELSE ISNULL(iden.name, '') END AS ConsignatarioTipoIdent,
	ISNULL(FCI.numberIdentification, '') AS ConsignatarioIdent,
	NBOOKING =isnull(CONVERT(VARCHAR,[InvoiceExterior].BookingNumber,25),''),
	REFERENCIAPROFORMA= ISNULL(CONVERT(VARCHAR,[SalesQuotationExteriorDocument].referenceDocument,25),''),
	--EXPORTCARRIER = ISNULL([Port].[nombre],'')+' '+ISNULL([InvoiceExterior].[shipName],''),
	EXPORTCARRIER =ISNULL([InvoiceExterior].[shipName],'') + ' '+ ISNULL(InvoiceExterior.shipNumberTrip,''),
	PuertoDescargaBL =  ISNULL(UPPER([Port].[nombre]),'')+', '+isnull(UPPER(coun2.name),''),
	BUYER.fullname_businessName AS CLIENTEEXTERIORBL,
	UPPER((select top 1 addressForeign from ForeignCustomerIdentification where id_ForeignCustomer=InvoiceExterior.id_notifier and id=InvoiceExterior.id_addressCustomer)) AS DIRECCIONCLIEXBL,
	upper(BUYER.email) as MAILCLIEXTBL,
	BUYER.cellPhoneNumberPerson AS TELEFONOCLIEXBL,
	ISNULL(InvoiceExterior.seals,'') as SELLOBL,
	CARTONESBL = CONVERT(VARCHAR,[InvoiceDetail].[numBoxes],25) +CHAR(13)+ 'CARTONS' ,
	CARTONESBL2 = CONVERT(VARCHAR,(select sum(ID.numboxes) from invoicedetail id
	   where id.id_invoice =@id and id.isActive = 1 group by id.id_invoice),25) + ' CARTONS',
	NCONTENDORESBL = rtrim(convert(varchar(30),[InvoiceExterior].[numeroContenedores])) + 'X'+ [CapacityContainer].[code],
	InvoiceDetail.description as DESCRIPCIONBL	,
	NINVOICEBL = 'INVOICE : ' + Document.number,
	DAEBL = 'DAE : '+ isnull([InvoiceExterior].[daeNumber],'') + '-'+ ISNULL([InvoiceExterior].[daeNumber2],'') + '-'+ISNULL([InvoiceExterior].[daeNumber3],'')+ '-'+ ISNULL([InvoiceExterior].[daeNumber4],''),
	  (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 1 and iew.id_metricUnit = 1 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesonetokilosBL,      
  (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 1 and iew.id_metricUnit = 4 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesonetolibrasBL,      
  (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 2 and iew.id_metricUnit = 1 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesobrutokilosBL,      
  (select TOP 1 peso from InvoiceExteriorWeight [iew]      
   where  iew.id_WeightType = 2 and iew.id_metricUnit = 4 and [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as pesobrutolibrasBL,
  (select TOP 1 id_metricUnit from InvoiceExteriorWeight [iew]      
   where [iew].[id_invoice] = [Invoice].[id] and [iew].[isActive] = 1) as TIPOpesoBL,
   ISNULL(InvoiceExterior.temperatureInstruction,'') as INSTRUCCIONTEMPBL,
   ISNULL(InvoiceExterior.idTipoTemperatura,'') as IDTEMPBL,
   ISNULL(InvoiceExterior.BLNumber,'') AS BLNUMBER,
   --invoiceexterior.PO AS PO,      --habilitar con base actualizada
   ISNULL(InvoiceExterior.etdDate,'') AS FECHAETDBL,
   [Item].foreignName + ' '+CONVERT(VARCHAR,Presentation.maximum,25) + 'X' + Convert(Varchar,Convert(decimal(10,2),Round(([Presentation].minimum),2))) + ' ' +upper(MetricUnit.code) AS DESCRIPCIONPR
   ,'PO: '+ InvoiceExterior.PO as PO
   ,'CONTRACT: ' + InvoiceExterior.noContrato as NCONTRATO
   ,DOC.description AS OBSERVACIONBL
   ,isnull(upper([InvoiceExterior].transport),'')  as transport
   ,isnull(prof.number,'') as Proforma
	from ((((((((((((((((((((((((((((([dbo].[Invoice] [Invoice]      
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
       on ([emissionType].[id] = [companyElectronicFacturation].[id_emissionType]))    
	   --------------------------------------------BL--------------------------------------------------------------------------------------------------------------
	LEFT JOIN [dbo].[SalesQuotationExterior][salesQuotationExterior] ON [salesQuotationExterior].[id] = [invoice].[id]
		AND [salesQuotationExterior].[id] = [doc].[id]
	LEFT JOIN [SalesQuotationExteriorDocument] [SalesQuotationExteriorDocument] ON
		     salesQuotationExterior.ID = SalesQuotationExteriorDocument.id_salesQuotationExterior
	left join person buyer on
			  buyer.id = Invoice.id_buyer
  LEFT OUTER JOIN [dbo].[Presentation] [Presentation]
        ON [Presentation].[id] = [item] .[id_presentation] 
   LEFT OUTER JOIN  document prof  on prof.id = doc.id_documentOrigen
   LEFT JOIN ForeignCustomerIdentification FCI ON FCI.ID = InvoiceExterior.id_addressCustomer
   -- Tablas para unión de Tipo de Identificación e Identificación
   LEFT OUTER JOIN Country_IdentificationType coiT ON coiT.id = FCI.id_Country_IdentificationType
   LEFT OUTER JOIN IdentificationType iden ON iden.id = coiT.id_identificationType
    --INNER JOIN   ForeignCustomerIdentification FCI    
    --ON FCI.id_ForeignCustomer = [Person].ID --------------------------EV    
        where [Invoice].[id] = convert(int,@id)   
		order by Item.masterCode  

/*
	EXEC spPar_FactExteriorBL 985731
*/

GO