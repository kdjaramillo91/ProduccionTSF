/*
 Script de Implementación de Vistas para Exportación de Datos
 Fecha: 2019-01-30 --> Creación.
 Fecha: 2019-02-01 --> Ajustes de campos de productos.
 */

If Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.vieProveedoresReport') And type = 'V')
	Drop View dbo.vieProveedoresReport
Go
Create View dbo.vieProveedoresReport
As
	Select	a.[id] As [id],
			b.[name] As [TipoPersona],
			c.[name] As [TipoIdentificacion] ,
			a.[identification_number] As [Identificacion],
			a.[fullname_businessName] As [NombrePersona],
			a.[address] As [Direccion],
			a.[email] As [CorreoElectronico],
			a.[tradeName] As [NombreComercial],
			d.[cod_alternate] As [CodigoAlternativo],
			e.[name] As [TipoProveedor],
			d.[phoneNumber1] As [NumeroTelefonico],
			d.[phoneNumber2] As [NumeroTelefonico2],
			d.[contactName] As [NombreContacto],
			d.[contactPhoneNumber] As [ContactoFax],
			d.[electronicPayment] As [PagoElectronico],
			d.[rise] As [Rise],
			d.[specialTaxPayer] As [ContribuyenteEspecial],
			d.[forcedToKeepAccounts] As [ObligadoLlevarContabilidad],
			f.[name] As [TipoCuenta],
			g.[name] As [Banco],
			d.[cellPhoneNumber] As [Celular],
			i.[PETipoIdentificacion],
			i.[PEMetodoPago],
			i.[PEBancoAcreedor],
			i.[PETipoCuenta],
			i.[PENumeroCuenta],
			i.[PERefTransferencia],
			i.[PEDirección],
			i.[PECorreoElectronico],
			j.[CategoriaRise],
			j.[ActividadRise],
			j.[MontoRise],
			k.[CAMCodigo_Camaronero],
			k.[CAMNombre_Camaronero],
			k.[CAMNumero_Tramite],
			k.[CAMAcuerdo_Ministerial],
			k.[CAMINP],
			k.[CAMNumero_PiscinAs],
			k.[CAMCodigo_Zona],
			k.[CAMZona],
			k.[CAMCodigo_Sitio],
			k.[CAMSitio],
			Case When a.[isActive] = 1 Then 'ACTIVO' Else 'INACTIVO' End As Estado

	From	[dbo].[Person] a
			Inner Join [dbo].[PersonType] b On a.[id_personType] = b.[id]
			Inner Join [dbo].[IdentificationType] c On a.[id_identificationType] = c.[id]
			Inner Join [dbo].[ProviderGeneralData] d On a.[id] = d.[id_provider]
			Inner Join [dbo].[ProviderType] e On d.[id_providerType] = e.[id]
			Left Outer Join [dbo].[TypeBoxCardAndBank] f On d.[id_typeBoxCardAndBankAD] = f.[id]
			Left Outer Join [dbo].[BoxCardAndBank] g On d.[id_boxCardAndBankAD] = g.[id]
			Left Outer Join [dbo].[EconomicGroup] h On d.[id_economicGroup] = h.[id]

			Left Outer Join (
				Select	a1.[id_provider],
						b1.[name] As [PETipoIdentificacion],
						c1.[name] As [PEMetodoPago],
						d1.[name] As [PEBancoAcreedor],
						e1.[name] As [PETipoCuenta],
						a1.[noAccountEP] As [PENumeroCuenta],
						a1.[noRefTransfer] As [PERefTransferencia],
						a1.[noRoute] As [PEDirección],
						a1.[emailEP] As [PECorreoElectronico]
				From	[dbo].[ProviderGeneralDataEP] a1
						Left Outer Join [dbo].[IdentificationType] b1 On a1.[id_identificationTypeEP] = b1.[id]
						Left Outer Join [dbo].[PaymentMethod] c1 On a1.[id_paymentMethodEP] = c1.[id]
						Left Outer Join [dbo].[BoxCardAndBank] d1 On a1.[id_bankToBelieve] = d1.[id]
						Left Outer Join [dbo].[AccountTypeGeneral] e1 On a1.[id_accountTypeGeneralEP] = e1.[id]
			) i On i.[id_provider] = a.[id]

			Left Outer Join (
				Select	a2.[id_provider],
						b2.[name] As [CategoriaRise],
						c2.[name] As [ActividadRise],
						a2.[invoiceAmountRise] As [MontoRise]
				From	[dbo].[ProviderGeneralDataRise] a2
						Inner Join [dbo].[CategoryRise] b2 On a2.[id_categoryRise] = b2.[id]
						Inner Join [dbo].[ActivityRise] c2 On a2.[id_activityRise] = c2.[id]
			) j On j.[id_provider] = a.[id]

			Left Outer Join (
				Select	pers.[id],
						pers.[identification_number] As Numero_Identificacoin,
						pers.[fullname_businessName] As Nombre,
						pers.[tradeName] As Nombre_Comercial,
						pers.[email] As Correo_Electronico,
						pers.[cellPhoneNumberPerson] As Celular,
						pup.[code] As CAMCodigo_Camaronero,
						pup.[name] As CAMNombre_Camaronero,
						pup.[tramitNumber] As CAMNumero_Tramite,
						pup.[ministerialAgreement] As CAMAcuerdo_Ministerial,
						pup.[INPnumber] As CAMINP,
						pup.[poolNumber] As CAMNumero_PiscinAs,
						fz.[code] As CAMCodigo_Zona,
						fz.[name] As CAMZona,
						fs.[code] As CAMCodigo_Sitio,
						fs.[name] As CAMSitio
				From	[dbo].[ProductionUnitProvider] pup
						Inner Join [dbo].[Person] pers On pup.[id_provider] = pers.[id]
						Inner Join [dbo].[FishingSite] fs On pup.[id_FishingSite] = fs.[id]
						Inner Join [dbo].[FishingZone] fz On pup.[id_FishingZone] = fz.[id]
				) k On a.[id] = k.[id]
