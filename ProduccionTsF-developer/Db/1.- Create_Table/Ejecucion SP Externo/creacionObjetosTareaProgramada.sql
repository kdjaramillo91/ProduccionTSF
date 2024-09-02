
----------------------------------------
-- Tarea: Ejecución de Proceso Externo
-- Responsable: Ronny Avilés Morales
-- Fecha Entrega: 2023-10-26
-- Cliente: Proexpo
----------------------------------------

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
				WHERE TABLE_TYPE = 'BASE TABLE'
				AND TABLE_NAME LIKE 'JobScheduler')
Begin
	create table JobScheduler
	(
		id					int identity not null,
		dateInit			DateTime not null,
		dateEnd				DateTime not null,
		timeHourExecute		Time  null,
		serverHost			varchar(30) not null,
		databaseHost		varchar(30) not null,
		userdb				varchar(15) not null,
		passwordb			varchar(15) not null,
		storeProcedure		varchar(80) not null,
		id_documentState	int not null, 
		id_statusProcess	int not null,  
		dataResult			nvarchar(max) null,
		id_userCreate		int not null, 
		dateCreate			datetime not null, 
		id_userDelete		int null,
		dateDelete			datetime null, 
		dateInitExecution	datetime null, 
		dateEndExecution	datetime null, 
		primary key(id)
	);

	alter table JobScheduler
	ADD FOREIGN KEY (id_documentState) REFERENCES DocumentState(Id); 

End
Go

--------------------------------------------------------------------------------
--------------------------------------------------------------------------------
-- Documentación:
-- PV de id_statusProcess : 1 Enviado | 2 Iniciado | 3 Finalizado | 4 Fallido
-- TODO:
-- Controlar el estado del ultimo procesoque no se va a ejecutar si 
-- se baja el servicio de la publicacion y un job queda pendiente de ejecucion


