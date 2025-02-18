If Exists(
	Select	*
	From	sys.objects
	Where	name = 'FUN_CantidadConLetra'
	And		type = 'FN'
	)
Begin
	Drop Function dbo.FUN_CantidadConLetra
End
Go
Create Function dbo.FUN_CantidadConLetra
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
WHEN @lnUnidades = 1 AND @lnTerna = 1 THEN 'ONE ' + @lcCadena
WHEN @lnUnidades = 1 AND @lnTerna <> 1 THEN 'ONE ' + @lcCadena
WHEN @lnUnidades = 2 THEN 'TWO ' + @lcCadena
WHEN @lnUnidades = 3 THEN 'THREE ' + @lcCadena
WHEN @lnUnidades = 4 THEN 'FOUR ' + @lcCadena
WHEN @lnUnidades = 5 THEN 'FIVE ' + @lcCadena
WHEN @lnUnidades = 6 THEN 'SIX ' + @lcCadena
WHEN @lnUnidades = 7 THEN 'SEVEN ' + @lcCadena
WHEN @lnUnidades = 8 THEN 'EIGHT ' + @lcCadena
WHEN @lnUnidades = 9 THEN 'NINE ' + @lcCadena
ELSE @lcCadena
END /* UNIDADES */

--print @lcCadena

-- Analizo las decenas
SELECT @lcCadena =
CASE /* DECENAS */
WHEN @lnDecenas = 1 THEN
CASE @lnUnidades
WHEN 0 THEN 'TEN '
WHEN 1 THEN 'ELEVEN '
WHEN 2 THEN 'TWELVE '
WHEN 3 THEN 'THIRTEEN '
WHEN 4 THEN 'FOORTEEN '
WHEN 5 THEN 'FIFTEEN '
WHEN 6 THEN 'SIXTEEN '
WHEN 7 THEN 'SEVENTEEN '
WHEN 8 THEN 'EIGHTEEN '
WHEN 9 THEN 'NINETEEN '
--ELSE 'DIECI' + @lcCadena
END
WHEN @lnDecenas = 2 AND @lnUnidades = 0 THEN 'TWENTY ' + @lcCadena
WHEN @lnDecenas = 2 AND @lnUnidades <> 0 THEN 'TWENTY ' + @lcCadena
WHEN @lnDecenas = 3 AND @lnUnidades = 0 THEN 'THIRTY ' + @lcCadena
WHEN @lnDecenas = 3 AND @lnUnidades <> 0 THEN 'THIRTY ' + @lcCadena
WHEN @lnDecenas = 4 AND @lnUnidades = 0 THEN 'FORTY ' + @lcCadena
WHEN @lnDecenas = 4 AND @lnUnidades <> 0 THEN 'FORTY ' + @lcCadena
WHEN @lnDecenas = 5 AND @lnUnidades = 0 THEN 'FIFTY ' + @lcCadena
WHEN @lnDecenas = 5 AND @lnUnidades <> 0 THEN 'FIFTY ' + @lcCadena
WHEN @lnDecenas = 6 AND @lnUnidades = 0 THEN 'SIXTY ' + @lcCadena
WHEN @lnDecenas = 6 AND @lnUnidades <> 0 THEN 'SIXTY ' + @lcCadena
WHEN @lnDecenas = 7 AND @lnUnidades = 0 THEN 'SEVENTY ' + @lcCadena
WHEN @lnDecenas = 7 AND @lnUnidades <> 0 THEN 'SEVENTY ' + @lcCadena
WHEN @lnDecenas = 8 AND @lnUnidades = 0 THEN 'EIGHTY ' + @lcCadena
WHEN @lnDecenas = 8 AND @lnUnidades <> 0 THEN 'EIGHTY ' + @lcCadena
WHEN @lnDecenas = 9 AND @lnUnidades = 0 THEN 'NINETY ' + @lcCadena
WHEN @lnDecenas = 9 AND @lnUnidades <> 0 THEN 'NINETY ' + @lcCadena
ELSE @lcCadena
END /* DECENAS */

-- print @lcCadena

-- Analizo las centenas
SELECT @lcCadena =
CASE /* CENTENAS */
WHEN @lnCentenas = 1 AND @lnUnidades = 0 AND @lnDecenas = 0 THEN 'ONE HUNDRED ' + @lcCadena
WHEN @lnCentenas = 1 AND NOT(@lnUnidades = 0 AND @lnDecenas = 0) THEN 'ONE HUNDRED ' + @lcCadena
WHEN @lnCentenas = 2 THEN 'TWO HUNDRED ' + @lcCadena
WHEN @lnCentenas = 3 THEN 'THREE HUNDRED ' + @lcCadena
WHEN @lnCentenas = 4 THEN 'FOUR HUNDRED ' + @lcCadena
WHEN @lnCentenas = 5 THEN 'FIVE HUNDRED ' + @lcCadena
WHEN @lnCentenas = 6 THEN 'SIX HUNDRED ' + @lcCadena
WHEN @lnCentenas = 7 THEN 'SEVEN HUNDRED ' + @lcCadena
WHEN @lnCentenas = 8 THEN 'EIGHT HUNDRED ' + @lcCadena
WHEN @lnCentenas = 9 THEN 'NINE HUNDRED ' + @lcCadena
ELSE @lcCadena
END /* CENTENAS */
--print @lcCadena

-- Analizo los millares
SELECT @lcCadena =
CASE /* TERNA */
WHEN @lnTerna = 1 THEN @lcCadena
WHEN @lnTerna = 2 AND (@lnUnidades + @lnDecenas + @lnCentenas <> 0) THEN @lcCadena + ' THOUSAND, '
WHEN @lnTerna = 3 AND (@lnUnidades + @lnDecenas + @lnCentenas <> 0) AND
@lnUnidades = 1 AND @lnDecenas = 0 AND @lnCentenas = 0 THEN @lcCadena + ' MILLION, '
WHEN @lnTerna = 3 AND (@lnUnidades + @lnDecenas + @lnCentenas <> 0) AND
NOT (@lnUnidades = 1 AND @lnDecenas = 0 AND @lnCentenas = 0) THEN @lcCadena + ' MILLIONS, '
WHEN @lnTerna = 4 AND (@lnUnidades + @lnDecenas + @lnCentenas <> 0) THEN @lcCadena + ' ONE THOUSAND MILLIONS, '
ELSE''
END /* MILLARES */
--print @lcCadena

-- Armo el retorno columna a columna
--print @lcCadena
SELECT @lcRetorno = @lcCadena + @lcRetorno
SELECT @lnTerna = @lnTerna + 1

END /* WHILE */

IF @lnTerna = 1
SELECT @lcRetorno = 'ZERO'

RETURN RTRIM(@lcRetorno)
END

Go