Go

If Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.vieProductosReport') And type = 'V')
	Drop View dbo.vieProductosReport
Go
Create View dbo.vieProductosReport
As
	Select	it.[id] As [id],
			Case When it.[isPurchased] = 1 Then 'SI' Else 'NO' End As [Compra],
			Case When it.[isSold] = 1 Then 'SI' Else 'NO' End As [Venta],
			Case When it.[inventoryControl] = 1 Then 'SI' Else 'NO' End As [Inventario],
			ivl.[name] As [Linea_Inventario],
			itt.[name] As [Tipo_Producto],
			itc.[name] As [Categoria],
			it.[masterCode] As [Codigo_Maestro],
			IsNull(it.[auxCode], '') As [Codigo_Auxiliar],
			IsNull(it.[barCode], '') As [Código de Barras],
			it.[name] As [Nombre],
			IsNull(it.[description], '') As [Descripcion_1],
			IsNull(it.[description2], '') As [Descripcion_2],
			IsNull(it.[foreignName], '') As [Nombre Extranjero],
			IsNull(mty.[name], '') As [Tipo de UM],
			IsNull(pre.[name], '') As [Presentación],
			IsNull(itgr.[name],'') As [Nombre_Grupo],
			IsNull(itsgr.[name],'') As [Nombre_SubGrupo],
			IsNull(itm.[name], '' ) As [Marca],
			IsNull(itmo.[name],'') As [Modelo],
			IsNull(its.[name], '') As [Talla],
			IsNull(itco.[code], '') As [Color],
			IsNull(fcu.[code], '') As [Cód.Cliente del Exterior],
			IsNull(fcu.[name], '') As [Cliente del Exterior],
			IsNull(cfc.[name], '') As [País Origen],
			IsNull(itw.[itemWeightGrossWeight], 0) As [Peso_Bruto],
			IsNull(itw.[itemWeightNetWeight], 0) As [Peso_Neto],
			IsNull(itw.[weightWithGlaze], 0) As [Peso_Glaseo],
			IsNull(itw.[conversionToKilos], 0) As [Conv/Kls],
			IsNull(itw.[conversionToPounds], 0) As [Conv/Lbs],
			IsNull(pre.[minimum], 0) As [Cant.Empaque],
			IsNull(pre.[maximum], 0) As [Cant.Master],
			Case When it.[isActive] = 1 Then 'SI' Else 'NO' End As [Activo]

	From	[dbo].[Item] it
			Inner Join [dbo].[InventoryLine] ivl On it.[id_inventoryLine]= ivl.[id]
			Inner Join [dbo].[ItemType] itt On it.[id_itemType] = itt.[id]
			Inner Join [dbo].[ItemTypeCategory] itc On it.[id_itemTypeCategory] = itc.[id]
			Left Outer Join [dbo].[ItemGeneral] itg On it.[id] = itg.[id_item]
			Left Outer Join [dbo].[ItemGroup] itgr On itg.[id_group] = itgr.[id]
			Left Outer Join [dbo].[ItemGroup] itsgr On itg.[id_subgroup] = itsgr.[id]
			Left Outer Join [dbo].[ItemTrademark] itm On itg.[id_trademark] = itm.[id]
			Left Outer Join [dbo].[ItemTrademarkModel] itmo On itg.[id_trademarkModel] = itmo.[id]
			Left Outer Join [dbo].[ItemSize] its On its.[id] = itg.[id_size]
			Left Outer Join [dbo].[ItemColor] itco On itg.[id_color] = itco.[id]
			Left Outer Join [dbo].[ItemWeightConversionFreezen] itw On itw.[id_Item] = it.[id]
			Left Outer Join [dbo].[MetricType] mty On it.[id_metricType] = mty.[id]
			Left Outer Join [dbo].[Presentation] pre On it.[id_presentation] = pre.[id]
			Left Outer Join [dbo].[ForeignCustomer] fcu On itg.[id_Person] = fcu.[id]
			Left Outer Join [dbo].[Country] cfc On fcu.[id_country] = cfc.[id]
