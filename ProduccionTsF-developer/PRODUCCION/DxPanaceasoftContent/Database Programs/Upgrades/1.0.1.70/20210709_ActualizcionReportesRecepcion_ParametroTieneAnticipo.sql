-- Libras Liquidadas valoradas por numero de Liquidacvion
/*
select * 
from   tbsysPathReportProduction 
where  code = 'RMPP' 

select * 
from   tbsysPathReportProduction 
where  code = 'RMPPA' 

select * 
from   tbsysPathReportProduction 
where  code = 'RPVA' 

select * 
from   tbsysPathReportProduction 
where  code = 'RPVPA' 
*/

--- Insertar parametro de sistema maneja anticipo a proveedores
insert into AdvanceParameters
(code,description, hasDetail,valueInteger)
values
('TANT','Implementa Antipo a Proveedores',0,0);
-- PV 0 NO MANEJA || 1 SI MANEJA