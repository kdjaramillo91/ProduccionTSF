@Echo Off

Echo Procesando las actualizaciones al modelo...

For %%i In ( Upgrades\*.sql ) Do (
	Echo - Procesando %%~nxi
	SQLCMD %* -i %%i -o %%i.out
)

Echo Procesando las funciones de usuario...

For %%i In ( Functions\*.sql ) Do (
	Echo - Procesando %%~nxi
	SQLCMD %* -i %%i -o %%i.out
)

Echo Procesando los procedimientos almacenados...

For %%i In ( StoredProcedures\*.sql ) Do (
	Echo - Procesando %%~nxi
	SQLCMD %* -i %%i -o %%i.out
)

Echo Proceso finalizado.
Pause
