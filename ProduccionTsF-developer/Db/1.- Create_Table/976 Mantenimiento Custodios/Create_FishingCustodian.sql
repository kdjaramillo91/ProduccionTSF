-- FishingCustodian
-- use ProduccionExpotuna_20231031


create table FishingCustodian
(
	id int identity not null,
	code varchar(20)  not null,
	patrol decimal(13,6) null,
	semiComplete decimal(13,6) null,
	truckDriver decimal(13,6) null,
	changeHG decimal(13,6) null,
	cabinHR decimal(13,6) null,
	id_FishingSite int not null,
	isActive bit not null default(1),
	id_company int  not null,
	id_userCreate int not null,
	dateCreate DateTime  not null,
	id_userUpdate int not null,
	dateUpdate DateTime  not null,
    primary key (Id)
)

alter table FishingCustodian
ADD FOREIGN KEY (id_FishingSite) REFERENCES FishingSite(Id);


-- select  * from FishingCustodian
-- select  * from FishingSite
