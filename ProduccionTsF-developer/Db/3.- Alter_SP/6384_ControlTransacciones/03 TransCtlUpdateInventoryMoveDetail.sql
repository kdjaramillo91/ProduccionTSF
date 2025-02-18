[dbo].[TransCtlUpdateInventoryMoveDetail]
'[{"id":0,"id_lot":1510479,"id_item":27722,"id_inventoryMove":1511930,"entryAmount":0.0,"entryAmountCost":0.0,"exitAmount":30.000000,"exitAmountCost":0.000000,"id_metricUnit":3,"id_warehouse":21,"id_warehouseLocation":6393,"id_warehouseEntry":26,"id_inventoryMoveDetailExit":null,"inMaximumUnit":false,"id_userCreate":1,"dateCreate":"2024-06-18T17:11:31.7016908-05:00","id_userUpdate":1,"dateUpdate":"2024-06-18T17:11:31.7016908-05:00","id_inventoryMoveDetailPrevious":null,"id_inventoryMoveDetailNext":null,"unitPrice":0.0,"balance":-30.000000,"averagePrice":0.0,"balanceCost":0.000000,"id_metricUnitMove":3,"unitPriceMove":0.0,"amountMove":30.000000,"id_costCenter":18,"id_subCostCenter":173,"natureSequential":null,"genSecTrans":false,"id_warehouseLocationEntry":6424,"id_costCenterEntry":2,"id_subCostCenterEntry":43,"id_productionCart":3,"ordenProduccion":null,"productoCost":0.0,"lastestProductoCost":0.0,"id_CostAllocationDetail":null,"id_lastestCostAllocationDetail":null,"lotMarked":null,"id_personProcessPlant":4995},{"id":0,"id_lot":1510479,"id_item":29045,"id_inventoryMove":1511930,"entryAmount":0.0,"entryAmountCost":0.0,"exitAmount":1.000000,"exitAmountCost":0.000000,"id_metricUnit":3,"id_warehouse":21,"id_warehouseLocation":6393,"id_warehouseEntry":26,"id_inventoryMoveDetailExit":null,"inMaximumUnit":false,"id_userCreate":1,"dateCreate":"2024-06-18T17:11:31.7056923-05:00","id_userUpdate":1,"dateUpdate":"2024-06-18T17:11:31.7056923-05:00","id_inventoryMoveDetailPrevious":null,"id_inventoryMoveDetailNext":null,"unitPrice":0.0,"balance":-1.000000,"averagePrice":0.0,"balanceCost":0.000000,"id_metricUnitMove":3,"unitPriceMove":0.0,"amountMove":1.000000,"id_costCenter":18,"id_subCostCenter":173,"natureSequential":null,"genSecTrans":false,"id_warehouseLocationEntry":6424,"id_costCenterEntry":2,"id_subCostCenterEntry":43,"id_productionCart":162,"ordenProduccion":null,"productoCost":0.0,"lastestProductoCost":0.0,"id_CostAllocationDetail":null,"id_lastestCostAllocationDetail":null,"lotMarked":null,"id_personProcessPlant":4995}]',
0


alter procedure [dbo].[TransCtlUpdateInventoryMoveDetail]
(@inventoryMoveDetails NVARCHAR(MAX),
 @modeInsert			 bit
)
As

DECLARE @SI BIT =1;
DECLARE @NO BIT =0;

Begin


