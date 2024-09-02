
--Este es el resultado que recibe de Panacea
--El campo idDrainingTest es nulleable y se actualiza cuando forma parte de un Documento Prueba de Escurrido
--El resto de campos está en idioma inglés si se le dificulta puede buscar un diccionario
SET DATEFORMAT ymd
SET NOCOUNT ON
insert into [dbo].[ResultProdLotReceptionDetail] ([idProductionLotReceptionDetail],[idDrainingTest],[idRemissionGuide],[numberRemissionGuide],[dateArrived],[poundsRemitted],[drawersNumber],[numberLot],[numberLotSequential],[namePool],[nameProvider],[INPnumber],[temperature],[idWarehouse],[nameWarehouse],[idWarehouseLocation],[nameWarehouseLocation],[idItem],[nameItem]) values (72,NULL,152637,'42537','2018-11-17 05:08:14',15000,300,'158469-1','REC00026','PI3','PROVEEDOR3','INP3',85,14,'BODEGA 14',16,'UBICACIÓN 16',1452,'CAMARÓN ENTERO')
insert into [dbo].[ResultProdLotReceptionDetail] ([idProductionLotReceptionDetail],[idDrainingTest],[idRemissionGuide],[numberRemissionGuide],[dateArrived],[poundsRemitted],[drawersNumber],[numberLot],[numberLotSequential],[namePool],[nameProvider],[INPnumber],[temperature],[idWarehouse],[nameWarehouse],[idWarehouseLocation],[nameWarehouseLocation],[idItem],[nameItem]) values (73,NULL,152638,'42538','2018-12-17 06:08:14',20000,350,'158470-1','REC00027','PI4','PROVEEDOR4','INP4',90,15,'BODEGA 15',17,'UBICACIÓN 17',1453,'CAMARÓN COLA')
insert into [dbo].[ResultProdLotReceptionDetail] ([idProductionLotReceptionDetail],[idDrainingTest],[idRemissionGuide],[numberRemissionGuide],[dateArrived],[poundsRemitted],[drawersNumber],[numberLot],[numberLotSequential],[namePool],[nameProvider],[INPnumber],[temperature],[idWarehouse],[nameWarehouse],[idWarehouseLocation],[nameWarehouseLocation],[idItem],[nameItem]) values (74,NULL,152639,'42539','2019-01-17 07:08:14',25000,275,'158469-2','REC00028','PI5','PROVEEDOR5','INP5',95,16,'BODEGA 16',18,'UBICACIÓN 18',1452,'CAMARÓN ENTERO')
insert into [dbo].[ResultProdLotReceptionDetail] ([idProductionLotReceptionDetail],[idDrainingTest],[idRemissionGuide],[numberRemissionGuide],[dateArrived],[poundsRemitted],[drawersNumber],[numberLot],[numberLotSequential],[namePool],[nameProvider],[INPnumber],[temperature],[idWarehouse],[nameWarehouse],[idWarehouseLocation],[nameWarehouseLocation],[idItem],[nameItem]) values (75,NULL,152640,'42540','2019-02-17 08:08:14',30000,278,'158470-2','REC00029','PI6','PROVEEDOR6','INP6',100,17,'BODEGA 17',19,'UBICACIÓN 19',1453,'CAMARÓN COLA')
insert into [dbo].[ResultProdLotReceptionDetail] ([idProductionLotReceptionDetail],[idDrainingTest],[idRemissionGuide],[numberRemissionGuide],[dateArrived],[poundsRemitted],[drawersNumber],[numberLot],[numberLotSequential],[namePool],[nameProvider],[INPnumber],[temperature],[idWarehouse],[nameWarehouse],[idWarehouseLocation],[nameWarehouseLocation],[idItem],[nameItem]) values (76,NULL,152641,'42541','2019-02-18 08:08:14',30000,278,'158470-3','REC00030','PI7','PROVEEDOR7','INP7',100,17,'BODEGA 18',20,'UBICACIÓN 20',1453,'CAMARÓN COLA')


UPDATE [dbo].[ResultProdLotReceptionDetail]
SET numberRemissionGuide = RTRIM(LTRIM(numberRemissionGuide)),numberLot = RTRIM(LTRIM(numberLot)),numberLotSequential = RTRIM(LTRIM(numberLotSequential)),namePool = RTRIM(LTRIM(namePool)),nameProvider = RTRIM(LTRIM(nameProvider)),INPnumber = RTRIM(LTRIM(INPnumber)),nameWarehouse = RTRIM(LTRIM(nameWarehouse)),nameWarehouseLocation = RTRIM(LTRIM(nameWarehouseLocation)),nameItem = RTRIM(LTRIM(nameItem))

go
--Esto es lo que sería un lote en producción se debe actualizar el campo:
--id_ProductionLotState, en el documento se debería indicar cuando y por qué
insert into ProductionLot
values(15,1)
insert into ProductionLot
values(16,1)
insert into ProductionLot
values(17,1)
insert into ProductionLot
values(18,1)
insert into ProductionLot
values(19,1)

-- Esto es un detalle del lote
-- el campo que debe actualizar es quantitydrained
-- en el documento se explica por qué
go
insert into ProductionLotDetail
values(15,null)
insert into ProductionLotDetail
values(16,null)
insert into ProductionLotDetail
values(17,null)
insert into ProductionLotDetail
values(18,null)
insert into ProductionLotDetail
values(19,null)