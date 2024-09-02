if object_id(N'[dbo].[ProductionLotDetail]') is not null
	drop table [dbo].[ProductionLotDetail]
go
create table [dbo].[ProductionLotDetail]
(
	[id]	int identity(1,1) not null,
	[id_productionLot]	int not null,
	[quantitydrained]   decimal(20,6)
)
go
alter table [dbo].[ProductionLotDetail]
add constraint PK_ProductionLotDetail primary key(id)
go
alter table [dbo].[ProductionLotDetail]
add constraint PK_ProductionLotDetail_ProductionLot 
foreign key(id_productionLot) references ProductionLot(id)
go