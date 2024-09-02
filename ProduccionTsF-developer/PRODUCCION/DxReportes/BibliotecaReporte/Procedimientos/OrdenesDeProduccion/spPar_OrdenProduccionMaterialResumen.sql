SET NOCOUNT ON 
GO

CREATE OR ALTER PROCEDURE [dbo].[spPar_OrdenProduccionMaterialResumen] 
	@id INT
AS
SELECT i.masterCode,i.name,
(CASE WHEN sod.id_item = sod.id_item THEN SUM(sod.quantity) END) AS Cantidad,
(CASE WHEN sod.id_item = sod.id_item THEN SUM(sod.quantityRequiredForFormulation) END) AS CantidadFormulada
,mu.code as MU
from SalesOrderMPMaterialDetail sod
inner join item i on sod.id_item=i.id
inner join MetricUnit mu on sod.id_metricUnit=mu.id
where sod.id_salesOrder=@id
group by i.masterCode,i.name,mu.code,sod.id_item
