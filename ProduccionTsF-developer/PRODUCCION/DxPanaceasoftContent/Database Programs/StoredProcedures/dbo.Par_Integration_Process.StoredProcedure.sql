If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'Par_Integration_Process'
	)
Begin
	Drop Procedure dbo.Par_Integration_Process
End
Go
Create Procedure dbo.Par_Integration_Process
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
		   DOCUMENTO,
		   FECHA_EMISION	,    
		   GlossData3 as GlossData1,
		   VALOR_DOCUMENTO,
		   TOTAL_LOTE_INTEGRACION,
		   NUMERO_DOCUMENTOS		    
FROM 
(
   select 
	   IntegrationProcess.codeLote CODE_LOTE,  
	   DocumentType.NAME TIPO_INTEGRACION, 
	   IntegrationState.NAME ESTADO, 
	   IntegrationProcess.dateAccounting FECHA_CONTABILIZACION,
	   IntegrationProcess.description OBSERVACION, 
	   Document.number DOCUMENTO,
	   Document.emissionDate FECHA_EMISION	,
	   CAST(substring(IntegrationProcessDetail.glossData,1,250)  as VARCHAR(250)) GlossData3,
	   IntegrationProcessDetail.totalValueDocument VALOR_DOCUMENTO,
	   IntegrationProcess.totalValue TOTAL_LOTE_INTEGRACION,
	   IntegrationProcess.countTotalDocuments NUMERO_DOCUMENTOS	  
   FROM IntegrationProcess IntegrationProcess
   INNER JOIN IntegrationState IntegrationState 
   ON IntegrationState.id = IntegrationProcess.id_StatusLote
   INNER JOIN  DocumentType DocumentType 
   ON  DocumentType.ID = IntegrationProcess.id_DocumentType
   left JOIN IntegrationProcessDetail IntegrationProcessDetail 
   ON IntegrationProcessDetail.id_Lote = IntegrationProcess.id
   left JOIN IntegrationState IntegrationStateDocument 
   ON IntegrationStateDocument.id = IntegrationProcessDetail.id_StatusDocument 
   left JOIN Document Document 
   ON Document.id = IntegrationProcessDetail.id_Document
   WHERE IntegrationStateDocument.code != '08' 
		 and  IntegrationProcess.id = @id
)as a

Go
