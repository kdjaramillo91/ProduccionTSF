If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'Par_Integration_ProcessGroup'
	)
Begin
	Drop Procedure dbo.Par_Integration_ProcessGroup
End
Go
Create Procedure dbo.Par_Integration_ProcessGroup
(
	@id int
)	
As
set nocount on 
select	CODE_LOTE,  
		TIPO_INTEGRACION, 
		ESTADO, 
		FECHA_CONTABILIZACION,
		OBSERVACION, 		
		TOTAL_LOTE_INTEGRACION,
		NUMERO_DOCUMENTOS,
		descriptionGroup,
		valueTotal,
		id_company   
FROM 
(
   select 
	   IntegrationProcess.codeLote CODE_LOTE,  
	   DocumentType.NAME TIPO_INTEGRACION, 
	   IntegrationState.NAME ESTADO, 
	   IntegrationProcess.dateAccounting FECHA_CONTABILIZACION,
	   IntegrationProcess.description OBSERVACION, 	   
	   IntegrationProcess.totalValue TOTAL_LOTE_INTEGRACION,
	   IntegrationProcess.countTotalDocuments NUMERO_DOCUMENTOS,	  
	   c.descriptionGroup,
	   c.valueTotal,
	   IntegrationProcess.id_company
	   
   FROM IntegrationProcess IntegrationProcess
   INNER JOIN IntegrationState IntegrationState 
   ON IntegrationState.id = IntegrationProcess.id_StatusLote
   INNER JOIN  DocumentType DocumentType 
   ON  DocumentType.ID = IntegrationProcess.id_DocumentType
   INNER JOIN  IntegrationProcessPrintGroup c on 
   c.id_Lote = IntegrationProcess.id
 
   WHERE    IntegrationProcess.id = @id			
	
)as a

 
Go
