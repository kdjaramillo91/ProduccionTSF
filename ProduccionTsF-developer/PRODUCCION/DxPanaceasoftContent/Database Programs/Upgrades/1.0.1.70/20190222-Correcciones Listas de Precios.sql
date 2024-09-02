/*
 Script de Correcciones de Novedades reportadas a Listas de Precios
 Fecha: 2019-02-22
 */


If Not Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.UQ_RegisterInfReplicationSource') And type = 'UQ')
Begin
	Alter Table dbo.RegisterInfReplicationSource
		Add Constraint UQ_RegisterInfReplicationSource Unique (idRegisterSource)
End

If Not Exists (Select * From sys.indexes where name = 'IX_ReplicationMasterProduction' And object_id = OBJECT_ID('dbo.ReplicationMasterProduction'))
	Create Index IX_ReplicationMasterProduction On dbo.ReplicationMasterProduction(idPrincipalSchemaDestination)
Go
If Not Exists (Select * From sys.indexes where name = 'IX_ReplicationMasterProduction_1' And object_id = OBJECT_ID('dbo.ReplicationMasterProduction'))
	Create Index IX_ReplicationMasterProduction_1 On dbo.ReplicationMasterProduction(idPrincipalSchemaSource)
Go

If Not Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.PK_MailConfiguration') And type = 'PK')
Begin
	Alter Table dbo.MailConfiguration
		Add Constraint PK_MailConfiguration Primary Key (id)
End

If Not Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.PK_RepoCompany') And type = 'PK')
Begin
	Alter Table dbo.RepoCompany
		Add Constraint PK_RepoCompany Primary Key (idCompany)
End

If Not Exists (Select * From sys.objects Where object_id = OBJECT_ID('dbo.PK_RepoKardexSaldo') And type = 'PK')
Begin
	Alter Table dbo.RepoKardexSaldo
		Add Constraint PK_RepoKardexSaldo Primary Key (idDetalleInventario)
End
