-- Tipo Documento Generacion Saldo Inventario
--select  * from DocumentType
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
values(169, 'Generación Saldo Inventario','Generación Saldo Inventario',1,0,0,'',2,1,1, GETDATE(),1,GETDATE() );


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
values('69','ERROR','ERROR',2,1,1,GETDATE(),1,GETDATE());