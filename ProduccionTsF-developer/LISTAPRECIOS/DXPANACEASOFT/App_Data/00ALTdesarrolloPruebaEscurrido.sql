INSERT INTO [dbo].[DocumentType]
values('93','Prueba de escurrido','Prueba de escurrido',1,0,0,null,2,1,1,getdate(),1,getdate())
go
if object_id(N'[dbo].[DrainingTest]') is not null
	drop table [dbo].[DrainingTest]
go
create table [dbo].[DrainingTest]
(
	[id]			int not null,
	[idAnalist]		int not null,
	[dateTesting]	date not null,
	[timeTesting]	time not null,
	[drawersNumberSampling]		integer not null,
	[poundsDrained]			decimal(20,6) not null,
	[poundsAverage]			decimal(20,6) not null,
	[poundsProjected]		decimal(20,6) not null
)
go
alter table [dbo].[DrainingTest]
add constraint PK_DrainingTest primary key([id])
go
alter table [dbo].[DrainingTest]
add constraint FK_DrainingTest_Document 
foreign key(id) references Document(id)
go
alter table [dbo].[DrainingTest]
add constraint FK_DrainingTest_Employee 
foreign key(idAnalist) references Employee(id)
go
if object_id(N'[dbo].[DrainingTestDetail]') is not null
	drop table [dbo].[DrainingTestDetail]
go
create table [dbo].[DrainingTestDetail]
(
	[id]	int identity(1,1)  not null,
	[idDrainingTest]	int not null,
	[order]				int not null,
	[quantity]			decimal(20,6) not null,
	[idMetricUnit]		int not null,
)
go
alter table [dbo].[DrainingTestDetail]
add constraint PK_DrainingTestDetail primary key(id)
go
alter table [dbo].[DrainingTestDetail]
add constraint FK_DrainingTestDetail_DrainingTest 
foreign key(idDrainingTest) references DrainingTest(id)
go
alter table [dbo].[DrainingTestDetail]
add constraint FK_DrainingTestDetail_MetricUnit 
foreign key(idMetricUnit) references MetricUnit(id)
go
if object_id(N'[dbo].[ProductionLotState]') is not null
	drop table [dbo].[ProductionLotState]
go
CREATE TABLE [dbo].[ProductionLotState](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_ProductionLotState] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
if object_id(N'[dbo].[ProductionLot]') is not null
	drop table [dbo].[ProductionLot]
go
create table [dbo].[ProductionLot]
(
	[id]	int not null,
	[id_ProductionLotState]	int not null,
)
go
alter table [dbo].[ProductionLot]
add constraint PK_ProductionLot primary key(id)
go
alter table [dbo].[ProductionLot]
add constraint PK_ProductionLot_ProductionLotState 
foreign key(id_ProductionLotState) references ProductionLotState(id)
go
if object_id(N'[dbo].[ResultProdLotReceptionDetail]') is not null
	drop table [dbo].[ResultProdLotReceptionDetail]
go
create table [dbo].[ResultProdLotReceptionDetail]
(
	[idProductionLotReceptionDetail]	int not null,
	[idDrainingTest]					int null,
	[idRemissionGuide]					int not null,
	[numberRemissionGuide]				varchar(50) not null,
	[dateArrived]						datetime,
	[poundsRemitted]					decimal(18,6) not null,
	[drawersNumber]						int not null,
	[numberLot]							varchar(20) not null,
	[numberLotSequential]				varchar(20) not null,
	[namePool]							varchar(50) not null,
	[nameProvider]						varchar(250) not null,
	[INPnumber]							varchar(250) not null,
	[temperature]						decimal(12,6) not null,
	[idWarehouse]						int not null,
	[nameWarehouse]						varchar(250) not null,
	[idWarehouseLocation]				int not null,
	[nameWarehouseLocation]				varchar(250) not null,
	[idItem]							int not null,
	[nameItem]							varchar(250) not null,
)
go
alter table [dbo].[ResultProdLotReceptionDetail]
add constraint PK_ResultProdLotReceptionDetail
primary key(idProductionLotReceptionDetail)
go
alter table [dbo].[ResultProdLotReceptionDetail]
add constraint FK_ResultProdLotReceptionDetail_DrainingTest
foreign key(idDrainingTest) references DrainingTest(id)



