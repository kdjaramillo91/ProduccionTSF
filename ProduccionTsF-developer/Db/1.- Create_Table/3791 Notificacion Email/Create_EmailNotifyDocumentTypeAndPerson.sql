
-- Creacion Mantenimiento Envio de notificacion Email
-- drop table EmailNotifyDocumentType
create table EmailNotifyDocumentType
(
	id int identity not null,
	id_DocumentType int not null,
	[description] varchar(MAX),
	id_company int  not null,
	isActive bit not null default(1),
	id_userCreate int not null,
	dateCreate datetime  not null,
	id_userUpdate int not null,
	dateUpdate datetime  not null,
	constraint PK_EmailNotifyDocumentType primary key (id)
)

ALTER TABLE EmailNotifyDocumentType
ADD CONSTRAINT FK_EmailNotifyDocumentType_DocumentType
FOREIGN KEY (id_DocumentType) REFERENCES DocumentType(id); 

alter table EmailNotifyDocumentType
add constraint UN_EmailNotifyDocumentType_DocumentType
UNIQUE (id_DocumentType)
			   

--drop table  EmailNotifyDocumentTypePerson
create table EmailNotifyDocumentTypePerson
(
	id int identity not null,
	id_EmailNotifyDocumentType int not null,
	id_PersonReceiver int not null,
	constraint PK_EmailNotifyDocumentTypePerson primary key (id)
)
			   

ALTER TABLE EmailNotifyDocumentTypePerson
ADD CONSTRAINT FK_NotifyDocumentTypePerson_EmailNotifyDocument
FOREIGN KEY (id_EmailNotifyDocumentType) REFERENCES EmailNotifyDocumentType(id); 

ALTER TABLE EmailNotifyDocumentTypePerson
ADD CONSTRAINT FK_NotifyDocumentTypePerson_Person
FOREIGN KEY (id_PersonReceiver) REFERENCES Person(id); 

