If Exists(
	Select	*
	From	sys.objects
	Where	name = 'FUN_CantidadConLetraCastellano'
	And		type = 'FN'
	)
Begin
	Drop Function dbo.FUN_CantidadConLetraCastellano
End
Go
Create Function dbo.FUN_CantidadConLetraCastellano
(
@Numero Decimal(19,4)
)
Returns Varchar(180)
As
BEGIN

DECLARE @lnEntero INT,
@lcRetorno VARCHAR(512),
@lnTerna INT,
@lcMiles VARCHAR(512),
@lcCadena VARCHAR(512),
@lnUnidades INT,
@lnDecenas INT,
@lnCentenas INT,
@lnFraccion INT

SELECT @lnEntero = CAST(@Numero AS INT),
@lnFraccion = (@Numero - @lnEntero) * 100,
@lcRetorno = '',
@lnTerna = 1

WHILE @lnEntero > 0
BEGIN /* WHILE */

-- Recorro columna por columna
SELECT @lcCadena =''
SELECT @lnUnidades = @lnEntero % 10
SELECT @lnEntero = CAST(@lnEntero/10 AS INT)
SELECT @lnDecenas = @lnEntero % 10
SELECT @lnEntero = CAST(@lnEntero/10 AS INT)
SELECT @lnCentenas = @lnEntero % 10
SELECT @lnEntero = CAST(@lnEntero/10 AS INT)

--print cast(@lnCentenas as varchar(3)) + cast(@lnDecenas as varchar(3))+ cast(@lnUnidades as varchar(3))
--print @lnEntero

-- Analizo las unidades
SELECT @lcCadena =
CASE /* UNIDADES */
WHEN @lnUnidades = 1 AND @lnTerna = 1 THEN 'UNO ' + @lcCadena
WHEN @lnUnidades = 1 AND @lnTerna <> 1 THEN 'UNO ' + @lcCadena
WHEN @lnUnidades = 2 THEN 'DOS ' + @lcCadena
WHEN @lnUnidades = 3 THEN 'TRES ' + @lcCadena
WHEN @lnUnidades = 4 THEN 'CUATRO ' + @lcCadena
WHEN @lnUnidades = 5 THEN 'CINCO ' + @lcCadena
WHEN @lnUnidades = 6 THEN 'SEIS ' + @lcCadena
WHEN @lnUnidades = 7 THEN 'SIETE ' + @lcCadena
WHEN @lnUnidades = 8 THEN 'OCHO ' + @lcCadena
WHEN @lnUnidades = 9 THEN 'NUEVE ' + @lcCadena
ELSE @lcCadena
END /* UNIDADES */

--print @lcCadena

-- Analizo las decenas
SELECT @lcCadena =
CASE /* DECENAS */
WHEN @lnDecenas = 1 THEN
CASE @lnUnidades
WHEN 0 THEN 'DIEZ '
WHEN 1 THEN 'ONCE '
WHEN 2 THEN 'DOCE '
WHEN 3 THEN 'TRECE '
WHEN 4 THEN 'CATORCE '
WHEN 5 THEN 'QUINCE '
WHEN 6 THEN 'DIECISEIS '
WHEN 7 THEN 'DIECISIETE '
WHEN 8 THEN 'DIECIOCHO '
WHEN 9 THEN 'DIECINUEVE '
--ELSE 'DIECI' + @lcCadena
END
WHEN @lnDecenas = 2 AND @lnUnidades = 0 THEN 'VEINTE ' + @lcCadena
WHEN @lnDecenas = 2 AND @lnUnidades <> 0 THEN 'VEINTE ' + @lcCadena
WHEN @lnDecenas = 3 AND @lnUnidades = 0 THEN 'TREINTA ' + @lcCadena
WHEN @lnDecenas = 3 AND @lnUnidades <> 0 THEN 'TREINTA ' + @lcCadena
WHEN @lnDecenas = 4 AND @lnUnidades = 0 THEN 'CUARENTA ' + @lcCadena
WHEN @lnDecenas = 4 AND @lnUnidades <> 0 THEN 'CUARENTA ' + @lcCadena
WHEN @lnDecenas = 5 AND @lnUnidades = 0 THEN 'CINCUENTA ' + @lcCadena
WHEN @lnDecenas = 5 AND @lnUnidades <> 0 THEN 'CINCUENTA ' + @lcCadena
WHEN @lnDecenas = 6 AND @lnUnidades = 0 THEN 'SESENTA ' + @lcCadena
WHEN @lnDecenas = 6 AND @lnUnidades <> 0 THEN 'SESENTA ' + @lcCadena
WHEN @lnDecenas = 7 AND @lnUnidades = 0 THEN 'SETENTA ' + @lcCadena
WHEN @lnDecenas = 7 AND @lnUnidades <> 0 THEN 'SETENTA ' + @lcCadena
WHEN @lnDecenas = 8 AND @lnUnidades = 0 THEN 'OCHENTA ' + @lcCadena
WHEN @lnDecenas = 8 AND @lnUnidades <> 0 THEN 'OCHENTA ' + @lcCadena
WHEN @lnDecenas = 9 AND @lnUnidades = 0 THEN 'NOVENTA ' + @lcCadena
WHEN @lnDecenas = 9 AND @lnUnidades <> 0 THEN 'NOVENTA ' + @lcCadena
ELSE @lcCadena
END /* DECENAS */

