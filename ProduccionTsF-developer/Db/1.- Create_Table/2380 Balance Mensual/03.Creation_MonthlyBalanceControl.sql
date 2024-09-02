drop table [dbo].[MonthlyBalanceControl]

create table [dbo].[MonthlyBalanceControl]
(
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_company] [int] NOT NULL,
	[id_warehouse] [int] NOT NULL,
	[Anio] [int] NOT NULL,
	[Mes] [int] NOT NULL,
	[IsValid] [bit] NOT NULL default(0),
	[DateIsNotValid] [DateTime] NULL,	
	[LastDateProcess]  [DateTime] NOT NULL,

	CONSTRAINT [PK_MonthlyBalanceControl] PRIMARY KEY CLUSTERED 
    (
    	[id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF ) ON [PRIMARY]
)