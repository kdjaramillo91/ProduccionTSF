SET NOCOUNT ON
GO

CREATE OR ALTER PROC spPar_CompaniaInfo
AS 

SELECT TOP 1
code AS [Codigo],
businessName AS [Nombre],
trademark AS [NombreComercial],
logo AS [Logo], 
address as [Direccion],
email as [Correo],
phoneNumber AS [NumeroTelefono]
FROM Company