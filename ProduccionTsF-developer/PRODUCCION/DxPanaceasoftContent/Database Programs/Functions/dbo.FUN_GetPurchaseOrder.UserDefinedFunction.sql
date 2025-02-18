If Exists(
	Select	*
	From	sys.objects
	Where	name = 'FUN_GetPurchaseOrder'
	And		type = 'FN'
	)
Begin
	Drop Function dbo.FUN_GetPurchaseOrder
End
Go
Create Function dbo.FUN_GetPurchaseOrder
(
@id Integer
)
Returns Varchar(1000)
As
BEGIN

DECLARE @strConcatenated VARCHAR(1000)
SELECT @strConcatenated= COALESCE(@strConcatenated + ', ', '') +  COALESCE(@strConcatenated + ', ', '') +DOCUMENT.number  
 from ProductionLot
INNER JOIN ProductionLotDetail
	ON ProductionLotDetail.id_productionLot = ProductionLot.id
INNER JOIN  ProductionLotDetailPurchaseDetail
	ON  ProductionLotDetailPurchaseDetail.id_productionLotDetail = ProductionLotDetail.ID
INNER JOIN PurchaseOrderDetail
	ON PurchaseOrderDetail.id = ProductionLotDetailPurchaseDetail.id_remissionGuideDetail
INNER JOIN PurchaseOrder
	ON PurchaseOrder.id = PurchaseOrderDetail.id_purchaseOrder  
INNER JOIN Document
	ON Document.id = PurchaseOrder.id
	WHERE ProductionLot.id = @id

RETURN RTRIM(LTRIM(@strConcatenated))

END

Go
