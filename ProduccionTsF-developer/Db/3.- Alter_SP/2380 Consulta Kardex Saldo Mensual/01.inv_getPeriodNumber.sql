-- drop function inv_getPeriodNumber

create function dbo.inv_getPeriodNumber( @codeTypePeriod char(1), @fechaDocumento Datetime)
RETURNS int
 as
begin
	return case @codeTypePeriod
		when 'D' then DATEPART(dayofyear, @fechaDocumento)
		when 'M' then month(@fechaDocumento)
	end	
end