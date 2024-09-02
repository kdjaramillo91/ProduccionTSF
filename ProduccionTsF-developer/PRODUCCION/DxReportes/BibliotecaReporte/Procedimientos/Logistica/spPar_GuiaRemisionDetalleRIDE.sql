SET NOCOUNT ON
GO

CREATE OR ALTER PROCEDURE [dbo].[spPar_GuiaRemisionDetalleRIDE]
 @id_RemissionGuide INT  
  
AS   

SELECT c.name Producto, b.sourceExitQuantity Cantidad,c.masterCode Codigo,c.auxCode Auxiliar
FROM RemissionGuide a
INNER JOIN RemissionGuideDispatchMaterial b ON b.id_remisionGuide = a.id
LEFT JOIN item c ON c.id = b.id_item
WHERE  a.id = @id_RemissionGuide 
ORDER BY c.masterCode