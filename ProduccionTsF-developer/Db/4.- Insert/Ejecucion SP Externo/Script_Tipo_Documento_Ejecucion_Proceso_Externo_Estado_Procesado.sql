-- Tipo Documento Ejecucion Proceso Externo
insert into DocumentType
(
	code
	,name
	,description
	,currentNumber
	,daysToExpiration
	,isElectronic
	,codeSRI
	,id_company
	,isActive
	,id_userCreate
	,dateCreate
	,id_userUpdate
	,dateUpdate
)
values(168, 'Ejecucion Proceso Externo','Ejecucion Proceso Externo',1,0,0,'',2,1,1, GETDATE(),1,GETDATE() )

-- Insertar estado Procesado, Proceso en segundo plano
insert into DocumentState
(
	code
	,name
	,description
	,id_company
	,isActive
	,id_userCreate
	,dateCreate
	,id_userUpdate
	,dateUpdate
)
values('66','PROCESADO','PROCESADO',2,1,1,GETDATE(),1,GETDATE());