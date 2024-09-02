CREATE PROCEDURE dbo.spPilotoCrytalReport
	@id int
AS
	SELECT
		dt.code 'Codigo',
		dt.name 'Nombre',
		ex.direccion 'Direccion',
		p.cellPhoneNumberPerson 'NumeroTelefono'
	FROM Document d
	INNER JOIN DocumentType dt ON d.id_documentType = dt.id
	INNER JOIN SalesQuotationExterior ex ON d.id = ex.id
	INNER JOIN Person p on ex.id_consignee = p.id
	WHERE d.id = @id

