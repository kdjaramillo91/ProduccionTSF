
-- Tabla de configuracion de Proceso

alter table TransCtlDocumentTypeConfigDetail
drop constraint FK_TransCtlDocumentTypeDetail_TransCtl;
--FOREIGN key(TransCtlConfigId) references TransCtlDocumentTypeConfig(Id);



--drop  table TransCtlDocumentTypeConfig;
create table TransCtlDocumentTypeConfig
(
	Id				int not null,  -->  DocumentType Id
	DtName			varchar(max) not null,
	Peso			int not null default(0),  -- Manejo de peso/costo de documento/transaccion en el conjunto de las demas transacciones
	EstTimePerform	int not null default(0), -- Estadisticas de tiempo ejecucion tipo de documento
	Controller		varchar(max) not null,
	Method			varchar(50) not null,
	CodeStateOK		varchar(20)	 not null,
	CodeStateError	varchar(20)	 not null,
	primary Key (Id)
);

-- Detalle, tabla prioridad
--drop  table TransCtlDocumentTypeConfigDetail;
Create table TransCtlDocumentTypeConfigDetail
(
	Id int identity not null,
	TransCtlConfigId int not null,
	TableName varchar(35) not null,
	OrderExec int not null,
	Stage varchar(8) not null,

	primary Key (Id)
);


alter table TransCtlDocumentTypeConfigDetail
add constraint FK_TransCtlDocumentTypeDetail_TransCtl
FOREIGN key(TransCtlConfigId) references TransCtlDocumentTypeConfig(Id);

-- Cola de procesamiento
--drop table TransCtlQueue;
create table TransCtlQueue
(
	Identificador UNIQUEIDENTIFIER  not null,
	FechaInicio Datetime not null,	
	FechaFin Datetime null,
	TiempoEjecucion Time not null,
	Intentos int not null default(0),
	DocumentTypeId int not null,	
	DocumentId int not null,
	DocumentNumber varchar(max) not null,
	Stage varchar(8) not null,
	DataExecution varchar(max) null, -- Almacenar parametros en formato String[], en el orden que lo espera el metodo	
	DataExecutionTypes varchar(max) null, -- Almacenar Tipos de los parametros en formato String[]
	DataTempKeys varchar(max) null, -- almacena en un array string[], deseralizable las claves en el orden de los valores,	
	DataTempValues varchar(max) null, -- almacena en un array string[], deseralizable el modelo de los valores en el orden de las claves,
	DataTempTypes varchar(max) null, -- almacena en un array string[], deseralizable el tipo,	
	DataSession varchar(max) null, -- string JSON del modelo Session ActiveUserDto
	NumDetails int not null,
	QueueSec	int not null, -- secuencia de ejecucion 
	QueueEstate varchar(3) not null, -- PV: PEN PENDIENTE, EXE EJECUCION, FIN FINALIZADO
	primary Key (Identificador)
);

--drop table TransCtlQueueLog;
create table TransCtlQueueLog(
	Id int identity not null,
	Identificador UNIQUEIDENTIFIER  not null,
	QueueEstate varchar(3),
	DateTimeLog Datetime not null,
	InfoBlock varchar(Max) null,
	InfoEnvironment  varchar(Max) null,
	primary Key (Id)

);

--drop table TransCtlQueueHistory;
create table TransCtlQueueHistory
(
	Identificador UNIQUEIDENTIFIER  not null,
	FechaInicio Datetime not null,	
	TiempoEjecucion TimeStamp not null,
	Intentos int not null default(0),
	DocumentTypeId int not null,	
	DataExecution varchar(max) not null,
	QueueSec	int not null, -- secuencia de ejecucion 
	QueueEstate varchar(3) not null, -- PV: PEN PENDIENTE, EXE EJECUCION, FIN FINALIZADO
	primary Key (Identificador)
);
