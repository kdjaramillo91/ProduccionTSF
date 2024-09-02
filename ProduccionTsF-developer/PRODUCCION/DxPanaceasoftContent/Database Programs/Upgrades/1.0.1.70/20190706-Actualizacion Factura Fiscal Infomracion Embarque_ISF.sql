-- Fecha Emisión ISF:    Mayor o igual a la fecha de emisión de la factura
alter table InvoiceExterior
add isfDate DateTime null;