If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'pac_Guia_Remision_Resultado'
	)
Begin
	Drop Procedure dbo.pac_Guia_Remision_Resultado
End
Go
Create Procedure dbo.pac_Guia_Remision_Resultado
(
	@P_xml xml
)
As
set nocount on
SET QUOTED_IDENTIFIER ON

-- Declaro Variables
declare @id_rgType char(2)
declare @id_docState integer
declare @str_numberDoc varchar(200)
declare @str_referenceDoc varchar(200)
declare @str_startEmissionDate varchar(10) 
declare @str_endEmissionDate varchar(10) 
declare @str_startAuthDate varchar(10) 
declare @str_endAuthDate varchar(10)
declare @str_accesKey varchar(200)
declare @str_authNumber  varchar(200)
declare @str_startDespDate  varchar(10)
declare @str_endDespDate  varchar(10)
declare @str_startExitPlanctDate  varchar(10)
declare @str_endExitPlanctDate  varchar(10)
declare @str_startEntrancePlanctDate  varchar(10)
declare @str_endEntrancePlanctDate  varchar(10)
declare @str_carRegistration varchar(200) 
-- Obtengo Atributos del XML
select 
@id_rgType = nref.value('@id_rgType','char(2)'),
@id_docState = nref.value('@id_docState','integer'),
@str_numberDoc = nref.value('@str_numberDoc','varchar(200)'),
@str_referenceDoc = nref.value('@str_referenceDoc','varchar(200)'),
@str_startEmissionDate = nref.value('@str_startEmissionDate','varchar(10)'),
@str_endEmissionDate = nref.value('@str_endEmissionDate','varchar(10)'),
@str_startAuthDate = nref.value('@str_startAuthDate','varchar(10)'),
@str_endAuthDate = nref.value('@str_endAuthDate','varchar(10)'),
@str_accesKey = nref.value('@str_accesKey','varchar(200)'),
@str_authNumber = nref.value('@str_authNumber','varchar(200)'),
@str_startDespDate = nref.value('@str_startDespDate','varchar(10)'),
@str_endDespDate = nref.value('@str_endDespDate','varchar(10)'),
@str_startExitPlanctDate = nref.value('@str_startExitPlanctDate','varchar(10)'),
@str_endExitPlanctDate = nref.value('@str_endExitPlanctDate','varchar(10)'),
@str_startEntrancePlanctDate = nref.value('@str_startEntrancePlanctDate','varchar(10)'),
@str_endEntrancePlanctDate = nref.value('@str_endEntrancePlanctDate','varchar(10)'),
@str_carRegistration = nref.value('@str_carRegistration','varchar(200)')
from @P_xml.nodes('/RGQ') AS R(nref)

set @id_rgType = isnull(@id_rgType,'')
set @id_docState = isnull(@id_docState,0)
set @str_numberDoc = isnull(rtrim(ltrim(@str_numberDoc)),'') 
set @str_referenceDoc = isnull(rtrim(ltrim(@str_referenceDoc)),'')
set @str_startEmissionDate = isnull(rtrim(ltrim(@str_startEmissionDate)),'') 
set @str_endEmissionDate = isnull(rtrim(ltrim(@str_endEmissionDate)),'')
set @str_startAuthDate = isnull(rtrim(ltrim(@str_startAuthDate)),'')
set @str_endAuthDate = isnull(rtrim(ltrim(@str_endAuthDate)),'')
set @str_accesKey = isnull(rtrim(ltrim(@str_accesKey)),'')
set @str_authNumber = isnull(rtrim(ltrim(@str_authNumber)),'')
set @str_startDespDate = isnull(rtrim(ltrim(@str_startDespDate)),'')
set @str_endDespDate = isnull(rtrim(ltrim(@str_endDespDate)),'')
set @str_startExitPlanctDate = isnull(rtrim(ltrim(@str_startExitPlanctDate)),'')
set @str_endExitPlanctDate = isnull(rtrim(ltrim(@str_endExitPlanctDate)),'')
set @str_startEntrancePlanctDate = isnull(rtrim(ltrim(@str_startEntrancePlanctDate)),'')
set @str_endEntrancePlanctDate = isnull(rtrim(ltrim(@str_endEntrancePlanctDate)),'')
set @str_carRegistration = isnull(rtrim(ltrim(@str_carRegistration)),'')

set @str_startEmissionDate = convert(date,isnull(@str_startEmissionDate,''))
set @str_endEmissionDate = convert(date,isnull(@str_endEmissionDate,''))
set @str_startAuthDate = convert(date,isnull(@str_startAuthDate,''))
set @str_endAuthDate = convert(date,isnull(@str_endAuthDate,''))
set @str_startDespDate = convert(date,isnull(@str_startDespDate,''))
set @str_endDespDate = convert(date,isnull(@str_endDespDate,''))
set @str_startExitPlanctDate = convert(date,isnull(@str_startExitPlanctDate,''))
set @str_endExitPlanctDate = convert(date,isnull(@str_endExitPlanctDate,''))
set @str_startEntrancePlanctDate = convert(date,isnull(@str_startEntrancePlanctDate,''))
set @str_endEntrancePlanctDate = convert(date,isnull(@str_endEntrancePlanctDate,''))

declare @tmpDocumentoElectronico table
(
	[IdDocumentoElectronico] int not null,
	[EstadoElectronico]		 varchar(200)
)

declare @tmpDocumentoOrdenCompra table
(
	[IdGuiaRemision]		int not null,
	[NumeroOrdenCompra]		 varchar(200)
)

