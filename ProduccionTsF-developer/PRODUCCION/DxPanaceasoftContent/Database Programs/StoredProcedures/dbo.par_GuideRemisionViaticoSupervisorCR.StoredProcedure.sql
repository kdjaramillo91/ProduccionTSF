If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_GuideRemisionViaticoSupervisorCR'
	)
Begin
	Drop Procedure dbo.par_GuideRemisionViaticoSupervisorCR
End
Go
Create Procedure dbo.par_GuideRemisionViaticoSupervisorCR
(
@id int 
)
As
set nocount on
declare @viatico decimal

select  @viatico =   SUM(viaticPrice) from RemissionGuideAssignedStaff rgas where id_remissionGuide = @id and rgas.isActive = 1
select distinct 
RG.id  as RG_id ,
id_reciver as RG_id_reciver,
id_reason as RG_id_reason 
,  route as route,
startAdress as RG_startAdress ,
despachureDate as RG_despachureDate ,
arrivalDate as RG_arrivalDate,    
returnDate as RG_returnDate,
uniqueCustomDocument as RG_uniqueCustomDocument,
isExport as RG_isExport,
id_providerRemisionGuide as RG_id_providerRemisionGuide,
id_priceList as RG_id_priceList,
id_buyer as RG_id_buyer ,
id_protectiveProvider as RG_id_protectiveProvider,
id_productionUnitProvider as RG_id_productionUnitProvider ,
id_productionUnitProviderPool as id_productionUnitProviderPool,
descriptionpurchaseorder as RG_descriptionpurchaseorder ,isInternal as RG_isInternal,
id_RemisionGuideReassignment as RG_id_RemisionGuideReassignment, RG.id_shippingType as RG_id_shippingType, hasEntrancePlanctProduction as RG_hasEntrancePlanctProduction,
hasExitPlanctProduction as  RG_hasExitPlanctProduction, despachurehour as RG_despachurehour,
id_TransportTariffType as RG_id_TransportTariffType , id_tbsysCatalogState as RG_id_tbsysCatalogState ,
Person.fullname_businessName as Nameproveedor,
Person.fullname_businessName as Namerecibidor,
RGT.driverName as conductorm,
Person2.identification_number asCIconductor,
vh.carRegistration as placa,
vh.mark as  marca,
vh.model as  modelo,
RGT.descriptionTrans as transporte,
RGT.advancePrice as valor,
dbo.FUN_CantidadConLetracastellano(convert(INTEGER,@viatico))+ ' ' + SUBSTRING(RTRIM(LTRIM(PARSENAME(@viatico,1))),1,2) +   '/100 DOLARES' as  letras,
cantidad = (select SUM(quantityprogrammed) from RemissionGuideDetail RGD where RGD.id_remisionguide = RG.id),
Person3.fullname_businessName ciaTransporte,
person3.identification_number as ciciatransporte,
documento = CONVERT(VARCHAR(20),Document.sequential),
FS.description as sitio,
document.emissionDate,RGAS.viaticPrice as viatico,
person4.fullname_businessName as persona,
person4.identification_number as cipersona,
rgasr.description as rol,rgtt.name as viaje,@viatico as total,
PUP.address as destino
, CONVERT(VARCHAR(20), DRGC.[sequential]) as NumeroViatico
from RemissionGuide RG
left outer join [dbo].[RemissionGuideCustomizedViaticPersonalAssigned] rgcvpa on rgcvpa.[id_remissionguide] = rg.[id]
left outer join [dbo].[Document] drgc on rgcvpa.[id_ViaticPersonalAssigned] = drgc.[id]
inner join Person Person 
on Person.id =RG.id_providerRemisionGuide
inner join RemissionGuideAssignedStaff RGAS
on RGAS.id_remissionGuide = rg.id
inner join Person person4
on person4.id = RGAS.id_person
inner join RemissionGuideAssignedStaffRol rgasr
on rgasr.id = RGAS.id_assignedStaffRol
inner join RemissionGuideTravelType rgtt
on rgtt.id = RGAS.id_travelType
inner join Person Person1 
on Person1.id =RG.id_reciver 
inner join   RemissionGuideTransportation RGT
on RGT.id_remionGuide = RG.id
inner join FishingSite FS
on FS.id = RGT.id_FishingSiteRG
left join   VeicleProviderTransport DVPT
on RGT.id_vehicle = DVPT.id_vehicle
and RGT.id_provider = DVPT.id_provider
left join Person Person3 
on Person3.id = DVPT.id_Provider
left join vehicle VH
on RGT.id_vehicle = VH.id
left join Person Person2 
on Person2.id =RGT.id_driver
inner join ProductionUnitProvider PUP
on PUP.id = RG.id_productionunitprovider
inner join dbo.Document Document
on (Document.id = RG.id)
left outer join dbo.EmissionPoint EmissionPoint
on (EmissionPoint.id = Document.id_emissionPoint)
left outer join dbo.Company Company
on (Company.id = EmissionPoint.id_company)
where RG.id = @id 
and rgas.isActive = 1 
and isnull(rgas.viaticPrice ,0) >0
--order by sequential desc
	   --execute par_GuideRemisionViaticoSupervisorCR 141681
Go
