alter table  InventoryMoveDetail
add productoCost decimal(20,6) not null default(0)

alter table  InventoryMoveDetail
add lastestProductoCost decimal(20,6) not null default(0)

alter table InventoryMoveDetail
add id_CostAllocationDetail int null 

alter table InventoryMoveDetail
add id_lastestCostAllocationDetail int null 



select *  from  CostAllocationDetail

select *  from   DocumentState







