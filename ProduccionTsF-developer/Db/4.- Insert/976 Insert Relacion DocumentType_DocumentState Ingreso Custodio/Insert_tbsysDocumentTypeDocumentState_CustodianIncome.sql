
-- creacion relacion tipo dcument Custodian Income y sus estados
insert into tbsysDocumentTypeDocumentState
(
	id_DocumentType
	,id_DocumenteState
	,orderState
	,previewState
	,nextState
	,isFinalState
)
values
( 1190, 1,0,0,0,0),
( 1190, 3,0,0,0,0),
( 1190, 5,0,0,0,0);
-- 1190	167	Ingreso de Custodio
-- 1	01	PENDIENTE
-- 3	03	APROBADA
-- 5	05	ANULADA