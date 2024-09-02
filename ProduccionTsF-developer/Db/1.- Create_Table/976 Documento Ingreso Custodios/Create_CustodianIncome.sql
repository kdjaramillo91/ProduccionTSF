-- use ProduccionExpotuna_20231031
-- select * from CustodianIncome
-- CUSTODIAN INCOME
-- Crear tipo de Document


-- drop TABLE CustodianIncome
CREATE TABLE CustodianIncome
(
	Id int not null,
	id_RemissionGuide int not null,
	id_PersonCompanyCustodian1 int not null,
	id_PersonCompanyCustodian2 int null,
	
	-- Fishing  Custodian
	id_FishingSite1 int not null,
	id_FishingSite2 int null,
	id_FishingCustodian1 int not null,
	id_FishingCustodian2 int null,
	fishingCustodianField1 varchar(14) not null,
	fishingCustodianField2 varchar(14) null,
	fishingCustodianValue1 decimal not null,
	fishingCustodianValue2 decimal null,

	-- Campos de Ayuda Remission Guide
	remissionGuideProviderName varchar(max) null,
	remissionGuideProductionUnitProviderName varchar(max) null,
	remissionGuidePoolReference varchar(150) null,
	remissionGuidePoundTotal decimal(14,6) not null default(0),
	remissionGuideFishingZoneName varchar(max) null,
	remissionGuideFishingSiteName  varchar(max) null,
	remissionGuideRoute varchar(max) null,
	remissionGuideShippingTypeName varchar(20) null,
	remissionGuidesDriverName varchar(250) null,
	remissionGuidesProcessPlant varchar(max) null,
	remissionGuidesProviderTransportName  varchar(max) null,
	remissionGuidesCarRegistration varchar(50) null,
	remissionGuidesTransportBillingName varchar(max) null,
	remissionGuidesTransportValuePrice decimal(14,6) not null default(0),


	-- Validacion, cambio de estado Guia, validarr posible cambio de informacion
	--remissionGuideValidateData bit not null default(0),


	primary key( Id)
)

alter table CustodianIncome
ADD FOREIGN KEY (Id) REFERENCES Document(Id);

alter table CustodianIncome
ADD FOREIGN KEY (id_RemissionGuide) REFERENCES RemissionGuide(Id);

alter table CustodianIncome
ADD FOREIGN KEY (id_PersonCompanyCustodian1) REFERENCES Person(Id);

alter table CustodianIncome
ADD FOREIGN KEY (id_PersonCompanyCustodian2) REFERENCES Person(Id);

alter table CustodianIncome
ADD FOREIGN KEY (id_FishingSite1) REFERENCES FishingSite(Id);

alter table CustodianIncome
ADD FOREIGN KEY (id_FishingSite2) REFERENCES FishingSite(Id);

alter table CustodianIncome
ADD FOREIGN KEY (id_FishingCustodian1) REFERENCES FishingCustodian(Id);

alter table CustodianIncome
ADD FOREIGN KEY (id_FishingCustodian2) REFERENCES FishingCustodian(Id);

-- select  * from  DocumentType
-- Tipo Documento  CustodianIncome

insert into DocumentType
(
	code
	,name
	,description
	,currentNumber
	,daysToExpiration
	,isElectronic
	,codeSRI
	,id_company
	,isActive
	,id_userCreate
	,dateCreate
	,id_userUpdate
	,dateUpdate
)
values(167, 'Ingreso de Custodio','Ingreso de Custodio',1,0,0,'',2,1,1, GETDATE(),1,GETDATE() )



--select * from RemissionGuide