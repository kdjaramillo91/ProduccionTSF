IF OBJECT_ID('prod_Consult_ProductionLotFinalize') IS NULL
EXEC('CREATE PROCEDURE prod_Consult_ProductionLotFinalize AS')

GO
ALTER   Procedure [dbo].[prod_Consult_ProductionLotFinalize]

As
BEGIN

DECLARE @Init AS INT, @End AS INT
DECLARE @Id_Lot AS INT

DECLARE @tblLotClose AS TABLE(
		Id INT,
        IdLote INT,
        TipoLote NVARCHAR(15),
        SecuenciaTransaccional NVARCHAR(20),  
        NumeroLote NVARCHAR(15),
        LoteJuliano NVARCHAR(10),
        FechaProceso DATETIME,
        UnidadProduccion NVARCHAR(50),
        Proceso NVARCHAR(100),
        Estado NVARCHAR(100),
		Stock DECIMAL(18,6)
)

DECLARE @tblLotStock AS TABLE (
IdLot INT,
Stock DECIMAL(12,4)
)

INSERT INTO @tblLotClose
SELECT 
ROW_NUMBER() OVER(ORDER BY L.ID),
L.id, 'Producción' TipoLote, L.number SecuenciaTransaccional,  L.internalNumber NumeroLote,
CASE 
        WHEN internalNumber IS NOT NULL AND LEN(internalNumber) > 5 
        THEN SUBSTRING(internalNumber, 1, 5) 
        ELSE '' 
    END AS LoteJuliano,
 L.receptionDate FechaProceso,
 U.name UnidadProduccion,
 P.processPlant Proceso,
 PL.name Estado,0
 FROM ProductionLot L
INNER JOIN ProductionUnit U WITH(NOLOCK)  ON L.id_productionUnit = U.id 
INNER JOIN Person P WITH(NOLOCK) ON L.id_personProcessPlant = P.id
INNER JOIN ProductionLotState PL WITH(NOLOCK) ON L.id_ProductionLotState = PL.id 
AND PL.code NOT IN ('09', '12', '13', '100')
AND l.isClosed = 0


SET @Init = 1;
SELECT @End = COUNT(*) FROM @tblLotClose

WHILE @Init <= @End BEGIN


	SELECT @Id_Lot = IdLote FROM @tblLotClose WHERE Id = @Init

	INSERT INTO @tblLotStock
		exec inv_Consulta_Saldo_Mes_Control_General 2,null, null,'true', 'GITLOT', null,null,@Id_Lot,'true',null


		UPDATE T SET T.Stock = S.Stock 
		FROM @tblLotClose T
		INNER JOIN @tblLotStock S ON T.IdLote = S.IdLot
		WHERE T.Id = @Init

		DELETE FROM @tblLotStock

		SET @Init += 1
		PRINT @Init
END

SELECT IdLote, TipoLote,SecuenciaTransaccional,NumeroLote,LoteJuliano, FechaProceso,UnidadProduccion,
Proceso,Stock,Estado
FROM @tblLotClose

END




