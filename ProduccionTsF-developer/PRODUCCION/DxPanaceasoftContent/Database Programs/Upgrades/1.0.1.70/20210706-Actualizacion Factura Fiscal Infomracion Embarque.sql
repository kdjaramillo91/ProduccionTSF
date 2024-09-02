-- Campos Nuevos factura Fiscal

-- Fecha ETD / Zarpe./ Mayor o igual a la fecha de emision de la factura
alter table InvoiceExterior
add etdDate DateTime null;

-- Fecha Emisión Instrucciones de Temperatura:    Mayor o igual a la fecha de emisión de la factura
alter table InvoiceExterior
add temperatureInstrucDate DateTime null;

-- No. Booking:   Tipo Texto
alter table InvoiceExterior
add BookingNumber varchar(35) null;

-- Instrucciones de temperatura:  Número negativo,   
alter table InvoiceExterior
add temperatureInstruction int null;

-- Tipo de Temperatura (c) Celsius, (f) Fahrenheit 
-- Tabla tbsysCatalogue == 'TEMPE'
-- FK tbsysCatalogueDetail Id
alter table InvoiceExterior
add idTipoTemperatura int null;

--- Insercion Tabla de Tipos de Temperatura

Insert into tbsysCatalogue
(code, name, description,id_userCreate,dateCreate)
values ('TEMPE','TIPO TEMPERATURA', 'Tipo Temperatura', 1,'2021-07-05');

insert into tbsysCatalogueDetail
( id_Catalogue,
  code,
  name,
  description,
  isActive,
  id_userCreate,
  dateCreate,
  id_userUpdate,
  dateUpdate)
  values
  ( 
	(select top 1 id from tbsysCatalogue where code='TEMPE' ),
	'TFAR',
	'Fahrenheit',
	'Fahrenheit',
	1,
	1,
	'2021-07-05',
	1,
	'2021-07-05'
  );

  insert into tbsysCatalogueDetail
( id_Catalogue,
  code,
  name,
  description,
  isActive,
  id_userCreate,
  dateCreate,
  id_userUpdate,
  dateUpdate)
  values
  ( 
	(select top 1 id from tbsysCatalogue where code='TEMPE' ),
	'TCELS',
	'Celcius',
	'Celcius',
	1,
	1,
	'2021-07-05',
	1,
	'2021-07-05'
  );
   