-- print @lcCadena

-- Analizo las centenas
SELECT @lcCadena =
CASE /* CENTENAS */
WHEN @lnCentenas = 1 AND @lnUnidades = 0 AND @lnDecenas = 0 THEN 'CIEN ' + @lcCadena
WHEN @lnCentenas = 1 AND NOT(@lnUnidades = 0 AND @lnDecenas = 0) THEN 'CIEN ' + @lcCadena
WHEN @lnCentenas = 2 THEN 'DOSCIENTOS ' + @lcCadena
WHEN @lnCentenas = 3 THEN 'TRESCIENTOS ' + @lcCadena
WHEN @lnCentenas = 4 THEN 'CUATROCIENTOS ' + @lcCadena
WHEN @lnCentenas = 5 THEN 'QUINIENTOS' + @lcCadena
WHEN @lnCentenas = 6 THEN 'SEISCIENTOS ' + @lcCadena
WHEN @lnCentenas = 7 THEN 'SETECIENTOS ' + @lcCadena
WHEN @lnCentenas = 8 THEN 'OCHOCIENTOS ' + @lcCadena
WHEN @lnCentenas = 9 THEN 'NOVECIENTOS ' + @lcCadena
ELSE @lcCadena
END /* CENTENAS */
--print @lcCadena

-- Analizo los millares
SELECT @lcCadena =
CASE /* TERNA */
WHEN @lnTerna = 1 THEN @lcCadena
WHEN @lnTerna = 2 AND (@lnUnidades + @lnDecenas + @lnCentenas <> 0) THEN @lcCadena + ' MIL, '
WHEN @lnTerna = 3 AND (@lnUnidades + @lnDecenas + @lnCentenas <> 0) AND
@lnUnidades = 1 AND @lnDecenas = 0 AND @lnCentenas = 0 THEN @lcCadena + ' MILLON, '
WHEN @lnTerna = 3 AND (@lnUnidades + @lnDecenas + @lnCentenas <> 0) AND
NOT (@lnUnidades = 1 AND @lnDecenas = 0 AND @lnCentenas = 0) THEN @lcCadena + ' MILLONES, '
WHEN @lnTerna = 4 AND (@lnUnidades + @lnDecenas + @lnCentenas <> 0) THEN @lcCadena + ' UN MIL MILLONES, '
ELSE''
END /* MILLARES */
--print @lcCadena

-- Armo el retorno columna a columna
--print @lcCadena
SELECT @lcRetorno = @lcCadena + @lcRetorno
SELECT @lnTerna = @lnTerna + 1

END /* WHILE */

IF @lnTerna = 1
SELECT @lcRetorno = 'CERO'

RETURN RTRIM(@lcRetorno)
END

Go
