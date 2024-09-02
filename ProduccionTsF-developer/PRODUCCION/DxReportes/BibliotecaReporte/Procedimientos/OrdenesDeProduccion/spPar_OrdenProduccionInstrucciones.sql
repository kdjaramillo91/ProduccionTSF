/****** Object:  StoredProcedure [dbo].[OrdenProduccionInstruccionesReport]    Script Date: 16/06/2023 10:47:12 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

-- SP / Query Original: OrdenProduccionInstruccionesReport
CREATE OR ALTER PROCEDURE [dbo].[spPar_OrdenProduccionInstrucciones]
	@id INT
AS
	SET NOCOUNT ON

select So.id, document, copy, digital 
from SalesOrder So
Inner Join SalesOrderDetailInstructions Soi On Soi.id_salesOrder = So.id
Where So.id = @id


/*
	EXEC spPar_OrdenProduccionInstrucciones 0
*/

GO