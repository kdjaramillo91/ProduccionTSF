/****** Object:  StoredProcedure [dbo].[sp_FactComercial]    Script Date: 12/3/2024 12:38:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
-- insert into ObjectPermission values ('EXPFACTCOM','Factura Comercial','Reporte de Factura Comercial',1,1,GETDATE(),1,GETDATE())
-- HACER INSERT PRIMERO


ALTER     PROCEDURE [dbo].[sp_FactComercial]
@FechaEmisionInicio			datetime,
@FechaEmisionFinal			datetime
AS

SET NOCOUNT ON

-- Conversión de fechas
declare @fiDt date
declare @ffDt date

set @fiDt = convert(date,isnull(@FechaEmisionInicio,'1900-01-01'))

set @ffDt = convert(date,isnull(@FechaEmisionFinal,'1900-01-01'))


select id_invoiceCommercial,sum (totalOrigen) total
into #suma
from InvoiceCommercialDetail  
where isActive = 1
group by id_invoiceCommercial

select id_invoiceCommercial,sum (numBoxesOrigen) total
into #sumacajas
from InvoiceCommercialDetail  
where isActive = 1
group by id_invoiceCommercial

select id_invoice,peso 
into #netokg
from InvoiceExteriorWeight
where id_metricUnit = 1 and id_WeightType = 1 and isActive = 1

select id_invoice,peso 
into #netoLB
from InvoiceExteriorWeight
where id_metricUnit = 4 and id_WeightType = 1 and isActive = 1

select id_invoice,peso 
into #brutokg
from InvoiceExteriorWeight
where id_metricUnit = 1 and id_WeightType = 2 and isActive = 1

select id_invoice,peso 
into #brutolb
from InvoiceExteriorWeight
where id_metricUnit = 4 and id_WeightType = 2 and isActive = 1


select id_invoice,peso 
into #glakg
from InvoiceExteriorWeight
where id_metricUnit = 1 and id_WeightType = 3 and isActive = 1

select id_invoice,peso 
into #gllb
from InvoiceExteriorWeight
where id_metricUnit = 4 and id_WeightType = 3and isActive = 1


select id_invoice,peso 
into #Proformakg
from InvoiceExteriorWeight
where id_metricUnit = 1 and id_WeightType = 7 and isActive = 1

select id_invoice,peso 
into #Proformalb
from InvoiceExteriorWeight
where id_metricUnit = 4 and id_WeightType = 7 and isActive = 1





--select a.id_WeightType, c.name wt,a.id_metricUnit,d.name mu,peso from InvoiceExteriorWeight a  
--inner join InvoiceExterior b on b.id = a.id_invoice
--inner join WeightType c on c.id = a.id_WeightType
--inner join MetricUnit d on d.id = a.id_metricUnit


--where a.id_invoice =1009105 and a.isActive = 1








select  distinct
a.id as Id
,b.emissionDate as Fecha_Emision
,b.number as Numero_Factura
,b.sequential as Secuencia_Factura
,c.name as Punto_Emision
,d.name as Estado_Documento
,isnull(b.description,'-') as Observacion
,isnull(e.identification_number,'NO TIENE') as Identificacion_Consignatario
,e.fullname_businessName as  Nombre_Consignatario
,e.email as Correo_Electronico
,e.address as Direccion
,g.identification_number as Identificacion_Cliente
,g.fullname_businessName as Nombre_Cliente
,h.identification_number as Identificacion_Notificador
,h.fullname_businessName as Nombre_notificador
,a.purchaseorder as Orden_Compra
,i.name as [Term.Negociacion]
,isnull(v.number,'-') as Numero_Proforma
,j.name as Forma_Pago
,k.name as Plazo_Pago
,a.dateShipment as Fecha_Embarque
,l.nombre as Puerto_Embarque
,m.nombre as Puerto_Descarga
,n.nombre as Puerto_Destino
,o.name as Agencia_Envio
,p.name as Linea_Naviera
,a.shipName as Buque
,a.shipNumberTrip as Numero_Viaje
,a.BLNumber as Numero_BL
,a.daeNumber as DAE_Numero
,a.daeNumber2 as DAE_Numero2
,a.daeNumber3 as DAE_Numero3
,a.daeNumber4 as DAE_Numero4
,r.total as Numero_Cajas
,s.peso as Peso_neto_Kilos
,t.peso as Peso_neto_libras
,ss.peso as Peso_Bruto_Kilos
,tt.peso as Peso_Bruto_Libras
,ssss.peso as Peso_Glaeso_Kilos
,tttt.peso as Peso_glaseo_Libras
,sss.peso as Peso_Proforma_Kilos
,ttt.peso as Peso_Proforma_Libras
,a.valueDiscount as Descuento
,a.valueTotalFreight as ValorFleteInternacional
,q.total as ValorTotal
,u.fullname_businessName as Vendedor
,a.seals as Sellos
,a.containers as Contenedores


from InvoiceCommercial a
inner join document b on b.id = a.id
inner join EmissionPoint c on c.id = b.id_emissionPoint
inner join documentstate d on d.id = b.id_documentState
left join person e on e.id = a.id_Consignee
left join ForeignCustomerIdentification f on f.id_ForeignCustomer = a.id_ForeignCustomer
left join person g on g.id = f.id_ForeignCustomer
left join person h on h.id = a.id_Notifier
inner join TermsNegotiation i on i.id = a.id_termsNegotiation
inner join PaymentMethod j on j.id = a.id_PaymentMethod
inner join PaymentTerm k on k.id = a.id_PaymentTerm 
inner join Port l on l.id = a.id_portShipment
inner join port m on m.id = a.id_portDischarge
inner join port n on n.id = a.id_portDestination
inner join ShippingAgency o on o.id = a.id_shippingAgency
inner join ShippingLine p on p.id = a.id_shippingLine
left join  #suma q on q.id_invoiceCommercial = a.id
left join #sumacajas r on r.id_invoiceCommercial = a.id
left join #netokg s on s.id_invoice = b.id_documentOrigen
left join #netoLB t on t.id_invoice = b.id_documentOrigen
left join #brutokg ss on ss.id_invoice = b.id_documentOrigen
left join #brutolb tt on tt.id_invoice = b.id_documentOrigen
left join #Proformakg sss on sss.id_invoice = b.id_documentOrigen
left join #Proformalb ttt on ttt.id_invoice = b.id_documentOrigen
left join #glakg ssss on ssss.id_invoice = b.id_documentOrigen
left join #gllb tttt on tttt.id_invoice = b.id_documentOrigen
left join person u on u.id = a.idVendor
LEFT JOIN Document v on v.id = b.id_documentOrigen

WHERE
--a.id = 1008835

convert(date,b.emissionDate) >= case when year(@fiDt) = 1900 then convert(date, b.emissionDate) else @fiDt end
and convert(date,b.emissionDate) <= case when year(@ffDt) = 1900 then convert(date, b.emissionDate) else @ffDt end


order by b.emissionDate

--exec [dbo].[sp_FactComercial] '',''


