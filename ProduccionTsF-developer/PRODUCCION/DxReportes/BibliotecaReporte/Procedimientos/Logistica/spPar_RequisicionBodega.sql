/****** Object:  StoredProcedure [dbo].[par_GuideRemisionDispatchMaterialCR]    Script Date: 01/02/2023 12:34:07 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROC [dbo].[spPar_RequisicionBodega]
@id int=0,
@id_warehouse int  as

declare @secuencial VARCHAR(20)

select top 1 @secuencial = CONVERT(VARCHAR(20),sequential)
from [dbo].[DispatchMaterialSequential]
where id_RemissionGuide = @id and id_Warehouse = @id_warehouse

select 
	RG.id  as RG_id ,
	id_reciver as RG_id_reciver,
	id_reason as RG_id_reason 
	,  route as route,
	startAdress as RG_startAdress ,
	emissionDate as RG_despachureDate ,
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
	id_RemisionGuideReassignment as RG_id_RemisionGuideReassignment,
	rg.id_shippingType as RG_id_shippingType,
	hasEntrancePlanctProduction as RG_hasEntrancePlanctProduction,
	hasExitPlanctProduction as  RG_hasExitPlanctProduction, despachurehour as RG_despachurehour,
	id_TransportTariffType as RG_id_TransportTariffType , id_tbsysCatalogState as RG_id_tbsysCatalogState ,
	Person.fullname_businessName as Nameproveedor,
	Person.fullname_businessName as Namerecibidor,
	Person.identification_number as CIproveedor,
	RGT.driverName as conductorm,
	Person2.identification_number as CIconductor,
	Person2.address as adressconductor,
	vh.carRegistration as placa,
	vh.mark as  marca,
	vh.model as  modelo,
	RGT.descriptionTrans as observatrans,
	cantidad = (select SUM(quantityprogrammed) from RemissionGuideDetail RGD where RGD.id_remisionguide = @id),
	logo = Company.logo2,
	logo2 = Company.logo,
	Person3.fullname_businessName ciaTransporte,
	documento = CONVERT(VARCHAR(20),Document.sequential),
	case when isnull(item.[auxCode],'') <> '' then item.[auxCode] else item.masterCode end as codeitem,
	item.[name] as nameitem,
	case when item.[masterCode] in ('MI00093', 'MI00094') then 'Kg' else MU.code end as unidades,
	case when item.[masterCode] in ('MI00094') then (RGDM.sourceExitQuantity * 50)
	when item.[masterCode] = 'MI00093' then (RGDM.sourceExitQuantity * 25)
	else RGDM.sourceExitQuantity end as cantidad_1,
	zonasitioprovedor = fz.name + ', '+FS.name,
	sello1 = (select top 1 number from RemissionGuideSecuritySeal where RemissionGuideSecuritySeal.id_remissionGuide = RG.id  and RemissionGuideSecuritySeal.id_travelType = 1),
	sello2 = (select top 1 number from RemissionGuideSecuritySeal where RemissionGuideSecuritySeal.id_remissionGuide = RG.id  and RemissionGuideSecuritySeal.id_travelType = 3),
	WH.name as bodega,
	WH.name+'/'+WL.name as bodegaubicacion,
	PUP.address as direccionprovee,
	WH.id as codebodega
	, NumeroRequisicion =@secuencial
from RemissionGuide RG
inner join Person Person on Person.id =RG.id_providerRemisionGuide
inner join RemissionGuideDispatchMaterial RGDM on RGDM.id_remisionGuide = RG.id
inner join Item as item on item.id = RGDM.id_item
inner join MetricUnit MU on MU.id = item.id_metricType
inner join Person Person1 on Person1.id =RG.id_reciver 
inner join RemissionGuideTransportation RGT on RGT.id_remionGuide = RG.id
left join   DriverVeicleProviderTransport DVPT on RGT.id_remionGuide = DVPT.idVeicleProviderTransport
left join Person Person3 on Person3.id = DVPT.id_driver
left join vehicle VH on RGT.carRegistration = VH.carRegistration
left join Person Person2 on Person2.id =RGT.id_driver
inner join "dbo"."Document" "Document" on ("Document"."id" = RG.id)
left outer join "dbo"."EmissionPoint" "EmissionPoint" on ("EmissionPoint"."id" = "Document"."id_emissionPoint")
left outer join "dbo"."Company" "Company" on ("Company"."id" = "EmissionPoint"."id_company")
inner join ProductionUnitProvider PUP on PUP.id = RG.id_productionunitprovider
left join fishingsite FS on FS.id = PUP.id_FishingSite
left join fishingzone FZ on FZ.id = PUP.id_FishingZone
inner join Warehouse WH on WH.id = RGDM.id_warehouse and WH.id = RGDM.id_warehouse 
inner join WarehouseLocation WL on WL.id_warehouse = RGDM.id_warehouse
and WL.id = RGDM.id_warehouselocation
	    
where RG.id = @id and RGDM.id_warehouse = @id_warehouse --materiale e insumos 1 Materia Prima
and ISNULL(item.notShowInReport,0)=0

	 /*  execute par_GuideRemisionDispatchMaterialCR 138534,7
	   */





