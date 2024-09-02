GO
/****** Object:  StoredProcedure [dbo].[par_Factura_Comercial_Lista]    Script Date: 16/01/2023 22:23:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE procedure  [dbo].[spPar_FacturaComercial]
	  @idsInvoicesList varchar(255)
	as

	Declare @idsInvoices table 	(id int);
	Declare @boHavingIds bit;



	begin 

	set nocount on 

	insert into  @idsInvoices
	select id from dbo.SplitInts(  @idsInvoicesList , ','); 


	  select @boHavingIds = ( case  
								when  countIds > 0 then 1
								else 0 end 
							)
						  from 
						  (
							select count(*) countIds from @idsInvoices
						  ) boId;

	

 
	 select [id]
			,[NumeroFactura]
			,[RazonSocialComprador]
			,[RucCedulaComprador]
			,[NumeroDae]
			,[FechaEmision]
			,[FechaEmbarque]		
 			,[CodigoPuntoEmision]
			,[CodigoEstablecimiento]		
			,[SecuencialDocumento]
			,[FechaAutorizacion]
			,[ValorFob]
			,[NombrePuntoEmision]
			,[NombreCia]
			,[DireccionMatrizCia]
			,[RucCia]
			,[Logo2Cia]
			,[NombreComercialCia]
			,[Telefono]
			,[AmbienteDesc]
			,[PuntoEmisionDesc]
			,[EstadoDocumento]	
			,[ValorTotal],		
		case PortfolioFinancingCode
		when 'PFCAP' then ValorFob 
		else 0 
		end as 'IG CAPITAL', 
		case PortfolioFinancingCode
		when 'PFLET' then ValorFob 
		else 0
		end as 'CARTA DE CRÉDITO',
		case PortfolioFinancingCode
		when 'PFBB' then ValorFob 
		else 0
		end as 'BANCO BOLIVARIANO',
		case PortfolioFinancingCode
		when 'PFCOB' then ValorFob 
		else 0
		end as 'COBRANZAS BANCARIAS',
		case PortfolioFinancingCode
		when 'PFNAP' then ValorFob 
		else 0
		end as 'NO APROBADAS',
		case PortfolioFinancingCode
		when 'PFPP' then ValorFob 
		else 0
		end as 'PRONTO PAGO',
		valorKilos,
		valorLibras,
		totalLibras			
	 from 
	 (
			select 
				invo.[id] as [id]
				, isnull(referenceInvoice,'---') as [NumeroFactura]
				, comp.[fullname_businessName] as [RazonSocialComprador]
				, comp.[identification_number] as [RucCedulaComprador]
				, invo.[daeNumber4] as [NumeroDae]
				, doc.[emissionDate] as [FechaEmision]
				, convert(datetime,invo.[dateShipment],103) as [FechaEmbarque]			
 				, convert(varchar(10),ep.[code]) as  [CodigoPuntoEmision]
				, bo.[code] as  [CodigoEstablecimiento]
				, doc.[sequential] as  [SecuencialDocumento]
				, doc.[authorizationDate] as [FechaAutorizacion]
				, (((invo.[totalValue]) - valueTotalFreight )+ valueDiscount ) as [ValorFob]
				, ep.[name] as [NombrePuntoEmision]
				, co.[businessName] as [NombreCia]
				, co.[address] as [DireccionMatrizCia]
				, co.[ruc] as [RucCia]
				, co.[logo] as [Logo2Cia]
				, co.[trademark] as [NombreComercialCia]
				, co.phoneNumber as [Telefono]
				, envty.[description] as [AmbienteDesc]
				, emty.[description] as [PuntoEmisionDesc]
				, docs.[code] as [EstadoDocumento]	
				, invo.[totalValue] as [ValorTotal],			
				PortfolioFinancing.code as 	 PortfolioFinancingCode,
				invde.valorKilos,
				invde.valorLibras,
				invde.totalLibras

		from [dbo].[InvoiceCommercial] invo	
		inner join [dbo].[Document] doc on invo.[id] = doc.[id]
		inner join [dbo].[DocumentState] docs on doc.[id_documentState] = docs.[id]
		inner join 
		(
		  select	id_invoiceCommercial,
					sum(valorKilos) as valorKilos,
					sum(valorLibras) as valorLibras,
					sum(totalLibras) as totalLibras
		  from 
		  (
			select    id_invoiceCommercial,
					  case  MetricUnit.code
					  when  'Kg' then amount
					  when  'Lbs' then 0
					  when  'Gr' then 0
					  end as  valorKilos,
					  case  MetricUnit.code
					  when  'Kg' then 0
					  when  'Lbs' then amount
					  when  'Gr' then 0
					  end as  valorLibras,
					  case  MetricUnit.code
					  when  'Kg' then  ( amount *  icn.conversionToPounds )
					  when  'Lbs' then amount
					  when  'Gr' then  ( amount  * 0.00220462)
					  end as  totalLibras
			from	  [dbo].[InvoiceCommercialDetail]  invde0
					  inner join ItemWeightConversionFreezen icn on 
					  icn.id_Item = invde0.id_item
					  inner join MetricUnit on 
					  MetricUnit.id  = icn.id_MetricUnit 
			where	  invde0.isActive = 1		  				  
			) invde1
			group by  id_invoiceCommercial	
		) as invde	on  invde.[id_invoiceCommercial] = invo.[id]
		left join [dbo].[Person] comp on comp.[id]= invo.id_ForeignCustomer
		left join [dbo].[ForeignCustomer] clext on clext.[id] = invo.id_ForeignCustomer
		inner join [dbo].[EmissionPoint] ep on ep.[id] = doc.[id_emissionPoint]
		inner join [dbo].[BranchOffice] bo on bo.[id] = ep.[id_branchOffice]
		inner join [dbo].[Division] div on div.[id] = bo.[id_division]
		inner join [dbo].[Company] co on co.[id] = div.[id_company]
		inner join [dbo].[CompanyElectronicFacturation] coef on co.[id] = coef.[id_company]
		inner join [dbo].[EnvironmentType] envty on envty.[id] = coef.[id_enviromentType]
		inner join [dbo].[EmissionType] emty on emty.[id] = coef.[id_emissionType]
		left join tbsysCatalogueDetail PortfolioFinancing on PortfolioFinancing.id =  invo.idPortfolioFinancing
	  
	)
	as a
	where (
			(
				(@boHavingIds = 1 ) 
				and exists
				(
		 			select 'x' from @idsInvoices b
					where b.id = a.id
				)
		   )
		   or
			(@boHavingIds = 0 ) 
		   )
	order by [FechaEmision] desc;

	end;