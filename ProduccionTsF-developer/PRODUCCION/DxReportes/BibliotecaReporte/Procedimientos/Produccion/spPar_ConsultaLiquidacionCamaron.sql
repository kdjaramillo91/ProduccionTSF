GO
/****** Object:  StoredProcedure [dbo].[parc_Liquidacion_Compra_Proveedores_Lista]    Script Date: 21/02/2023 11:48:08 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spPar_ConsultaLiquidacionCamaron]
@id_provider int = 0,
@str_liquidationDateStart varchar(10) = '',
@str_liquidationDateEnd varchar(10) = ''

as
set nocount on 

set @id_provider = isnull(@id_provider,0)
set @str_liquidationDateStart = convert(date,isnull(@str_liquidationDateStart,'1900-01-01'))
set @str_liquidationDateEnd = convert(date,isnull(@str_liquidationDateEnd,'1900-01-01'),111)

select 
	@str_liquidationDateStart AS Inicio,
	@str_liquidationDateEnd as Fin,
	convert(integer,ROW_NUMBER() OVER(ORDER BY pl.[id])) AS Id
	, pl.[id] as [IdLoteProduccion]
	, comp.[businessName] as [NombreCompania]
	, comp.[ruc] as [RucCompania]
	, comp.[logo] as [LogoCompania]
	, comp.[phoneNumber] as [NumeroCompania]
	, comp.[email] as [MailCompania]
	, comp.[address] as [DireccionCompania]
	, prov.[fullname_businessName] as [NombreProveedor]
	, pl.[liquidationDate] as [FechaLiquidacion]
	, pl.[number] as [SecuenciaTransaccional]
	, pl.[internalNumber] as [NumeroLoteInterno]
	, [LibrasRecibidas] = (select sum([quantitydrained]) 
							from [dbo].[ProductionLotDetail] pld 
							where pld.[id_productionLot] = pl.[id])
	, prty.[name] as [NombreProceso]
	, dost.[name] as [EstadoDocumento]
	, pl.[totalToPay] as [TotalPagar]
	, prop.processPlant as [personProcesPlant],
	comp.logo as Logo,
	comp.logo2 as Logo2
from  [dbo].[ProductionLot] pl
inner join [dbo].[Document] doc on doc.[id] = pl.[id]
inner join [dbo].[DocumentState] dost on doc.[id_documentState] = dost.[id]
inner join [dbo].[ProductionLotState] pls on pl.[id_ProductionLotState] = pls.[id]
and pls.[code] in ('07','08')
inner join [dbo].[ProcessType] prty on prty.[id] = pl.[id_processtype]
left join [dbo].[PriceList] pril on pril.[id] = pl.[id_priceList]
inner join [dbo].[Person] prov on prov.[id] = pl.[id_provider]
inner join [dbo].[Person] prop on prop.[id] = pl.[id_personProcessPlant]
inner join [dbo].[ProductionUnitProviderPool] pupp on pupp.[id] = pl.[id_productionUnitProviderPool]
inner join [dbo].[EmissionPoint] emis on emis.[id] = doc.[id_emissionPoint]
inner join [dbo].[Company] comp	on comp.[id] = emis.[id_company]
where 1 = 1
and pl.[id_provider] = case when @id_provider = 0 then pl.[id_provider] else @id_provider end
and convert(date,pl.[liquidationDate]) >= case when year(@str_liquidationDateStart) = 1900 then convert(date, pl.[liquidationDate]) else @str_liquidationDateStart end
and convert(date, pl.[liquidationDate]) <= case when year(@str_liquidationDateEnd) = 1900 then convert(date,pl.[liquidationDate]) else @str_liquidationDateEnd end
order by pl.[id]

