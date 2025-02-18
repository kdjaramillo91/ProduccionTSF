If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_GuideRemisionViaticoCustomizeCR'
	)
Begin
	Drop Procedure dbo.par_GuideRemisionViaticoCustomizeCR
End
Go
Create Procedure dbo.par_GuideRemisionViaticoCustomizeCR
(
@id int
)
As
select 
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
Person.fullname_businessName as proveedor,
Person.fullname_businessName as Namerecibidor,
RGT.driverName as conductorm,
Person2.identification_number as CIconductor,
vh.carRegistration as placa,
vh.mark as  marca,
vh.model as  modelo,
RGT.descriptionTrans as transporte,
-- VH.description as transporte,
RGT.advancePrice as valor,
dbo.FUN_CantidadConLetracastellano(convert(INTEGER,RGT.advancePrice))+ ' ' + SUBSTRING(RTRIM(LTRIM(PARSENAME(RGT.advancePrice,1))),1,2) +   '/100 DOLAR' as  letras,
cantidad = (select SUM(quantityprogrammed) from RemissionGuideDetail RGD where RGD.id_remisionguide = RG.id),
'CHOFER' as rol,
'IDA Y VUELTA' as viaje
--,logo = Company.logo2
,Person3.fullname_businessName ciaTransporte,
person3.identification_number as ciciatransporte,
--VH.description ciaTransporte,
convert(varchar(20),Document.sequential) as guiademovilizacion ,
FS.description as sitio,
document.emissionDate as fecha , PUP.address as destino ,
Document.description as observacion
, convert(varchar(20),drgc.sequential) as NumeroAnticipo
from RemissionGuide RG
left outer join [dbo].[RemissionGuideCustomizedAdvancedTransportist] rgcat on rg.id = rgcat.id_remissionguide
left outer join [dbo].[Document] drgc on drgc.id = rgcat.id_AdvancedTransportist
inner join Person Person 
on Person.id =RG.id_providerRemisionGuide
inner join Person Person1 
on Person1.id =RG.id_reciver 
inner join   RemissionGuideTransportation RGT
on RGT.id_remionGuide = RG.id
inner join FishingSite FS
on FS.id = RGT.id_FishingSiteRG
left join   VeicleProviderTransport DVPT
on RGT.id_vehicle = DVPT.id_vehicle
and RGT.id_provider = DVPT.id_provider
AND DVPT.datefin IS NULL
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
	   --and RGAS.id_assignedStaffRol = 3 --126325

	 /*  execute par_GuideRemisionViaticoCustomizeCR 138760 
	 
 */
Go
