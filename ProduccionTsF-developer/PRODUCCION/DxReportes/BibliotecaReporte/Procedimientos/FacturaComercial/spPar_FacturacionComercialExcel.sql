GO
/****** Object:  StoredProcedure [dbo].[par_InvoiceExteriorCommercialCR]    Script Date: 13/03/2023 11:41:37 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create  procedure  [dbo].[spPar_FacturacionComercialExcel]         
        
@id int        
as        
SET NOCOUNT ON        
  
  
declare @id_PurchaseOrder int=(select id_documentOrigen from Document d where d.id =@id)  
declare @extra_BankTransferInfo nvarchar(300)=( select b.companyName from SalesQuotationExterior s inner join BoxCardAndBank b on b.id=s.id_bank  where s.id=@id_PurchaseOrder)  
 
select         
InvoiceCommercial.id as InvComm_id,        
id_documentInvoice as InvComm_document,        
id_Language as InvComm_lenguaje,        
id_ForeignCustomer as InvComm_cod_clte,        
id_Consignee as InvComm_cod_consignatario,        
id_Notifier as InvComm_notificador,        
upper(InvoiceCommercial.description) as InvComm_descrip_factura ,        
id_InvoiceType as InvComm_id_tipo_factura,        
id_metricUnitInvoice as InvComm_id_unidad_medida,        
purchaseOrder as InvComm_orden_pedido,        
id_termsNegotiation as InvComm_termino_negociacion,        
id_PaymentMethod as InvComm_id_forma_pago,        
InvoiceCommercial.id_PaymentTerm as InvComm_id_plazo,        
-- dateShipment as InvComm_fecha_embarque,        
InvComm_fecha_embarque = mesFechaEmbarque.mesIngles + ' '+convert(char(2),DAY(InvoiceCommercial.dateShipment))+', '+convert(char(4),YEAR(InvoiceCommercial.dateShipment) ),        
id_shippingAgency as InvComm_agencia_naviera,        
id_shippingLine as InvComm_id_linea_embarque,        
id_portShipment as InvComm_id_puerto_embarque,        
id_portDischarge as InvComm_id_puerto_decarga,        
id_portDestination as InvComm_id_puerto_destino,        
referenceInvoice AS Numero_Factura        
,shipName as Transportation-------------------------------------------------        
,upper(shipName + ', ' + shipNumberTrip) as BuquemasViaje,-------------------------------        
shipName as InvComm_nombre_buque ,        
shipNumberTrip as  InvComm_numero_de_viaje,        
daeNumber  as InvComm_dae,        
upper(BLNumber) as InvComm_numero_bl,        
numeroContenedores as  InvComm_numero_contenedores,        
id_capacityContainer as InvComm_capacidad_contenedor,        
id_tariffHeading as InvComm_partida_arancelaria,        
glazePercentage as InvComm_porcentaje_glaceado,        
totalBoxesOrigen  as InvComm_valor_total_caja_original,        
totalWeightOrigen as InvComm_peso_total_original,        
totalValueOrigen as InvComm_valor_total_factura_fiscal,        
totalBoxes as InvComm_total_cajas,        
totalWeight as InvComm_peso_total,        
totalValue as InvComm_valor_total,        
valueTotalFreight as InvComm_valor_flete_internacional,        
valueInternationalInsurance as InvComm_valor_seguro_internacional,        
valueCustomsExpenditures as InvComm_valor_gastos_aduaneros,        
valueTransportationExpenses as InvComm_valor_transporte,        
InvoiceCommercialDetail.id as InvCommDetail_id,        
invoicecommercialdetail.id_invoiceCommercial,        
id_itemOrigen,        
id_metricUnitOrigen,        
amountOrigen,        
codePresentationOrigen,        
presentationMinimumOrigen,        
presentationMaximumOrigen,        
numBoxesOrigen,        
unitPriceOrigen,        
totalOrigen,        
invoiceCommercialDetail.id_item as InvCommDet_id_item,        
InvoiceCommercialDetail.id_metricUnit as InvCommDet_id_unidad_medida,        
amount as InvCommDet_cantidad,        
codePresentation as InvCommDet_codigo_presentacion,        
presentationMinimum as InvCommDet_minimo_presentacion_item,        
presentationMaximum as InvCommDet_maximo_presentacion_item,        
amountInvoice as InvCommDet_cantidad_fac_comercial,        
numBoxes as InvCommDet_cantidad_cajas,        
unitPrice as InvCommDet_precio_unitario,        
total as InvCommDet_valor_total,        
InvoiceCommercialDetail.isActive as InvCommDet_estado_factura,        
Country_Destination.name as CountryDestination,        
ShippingLine.name as Shipping_Line,---------------------------------------        
ShippingAgency.name as ShippingAgency_name,        
TariffHeading.code as TariffHeadingCode,        
CapacityContainer.code as capacidad_containes,          
Company.logo2 as logo, 
company.logo as logo2,
Company.address as direccioncia,        
Company.trademark as Trademark_Company,        
('Tlf: ' + Company.phoneNumber) as Telefono_Compania,        
ltrim(rtrim(replace ( isnull(termsNegotiation.code,' ' ) ,'FOBFLET' ,'CFR' ))) as termsNegotiation_code,        
Document.number as documento,        
Item.name as Item_name,        
emissionDate = mesEmission.mesIngles + ' '+convert(char(2),DAY(Document.emissionDate))+', '+convert(char(4),YEAR(Document.emissionDate) ),        
emissionDateformat=Document.emissionDate,      
 consig.fullname_businessName,        
 consig.cellPhoneNumberPerson as Telefono,-------------------------------------------------------        
 Document.description as Good,        
 (select TOP 1 valueaux from Setting  s1, SettingDetail sd1 where s1.id = sd1.id_setting and code = 'FXIP' and sd1.value = 'BANK'  ) as BANK,        
  (select TOP 1 valueaux from Setting  s1, SettingDetail sd1 where s1.id = sd1.id_setting and code = 'FXIP' and sd1.value = 'ABA#'  ) as ABA,        
   (select TOP 1 valueaux from Setting  s1, SettingDetail sd1 where s1.id = sd1.id_setting and code = 'FXIP' and sd1.value = 'SWIF CODE'  ) as SWIFT,        
    (select TOP 1 valueaux from Setting  s1, SettingDetail sd1 where s1.id = sd1.id_setting and code = 'FXIP' and sd1.value = 'ACCOUNT T NAME'  ) as ACCOUNTT,        
  (select TOP 1 valueaux from Setting  s1, SettingDetail sd1 where s1.id = sd1.id_setting and code = 'FXIP' and sd1.value = 'ACCOUNT#'  ) as ACCOUNTN,        
  Port.nombre as portdescarga,        
    City.name as ciudaddescarga,        
       Port_1.nombre as Portembarque,        
       City_1.name as City_embarque,         
   Port_2.nombre as Portdestino,        
       City_2.name as Citydestino,         
    Country_embarque.name as Countryembarque,        
    Portofdeparture   = City_1.name+'-' + upper(Country_embarque.name),        
    Portofdestination = City_2.name + '-'+upper(Country_Destination.name),        
    Placeofdelivery =  City_3.name + '-'+ upper(Country_Delivery.name),        
    InvoiceCommercial.containers as numcontenedor,        
    --dbo.FUN_GetContainersInvoiceCommercial(InvoiceCommercial.id) as numcontenedor,        
    (select TOP 1 value from Setting where code = 'FPFF' ) as formapago,        
    Person.address as direccion,        
    PaymentMethod.code as formapagocode,        
    upper(PaymentMethod.descriptionEnglish) as descripformapago,        
    ItemSize.name as tallas        
    ,dbo.FUN_CantidadConLetra(convert(INTEGER,InvoiceCommercial.totalValue))+ ' ' + SUBSTRING(RTRIM(LTRIM(PARSENAME(InvoiceCommercial.totalValue,1))),1,2) + '/100 DOLLARS' as letras,        
     MetricUnit.code as medidafactura,        
     termsNegotiation.code as negociacion,        
  notificador = case id_ForeignCustomer when id_Notifier then  'SAME AS ABOVE' else personnotify.fullname_businessName end ,        
  direccnotifi= case id_ForeignCustomer when id_Notifier then '' else personnotify.address   end,        
     consignador = case id_ForeignCustomer when id_Consignee then  'SAME AS ABOVE' else personconsigna.fullname_businessName end ,        
  direccconsigna = case id_ForeignCustomer when id_Consignee then '' else personconsigna.address  end,        
  PaymentTerm.descriptionEnglish as plazo,        
  InvoiceCommercial.letterCredit as lettercredit,        
  notificador2 = convert(varchar(255),case id_ForeignCustomer when InvoiceCommercial.id_Notifier2 then  'SAME AS ABOVE' else isnull(personnotify2.fullname_businessName,'N') end ),        
  isnull(invoicecommercial.letterCredit,'N') as cartacredito,        
  case InvoiceCommercial.isChargeInUnitPrice when 1 then valueInternationalFreight+valueInternationalInsurance+valueCustomsExpenditures+valueTransportationExpenses else 0 end as cargos,        
  FOb = (totalValue - valueTotalFreight),        
  --case InvoiceCommercial.isChargeInUnitPrice when 1 then totalValue else totalValue+valueInternationalFreight+valueInternationalInsurance+valueCustomsExpenditures+valueTransportationExpenses end as CFR,        
  CFR = totalValue,        
  case when id_BankTransfer IS null then ' ' else upper(tbsysCatalogueDetail.fldFullText)+ isnull(@extra_BankTransferInfo,'')  end as BankTransferInfo,        
  isnull(PortfolioFinancing.fldvarchar1,' ') as PortfolioFinancing,        
  case         
   when umf.id is not null then umf.umedidaPlural        
   else umfd.umedidaPlural        
  end as umPlural,        
  case          when umf.id is not null then umf.umedidaSingular        
   else umfd.umedidaSingular        
  end as umSingular,        
  'GUAYAQUIL - ECUADOR' as CiudadCompany,        
  'CASES' AS Unit,          
  upper(isnull(City_2.name+' - ','') + isnull(Country_Destination.name,'') )as cityCountry,           
  case         
   when umf.id is not null then 'NET WEIGHT '+umf.umedidaPlural        
   else 'NET WEIGHT '+char(13)+'('+umfd.umedidaPlural+')'        
  end as lblNETWEIGHT,        
  -- select * from termsNegotiation        
  case         
   when umf.id is not null then 'PRICE '+ replace ( isnull(termsNegotiation.code,' ' ) ,'FOBFLET' ,'CFR' )+' USD x '+umf.umedidaSingular        
   else 'PRICE '+replace ( isnull(termsNegotiation.code,' ' ) ,'FOBFLET' ,'CFR' )+' USD x '+char(13)+'(' +umfd.umedidaSingular+')'        
  end as lblPRICECFR        
  ,bank.name as BeneficiaryBank        
  ,InvoiceCommercial.valueDiscount Descuento        
  ,itor.foreignName as Item_Origen        
  ,itsor.name as Talla_Origen        
  ,notif.fullname_businessName Notifier        
  ,notif.address direccionnotif        
  --,notif.cellPhoneNumberPerson TelefonoNotif        
  ,notif.identification_number RucNotif        
  ,consig.identification_number rucConsig        
  ,TelefonoNotif = (select top 1 phone1FC from ForeignCustomerIdentification where id_ForeignCustomer = InvoiceCommercial.id_Notifier)        
  ,Telefonoconsig = (select top 1 phone1FC from ForeignCustomerIdentification where id_ForeignCustomer = InvoiceCommercial.id_Consignee)        
 ,consig.id id_consig       
  --case         
  --when InvoiceCommercial.id_metricUnitInvoice is not null then (select top 1 code from MetricUnit where id =  InvoiceCommercial.id_metricUnitInvoice  )        
  -- select * from TermsNegotiation        
  from InvoiceCommercial        
  inner join InvoiceCommercialDetail        
       on InvoiceCommercialDetail.id_invoicecommercial = invoicecommercial.id AND invoicecommercialdetail.isActive =1        
  inner join Item Item on Item."id" = "InvoiceCommercialDetail"."id_item"        
  inner join ItemGeneral on ItemGeneral.id_item = Item.id        
  inner join ItemSize on ItemSize.id = ItemGeneral.id_size        
  LEFT OUTER join "dbo"."MetricUnit" "MetricUnit" on MetricUnit.id = InvoiceCommercial.id_metricUnitInvoice         
  left join Person personnotify on personnotify.id = "InvoiceCommercial"."id_Notifier"        
  left join Person personnotify2 on personnotify2.id = "InvoiceCommercial"."id_Notifier2"        
  left join Person personconsigna on personconsigna."id" = "InvoiceCommercial"."id_Consignee"        
  inner join Person on "Person"."id" = "InvoiceCommercial"."id_ForeignCustomer"        
  inner join ForeignCustomer ForCustomer on "ForCustomer"."id" = InvoiceCommercial.id_ForeignCustomer        
  inner join Document on Document.id = InvoiceCommercial.id        
  left join PaymentTerm on PaymentTerm.id = InvoiceCommercial.id_PaymentTerm        
  left join "dbo"."Port" "Port"  on "Port"."id" = "InvoiceCommercial"."id_portDischarge"    left join "dbo"."City" "City" on "Port"."id_city" = "City"."id"        
  left join "dbo"."Port" "Port_1" on "Port_1"."id" = "InvoiceCommercial"."id_portShipment"        
  left join "dbo"."City" "City_1" on "Port_1"."id_city" = "City_1"."id"        
  left join "dbo"."Port" "Port_2" on "Port_2"."id" = "InvoiceCommercial"."id_portDestination"        
  INNER join "dbo"."EmissionPoint" "EmissionPoint" on "EmissionPoint"."id" = "Document"."id_emissionPoint"        
  INNER join "dbo"."Company" "Company" on "Company"."code" = '01'        
  left join "dbo"."City" "City_2" on "City_2"."id" = "Port_2"."id_city"        
  left join "dbo"."Country" "Country_Destination"  on "Country_Destination"."id" = "City_2"."id_country"        
  left join "dbo"."City" "City_3" on "City_3"."id" = "InvoiceCommercial"."id_CityDelivery"        
  left join "dbo"."Country" "Country_Delivery" on "Country_Delivery"."id" = "City_3"."id_country"        
  left join "dbo"."Country" "Country_embarque" on "Country_embarque"."id" = "City_1"."id_country"        
  left join "dbo"."ShippingAgency" "ShippingAgency" on "ShippingAgency"."id" = "InvoiceCommercial"."id_shippingAgency"        
  left join "dbo"."TariffHeading" "TariffHeading" on "TariffHeading"."id" = "InvoiceCommercial"."id_tariffHeading"        
  left join "dbo"."CapacityContainer" "CapacityContainer" on "CapacityContainer"."id" = "InvoiceCommercial"."id_capacityContainer"        
  INNER join "dbo"."BranchOffice" "BranchOffice" on "BranchOffice"."id" = "EmissionPoint"."id_branchOffice"         
  left join "dbo"."termsNegotiation" "termsNegotiation" on "InvoiceCommercial"."id_termsNegotiation" = "termsNegotiation"."id"        
  --inner join "dbo"."CompanyElectronicFacturation" "companyElectronicFacturation" on "Company"."id" = "companyElectronicFacturation".id_company        
  --INNER join "dbo"."EnvironmentType" "environmentType" on "environmentType"."id" = "companyElectronicFacturation"."id_enviromentType"        
  --INNER join "dbo"."EmissionType" "emissionType" on "emissionType"."id" = "companyElectronicFacturation"."id_emissionType"        
  left join "dbo"."PaymentMethod" on "PaymentMethod"."id" = "InvoiceCommercial"."id_PaymentMethod"        
  left join "dbo"."tbsysCatalogueDetail" on tbsysCatalogueDetail.id = InvoiceCommercial.id_BankTransfer         
  left join "dbo"."tbsysCatalogueDetail"  PortfolioFinancing on PortfolioFinancing.id = InvoiceCommercial.idPortfolioFinancing         
  left join "dbo"."ShippingLine" on "ShippingLine".id = InvoiceCommercial.id_shippingLine        
  left join         
  (        
             
  select  m1.id,         
    ctd.description   umedidaPlural,        
    ctd.fldvarchar1   umedidaSingular        
  from  MetricUnit m1			
    inner join tbsysCatalogueDetail ctd on ctd.code = m1.code        
    inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue        
     where      ct.code ='LBLMU'         
  ) umf on InvoiceCommercial.id_metricUnitInvoice = umf.id        
  left join         
  (        
             
  select  m1.id,         
    ctd.description   umedidaPlural,        
    ctd.fldvarchar1   umedidaSingular        
  from  MetricUnit m1         
    inner join tbsysCatalogueDetail ctd on ctd.code = m1.code        
    inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue        
     where      ct.code ='LBLMU'         
  ) umfd on InvoiceCommercialDetail.id_metricUnit= umfd.id        
   left join         
  (        
             
  select  ctd.code,         
    ctd.description   mesEspanol,        
    ctd.fldvarchar1   mesIngles        
 from  tbsysCatalogueDetail ctd         
    inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue        
     where      ct.code ='MONTHT'         
  ) mesEmission on mesEmission.code =  cast(month( Document.emissionDate) as varchar)        
  left join         
  (        
             
  select  ctd.code,         
    ctd.description   mesEspanol,        
    ctd.fldvarchar1   mesIngles        
 from  tbsysCatalogueDetail ctd         
    inner join tbsysCatalogue ct on ct.id = ctd.id_Catalogue        
     where      ct.code ='MONTHT'         
  ) mesFechaEmbarque on mesFechaEmbarque.code =  cast(month( InvoiceCommercial.dateShipment) as varchar)        
  left join bank bank on bank.id = InvoiceCommercial.id_BankTransfer        
        
  left join item itor on itor.id = InvoiceCommercialDetail.id_itemMarked        
  left join itemgeneral itgmar on itgmar.id_item = itor.id        
  left join itemsize itsor on itsor.id = itgmar.id_size        
  left join person consig on consig.id = InvoiceCommercial.id_Consignee        
  left join person notif on notif.id = InvoiceCommercial.id_Notifier        
where "InvoiceCommercial"."id" = convert(int,@id)        
order by Item.id_itemType,ItemGeneral.id_size asc        
        