Go

If Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.vieFacturasFiscalesReport') And type = 'V')
	Drop View dbo.vieFacturasFiscalesReport
Go
Create View dbo.vieFacturasFiscalesReport
As
	Select	a.[id] As [id],
			b.[emissionDate] As Fecha_Emision,
			b.[number] As Numero_Factura,
			b.[sequential] As Secuencia_Factura,
			b.[authorizationDate] As Fecha_Autorizacion,
			b.[authorizationNumber] As Numero_Autorizacion,
			b.[accessKey] As Clave_Acceso,
			d.[name] As Punto_Emision,
			e.[name] As Estado_Documento,
			g.[name] As Metodo_Pago,
			h.[name] As Termino_Pago,
			i.[nombre] As Puerto_Envio,
			j.[nombre] As Puerto_Destino,
			k.[nombre] As Puerto_Descarga,
			a.[dateShipment] As Fecha_Envio,
			l.[name] As Agencia_Envio,
			a.[daeNumber] As Dae_Numero,
			a.[shipName] As Nombre_Envio,
			a.[shipNumberTrip] As Numero_Viaje,
			a.[totalBoxes] As Numero_Cajas,
			m.[name] As Capacidad_Contenedor,
			a.numeroContenedores As Numero_Contenedores,
			a.direccion As Direccion,
			a.email As Correo_Electronico,
			a.valueTotalFOB As ValorFOB,
			a.valueInternationalFreight As ValorFleteInternacional,
			a.valueInternationalInsurance As ValoraSeguroInternacional,
			a.valueCustomsExpenditures As ValorPersonalizado,
			a.valueTransportationExpenses As ValorTransportacion,
			a.valuetotalCIF As ValorCIF,
			n.[identification_number] As Identificacion_Consignatario,
			n.[fullname_businessName] As Nombre_Consignatario,
			o.[identification_number] As Identificacion_Notificador,
			o.[fullname_businessName] As Nombre_Notificador,
			a.[purchaseOrder] As Orden_Compra,
			p.[name] As Linea_Naviera,
			a.BLNumber As BL_numero,
			a.numberRemissionGuide As Numero_GuiaRemision,
			a.daeNumber2 As Dae_Numero2,
			a.daeNumber3 As Dae_Numero3,
			a.daeNumber4 As Dae_Numero4,
			a.tariffHeadingDescription As DescripcionTarifario,
			q.[identification_number] As Identificacion_Cliente,
			q.[fullname_businessName] As Nombre_Cliente,
			c.[subtotalIVA] As SubtotalIVa,
			c.[subTotalIVA0] As SubtotalIva0,
			c.[subTotalNoObjectIVA] As SubtotalNOIva,
			c.[subTotalExentIVA] As SubTotalIvaExcento,
			c.[subTotal] As Subtotal,
			c.[totalDiscount] As TotalDescuento,
			c.[valueICE] As Valor_Ice,
			c.[valueIRBPNR] As Valor_IRBPNR,
			c.[IVA] As Iva,
			c.[tip] As Propina,
			c.[totalValue] As ValorTotal,
			c.[subtotalNoTaxes] As SubTotalSinIva
	
	From	[dbo].[InvoiceExterior] a
			Inner Join [dbo].[Document] b On a.[id] = b.[id]
			Inner Join [dbo].[Invoice] c On a.[id] = c.[id]
			Inner Join [dbo].[EmissionPoint] d On b.[id_emissionPoint] = d.[id]
			Inner Join [dbo].[DocumentState] e On b.[id_documentState] = e.[id]
			Left Outer Join [dbo].[TermsNegotiation] f On a.[id_termsNegotiation] = f.[id]
			Left Outer Join [dbo].[PaymentMethod] g On a.[id_PaymentMethod] = g.[id]
			Left Outer Join [dbo].[PaymentTerm] h On a.[id_PaymentTerm] = h.[id]
			Left Outer Join [dbo].[Port] i On a.[id_portShipment] = i.[id]
			Left Outer Join [dbo].[Port] j On a.[id_portDestination] = j.[id]
			Left Outer Join [dbo].[Port] k On a.[id_portDischarge] = k.[id]
			Left Outer Join [dbo].[ShippingAgency] l On a.[id_shippingAgency] = l.[id]
			Left Outer Join [dbo].[CapacityContainer] m On a.[id_capacityContainer] = m.[id]
			Left Outer Join [dbo].[Person] n On a.[id_consignee] = n.[id]
			Left Outer Join [dbo].[Person] o On a.[id_notifier] = o.[id]
			Left Outer Join [dbo].[ShippingLine] p On a.[id_ShippingLine] = p.[id]
			Left Outer Join [dbo].[Person] q On c.[id_buyer] = q.[id]