--Select
--		id_lot
--		,id_item
--		,id_inventoryMove
--		,entryAmount
--		,entryAmountCost
--		,exitAmount
--		,exitAmountCost
--		,id_metricUnit
--		,id_warehouse
--		,id_warehouseLocation
--		,id_warehouseEntry
--		,id_inventoryMoveDetailExit
--		,inMaximumUnit
--		,id_userCreate
--		--,dateCreate
--		,id_userUpdate
--		--,dateUpdate
--		,id_inventoryMoveDetailPrevious
--		,id_inventoryMoveDetailNext
--		,unitPrice
--		,balance
--		,averagePrice
--		,balanceCost
--		,id_metricUnitMove
--		,unitPriceMove
--		,amountMove
--		,id_costCenter
--		,id_subCostCenter
--		,natureSequential
--		,genSecTrans
--		,id_warehouseLocationEntry
--		,id_costCenterEntry
--		,id_subCostCenterEntry
--		,id_productionCart
--		,ordenProduccion
--		,productoCost
--		,lastestProductoCost
--		,id_CostAllocationDetail
--		,id_lastestCostAllocationDetail
--		,lotMarked
--		,id_personProcessPlant
--		FROM OPENJSON(@inventoryMoveDetails)
--		WITH
--		(
--			id_lot				int					'$.id_lot'
--			,id_item			int					'$.id_item'
--			,id_inventoryMove	int					'$.id_inventoryMove'
--			,entryAmount		decimal	(14,6)		'$.entryAmount'    
--			,entryAmountCost	decimal	(14,6)		'$.entryAmountCost'        
--			,exitAmount			decimal	(14,6)		'$.exitAmount'        
--			,exitAmountCost		decimal	(20,6)	    '$.exitAmountCost'        
--			,id_metricUnit		int					'$.id_metricUnit'        
--			,id_warehouse		int					'$.id_warehouse'        
--			,id_warehouseLocation	int				'$.id_warehouseLocation' 
--			,id_warehouseEntry		int				'$.id_warehouseEntry' 
--			,id_inventoryMoveDetailExit	int		'$.id_inventoryMoveDetailExit' 
--			,inMaximumUnit				bit		'$.inMaximumUnit' 
--			,id_userCreate				int		'$.id_userCreate' 
--			--,dateCreate					datetime	'$.dateCreate' 
--			,id_userUpdate				int			'$.id_userUpdate' 
--			--,dateUpdate					datetime	'$.dateUpdate'    
--			,id_inventoryMoveDetailPrevious	int		'$.id_inventoryMoveDetailPrevious'    
--			,id_inventoryMoveDetailNext		int		'$.id_inventoryMoveDetailNext'    
--			,unitPrice					decimal(14,6 )   '$.unitPrice'    
--			,balance					decimal	 (14,6)     '$.balance'    
--			,averagePrice				decimal	(14,6)    '$.averagePrice'    
--			,balanceCost				decimal(20,6)     '$.balanceCost'    
--			,id_metricUnitMove			int					'$.id_metricUnitMove'    
--			,unitPriceMove				decimal(14,6)    	'$.unitPriceMove'    
--			,amountMove					decimal	(14,6)    	'$.amountMove'    
--			,id_costCenter				int					'$.id_costCenter'    
--			,id_subCostCenter			int					'$.id_subCostCenter'    
--			,natureSequential			varchar(50)			'$.natureSequential'    
--			,genSecTrans				bit					'$.genSecTrans'    
--			,id_warehouseLocationEntry	int					'$.id_warehouseLocationEntry'    
--			,id_costCenterEntry			int					'$.id_costCenterEntry'    
--			,id_subCostCenterEntry		int					'$.id_subCostCenterEntry'    
--			,id_productionCart			int					'$.id_productionCart'    
--			,ordenProduccion			varchar(50)	     	'$.ordenProduccion'    		
--			,productoCost				decimal(20,6)		'$.productoCost'    		
--			,lastestProductoCost		decimal	(20,6)		'$.lastestProductoCost'    		
--			,id_CostAllocationDetail	int					'$.id_CostAllocationDetail'    		
--			,id_lastestCostAllocationDetail	int				'$.id_lastestCostAllocationDetail'    		
--			,lotMarked					varchar(20)	     	'$.lotMarked'    				
--			,id_personProcessPlant		int					'$.id_personProcessPlant'    				
--		) AS jsonValues

--		return;





CREATE TABLE #InsertedIds (
    inventoryMoveDetailId INT
);


