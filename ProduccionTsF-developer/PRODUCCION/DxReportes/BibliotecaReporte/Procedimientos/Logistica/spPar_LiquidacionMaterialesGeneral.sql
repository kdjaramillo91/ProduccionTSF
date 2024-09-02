/****** Object:  StoredProcedure [dbo].[spPar_LiquidacionMaterialesGeneral]    Script Date: 08/06/2023 02:49:31 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROC [dbo].[spPar_LiquidacionMaterialesGeneral]
(
	@dateInit		VarChar(10),
	@dateEnd		VarChar(10),
	@detallado		Bit = 0
)
As
Begin
	Set NoCount On 

	-- Selección de documentos en el rango de fechas indicado...
	Declare @docs Table ( id Int Primary Key )

	If (@dateInit <> '') And (@dateEnd <> '')
	Begin
		-- Se indicó fecha inicial y final...
		Insert	Into @docs
		Select	f.[id]
		From	[dbo].[Document] f
		Where	Convert(Date, f.[emissionDate]) Between Convert(DateTime, @dateInit, 103) And Convert(DateTime, @dateEnd, 103)
	End

	Else If (@dateInit <> '')
	Begin
		-- Solo se indicó fecha inicial...
		Insert	Into @docs
		Select	f.[id]
		From	[dbo].[Document] f
		Where	Convert(Date, f.[emissionDate]) >= Convert(DateTime, @dateInit, 103)
	End

	Else If (@dateEnd <> '')
	Begin
		-- Solo se indicó fecha final...
		Insert	Into @docs
		Select	f.[id]
		From	[dbo].[Document] f
		Where	Convert(Date, f.[emissionDate]) <= Convert(DateTime, @dateEnd, 103)
	End

	Else
	Begin
		-- No se indicó ninguna fecha (TODOS)...
		Insert	Into @docs
		Select	f.[id]
		From	[dbo].[Document] f
	End


	-- Recuperación del resultado del reporte
	Select
		  b.[id] As [IdLiquidacionMateriales]
		, a.[id] As [IdLiquidacionMaterialesDetalle]
		, d.[id] As  [IdProveedor]
		, d.[fullname_businessName] As [NombreProveedor]
		, d.[identification_number] As [Ruc]
		, k.[INPnumber] As [INP]
		, f.[emissionDate] As [FechaLiquidacion]
		, Convert(VarChar(50), f.[sequential]) As [NumeroLiquidacion]
		, Convert(VarChar(50), l.[sequential]) As [SecuenciaGuia]
		, l.[emissionDate] As [FechaEmisionGuia]
		, c.[id] As [IdProducto]
		, c.[name] As [NombreProducto]
		, c.[mastercode] As [CodigoProducto]
		, a.[aprovedComertial] As [AprobacionComercial]
		, a.[aprovedLogist] As [AprobacionLogistica]
		, a.[descriptionComertial] As [DescripcionComercial]
		, a.[descriptionLogist] As [DescripcionLogistica]
		, a.[quantity] As [CantidadDetail]
		, Case When a.[aprovedComertial] = 1 Then a.[quantity] Else 0 End As [CantidadFacturada]
		, a.[priceUnit] As [PrecioUnitarioDetail]
		, Case When a.[aprovedComertial] = 1 Then a.[subTotal] Else 0 End As [SubTotalDetail]
		, Case When a.[aprovedComertial] = 1 Then a.[tax] Else 0 End As [TaxDetail]
		, Case When a.[aprovedComertial] = 1 Then a.[subTotalTax] Else 0 End As [SubTotalTaxDetail]
		, Case When a.[aprovedComertial] = 1 Then a.[total] Else 0 End As [TotalDetail]
		, m.[id] As [IdCompania]
		, n.[address] As [Direccion]
		, n.[businessName] As [NombreCompania]
		, n.[phoneNumber] As [Telefono]
		, e.[name] As [Unidad_Medida]
		, n.[ruc] As [Ruc_Compania]
		, z.[name] As [Estado]
		, pRg.[processPlant] As [Proceso]
		, CASE WHEN ISNULL(@dateInit, '') = '' THEN '' ELSE CONVERT(varchar(10), @dateInit, 103) END AS DateInit
		, CASE WHEN ISNULL(@dateEnd, '') = '' THEN '' ELSE CONVERT(varchar(10), @dateEnd, 103) END AS DateEnd
	From [dbo].[LiquidationMaterialSuppliesDetail] a
	Join [dbo].[ResultReceptionDispatchMaterialDetail] h On a.[id] = h.[idLiquidationMaterialSuppliesDetail]
	Join [dbo].[LiquidationMaterialSupplies] b On a.[idLiquidationMaterialSupplies] = b.[id]
	Join [dbo].[Document] f On b.[id] = f.[id]
	Join [dbo].[EmissionPoint] m On f.[id_emissionPoint] = m.[id]
	Join [dbo].[Company] n On m.[id_company] = n.[id]
	Join [dbo].[ResultReceptionDispatchMaterial] g On b.[id] = g.[idLiquidationMaterialSupplies] And g.[idReceptionDispatchMaterial] = h.[idResultReceptionDispatchMaterial]
	Join [dbo].[ReceptionDispatchMaterials] i On g.[idReceptionDispatchMaterial] = i.[id]
	Join [dbo].[RemissionGuide] j On i.[id_remissionGuide] = j.[id]
	inner join [dbo].[Person] pRg On pRg.id = j.id_personProcessPlant
	Join [dbo].[Document] l On j.[id] = l.[id]
	Join [dbo].[ProductionUnitProvider] k On j.[id_productionUnitProvider] = k.[id]
	Join [dbo].[MetricUnit] e On a.[idMetricUnit] = e.[id]
	Join [dbo].[Item] c On a.[idItem] = c.[id]
	Join [dbo].[Person] d On b.[idProvider] = d.[id]
	Join [dbo].[Documentstate] z On z.[id] = f.[Id_DocumentState]

	Where f.[id] In ( Select id From @docs )
	And	  (@detallado = 1 Or a.[quantity] != 0)
End

/*
	EXEC spPar_LiquidacionMaterialesGeneral @dateInit=N'1/6/2022',@dateEnd=N'30/11/2022',@detallado=0
*/
GO