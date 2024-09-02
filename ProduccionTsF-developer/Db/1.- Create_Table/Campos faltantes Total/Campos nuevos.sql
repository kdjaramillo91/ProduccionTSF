
alter table ProductionLotClose add tipo char(3)

ALTER TABLE ProductionLotClose DROP CONSTRAINT PK_ProductionLotClose;

ALTER TABLE ProductionLotClose drop column Id 
ALTER TABLE ProductionLotClose ADD Id INT IDENTITY(1,1);
ALTER TABLE ProductionLotClose ADD CONSTRAINT PK_ProductionLotClose PRIMARY KEY (Id);


