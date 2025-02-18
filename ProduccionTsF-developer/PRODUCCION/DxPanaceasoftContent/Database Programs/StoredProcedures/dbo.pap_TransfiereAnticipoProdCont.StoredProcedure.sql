If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'pap_TransfiereAnticipoProdCont'
	)
Begin
	Drop Procedure dbo.pap_TransfiereAnticipoProdCont
End
Go
Create Procedure dbo.pap_TransfiereAnticipoProdCont
(
	@str_IdsDocumentos	varchar(5000)
)
As
set nocount on 

--Declaración de Variables
declare @pos		integer
declare @IdDocTmp	integer
declare @iTmp		integer
--Tablas
declare @tmpIdsDocumentos	table
(
	[IdDocumento]	int NOT NULL
)
--Obtiene Parámetros de Stored Procedures
while charindex(',',@str_IdsDocumentos) > 0
begin
	set @pos = CHARINDEX(',',@str_IdsDocumentos)
	set @IdDocTmp = SUBSTRING(@str_IdsDocumentos,1,@pos-1)
	set @str_IdsDocumentos = SUBSTRING(@str_IdsDocumentos,@pos+1,LEN(@str_IdsDocumentos))

	select @iTmp = CONVERT(INTEGER,@IdDocTmp)

	select @iTmp = id from [dbo].[AdvanceProvider] with(nolock) where [id] = @iTmp

	if isnull(@iTmp,0) > 0
	begin
		insert into @tmpIdsDocumentos
		select @iTmp
	end
end

--Obtiene Parámetros de Proceso

--Chequeo si se puede insertar el registro

--Inserto Registros

--Retorna Mensaje


--Pruebas
--exec [pap_TransfiereAnticipoProdCont] ','
--select * from advanceprovider
--exec [pap_TransfiereAnticipoProdCont] '126714,127298,127325,127326,127327,128602,0,'
Go
