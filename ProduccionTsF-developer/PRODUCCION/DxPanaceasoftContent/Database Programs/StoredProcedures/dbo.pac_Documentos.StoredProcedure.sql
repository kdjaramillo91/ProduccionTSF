If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'pac_Documentos'
	)
Begin
	Drop Procedure dbo.pac_Documentos
End
Go
Create Procedure dbo.pac_Documentos
(
	@id_documentType integer,
	@str_emissionDateStart varchar(10),
	@str_emissionDateEnd	varchar(10)
)
As
set nocount on 

set @id_documentType = isnull(@id_documentType,0)
set @str_emissionDateStart = convert(date,isnull(@str_emissionDateStart,'1900-01-01'))
set @str_emissionDateEnd = convert(date,isnull(@str_emissionDateEnd,'1900-01-01'),111)

----------------Variables Auxiliares--------------------------
declare @cad					varchar(8000)
declare @valueParamFUP1			bit
declare @valueParamOGFR1		bit
declare @tmpGuiasSumarizadas	table
(
	[IdGuiaRemision]	INT NOT NULL,
	[CantidadProgramada]	DECIMAL(20,6)
)
declare @tmpRecepcionSumarizadas table
(
	[IdRecepcion]	INT NOT NULL,
	[CantidadRecibida]	DECIMAL(20,6)
)
declare @tmpOrdenesCompras table
(
	[id]	int NOT NULL,
	[numeroDocumento]	varchar(100) collate database_default not null,
	[FechaEmision] datetime not null,
	[codeTipoDocumento]	varchar(20) not null,
	[NombreTipoDocumento] varchar(100) not null,
	[NombreProveedor]	varchar(250) not null,
	[NombreEstado] varchar(100) not null,
	[bEstado]	bit not null
)
declare @tmpGuiasRemision table
(
	[id]	int NOT NULL,
	[numeroDocumento]	varchar(100) collate database_default not null,
	[FechaEmision] datetime not null,
	[codeTipoDocumento]	varchar(20) not null,
	[NombreTipoDocumento] varchar(100) not null,
	[NombreProveedor]	varchar(250) not null,
	[NombreEstado] varchar(100) not null,
	[bEstado]	bit not null
)
declare @tmpGuiasRemisionFiltro1 table
(
	[id]	int NOT NULL,
	[numeroDocumento]	varchar(100) collate database_default not null,
	[FechaEmision] datetime not null,
	[codeTipoDocumento]	varchar(20) not null,
	[NombreTipoDocumento] varchar(100) not null,
	[NombreProveedor]	varchar(250) not null,
	[NombreEstado] varchar(100) not null,
	[bEstado]	bit not null
)
declare @tmpGuiasRemisionPendientes table
(
	[id]	int not null
)
--------------------------------------------------------------
----------------obtengo lista de parametros-------------------
select @valueParamFUP1 = isnull([valueBit],0)
from [dbo].[tbsysOpenCloseDocumentParam]
where [code] = 'FUP1'

--Parametro para Guías de Remisión
select @valueParamOGFR1 = isnull([valueBit],0)
from [dbo].[tbsysOpenCloseDocumentParam]
where [code] = 'OGFR1'
--------------------------------------------------------------

set @cad = ''
set @cad = @cad + 'select' + char(13)
set @cad = @cad + '	doc.[id] as [id]' + char(13)
set @cad = @cad + '	, doc.[number] as [numeroDocumento]' + char(13)
set @cad = @cad + '	, doc.[emissionDate] as [FechaEmision]' + char(13)
set @cad = @cad + '	, doct.[code] as [codeTipoDocumento]' + char(13)
set @cad = @cad + '	, doct.[name]	as [NombreTipoDocumento]' + char(13)
set @cad = @cad + ' , pe.[fullname_businessName] as [NombreProveedor]' + char(13)
set @cad = @cad + '	, docs.[name]	as [NombreEstado]' + char(13)
set @cad = @cad + '	, convert(bit,isnull(doc.[isOpen],0)) as [bEstado]' + char(13)
set @cad = @cad + 'from [dbo].[Document] doc' + char(13)
set @cad = @cad + 'inner join [dbo].[DocumentState] docs on doc.[id_documentState] = docs.[id]' + char(13)
set @cad = @cad + 'inner join [dbo].[DocumentType] doct on doc.[id_documentType] = doct.[id]' + char(13)
set @cad = @cad + '	and doct.[code] in (''02'')' + char(13)
set @cad = @cad + 'inner join [dbo].[PurchaseOrder] po on doc.[id] = po.[id]' + char(13)
set @cad = @cad + 'inner join [dbo].[Person] pe on pe.[id] = po.[id_provider]' + char(13)
set @cad = @cad + 'where 1 = 1 ' + char(13)

if @valueParamFUP1 = 1 
	set @cad = @cad + 'and doc.[id_userCreate] <> 1' + char(13)
if @id_documentType <> 0 
	set @cad = @cad + 'and doc.[id_documentType] = ' + rtrim(ltrim(@id_documentType)) + char(13)
