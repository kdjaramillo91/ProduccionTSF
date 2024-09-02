
GO
/****** Object:  StoredProcedure [dbo].[par_liquidacion_materiales_individual]    Script Date: 03/02/2023 8:58:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[spPar_LiquidacionMateriales]
(
	@id			Int,
	@detallado	Bit = 0
)
As
Begin
	Set NoCount On 

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
		,n.logo as Logo
		,n.logo2 as Logo2
		, Case When z.[code] != '06' Then 1 Else 0 End As [EsPreliminar]
		, Prg.[processPlant] as [Proceso]
	From [dbo].[LiquidationMaterialSuppliesDetail] a
	Join [dbo].[ResultReceptionDispatchMaterialDetail] h On a.[id] = h.[idLiquidationMaterialSuppliesDetail]
	Join [dbo].[LiquidationMaterialSupplies] b On a.[idLiquidationMaterialSupplies] = b.[id]
	Join [dbo].[Document] f On b.[id] = f.[id]
	Join [dbo].[EmissionPoint] m On f.[id_emissionPoint] = m.[id]
	Join [dbo].[Company] n On m.[id_company] = n.[id]
	Join [dbo].[ResultReceptionDispatchMaterial] g On b.[id] = g.[idLiquidationMaterialSupplies] And g.[idReceptionDispatchMaterial] = h.[idResultReceptionDispatchMaterial]
	Join [dbo].[ReceptionDispatchMaterials] i On g.[idReceptionDispatchMaterial] = i.[id]
	Join [dbo].[RemissionGuide] j On i.[id_remissionGuide] = j.[id]
	inner join [dbo].[Person] Prg On Prg.[id] = j.[id_personProcessPlant]
	Join [dbo].[Document] l On j.[id] = l.[id]
	Join [dbo].[ProductionUnitProvider] k On j.[id_productionUnitProvider] = k.[id]
	Join [dbo].[MetricUnit] e On a.[idMetricUnit] = e.[id]
	Join [dbo].[Item] c On a.[idItem] = c.[id]
	Join [dbo].[Person] d On b.[idProvider] = d.[id]
	Join [dbo].[Documentstate] z On z.[id] = f.[Id_DocumentState]

	Where b.[id] = @id
	And	  (@detallado = 1 Or a.[quantity] != 0)
End