Go

If Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.vieFacturasFiscalesMatrizReport') And type = 'V')
	Drop View dbo.vieFacturasFiscalesMatrizReport
Go
Create View dbo.vieFacturasFiscalesMatrizReport
As

	Select	a.[id] As [id],
			a.daeNumber + '-' + a.daeNumber2 + '-' + a.daeNumber3 + '-' + a.daeNumber4 As [REFRENDO],
			a.daeNumber4 As [DAU],
			a.daeNumber4 As [FUE],
			convert(varchar,b.[emissionDate],112) As [FCH_FUE],
			q.[fullname_businessName] As [RAZ_SOCIAL],
			h.[name2] As [PAIS_ORIGE],
			SUBSTRING(b.[number], 1, 7) As [NRO_SERIE],
			SUBSTRING(b.[number], 9, 9)  As [NRO_SECUEN],
			convert(varchar,b.[emissionDate],112) As [Fecha_Emision],
			b.[authorizationNumber] As [NRO_AUTORI],
			d.[numBoxes] As [CARTONES],
			Case When l.id = 4 Then Round((v.[peso] / u.[numBoxes]) * d.[numBoxes],2) else 0 End As [PES_BRU_LB],
			Case When l.id = 1 Then Round((w.[peso] / u.[numBoxes]) * d.[numBoxes],2) else 0 End As [PES_BRU_KL],
			Round(d.[numBoxes] * Case When l.id = 4 Then Round((v.[peso] / u.[numBoxes]) * d.[numBoxes],2)
									  When l.id = 1 Then Round((w.[peso] / u.[numBoxes]) * d.[numBoxes],2) Else 1 End,2) As [PES_BRU_CRT],
			Case When l.id = 4 Then Round((d.[amount] * y.[conversionToPounds]),2) else 0 End As [LIBRAS],
			Case When l.id = 1 Then Round(d.[amount],2) else 0 End As [KILOS],
			Round((d.[amount] * y.[conversionToPounds]),2) As [PES_NET_LB],
			Case When l.id = 4 Then Round((d.[unitPrice] / y.[conversionToPounds]),6) else 0 End As [PRECIO_LB],
			Case When l.id = 1 Then Round(d.[unitPrice],6) else 0 End As [PRECIO_KL],
			Case When l.id = 4 Then Round(d.[unitPrice],6)
				 When l.id = 1 Then Round((d.[unitPrice] / y.[conversionToPounds]),6) Else 0 End As [PRE_CONV_LB],		
			Case When l.id = 4 Then Round((d.[unitPrice] / y.[conversionToPounds]),6)
				 When l.id = 1 Then Round(d.[unitPrice],6) else 0 End As [PRE_UNI],
			'' As [PRECIO_REF],
			t.[description2] As [NOMBRE_MAR],
			f.[name] As [MARCA],
			convert(varchar,a.[dateShipment],112) As [FCH_EMBARQ],
			j.[nombre] + ', ' + i.[name2] As [PAIS],
			j.[nombre] + ', ' + i.[name2] As [PUERTO],
			Case When o.[code] = 'C' Then 'ENT'
				 When o.[code] = 'S' Then 'COL'
				 When o.[code] = 'V' Then 'VAG' Else '' End As [TIPO_SC],
			n.[name] As [TIPO_ST],
			m.[name] As [TALLA],
			Round((d.[amount] * y.[conversionToPounds]),2) As [LBRS_NETA],
			Round(d.[total],2) As [VALOR_NETO],
			a.[id] As [FCTA_INTER],
			e.[code] As [SUBPARTIDA],
			l.[code] As [UNIDAD]
	From	[dbo].[InvoiceExterior] a
			Inner Join [dbo].[Document] b On a.[id] = b.[id]
			Inner Join [dbo].[Invoice] c On a.[id] = c.[id]
			Inner Join [dbo].[InvoiceDetail] d On a.[id] = d.[id_invoice] and d.[isActive] = 1
			Inner Join [dbo].[TariffHeading] e On d.[id_tariffHeadingDetail] = e.[id]
			Inner Join [dbo].[Item] t On d.[id_item] = t.[id]
			Inner Join [dbo].[ItemGeneral] z On z.[id_item] = t.[id]
			Inner Join [dbo].[ItemTrademark] f On f.[id] = z.[id_trademark]
			Inner Join [dbo].[MetricUnit] l On l.[id] = d.[id_metricUnit]	
			Inner Join [dbo].[ItemSize] m On m.[id] = z.[id_size]	
			Inner Join [dbo].[ItemGroup] n On n.[id] = z.[id_group]				
			Inner Join [dbo].[ItemType] o On o.[id] = t.[id_itemtype]	
			Inner Join (Select [id_invoice],Sum(numBoxes)numBoxes from [dbo].[InvoiceDetail] Group by [id_invoice]) u On a.[id] = u.[id_invoice]
			Left Outer Join [dbo].[InvoiceExteriorWeight] v On a.[id] = v.[id_invoice] and v.[id_WeightType] = 2 and v.[id_metricUnit] = 4
			Left Outer Join [dbo].[InvoiceExteriorWeight] w On a.[id] = w.[id_invoice] and w.[id_WeightType] = 2 and w.[id_metricUnit] = 1
			Left Outer Join [dbo].[ItemWeightConversionFreezen] y On t.[id] = y.[id_item]
			Left Outer Join [dbo].[Port] j On a.[id_portDestination] = j.[id]
			Left Outer Join [dbo].[City] k On j.[id_city] = k.[id]
			Left Outer Join [dbo].[Country] i On i.[id] = k.[id_country]
			Left Outer Join [dbo].[Person] q On c.[id_buyer] = q.[id]
			Left Outer Join [dbo].[ForeignCustomer] g On q.[id] = g.[id]
			Left Outer Join [dbo].[Country] h On h.[id] = g.[id_country]
					

GO