if year(convert(date,@str_emissionDateStart)) <> 1900
	set @cad = @cad + 'and convert(date,doc.[emissionDate]) >= ''' + @str_emissionDateStart + '''' + char(13)
if year(convert(date,@str_emissionDateEnd)) <> 1900
	set @cad = @cad + 'and convert(date, doc.[emissionDate]) <= ''' + @str_emissionDateEnd + '''' + char(13)

insert into @tmpOrdenesCompras
exec (@cad)


set @cad = ''
set @cad = @cad + 'select' + char(13)
set @cad = @cad + '	doc.[id] as [id]' + char(13)
set @cad = @cad + '	, doc.[number] as [numeroDocumento]' + char(13)
set @cad = @cad + '	, doc.[emissionDate] as [FechaEmision]' + char(13)
set @cad = @cad + '	, doct.[code] as [codeTipoDocumento]' + char(13)
set @cad = @cad + '	, doct.[name]	as [NombreTipoDocumento]' + char(13)
set @cad = @cad + ' , pe.[fullname_businessName] as [NombreProveedor]' + char(13)
set @cad = @cad + '	, docs.[name]	as [NombreEstado]' + char(13)
set @cad = @cad + '	, convert(bit,isnull(doc.[isOpen],0)) as [bEstado]' + char(13)
set @cad = @cad + 'from [dbo].[Document] doc' + char(13)
set @cad = @cad + 'inner join [dbo].[DocumentState] docs on doc.[id_documentState] = docs.[id]' + char(13)
set @cad = @cad + 'inner join [dbo].[DocumentType] doct on doc.[id_documentType] = doct.[id]' + char(13)
set @cad = @cad + '	and doct.[code] in (''08'')' + char(13)
set @cad = @cad + 'inner join [dbo].[RemissionGuide] rg on doc.[id] = rg.[id]' + char(13)
set @cad = @cad + 'inner join [dbo].[Person] pe on pe.[id] = rg.[id_providerRemisionGuide]' + char(13)
set @cad = @cad + 'where 1 = 1 ' + char(13)

if @valueParamFUP1 = 1 
	set @cad = @cad + 'and doc.[id_userCreate] <> 1' + char(13)
if @id_documentType <> 0 
	set @cad = @cad + 'and doc.[id_documentType] = ' + rtrim(ltrim(@id_documentType)) + char(13)
if year(convert(date,@str_emissionDateStart)) <> 1900
	set @cad = @cad + 'and convert(date,doc.[emissionDate]) >= ''' + @str_emissionDateStart + '''' + char(13)
if year(convert(date,@str_emissionDateEnd)) <> 1900
	set @cad = @cad + 'and convert(date, doc.[emissionDate]) <= ''' + @str_emissionDateEnd + '''' + char(13)

insert into @tmpGuiasRemision
exec(@cad)

--FILTRO 1
if(@valueParamOGFR1 = 1)
begin
	insert into @tmpGuiasSumarizadas
	select rgd.[id_remisionGuide] as IdGuiaRemision, sum(rgd.[quantityProgrammed]) as CantidadProgramada
	from [dbo].[RemissionGuideDetail] rgd
	group by rgd.[id_remisionGuide]

	insert into @tmpRecepcionSumarizadas
	select pld.[id_productionLot] as IdRecepcion, sum(pld.[quantityRecived]) as CantidadRecibida
	from [dbo].[ProductionLotDetail] pld
	group by pld.[id_productionLot]

	insert into @tmpGuiasRemisionPendientes
	select distinct rg.[id]
	from [dbo].[RemissionGuide] rg
	inner join [dbo].[Document] doc on rg.[id] = doc.[id]
	inner join [dbo].[DocumentState] docs on doc.[id_documentState] = docs.[id]
	and docs.[code] = '06'
	where exists(select *
				from [dbo].[ProductionLotDetailPurchaseDetail] pldpd
				inner join [dbo].[RemissionGuideDetail] rgd on rgd.[id] = pldpd.[id_remissionGuideDetail]
				inner join [dbo].[ProductionLotDetail] pld on pld.[id] = pldpd.[id_purchaseOrderDetail]
				inner join @tmpGuiasSumarizadas tgs on rgd.[id_remisionGuide] = tgs.[IdGuiaRemision]
				inner join @tmpRecepcionSumarizadas trs on pld.[id_productionLot] = trs.[IdRecepcion]
				where tgs.[IdGuiaRemision] = rg.[id]
				and tgs.[CantidadProgramada] > trs.CantidadRecibida)

	insert into @tmpGuiasRemisionFiltro1
	select * 
	from @tmpGuiasRemision

	delete from @tmpGuiasRemision

	insert into @tmpGuiasRemision
	select tgrf.*
	from @tmpGuiasRemisionFiltro1 tgrf 
	inner join @tmpGuiasRemisionPendientes tgrp on tgrf.[id] = tgrp.[id]
end

select * 
from @tmpOrdenesCompras
union all
select * 
from @tmpGuiasRemision 




 

--[dbo].[pac_Documentos] 0,'',''
Go
