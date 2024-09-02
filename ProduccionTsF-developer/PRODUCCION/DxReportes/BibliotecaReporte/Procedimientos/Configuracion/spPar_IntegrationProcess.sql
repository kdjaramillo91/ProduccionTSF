--exec spPar_IntegrationProcess @id=N'45438'
GO
/****** Object:  StoredProcedure [dbo].[Par_Integration_Process]    Script Date: 01/02/2023 11:58:48 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- exec Par_Integration_Process 25

CREATE procedure [dbo].[spPar_IntegrationProcess]

	@id int
	
as
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
		   NUMERO_DOCUMENTOS,
		   Logo,
		   Logo2
FROM 
(
   select 
	   IntegrationProcess.codeLote CODE_LOTE,  
	   DocumentType.NAME TIPO_INTEGRACION, 
	   IntegrationState.NAME ESTADO, 
	   IntegrationProcess.dateAccounting FECHA_CONTABILIZACION,
	   IntegrationProcess.description OBSERVACION, 
	   Document.number DOCUMENTO,
	   com.logo as Logo,
	   com.logo2 as Logo2,
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
     LEFT JOIN EmissionPoint emi on Document.id_emissionPoint = emi.id 
	 INNER join Company com on emi.id_company=com.id
   WHERE IntegrationStateDocument.code != '08' 
		 and  IntegrationProcess.id = @id
)as a

