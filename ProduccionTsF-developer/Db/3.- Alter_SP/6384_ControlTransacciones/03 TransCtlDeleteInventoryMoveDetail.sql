USE [panaceaProExpoNew0306]
GO
/****** Object:  StoredProcedure [dbo].[TransCtlDeleteInventoryMoveDetail]    Script Date: 6/6/2024 4:56:51 PM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[TransCtlDeleteInventoryMoveDetail]
(@inventoryMoveDetailIds NVARCHAR(MAX) )
As
Begin
	
	--CREATE TABLE #TempTable (Id INT);
	--
	--INSERT INTO #TempTable (Id)
	--SELECT value from STRING_SPLIT ( @inventoryMoveDetailIds, '|' )


	DELETE T
    FROM InventoryMoveDetail T
    INNER JOIN (SELECT value as Id from STRING_SPLIT ( @inventoryMoveDetailIds, '|' )) TT
    ON T.Id = TT.Id;

	
	--DELETE T
    --FROM InventoryMoveDetail T
    --INNER JOIN #TempTable TT
    --ON T.Id = TT.Id;

End;