insert into @tmpDocumentoElectronico
select 
	a.[id]
	, b.[name]
from [dbo].[ElectronicDocument] a 
join [dbo].[ElectronicDocumentState] b on a.[id_electronicDocumentState] = b.[id]


insert into @tmpDocumentoOrdenCompra
select 
rg1.[id]
, doc1.[number] 
from [dbo].[RemissionGuideDetailPurchaseOrderDetail] rgdpod
inner join [dbo].[RemissionGuideDetail] rgd on rgd.[id] = rgdpod.[id_remissionGuideDetail]
inner join [dbo].[RemissionGuide] rg1 on rgd.[id_remisionGuide] = rg1.[id]
inner join [dbo].[PurchaseOrderDetail] pod on pod.[id] = rgdpod.[id_purchaseOrderDetail]
inner join [dbo].[PurchaseOrder] po1 on pod.[id_purchaseOrder] = po1.[id]
inner join [dbo].[Document] doc1 on po1.[id] = doc1.[id]

select
	rg.[id] as [id]
	, doc.[number] as [NumeroDocumento]
	, [NumeroOrdenCompra] = (select Top 1 rgdpod.[NumeroOrdenCompra] 
							from @tmpDocumentoOrdenCompra rgdpod
							where rgdpod.[IdGuiaRemision] = rg.[id] 
							)
	, doc.[emissionDate] as [FechaEmision]
	, prov.[fullname_businessName] as [NombreProveedor]
	, pup.[name] as [NombreUnidadProd]
	, cer.[name] as [NombreCertificado]
	, rg.[despachureDate] as [FechaDespacho]
	, convert(datetime,rgcv.[exitDateProductionBuilding] )+convert(datetime, rgcv.[exitTimeProductionBuilding]) as [SalidaPlanta]
	, convert(datetime,rgcv.[entranceDateProductionBuilding] )+convert(datetime, rgcv.[entranceTimeProductionBuilding]) as [EntradaPlanta]
	, rgt.isOwn as [LogisticaPropia]
	, docs.[name] as [EstadoDocumento]
	, isnull(ed.[EstadoElectronico],'SIN ESTADO') as [EstadoElectronico]
	, prop.processPlant as [personProcesPlant]
from [dbo].[RemissionGuide] rg
inner join [dbo].[Document] doc on rg.[id] = doc.[id]
inner join [dbo].[DocumentType] doct on doc.[id_documentType] = doct.[id]
inner join [dbo].[DocumentState] docs on doc.[id_documentState] = docs.[id]
inner join [dbo].[RemissionGuideType] rgty on rg.[id_RemissionGuideType] = rgty.[id]
inner join [dbo].[Person] prov on prov.[id] = rg.[id_providerRemisionGuide]
inner join [dbo].[Person] prop on prop.[id] = rg.[id_personProcessPlant]
inner join [dbo].[ProductionUnitProvider] pup on pup.[id] = rg.[id_productionUnitProvider]
left join [dbo].[Certification] cer on cer.[id] = rg.[id_certification]
inner join [dbo].[RemissionGuideTransportation] rgt on rg.[id] = rgt.[id_remionGuide]
left outer join [dbo].[RemissionGuideControlVehicle] rgcv on rg.[id] = rgcv.[id_remissionGuide]
left outer join @tmpDocumentoElectronico ed on ed.[IdDocumentoElectronico] = doc.[id]

where 1 = 1
and rtrim(ltrim(rgty.[code])) = case when @id_rgType = '' then rtrim(ltrim(rgty.[code])) else @id_rgType end
and doc.[id_documentState] = case when @id_docState = 0 then doc.[id_documentState] else @id_docState end
and doc.[number] like case when @str_numberDoc <> '' then  '%' + @str_numberDoc + '%' else doc.[number] end
and isnull(doc.[reference],'') like case when @str_referenceDoc <> '' then  '%' + @str_referenceDoc + '%' else isnull(doc.[reference],'') end
and convert(date,doc.[emissionDate]) >= case when year(@str_startEmissionDate) = 1900 then convert(date, doc.[emissionDate]) else @str_startEmissionDate end
and convert(date,doc.[emissionDate]) <= case when year(@str_endEmissionDate) = 1900 then convert(date, doc.[emissionDate]) else @str_endEmissionDate end
and isnull(rgt.[carRegistration],'') like case when @str_carRegistration <> '' then  '%' + @str_carRegistration + '%' else isnull(rgt.[carRegistration],'') end

/*
[pac_Guia_Remision_Resultado]'<Root>
  <RGQ id_rgType = "TA" id_docState="0" str_numberDoc="" str_referenceDoc=" " str_startEmissionDate="" str_endEmissionDate="" 
  str_startAuthDate="" str_endAuthDate="" str_accesKey="" str_authNumber="" str_startDespDate="" 
  str_endDespDate="" str_startExitPlanctDate="" str_endExitPlanctDate="" str_startEntrancePlanctDate="" str_endEntrancePlanctDate="" />
</Root>'

[pac_Guia_Remision_Resultado]'<Root>
  <RGQ id_rgType="TA" id_docState="0" str_numberDoc="" str_referenceDoc="" str_startEmissionDate="" 
  str_endEmissionDate="" str_startAuthDate="" str_endAuthDate="" str_accesKey="" 
  str_authNumber="" str_startDespDate="" str_endDespDate="" str_startExitPlanctDate="" 
  str_endExitPlanctDate="" str_startEntrancePlanctDate="" str_endEntrancePlanctDate="" 
  str_carRegistration =""/>
</Root>'


*/


Go
