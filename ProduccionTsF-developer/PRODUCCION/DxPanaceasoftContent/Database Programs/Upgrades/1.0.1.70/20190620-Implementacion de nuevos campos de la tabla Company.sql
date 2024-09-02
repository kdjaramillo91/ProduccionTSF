If (Select count(column_name) from information_schema.columns
	where column_name = 'plantCode' and table_name = 'Company') = 0
Begin
	Alter Table Company Add plantCode Varchar(10)
End
go
If (Select count(column_name) from information_schema.columns
	where column_name = 'registryFDA' and table_name = 'Company') = 0
Begin
	Alter Table Company Add registryFDA Varchar(15)
End
go
If (Select count(column_name) from information_schema.columns
	where column_name = 'websiteCompany' and table_name = 'Company') = 0
Begin
	Alter Table Company Add websiteCompany Varchar(250)
End

go
If (Select count(column_name) from information_schema.columns
	where column_name = 'id_city' and table_name = 'ForeignCustomer') = 0
Begin
	Alter Table ForeignCustomer Add id_city int 
End

If	Not Exists(
		Select	*
		From	sys.foreign_keys
		Where	name = 'FK_ForeignCustomer_City'
		And		parent_object_id = Object_ID('ForeignCustomer')
	)
Begin
	Alter Table ForeignCustomer 
	Add Constraint FK_ForeignCustomer_City Foreign Key (id_city)
	References City(id)
End



