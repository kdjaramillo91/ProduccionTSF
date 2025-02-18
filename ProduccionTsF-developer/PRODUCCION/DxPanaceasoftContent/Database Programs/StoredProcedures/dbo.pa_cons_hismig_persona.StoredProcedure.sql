If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'pa_cons_hismig_persona'
	)
Begin
	Drop Procedure dbo.pa_cons_hismig_persona
End
Go
Create Procedure dbo.pa_cons_hismig_persona
As
SET NOCOUNT ON



SELECT CONVERT(BIGINT,a.[id]) AS id_MigracionHistorial
, CONVERT(BIGINT,a.[id_migrationPerson]) AS id_MigracionPersona
, CONVERT(VARCHAR(255),b.[fullname_businessName]) AS NombrePersona
, CONVERT(VARCHAR(10),CONVERT(date, a.[dateCreate] )) AS FechaMigracion
, CONVERT(VARCHAR(8),CONVERT(time, a.[dateCreate])) AS HoraMigracion
, CONVERT(VARCHAR(1000),a.[message]) AS Mensaje
, CASE WHEN CHARINDEX('Migrado satisfactoriamente',a.[message]) > 0 THEN 'OK' 
ELSE SUBSTRING(a.[message],CHARINDEX('Error no esperado', a.[message]), (LEN(a.[message]) - CHARINDEX('Error no esperado', a.[message])))
END AS MensajeRespuesta
, (CONVERT(BIT,(CASE WHEN CHARINDEX('Migrado satisfactoriamente',a.[message]) > 0 THEN 1 ELSE 0 END))) AS Migrado
, a.[mode] AS Modo, c.[username] AS UsuarioReplicador
, d.[name] AS Rol
FROM [dbo].[HistoryMigrationPerson] a
JOIN [dbo].[Person] b ON a.id_person = b.id
LEFT OUTER JOIN [dbo].[User] c ON a.id_user_replicate = c.id
LEFT OUTER JOIN [dbo].[Rol] d ON a.[id_rol] = d.[id]
--ORDER BY [FechaMigracion] DESC, [HoraMigracion] DESC



Go
