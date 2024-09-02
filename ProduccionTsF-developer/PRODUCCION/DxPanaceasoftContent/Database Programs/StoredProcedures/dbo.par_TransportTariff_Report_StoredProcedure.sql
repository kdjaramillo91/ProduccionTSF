If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_TransportTariff_Report'
	)
Begin
	Drop Procedure dbo.par_TransportTariff_Report
End
Go
Create  PROCEDURE par_TransportTariff_Report
AS

	SELECT
		 CONVERT(VARCHAR(250),[transTariff].[name]) AS 'Tarifario'
		,CONVERT(VARCHAR(250),[transTariffType].[name]) AS 'TipoTarifario'
		,CONVERT(VARCHAR(10),[transTariff].[dateInit],103) AS 'FechaInicioVigencia'
		,CONVERT(VARCHAR(10),[transTariff].[dateEnd],103) AS 'FechaFinVigencia'
		,[fishingSite].[name] AS 'SitioCosecha'
		,[transTariffDetail].[tariff] AS 'Tarifa'
		,[cmp].[businessName] AS 'NombreCIA'
		,[cmp].[ruc] AS 'Ruc'
		--,(select [logo] from company where id = [transTariff].[id_company]) AS 'Logo'
		,[cmp].[phoneNumber] AS 'TelefonoCIA'
		,[transportSize].[name] AS 'Tamaño'
		,[transportSize].[id] AS 'OrdenTamaño'
	FROM [dbo].[TransportTariff][transTariff]
	INNER JOIN [dbo].[TransportTariffType][transTariffType]
		ON [transTariffType].[id] = [transTariff].[id_transportTariffType]
	INNER JOIN [dbo].[TransportTariffDetail][transTariffDetail]
		ON [transTariffDetail].[id_TransportTariff] = [transTariff].[id]
	INNER JOIN [dbo].[FishingSite][fishingSite]
		ON [fishingSite].[id] = [transTariffDetail].[id_FishingSite]
	INNER JOIN [dbo].[Company][cmp]
		ON [cmp].[id] = [transTariff].[id_company]
		AND [cmp].[id] = [transTariffType].[id_Company]
		AND [cmp].[id] = [transTariffDetail].[id_Company]
		AND [cmp].[id] = [fishingSite].[id_Company]
	LEFT OUTER JOIN [TransportSize][transportSize]
		ON [transportSize].[id] = [transTariffDetail].[id_transportSize]
	WHERE [TransTariffDetail].[isActive] = 1
	ORDER BY [transTariff].[name],[fishingSite].[name],[transportSize].[id]