-- Fecha Emisi�n ISF:    Mayor o igual a la fecha de emisi�n de la factura
alter table InvoiceExterior
add isfDate DateTime null;