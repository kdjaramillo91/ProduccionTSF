If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_detailpooladvanprovider'
	)
Begin
	Drop Procedure dbo.par_detailpooladvanprovider
End
Go
Create Procedure dbo.par_detailpooladvanprovider
(
@id int
)
As
set nocount on 

select 
	AP.id
	, PUPP.name as piscina
	, PLD.quantityRecived as cantidaddesp
	, PLD.quantitydrained as cantidadrecibida
	,QC.grammageReference as gramage 
	,guia = dorg.number 
from ProductionLot PL
inner join ProductionLotDetail PLD on PL.id = PLD.id_productionLot
inner join ProductionLotDetailPurchaseDetail PLDPD on PLD.id = PLDPD.id_productionLotDetail
inner join 	RemissionGuideDetail RGD on PLDPD.id_remissionGuideDetail = RGD.id
inner join   RemissionGuide RG on RGD.id_remisionGuide = RG.id
inner join QualityControl QC on QC.id_lot = PLd.id_productionLot and qc.id_lot = pl.id
inner join Document do on QC.id = do.id AND do.id_documentState != 5
inner join productionlotdetailqualitycontrol pldqc on pldqc.id_productionLotDetail = pld.id and pldqc.id_qualityControl = qc.id 
left join ProductionUnitProviderPool PUPP on PUPP.id = PL.id_productionUnitProviderPool
inner join Document dorg on dorg.id = rg.id
INNER JOIN DocumentState DORGS ON DORG.id_documentState = DORGS.id AND DORGS.code NOT IN ('05','08','01')
inner join AdvanceProvider AP on AP.id_Lot = PL.id
where  AP.id = @id
group by AP.id, PUPP.[name], PLD.[quantityRecived], PLD.quantitydrained,QC.[grammageReference],dorg.[number], dorg.sequential
order by dorg.sequential desc

--execute par_detailpooladvanprovider 141146

--select * from RemissionGuide
Go