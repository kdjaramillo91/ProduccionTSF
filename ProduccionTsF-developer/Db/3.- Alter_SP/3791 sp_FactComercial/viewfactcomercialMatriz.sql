CREATE or alter View [dbo].[vieFacturasComercialMatrizReport]  
As  
select DISTINCT
a.daeNumber + '-' + a.daeNumber2 + '-' + a.daeNumber3 + '-' + a.daeNumber4 As [REFRENDO]
,a.daeNumber4 As [DAU]
,a.daeNumber4 As [FUE]
,convert(varchar,b.[emissionDate],112) As [FCH_FUE]
,d.fullname_businessName as [RAZ_SOCIAL]
,e.fullname_businessName as CONSIGNATARIO
,UPPER(h.name) as PAIS_ORIGEN
,SUBSTRING(b.[number], 1, 7) As [NRO_SERIE]
,SUBSTRING(b.[number], 9, 9)  As [NRO_SECUEN]
,convert(varchar,b.[emissionDate],112) As [Fecha_Emision]
,ISNULL(b.[authorizationNumber],'-') As [NRO_AUTORI]
,I.numBoxes AS CARTONES
,Case When J.id = 4 Then Round(L.itemWeightGrossWeight,2) else 0 End As [PES_BRU_LB]
,Case When J.id = 1 Then Round(L.itemWeightGrossWeight,2) else 0 End As [PES_BRU_KL]
,Round(I.[numBoxes] * Case When J.id = 4 Then Round(L.itemWeightGrossWeight,2)  
When J.id = 1 Then Round(L.itemWeightGrossWeight,2) Else 1 End,2) As [PES_BRU_CRT]
,Case When J.id = 4 Then Round((I.[amount] * L.[conversionToPounds]),2) else 0 End As [LIBRAS],  
Case When J.id = 1 Then Round(I.[amount],2) else 0 End As [KILOS],  
Round(I.amount * L.[conversionToPounds],2) As [PES_NET_LB],
Case When J.id = 4 Then Round((I.unitPrice),6) else 0 End As [PRECIO_LB],  
Case When J.id = 1 Then Round(I.[unitPrice],6) else 0 End As [PRECIO_KL],  
Case When J.id = 4 Then Round(I.[unitPrice],6)  
When J.id = 1 Then Round((I.[unitPrice] / L.[conversionToPounds]),6) Else 0 End As [PRE_CONV_LB],    
Case When J.id = 4 Then Round((I.[unitPrice] / L.[conversionToPounds]),6)  
When J.id = 1 Then Round(I.[unitPrice],6) else 0 End As [PRE_UNI] 
,'' As [PRECIO_REF]
,K.masterCode AS COD_PRODUCTO
,K.description2 AS NOMBRE_MAR
,m.name as MARCA
,convert(varchar,a.[dateShipment],112) As [FCH_EMBARQ]
,UPPER (o.nombre + ' - ' + q.name) as PAIS
,UPPER (o.nombre + ' - ' + q.name) as PUERTO
,Case When r.[code] = 'C' Then 'ENT'  
     When r.[code] = 'S' Then 'COL'  
     When r.[code] = 'V' Then 'VAG' Else '' End As [TIPO_SC]
, s.[name] As [TIPO_ST],  
   m.[name] As [TALLA],  
   Round((i.[amount] * l.[conversionToPounds]),2) As [LBRS_NETA],  
   Round(i.[total],2) As [VALOR_NETO],  
   u.[code] As [SUBPARTIDA],  
   j.[code] As [UNIDAD],  
   k.[masterCode] As [CODIGOMASTER],  
   Isnull(b.[description],'') As [OBSERVACIÓN]  



from InvoiceCommercial a
inner join document b on b.id = a.id
inner join person e on e.id = a.id_Consignee
INNER JOIN InvoiceCommercialDetail I ON I.id_invoiceCommercial = A.id AND I.isActive = 1
INNER JOIN ITEM K ON K.ID = I.ID_ITEM	
INNER JOIN METRICUNIT J ON J.ID = I.ID_METRICUNIT
Left Outer Join [dbo].[ItemWeightConversionFreezen] L On K.[id] = L.[id_item] 
left join invoice c on c.id = b.id_documentOrigen
left join person d on d.id = c.id_buyer
left join SalesQuotationExterior f on f.id = b.id_documentOrigen
left join ForeignCustomerIdentification g on g.id = a.id_addressCustomer
left join Country h on h.id = g.id_country
LEFT JOIN ItemGeneral N ON N.id_item = K.id
LEFT JOIN ItemTrademark M ON M.ID = N.id_trademark
left join [port] o on o.id = a.id_portDestination
left join city p on p.id = o.id_city
left join country q on q.id = p.id_country
left join ItemType r on r.id = k.id_itemType
left join itemgroup s on s.id = n.id_group
left join ItemSize t on t.id = n.id_size
left join TariffHeading u on u.id = a.id_tariffheading
 where  b.id_documentState <> 5  


--SELECT * FROM InvoiceCommercial WHERE ID= 1346840

--select * from [vieFacturasComercialMatrizReport]