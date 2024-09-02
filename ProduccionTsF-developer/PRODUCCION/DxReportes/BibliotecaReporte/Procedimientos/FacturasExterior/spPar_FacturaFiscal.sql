-- exec "par_Factura_Fiscal_Lista";1 '', '', '', ''
GO
/****** Object:  StoredProcedure [dbo].[par_Factura_Fiscal_Lista]    Script Date: 28/12/2022 13:11:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[spPar_FacturaFiscal] 
(
@str_startEmissionDate varchar(10),
@str_endEmissionDate varchar(10),
@str_startDateShipment varchar(10),
@str_endDateShipment varchar(10)
)
As
set nocount on 

set @str_startEmissionDate = convert(date,isnull(@str_startEmissionDate,'1900-01-01'))
set @str_endEmissionDate = convert(date,isnull(@str_endEmissionDate,'2200-01-01'))
set @str_startDateShipment = convert(date,isnull(@str_startDateShipment,'1900-01-01'))
set @str_endDateShipment = convert(date,isnull(@str_endDateShipment,'2200-01-01'))

select 
	invo.[id] as [id]
	, convert(varchar(50),doc.[number]) as [NumeroFactura]
	, comp.[fullname_businessName] as [RazonSocialComprador]
	, comp.[identification_number] as [RucCedulaComprador]
	, inext.[daeNumber4] as [NumeroDae]
	, doc.[emissionDate] as [FechaEmision]
	, convert(datetime,inext.[dateShipment],103) as [FechaEmbarque]
	, Case When docs.[code] <> '05' Then invde.[totalPriceWithoutTax] Else 0 End as [ValorFob]
    , invde.[id_metricUnitInvoiceDetail] as [idPesoDetalle]
    , Case When docs.[code] <> '05' Then case  invde.[id_metricUnit] when 1 then  invde.[id_amountInvoice] end Else 0 End as [PesoKilos]
	, Case When docs.[code] <> '05' Then case  invde.[id_metricUnit] when 4 then  invde.[id_amountInvoice] end Else 0 End as [PesoLibras]
	, Case When docs.[code] <> '05' Then case  invde.[id_metricUnit] when 1 then  invde.[id_amountInvoice]*2.2046 else invde.[id_amountInvoice] end Else 0 End as [TotalLibras]
	, inext.[blNumber] as [NumeroBL]
	, inext.[totalBoxes] as [TotalCajasCM]
	, convert(varchar(10),ep.[code]) as  [CodigoPuntoEmision]
	, bo.[code] as  [CodigoEstablecimiento]
	, invde.[numBoxes] as [NumeroCartones]
	, doc.[sequential] as  [SecuencialDocumento]
	, doc.[authorizationDate] as [FechaAutorizacion]
	, inext.[valueTotalFOB] as [ValorTotalFob]
	, inext.[id_metricUnitInvoice] as [idUnidadMedidaFactura]
	, meun.[code] as [CodigoUnidadMedida]
	, ep.[name] as [NombrePuntoEmision]
	, co.[businessName] as [NombreCia]
	, co.[address] as [DireccionMatrizCia]
	, co.[ruc] as [RucCia]
	, co.[logo] as [Logo2Cia]
	, co.[trademark] as [NombreComercialCia]
	, co.phoneNumber as [Telefono]
	, envty.[description] as [AmbienteDesc]
	, emty.[description] as [PuntoEmisionDesc]
	, inext.[valuetotalCIF] as [ValorTotalCif]
	, docs.[code] as [EstadoDocumento]
	, invo.[subTotal] as [SubTotal]
	, invo.[totalValue] as [ValorTotal]
	, invde.[total] as [PrecioTotalDetalle]
from [dbo].[Invoice] invo
inner join [dbo].[Document] doc on invo.[id] = doc.[id]
inner join [dbo].[DocumentState] docs on doc.[id_documentState] = docs.[id]
inner join [dbo].[InvoiceDetail] invde on invde.[id_invoice] = invo.[id]
inner join [dbo].[InvoiceExterior] inext on inext.[id] = invo.[id]
inner join [dbo].[MetricUnit] meun on meun.[id] = invde.[id_metricUnitInvoiceDetail]
inner join [dbo].[Person] comp on comp.[id]= invo.[id_buyer]
inner join [dbo].[ForeignCustomer] clext on clext.[id] = invo.[id_buyer]
inner join [dbo].[EmissionPoint] ep on ep.[id] = doc.[id_emissionPoint]
inner join [dbo].[BranchOffice] bo on bo.[id] = ep.[id_branchOffice]
inner join [dbo].[Division] div on div.[id] = bo.[id_division]
inner join [dbo].[Company] co on co.[id] = div.[id_company]
inner join [dbo].[CompanyElectronicFacturation] coef on co.[id] = coef.[id_company]
inner join [dbo].[EnvironmentType] envty on envty.[id] = coef.[id_enviromentType]
inner join [dbo].[EmissionType] emty on emty.[id] = coef.[id_emissionType]
where 1 = 1
and docs.[code] <> '05'
and convert(date,doc.[emissionDate]) >= case when year(@str_startEmissionDate) = 1900 then convert(date, doc.[emissionDate]) else @str_startEmissionDate end
and convert(date,doc.[emissionDate]) <= case when year(@str_endEmissionDate) = 1900 then convert(date, doc.[emissionDate]) else @str_endEmissionDate end
and convert(date,inext.[dateShipment]) >= case when year(@str_startDateShipment) = 1900 then convert(date, inext.[dateShipment]) else @str_startDateShipment end
and convert(date,inext.[dateShipment]) <= case when year(@str_endDateShipment) = 1900 then convert(date, inext.[dateShipment]) else @str_endDateShipment end
order by  invo.[id] desc

--[Invoice].[id] = convert(int,@id)
-- execute par_Factura_Fiscal_Lista null,null,null,null
---select * from InvoiceExterior





 