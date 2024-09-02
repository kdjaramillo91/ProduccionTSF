
GO
/****** Object:  StoredProcedure [dbo].[par_prueba_escurrido]    Script Date: 20/01/2023 2:06:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create procedure [dbo].[spPar_PruebaDeEscurrido]

	@idDraining int
as
set nocount on


declare @dateDraining	datetime
declare @nameProvider varchar(200)
declare @nameProviderShrimp varchar(200)
declare @poolNumber			varchar(200)
declare @lotNumber			varchar(200)
declare @lotSystem			varchar(200)
declare @poundsRemitted		decimal(20,6)
declare @drawersNumbers		integer
declare @guideRemission		varchar(200)
declare @processPlant		varchar(200)
declare @poundsAverage		decimal(20,6)
declare @poundsProyected    decimal(20,6)
declare @fullNameBusisnessName varchar(200)


declare @numberSampling		integer
declare @countWeight		integer

declare @countColumnNumber	integer
declare @countRowNumber		integer
declare @orderTmp			integer
declare @quantityTmp		decimal(20,6)
declare @sqlCommand			varchar(1000)

declare @sqlCommand1			varchar(1000)
select 
@numberSampling = dt.[drawersNumberSampling],
@dateDraining = dt.[dateTimeTesting],
--@nameProvider = rplrd.[nameProvider],
--@nameProviderShrimp = rplrd.[nameProviderShrimp],
--@poolNumber = rplrd.[namePool],
--@lotNumber = rplrd.[numberLot],
--@lotSystem = rplrd.[numberLotSequential],
--@poundsRemitted = rplrd.[poundsRemitted],
--@drawersNumbers = rplrd.[drawersNumber],
--@guideRemission = rplrd.[numberRemissionGuide],
@poundsAverage = dt.[poundsAverage],
@poundsProyected = dt.[poundsProjected],
@fullNameBusisnessName = per.[fullname_businessName]
from [dbo].[DrainingTest] dt
--inner join [dbo].[ResultProdLotReceptionDetail] rplrd on rplrd.idDrainingTest = dt.id
--inner join [dbo].[ProductionLotDetail] pld on pld.id = rplrd.idProductionLotReceptionDetail
--inner join [dbo].[ProductionLot] pl on pl.id = pld.id_productionLot
inner join [dbo].[person] per on per.id = dt.idAnalist
where dt.id = @idDraining

declare @idReceptionDetail integer

select top 1 @idReceptionDetail = [idReceptionDetail]
from [dbo].[ReceptionDetailDrainingTest]
where [idDrainingTest] = @idDraining 

select top 1
@nameProvider = rplrd.[nameProvider],
@nameProviderShrimp = rplrd.[nameProviderShrimp],
@poolNumber = rplrd.[namePool],
@lotNumber = rplrd.[numberLot],
@lotSystem = rplrd.[numberLotSequential],
@poundsRemitted = rplrd.[poundsRemitted],
@drawersNumbers = rplrd.[drawersNumber],
@guideRemission = rplrd.[numberRemissionGuide],
@processPlant = process.[processPlant]
from  [dbo].[ResultProdLotReceptionDetail] rplrd 
inner join [dbo].[ProductionLotDetail] pld on pld.id = rplrd.idProductionLotReceptionDetail
inner join [dbo].[ProductionLot] pl on pl.id = pld.id_productionLot
inner join [dbo].[Person] process on process.id = pl.id_personProcessPlant
where rplrd.[idProductionLotReceptionDetail] = @idReceptionDetail


if object_id(N'#tblResult') is not null
	drop table #tblResult

create table #tblResult
(
	[dateDraining]		datetime,
	[nameProvider]		varchar(200),
	[nameProviderShrimp]varchar(200),
	[poolNumber]		varchar(200),
	[lotNumber]			varchar(200),
	[lotSystem]			varchar(200),
	[poundsRemitted]	decimal (20,6),
	[drawersNumbers]	int,
	[guideRemission]	varchar(200),
	[poundsAverage]		decimal (20,6),
	[poundsProyected]	decimal (20,6),
	[fullNameBusisnessName] varchar(200),
	[sampling]			int,
	[columnWeight1]		decimal (20,6),
	[poundsAverage1]	decimal (20,6),
	[columnWeight2]		decimal (20,6),
	[poundsAverage2]	decimal (20,6),
	[columnWeight3]		decimal (20,6),
	[poundsAverage3]	decimal (20,6),
	[columnWeight4]		decimal (20,6),
	[poundsAverage4]	decimal (20,6),
	[columnWeight5]		decimal (20,6),
	[poundsAverage5]	decimal (20,6),
	[processPlant]		varchar(200)
)

declare @tblDetail table
(
	[order]				int	 not null,
	[quantityWeigth]	decimal(20,6)
)

--fill table basic
declare @rowindex integer
select @rowindex = 1

while @rowindex <= 10
begin
	insert into #tblResult([sampling])
	values(@rowindex)

	select @rowindex = @rowindex + 1
end


-- Get Sampling Count
declare @samplingInformation table
(
	[id] int not null,
	[capacity] decimal(18,0),
	[idMetricUnitCapacity] int,
	[idrainingTest]	int,
	[drawersNumber] int,
	[poundsDrained]	decimal(18,0),
	[poundsAverage]	decimal(18,0),
	[poundsProjected] decimal(18,0)
)


insert into @samplingInformation(
[id],	[capacity],	[idMetricUnitCapacity], [idrainingTest], 
[drawersNumber], [poundsDrained], [poundsAverage], [poundsProjected])
select [id],	[capacity],	[idMetricUnitCapacity], [idrainingTest], 
[drawersNumber], [poundsDrained], [poundsAverage], [poundsProjected]
from [dbo].[DrainingTestSampling]
where [idrainingTest] = @idDraining

-- Get Information Detail 
if object_id(N'#detailInformation') is not null
	drop table #detailInformation

create table #detailInformation
(
	[id] int not null,
	[idOrder] int not null,
	[idDrainingTest] int,
	[order] int,
	[quantity] decimal(20,6),
	[idMetricUnit] int,
	[idDrainingTestSampling] int
)




declare @idRegisterSamplingDelete	integer, @idRegisterDetailDelete		integer,	@idRowNumber				integer
, @poundsAverageSampling	 decimal(18,0)
select @idRegisterSamplingDelete = 0, @idRowNumber = 1


select @countColumnNumber = 1
while ((select count(*) from @samplingInformation) > 0)
begin
	select top 1 @idRegisterSamplingDelete = [id], @poundsAverageSampling = [poundsAverage]
	from @samplingInformation

	insert into #detailInformation(
	[id], [idOrder],	[idDrainingTest],	[order],	[quantity],	[idMetricUnit],	[idDrainingTestSampling])
	select [id], ROW_NUMBER() OVER(ORDER BY [id]),	[idDrainingTest],	[order],	[quantity],	[idMetricUnit],	[idDrainingTestSampling] 
	from [dbo].[DrainingTestDetail] a 
	where [idDrainingTestSampling] = @idRegisterSamplingDelete

--	select @idRegisterSamplingDelete
	select @sqlCommand = ''
	select @sqlCommand = @sqlCommand +  'UPDATE a' + char(13)
	select @sqlCommand = @sqlCommand +  'set [columnWeight'+ rtrim(ltrim(@countColumnNumber)) +'] = b.[quantity]' + char(13)
	-- select @sqlCommand = @sqlCommand +  ', [poundsAverage'+ rtrim(ltrim(@countColumnNumber)) +'] = '+ convert(varchar,@poundsAverageSampling) + char(13)
	select @sqlCommand = @sqlCommand +  'from #tblResult a	join #detailInformation b on a.[sampling] = b.[idOrder]' + char(13)
	select @sqlCommand = @sqlCommand +  'where b.[idDrainingTestSampling] = ' + convert(varchar,@idRegisterSamplingDelete) + char(13)
	
	--print(@sqlCommand)
	exec(@sqlCommand)

	select @sqlCommand = ''
	select @sqlCommand = @sqlCommand +  'UPDATE a' + char(13)
	select @sqlCommand = @sqlCommand +  'set [poundsAverage'+ rtrim(ltrim(@countColumnNumber)) +'] = '+ convert(varchar,@poundsAverageSampling) + char(13)
	select @sqlCommand = @sqlCommand +  'from #tblResult a' + char(13)
	exec(@sqlCommand)


	truncate table #detailInformation

	delete from @samplingInformation
	where [id] = @idRegisterSamplingDelete

	select @idRowNumber = 1
	select @countColumnNumber = @countColumnNumber + 1
end


update #tblResult set 
	[dateDraining] = @dateDraining ,	
	[nameProvider] = @nameProvider,
	[nameProviderShrimp] =  @nameProviderShrimp , 
	[poolNumber] = @poolNumber,			
	[lotNumber]= @lotNumber,		
	[lotSystem]= @lotSystem,		
	[poundsRemitted] = @poundsRemitted,		
	[drawersNumbers] = @drawersNumbers,		
	[guideRemission] = @guideRemission,		
	[poundsAverage] = @poundsAverage ,	
	[poundsProyected] = @poundsProyected,   
	[fullNameBusisnessName] = @fullNameBusisnessName,
	[processPlant] = @processPlant

select * from #tblResult 
-- [dbo].[par_prueba_escurrido] 167068