IF(@modeInsert = @SI)
BEGIN
	INSERT INTO  dbo.InventoryMoveDetail
	(	
		id_lot
		,id_item
		,id_inventoryMove
		,entryAmount
		,entryAmountCost
		,exitAmount
		,exitAmountCost
		,id_metricUnit
		,id_warehouse
		,id_warehouseLocation
		,id_warehouseEntry
		,id_inventoryMoveDetailExit
		,inMaximumUnit
		,id_userCreate
		,dateCreate
		,id_userUpdate
		,dateUpdate
		,id_inventoryMoveDetailPrevious
		,id_inventoryMoveDetailNext
		,unitPrice
		,balance
		,averagePrice
		,balanceCost
		,id_metricUnitMove
		,unitPriceMove
		,amountMove
		,id_costCenter
		,id_subCostCenter
		,natureSequential
		,genSecTrans
		,id_warehouseLocationEntry
		,id_costCenterEntry
		,id_subCostCenterEntry
		,id_productionCart
		,ordenProduccion
		,productoCost
		,lastestProductoCost
		,id_CostAllocationDetail
		,id_lastestCostAllocationDetail
		,lotMarked
		,id_personProcessPlant
	)
	OUTPUT inserted.id INTO #InsertedIds
	
	Select
		id_lot
		,id_item
		,id_inventoryMove
		,entryAmount
		,entryAmountCost
		,exitAmount
		,exitAmountCost
		,id_metricUnit
		,id_warehouse
		,id_warehouseLocation
		,id_warehouseEntry
		,id_inventoryMoveDetailExit
		,inMaximumUnit
		,id_userCreate
		,cast(substring(replace(dateCreate,'T',' '),1,19 )   as datetime ) as dateCreate		
		,id_userUpdate
		,cast(substring(replace(dateUpdate,'T',' '),1,19 )   as datetime ) as dateCreate
		,id_inventoryMoveDetailPrevious
		,id_inventoryMoveDetailNext
		,unitPrice
		,balance
		,averagePrice
		,balanceCost
		,id_metricUnitMove
		,unitPriceMove
		,amountMove
		,id_costCenter
		,id_subCostCenter
		,natureSequential
		,genSecTrans
		,id_warehouseLocationEntry
		,id_costCenterEntry
		,id_subCostCenterEntry
		,id_productionCart
		,ordenProduccion
		,productoCost
		,lastestProductoCost
		,id_CostAllocationDetail
		,id_lastestCostAllocationDetail
		,lotMarked
		,id_personProcessPlant
		FROM OPENJSON(@inventoryMoveDetails)
		WITH
		(
			id_lot				int					'$.id_lot'
			,id_item			int					'$.id_item'
			,id_inventoryMove	int					'$.id_inventoryMove'
			,entryAmount		decimal	(14,6)		'$.entryAmount'    
			,entryAmountCost	decimal	(14,6)		'$.entryAmountCost'        
			,exitAmount			decimal	(14,6)		'$.exitAmount'        
			,exitAmountCost		decimal	(20,6)	    '$.exitAmountCost'        
			,id_metricUnit		int					'$.id_metricUnit'        
			,id_warehouse		int					'$.id_warehouse'        
			,id_warehouseLocation	int				'$.id_warehouseLocation' 
			,id_warehouseEntry		int				'$.id_warehouseEntry' 
			,id_inventoryMoveDetailExit	int		'$.id_inventoryMoveDetailExit' 
			,inMaximumUnit				bit		'$.inMaximumUnit' 
			,id_userCreate				int		'$.id_userCreate' 
			,dateCreate					varchar(50)	'$.dateCreate' 
			,id_userUpdate				int			'$.id_userUpdate' 
			,dateUpdate					varchar(50)	'$.dateUpdate'    
			,id_inventoryMoveDetailPrevious	int		'$.id_inventoryMoveDetailPrevious'    
			,id_inventoryMoveDetailNext		int		'$.id_inventoryMoveDetailNext'    
			,unitPrice					decimal(14,6 )   '$.unitPrice'    
			,balance					decimal	 (14,6)     '$.balance'    
			,averagePrice				decimal	(14,6)    '$.averagePrice'    
			,balanceCost				decimal(20,6)     '$.balanceCost'    
			,id_metricUnitMove			int					'$.id_metricUnitMove'    
			,unitPriceMove				decimal(14,6)    	'$.unitPriceMove'    
			,amountMove					decimal	(14,6)    	'$.amountMove'    
			,id_costCenter				int					'$.id_costCenter'    
			,id_subCostCenter			int					'$.id_subCostCenter'    
			,natureSequential			varchar(50)			'$.natureSequential'    
			,genSecTrans				bit					'$.genSecTrans'    
			,id_warehouseLocationEntry	int					'$.id_warehouseLocationEntry'    
			,id_costCenterEntry			int					'$.id_costCenterEntry'    
			,id_subCostCenterEntry		int					'$.id_subCostCenterEntry'    
			,id_productionCart			int					'$.id_productionCart'    
			,ordenProduccion			varchar(50)	     	'$.ordenProduccion'    		
			,productoCost				decimal(20,6)		'$.productoCost'    		
			,lastestProductoCost		decimal	(20,6)		'$.lastestProductoCost'    		
			,id_CostAllocationDetail	int					'$.id_CostAllocationDetail'    		
			,id_lastestCostAllocationDetail	int				'$.id_lastestCostAllocationDetail'    		
			,lotMarked					varchar(20)	     	'$.lotMarked'    				
			,id_personProcessPlant		int					'$.id_personProcessPlant'    				
		) AS jsonValues

