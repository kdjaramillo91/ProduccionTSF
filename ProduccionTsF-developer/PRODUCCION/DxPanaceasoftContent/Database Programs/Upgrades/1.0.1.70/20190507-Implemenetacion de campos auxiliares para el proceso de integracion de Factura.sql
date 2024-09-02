If ColumnProperty(OBJECT_ID('dbo.IntegrationProcessOutput'), 'auxChar4', 'ColumnId') Is Null
Begin
	Alter Table dbo.IntegrationProcessOutput Add auxChar4 VarChar(255) Null
End
Go
If ColumnProperty(OBJECT_ID('dbo.IntegrationProcessOutput'), 'auxChar5', 'ColumnId') Is Null
Begin
	Alter Table dbo.IntegrationProcessOutput Add auxChar5 VarChar(255) Null
End
Go
If ColumnProperty(OBJECT_ID('dbo.IntegrationProcessOutput'), 'auxChar6', 'ColumnId') Is Null
Begin
	Alter Table dbo.IntegrationProcessOutput Add auxChar6 VarChar(255) Null
End
Go
If ColumnProperty(OBJECT_ID('dbo.IntegrationProcessOutput'), 'auxChar7', 'ColumnId') Is Null
Begin
	Alter Table dbo.IntegrationProcessOutput Add auxChar7 VarChar(255) Null
End
Go
If ColumnProperty(OBJECT_ID('dbo.IntegrationProcessOutput'), 'auxChar8', 'ColumnId') Is Null
Begin
	Alter Table dbo.IntegrationProcessOutput Add auxChar8 VarChar(255) Null
End
Go

