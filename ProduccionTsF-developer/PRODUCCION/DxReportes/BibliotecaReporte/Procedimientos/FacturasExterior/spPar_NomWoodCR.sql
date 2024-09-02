/****** Object:  StoredProcedure [dbo].[spPar_NomWoodCR]    Script Date: 08/06/2023 10:00:34 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


    
CREATE OR ALTER procedure  [dbo].[spPar_NomWoodCR]      
@id int      
    as      
 set nocount on    
 DECLARE @NetoKilos Decimal(15,6) 
 DECLARE @Marcas Varchar(250)
 Declare @GlaseoKilos Decimal(15,6)

 Create Table #Marcas(
 Nombre Varchar(250))
  
 Select @NetoKilos = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Kg' and wt.code = 'NET')

  select @GlaseoKilos = (select peso from InvoiceExteriorWeight itw
						 Inner Join WeightType wt
							On wt.id = itw.id_WeightType
						 Inner Join MetricUnit mu
							On mu.id = itw.id_metricUnit
						 where id_invoice = CONVERT(INT,@id) 
						   and mu.code = 'Kg'
						   and wt.code = 'GLS')

  Insert Into #Marcas
  SELECT distinct ItemTrademark.name Marcas from [dbo].[InvoiceDetail] [InvoiceDetail] 
   Inner Join Item Item On Item.id = [InvoiceDetail].id_item
   Inner Join ItemGeneral ItemGeneral On ItemGeneral.id_item = Item.id
   Inner Join ItemTrademark ItemTrademark On ItemTrademark.id = ItemGeneral.id_trademark
  Where [InvoiceDetail].[isActive] = 1 
   And id_invoice = CONVERT(INT,@id) 

  Select @Marcas = (SELECT STRING_AGG(Nombre, ', ')  prod from #Marcas)
    
 select [Invoice].[id],
	
    Contenedores =  [InvoiceExterior].[containers] +  ' / '+InvoiceExterior.seals,   
	FechaEmisiON = [doc].[emissiONDate], 
    TotalCartones = [invoiceExterior].[totalBoxes],      
    [Buyer]= (SELECT [Name] FROM ForeignCustomer [foreignCustomer] WHERE [foreignCustomer].[id] = [invoiceExterior].[id_cONsignee]),
	PuertoDeEmbaruqe = UPPER([City_1].[name]) + ', ' + UPPER(COULOA.name), 
	FechaEmbarque = Convert(Datetime,[InvoiceExterior].[dateShipment],102),
	[City_2].[name]+', '+upper([Country_Destination].[Name2]) as PaisDestino,
	PesoNeto = @NetoKilos,
	@GlaseoKilos As GlaseoKilos,
	Marcas = @Marcas,
	[salesQuotationExterior].Product AS ProductProforma,
	[InvoiceExterior].etdDate   As FechaETD, 
	invoiceexterior.shipName + ' ' + convert(varchar,[InvoiceExterior].shipNumberTrip,25) as NombreBuque,
	doc.number as Factura,
	co.logo as Logo
  from [dbo].[Invoice] [Invoice]      
  inner join [dbo].[Document] doc on [Invoice].[id] = doc.[id] 
  inner join EmissionPoint Emi on doc.id_emissionPoint=Emi.id
  inner join Company co on Emi.id_company=co.id
  inner join [dbo].[DocumentState] docs on doc.[id_documentState] = docs.[id]   
  inner join [dbo].[InvoiceExterior] [InvoiceExterior] on [InvoiceExterior].[id] = [Invoice].[id]   
  left outer join [dbo].[Port] [Port_1] on [Port_1].[id] = [InvoiceExterior].[id_portShipment]
  left outer join [dbo].[City] [City_1] on [Port_1].[id_city] = [City_1].[id]
  left outer join [dbo].[Port] [Port_2] on [Port_2].[id] = [InvoiceExterior].[id_portDestination]
  left outer join [dbo].[City] [City_2] on [City_2].[id] = [Port_2].[id_city]    
  left outer join [dbo].[Country] coun2 on coun2.[id] = [City_2].[id_country]   
  left outer join country couloa on couloa.id = City_1.id_country
  left outer join [dbo].[Country] [Country_Destination] on [Country_Destination].[id] = [City_2].[id_country]
  LEFT OUTER JOIN [dbo].[Document] [DocProf] ON [DocProf].[id] = [doc].[id_documentOrigen]
  LEFT OUTER JOIN [dbo].[SalesQuotationExterior][salesQuotationExterior] ON [salesQuotationExterior].[id] = [DocProf].[id]

  where [Invoice].[id] = convert(int,@id)   

/*
	EXEC spPar_NomWoodCR @id=936666
*/  
GO