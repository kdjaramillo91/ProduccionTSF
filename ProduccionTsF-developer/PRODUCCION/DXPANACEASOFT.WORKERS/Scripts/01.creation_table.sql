create table BackgroundProcess
(
	"id" int identity(1,1) not null,
	"code" varchar(10) not null,
	"state" varchar(20) not null,
	"dateCreation" datetime not null,
	"dateModification" datetime,
	constraint PK_BackgroundProcess primary key(id)
)
