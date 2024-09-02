-- CostAllocation Update 20210929

alter table CostAllocation
drop constraint FK_CostAllocation_InventoryPeriodDetail;

alter table CostAllocation
drop column id_InventoryPeriodDetail;

alter table CostAllocation
drop constraint FK_CostAllocation_Warehouse;

alter table CostAllocation
drop column id_Warehouse;

ALTER TABLE  CostAllocationDetail
ADD id_Warehouse int not null;

alter table CostAllocationDetail
add constraint FK_CostAllocationDetail_Warehouse 
    Foreign Key(id_Warehouse) 
	References Warehouse(Id);

create table CostAllocationWarehouse
(
	id int identity primary key not null,
	id_Warehouse int not null,
	id_InventoryPeriodDetail int not null
)


alter table CostAllocationWarehouse
add constraint FK_CostAllocationWarehouse_Warehouse 
    Foreign Key(id_Warehouse) 
	References Warehouse(Id);

alter table CostAllocationWarehouse
add constraint FK_CostAllocationWarehouse_InventoryPeriodDetail 
    Foreign Key(id_InventoryPeriodDetail) 
	References InventoryPeriodDetail (Id);