END
ELSE
BEGIN
	UPDATE	dbo.InventoryMoveDetail
	SET id_lot								=b.id_lot									  
		,entryAmount						=b.entryAmount							  
		,entryAmountCost					=b.entryAmountCost						  
		,exitAmount							=b.exitAmount								  
		,exitAmountCost						=b.exitAmountCost							  
		,id_metricUnit						=b.id_metricUnit							  
		,id_warehouseLocation				=b.id_warehouseLocation					  
		,id_warehouseEntry					=b.id_warehouseEntry						  
		,id_inventoryMoveDetailExit			=b.id_inventoryMoveDetailExit				  
		,inMaximumUnit						=b.inMaximumUnit							  
		,id_userCreate						=b.id_userCreate							  
		,dateCreate							=b.dateCreate								  
		,id_userUpdate						=b.id_userUpdate							  
		,dateUpdate							=b.dateUpdate								  
		,id_inventoryMoveDetailPrevious		=b.id_inventoryMoveDetailPrevious			  
		,id_inventoryMoveDetailNext			=b.id_inventoryMoveDetailNext				  
		,unitPrice							=b.unitPrice								  
		,balance							=b.balance								  
		,averagePrice						=b.averagePrice							  
		,balanceCost						=b.balanceCost							  
		,id_metricUnitMove					=b.id_metricUnitMove						  
		,unitPriceMove						=b.unitPriceMove							  
		,amountMove							=b.amountMove								  
		,id_costCenter						=b.id_costCenter							  
		,id_subCostCenter					=b.id_subCostCenter						  
		,natureSequential					=b.natureSequential						  
		,genSecTrans						=b.genSecTrans							  
		,id_warehouseLocationEntry			=b.id_warehouseLocationEntry				  
		,id_costCenterEntry					=b.id_costCenterEntry						  
		,id_subCostCenterEntry				=b.id_subCostCenterEntry					  
		,id_productionCart					=b.id_productionCart						  
		,ordenProduccion					=b.ordenProduccion						  
		,productoCost						=b.productoCost							  
		,lastestProductoCost				=b.lastestProductoCost					  
		,id_CostAllocationDetail			=b.id_CostAllocationDetail				  
		,id_lastestCostAllocationDetail		=b.id_lastestCostAllocationDetail			  
		,lotMarked							=b.lotMarked								  
		,id_personProcessPlant				=b.id_personProcessPlant
	FROM dbo.InventoryMoveDetail a
	INNER JOIN 
	(
			Select  id
		,id_lot
		,id_item
		,id_inventoryMove
		,entryAmount
		,entryAmountCost
		,exitAmount
		,exitAmountCost
		,id_metricUnit
		,id_warehouse
		,id_warehouseLocation
		,id_warehouseEntry
		,id_inventoryMoveDetailExit
		,inMaximumUnit
		,id_userCreate
		,cast(substring(replace(dateCreate,'T',' '),1,19 )   as datetime ) as dateCreate
		,id_userUpdate
		,cast(substring(replace(dateUpdate,'T',' '),1,19 )   as datetime ) as dateUpdate
		,id_inventoryMoveDetailPrevious
		,id_inventoryMoveDetailNext
		,unitPrice
		,balance
		,averagePrice
		,balanceCost
		,id_metricUnitMove
		,unitPriceMove
		,amountMove
		,id_costCenter
		,id_subCostCenter
		,natureSequential
		,genSecTrans
		,id_warehouseLocationEntry
		,id_costCenterEntry
		,id_subCostCenterEntry
		,id_productionCart
		,ordenProduccion
		,productoCost
		,lastestProductoCost
		,id_CostAllocationDetail
		,id_lastestCostAllocationDetail
		,lotMarked
		,id_personProcessPlant
		FROM OPENJSON(@inventoryMoveDetails)
		WITH
		(
			id					int					'$.id'
			,id_lot				int					'$.id_lot'
			,id_item			int					'$.id_item'
			,id_inventoryMove	int					'$.id_inventoryMove'
			,entryAmount		decimal	(14,6)		'$.entryAmount'    
			,entryAmountCost	decimal	(14,6)		'$.entryAmountCost'        
			,exitAmount			decimal	(14,6)		'$.exitAmount'        
			,exitAmountCost		decimal	(20,6)	    '$.exitAmountCost'        
			,id_metricUnit		int					'$.id_metricUnit'        
			,id_warehouse		int					'$.id_warehouse'        
			,id_warehouseLocation	int				'$.id_warehouseLocation' 
			,id_warehouseEntry		int				'$.id_warehouseEntry' 
			,id_inventoryMoveDetailExit	int		'$.id_inventoryMoveDetailExit' 
			,inMaximumUnit				bit		'$.inMaximumUnit' 
			,id_userCreate				int		'$.id_userCreate' 
			,dateCreate					varchar(50)	'$.dateCreate' 
			,id_userUpdate				int			'$.id_userUpdate' 
			,dateUpdate					varchar(50)	'$.dateUpdate'    
			,id_inventoryMoveDetailPrevious	int		'$.id_inventoryMoveDetailPrevious'    
			,id_inventoryMoveDetailNext		int		'$.id_inventoryMoveDetailNext'    
			,unitPrice					decimal(14,6 )   '$.unitPrice'    
			,balance					decimal	 (14,6)     '$.balance'    
			,averagePrice				decimal	(14,6)    '$.averagePrice'    
			,balanceCost				decimal(20,6)     '$.balanceCost'    
			,id_metricUnitMove			int					'$.id_metricUnitMove'    
			,unitPriceMove				decimal(14,6)    	'$.unitPriceMove'    
			,amountMove					decimal	(14,6)    	'$.amountMove'    
			,id_costCenter				int					'$.id_costCenter'    
			,id_subCostCenter			int					'$.id_subCostCenter'    
			,natureSequential			varchar(50)			'$.natureSequential'    
			,genSecTrans				bit					'$.genSecTrans'    
			,id_warehouseLocationEntry	int					'$.id_warehouseLocationEntry'    
			,id_costCenterEntry			int					'$.id_costCenterEntry'    
			,id_subCostCenterEntry		int					'$.id_subCostCenterEntry'    
			,id_productionCart			int					'$.id_productionCart'    
			,ordenProduccion			varchar(50)	     	'$.ordenProduccion'    		
			,productoCost				decimal(20,6)		'$.productoCost'    		
			,lastestProductoCost		decimal	(20,6)		'$.lastestProductoCost'    		
			,id_CostAllocationDetail	int					'$.id_CostAllocationDetail'    		
			,id_lastestCostAllocationDetail	int				'$.id_lastestCostAllocationDetail'    		
			,lotMarked					varchar(20)	     	'$.lotMarked'    				
			,id_personProcessPlant		int					'$.id_personProcessPlant'    				
		) AS jsonValues
	
	)   b on a.id = b.id

END


SELECT inventoryMoveDetailId FROM  #InsertedIds;

DROP TABLE #InsertedIds;
 
End;