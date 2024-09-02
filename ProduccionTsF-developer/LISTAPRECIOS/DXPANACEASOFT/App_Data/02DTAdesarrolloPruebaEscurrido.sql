SET DATEFORMAT ymd
SET NOCOUNT ON
insert into ResultProdLotReceptionDetail ([idProductionLotReceptionDetail],[idDrainingTest],[idRemissionGuide],[numberRemissionGuide],[dateArrived],[poundsRemitted],[drawersNumber],[numberLot],[numberLotSequential],[namePool],[nameProvider],[INPnumber],[temperature],[idWarehouse],[nameWarehouse],[idWarehouseLocation],[nameWarehouseLocation],[idItem],[nameItem]) values (70,NULL,152635,'42535','2018-09-17 03:08:15',5000,000000,200,'158469-0','REC00024','PI1','PROVEEDOR1','INP1',75,000000,12,'BODEGA 12',14,'UBICACIÓN 14',1452,'CAMARÓN ENTERO')
insert into ResultProdLotReceptionDetail ([idProductionLotReceptionDetail],[idDrainingTest],[idRemissionGuide],[numberRemissionGuide],[dateArrived],[poundsRemitted],[drawersNumber],[numberLot],[numberLotSequential],[namePool],[nameProvider],[INPnumber],[temperature],[idWarehouse],[nameWarehouse],[idWarehouseLocation],[nameWarehouseLocation],[idItem],[nameItem]) values (71,NULL,152636,'42536','2018-10-17 04:08:15',10000,000000,50,'158470-0','REC00025','PI2','PROVEEDOR2','INP2',80,000000,13,'BODEGA 13',15,'UBICACIÓN 15',1453,'CAMARÓN COLA')


UPDATE [dbo].[ResultProdLotReceptionDetail]
SET numberRemissionGuide = RTRIM(LTRIM(numberRemissionGuide)),numberLot = RTRIM(LTRIM(numberLot)),numberLotSequential = RTRIM(LTRIM(numberLotSequential)),namePool = RTRIM(LTRIM(namePool)),nameProvider = RTRIM(LTRIM(nameProvider)),INPnumber = RTRIM(LTRIM(INPnumber)),nameWarehouse = RTRIM(LTRIM(nameWarehouse)),nameWarehouseLocation = RTRIM(LTRIM(nameWarehouseLocation)),nameItem = RTRIM(LTRIM(nameItem))
