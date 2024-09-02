GO
/****** Object:  StoredProcedure [dbo].[par_Movimiento_Inventario_Todos]    Script Date: 03/02/2023 10:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[spPar_MovimientoInventarioTodos]
	@id int
as 
set nocount on 

Create Table #TempDocument(
	NNuSecuencia int,
	id_document int
)

Insert Into #TempDocument
select ltrim(ROW_NUMBER() OVER(order BY ds.id_document)) NNuSecuencia,
ds.id_document as id_document
From document d
inner join documentSource ds
   on d.[id] = ds.[id_documentOrigin]
inner join documentType dt
   on dt.id = d.id_documenttype
Where d.id = @id
  and dt.code = '95'

Create Table #TempDatos(
	id int,	
	TituloReporte varchar(50),
	Bodega varchar(50),
	Motivo varchar(50),
	FechaEmision DateTime,
	CodigoProducto varchar(50),
	DescripcionProducto	varchar(50),
	UnidadMedida varchar(50),
	CodigoUnidadMedida varchar(20),
	Cantidad decimal(9),
	NumeroSecuencia varchar(20),
	IdUbicacion	int,
	NombreUbicacion	varchar (250),
	CentroCosto varchar(250),
	SubCentroCosto	varchar(250),
	CodigoNaturaleza char(5),
	NombreNaturaleza varchar(250),
	SecuenciaGuiaRemision varchar(20),
	SecuenciaRequisicion varchar(20),
	SecuenciaLiquidacionMateriales varchar(20),
	IdCompania int,
	Descripcion varchar(50),
	Libras decimal(9),
	Kilos decimal(9)
)

declare @count int = 0,
  @countb int = 1
set @count = (select count( * ) from #TempDocument)
while @countb <= @count
begin
declare @ids int = (select top(1) id_document from #TempDocument where NNuSecuencia=@countb)
	insert into #TempDatos exec par_Movimiento_Inventario @ids 
set @countb = @countb + 1
end
	--select * from documentSource
	select * from #tempDatos