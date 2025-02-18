If Exists(
	Select	*
	From	sys.objects
	Where	name = 'SplitInts'
	And		type = 'IF'
	)
Begin
	Drop Function dbo.SplitInts
End
Go
Create Function dbo.SplitInts
(
	@List      VARCHAR(MAX),
	@Delimiter VARCHAR(255)
)
Returns TABLE
As
	
	  RETURN ( SELECT id = CONVERT(INT, id) FROM
		  ( SELECT id = x.i.value('(./text())[1]', 'varchar(max)')
			FROM ( SELECT [XML] = CONVERT(XML, '<i>'
			+ REPLACE(@List, @Delimiter, '</i><i>') + '</i>').query('.')
			  ) AS a CROSS APPLY [XML].nodes('i') AS x(i) ) AS y
		  WHERE id IS NOT NULL
	  );
 
Go
