
/****** Object:  Table [dbo].[Account]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
SET DATEFORMAT YMD
GO
CREATE TABLE [dbo].[Account](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_parentAccount] [int] NULL,
	[number] [varchar](50) NOT NULL,
	[formatted_number] [varchar](250) NOT NULL,
	[name] [varchar](250) NULL,
	[alias] [varchar](250) NULL,
	[mnemonic] [varchar](50) NULL,
	[id_account_plan] [int] NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[isMovement] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountDetailAssistantType]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountDetailAssistantType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_account] [int] NOT NULL,
	[id_assistantType] [int] NOT NULL,
 CONSTRAINT [PK_AccountDetailAssistantType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AccountFor]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountFor](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_AccountFor] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountingAssistant]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountingAssistant](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_AccountingAssistant] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountingAssistantDetailType]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountingAssistantDetailType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_accountingAssistant] [int] NOT NULL,
	[id_assistantType] [int] NOT NULL,
	[codeAlternate] [varchar](20) NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_AccountingAssistantDetailType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountPlan]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountPlan](
	[id] [int] NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](250) NOT NULL,
	[description] [varchar](max) NULL,
	[separator] [varchar](50) NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_AccountPlan] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountType]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_AccountType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountTypeGeneral]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountTypeGeneral](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_AccountTypeGeneral] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ActivityRise]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ActivityRise](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_ActivityRise] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AddressType]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AddressType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isDefault] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_AddressType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AssistantType]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AssistantType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_AssistantType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BasisForGeneralDiscounts]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BasisForGeneralDiscounts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_BasisForGeneralDiscounts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BoxCardAndBank]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BoxCardAndBank](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[id_typeBoxCardAndBank] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_BoxCardAndBank] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BranchOffice]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BranchOffice](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_division] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[ruc] [varchar](20) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[code] [varchar](20) NULL,
	[address] [varchar](max) NOT NULL,
	[email] [varchar](250) NOT NULL,
	[phoneNumber] [varchar](50) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__BranchOf__3213E83F977E6960] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BusinessGroup]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BusinessGroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[logo] [image] NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__Business__3213E83FA5796D65] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CalendarPriceList]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CalendarPriceList](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[id_calendarPriceListType] [int] NOT NULL,
	[startDate] [date] NOT NULL,
	[endDate] [date] NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_CalendarPriceList] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CalendarPriceListType]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CalendarPriceListType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_CalendarPriceListType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CategoryActivityRise]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategoryActivityRise](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_categoryRise] [int] NOT NULL,
	[id_activityRise] [int] NOT NULL,
	[invoiceAmountRise] [decimal](14, 6) NOT NULL,
 CONSTRAINT [PK_CategoryActivityRise] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CategoryRise]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CategoryRise](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_CategoryRise] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[City]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[City](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_country] [int] NOT NULL,
	[id_stateOfContry] [int] NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isCapital] [bit] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Class]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Class](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NULL,
 CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ClassShrimp]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ClassShrimp](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](50) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_ClassShrimp] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Company]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Company](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_businessGroup] [int] NULL,
	[code] [varchar](20) NOT NULL,
	[ruc] [varchar](max) NOT NULL,
	[businessName] [varchar](max) NOT NULL,
	[trademark] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[logo] [image] NULL,
	[address] [varchar](max) NOT NULL,
	[email] [varchar](max) NOT NULL,
	[phoneNumber] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__Company__3213E83F63BC8D21] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ComparisonOperator]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ComparisonOperator](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](50) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_ComparisonOperator] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Country]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Country](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](5) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[id_origin] [int] NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Contry] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Country_IdentificationType]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country_IdentificationType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_country] [int] NOT NULL,
	[id_identificationType] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NULL,
 CONSTRAINT [PK_Country_IdentificationType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Customer]    Script Date: 29/10/2018 12:45:54 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Customer](
	[id] [int] NOT NULL,
	[id_customerType] [int] NOT NULL,
	[name_customerType] [varchar](100) NOT NULL,
	[forceToKeepAccountsCusm] [bit] NOT NULL,
	[specialTaxPayerCusm] [bit] NOT NULL,
	[id_vendorAssigned] [int] NULL,
	[name_vendorAssigned] [varchar](100) NULL,
	[id_economicGroupCusm] [int] NULL,
	[name_economicGroup] [varchar](100) NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
	[applyIva] [bit] NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[def_id_paymentMethod] [int] NULL,
	[def_id_paymentTerm] [int] NULL,
	[def_id_priceList] [int] NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_CustomerType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DataType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DataType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[purchase_decimalPlace] [int] NOT NULL,
	[sale_decimalPlace] [int] NOT NULL,
	[inventory_decimalPlace] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_DataType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Department]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Department](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](50) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DiscountToDetailApplyTo]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountToDetailApplyTo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_DiscountToDetailApplyTo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Division]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Division](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[id_company] [int] NOT NULL,
	[ruc] [varchar](max) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[address] [varchar](max) NOT NULL,
	[email] [varchar](max) NOT NULL,
	[phoneNumber] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__Division__3213E83F835CD7C2] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Document]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Document](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[number] [varchar](max) NOT NULL,
	[sequential] [int] NOT NULL,
	[emissionDate] [datetime] NOT NULL,
	[authorizationDate] [datetime] NULL,
	[authorizationNumber] [varchar](max) NULL,
	[accessKey] [varchar](max) NULL,
	[description] [varchar](max) NULL,
	[reference] [varchar](max) NULL,
	[id_emissionPoint] [int] NOT NULL,
	[id_documentType] [int] NOT NULL,
	[id_documentState] [int] NOT NULL,
	[id_BaseDocument] [int] NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__Document__3213E83FB28C8E48] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentDocumentState]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentDocumentState](
	[id_state] [int] NOT NULL,
	[id_documentType] [int] NOT NULL,
 CONSTRAINT [PK_DocumentDocumentState] PRIMARY KEY CLUSTERED 
(
	[id_state] ASC,
	[id_documentType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentState]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentState](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__Document__3213E83F3F9F1B40] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentStateChange]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentStateChange](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_document] [int] NOT NULL,
	[id_documentStateOld] [int] NOT NULL,
	[id_documentStateNew] [int] NOT NULL,
	[id_user] [int] NOT NULL,
	[id_userGroup] [int] NOT NULL,
	[changeTime] [datetime] NOT NULL,
 CONSTRAINT [PK_DocumentStateChange] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[currentNumber] [int] NOT NULL,
	[daysToExpiration] [int] NOT NULL,
	[isElectronic] [bit] NOT NULL,
	[codeSRI] [varchar](2) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__Document__3213E83F4D4DF3EB] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DrainingTest]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DrainingTest](
	[id] [int] NOT NULL,
	[idAnalist] [int] NOT NULL,
	[dateTesting] [date] NOT NULL,
	[timeTesting] [time](7) NOT NULL,
	[drawersNumberSampling] [int] NOT NULL,
	[poundsDrained] [decimal](20, 6) NOT NULL,
	[poundsAverage] [decimal](20, 6) NOT NULL,
	[poundsProjected] [decimal](20, 6) NOT NULL,
 CONSTRAINT [PK_DrainingTest] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DrainingTestDetail]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DrainingTestDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idDrainingTest] [int] NOT NULL,
	[order] [int] NOT NULL,
	[quantity] [decimal](20, 6) NOT NULL,
	[idMetricUnit] [int] NOT NULL,
 CONSTRAINT [PK_DrainingTestDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EconomicGroup]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EconomicGroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](5) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_EconomicGroup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_EconomicGroup] UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmissionPoint]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmissionPoint](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_branchOffice] [int] NOT NULL,
	[id_division] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[code] [int] NOT NULL,
	[address] [varchar](max) NOT NULL,
	[email] [varchar](max) NOT NULL,
	[phoneNumber] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__Emission__3213E83FF8E33CA8] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmissionType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmissionType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[codeSRI] [int] NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [text] NULL,
	[isActive] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_EmissionType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[id] [int] NOT NULL,
	[id_department] [int] NOT NULL,
 CONSTRAINT [PK__Employee__3213E83F9C4FD594] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EnvironmentType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EnvironmentType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[codeSRI] [int] NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_EnvironmentType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Escurrido]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Escurrido](
	[id] [int] NOT NULL,
	[id_employeAnalist] [int] NOT NULL,
 CONSTRAINT [PK_Escurrido] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EscurridoDetails]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EscurridoDetails](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_escurrido] [int] NOT NULL,
 CONSTRAINT [PK_EscurridoDetails] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GeneralContactData]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GeneralContactData](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_person] [int] NOT NULL,
	[id_rol] [int] NULL,
	[id_ContactType] [int] NOT NULL,
	[id_typeN1] [int] NULL,
	[id_typeN2] [int] NULL,
	[id_typeN3] [int] NULL,
	[id_typeN4] [int] NULL,
	[id_typeN5] [int] NULL,
	[contactData] [varchar](250) NOT NULL,
	[isDefault] [bit] NOT NULL,
	[isRequired] [bit] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NULL,
 CONSTRAINT [PK_GeneralContactData] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GroupPersonByRol]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GroupPersonByRol](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[id_rol] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_GroupPersonByRol] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GroupPersonByRolDetail]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupPersonByRolDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_groupPersonByRol] [int] NOT NULL,
	[id_person] [int] NOT NULL,
 CONSTRAINT [PK_GroupPersonByRolDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GroupPersonByRolhomologation]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupPersonByRolhomologation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Id_GroupPersonByRol] [int] NOT NULL,
	[Id_LPGrupo] [int] NOT NULL,
 CONSTRAINT [PK_GroupPersonByRolhomologation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[IdentificationType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[IdentificationType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[codeSRI] [varchar](2) NULL,
	[description] [varchar](max) NULL,
	[is_Active] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__Identifi__3213E83F4758263A] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Item]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[masterCode] [varchar](50) NOT NULL,
	[name] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ItemSaleInformation]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSaleInformation](
	[id_item] [int] NOT NULL,
	[salePrice] [decimal](14, 6) NULL,
 CONSTRAINT [PK_ItemSaleInformation] PRIMARY KEY CLUSTERED 
(
	[id_item] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemSize]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemSize](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[orderSize] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_ItemSize] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ItemSizeClass]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSizeClass](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_ItemSize] [int] NOT NULL,
	[id_Class] [int] NOT NULL,
 CONSTRAINT [PK_ItemSizeClass] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemSizeProcessPLOrder]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSizeProcessPLOrder](
	[id_ItemSize] [int] NOT NULL,
	[id_ProcessType] [int] NULL,
	[Order] [smallint] NULL,
 CONSTRAINT [PK_ItemSizeProcessPLOrder] PRIMARY KEY CLUSTERED 
(
	[id_ItemSize] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemSizeProcessTypePriceList]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSizeProcessTypePriceList](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_ProcessTypePriceList] [int] NOT NULL,
	[id_itemsize] [int] NOT NULL,
 CONSTRAINT [PK_ItemSizeProcessTypePriceList] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemTaxation]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemTaxation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_item] [int] NOT NULL,
	[percentage] [decimal](14, 6) NOT NULL,
 CONSTRAINT [PK_ItemTaxation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LiquidationMaterialSupplies]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LiquidationMaterialSupplies](
	[id] [int] NOT NULL,
	[idProvider] [int] NOT NULL,
	[subTotal] [decimal](22, 6) NOT NULL,
	[subTotalTax] [decimal](18, 6) NOT NULL,
	[Total] [decimal](22, 6) NOT NULL,
 CONSTRAINT [PK_LiquidationMaterialSupplies] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LiquidationMaterialSuppliesDetail]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LiquidationMaterialSuppliesDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idLiquidationMaterialSupplies] [int] NOT NULL,
	[idItem] [int] NOT NULL,
	[idMetricUnit] [int] NOT NULL,
	[quantity] [decimal](20, 6) NULL,
	[priceUnit] [decimal](20, 6) NOT NULL,
	[subTotal] [decimal](20, 6) NOT NULL,
	[tax] [decimal](10, 4) NOT NULL,
	[subTotalTax] [decimal](20, 6) NOT NULL,
	[total] [decimal](20, 6) NOT NULL,
	[description] [varchar](250) NOT NULL,
 CONSTRAINT [PK_LiquidationMaterialSuppliesDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LoginLog]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoginLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_user] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_division] [int] NOT NULL,
	[id_branchoffice] [int] NOT NULL,
	[id_emissionPoint] [int] NOT NULL,
	[dateLogin] [datetime] NOT NULL,
	[dateLogout] [datetime] NOT NULL,
 CONSTRAINT [PK_LoginLog] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MailConfiguration]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MailConfiguration](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[host] [varchar](350) NOT NULL,
	[mail] [varchar](350) NOT NULL,
	[userName] [varchar](50) NOT NULL,
	[password] [varchar](500) NOT NULL,
	[port] [int] NOT NULL,
	[enableSsl] [bit] NOT NULL,
	[isActive] [bit] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Menu](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[icon] [varchar](50) NULL,
	[id_controller] [int] NULL,
	[id_action] [int] NULL,
	[id_parent] [int] NULL,
	[position] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MetricType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MetricType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[id_dataType] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__MetricTy__3213E83F47ACA52B] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MetricUnit]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MetricUnit](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[id_metricType] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__MetricUn__3213E83F88F8B132] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MetricUnitConversion]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetricUnitConversion](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_metricOrigin] [int] NOT NULL,
	[id_metricDestiny] [int] NOT NULL,
	[factor] [decimal](14, 6) NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_MetricUnitConversion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Module]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Module](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](30) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ModuleTController]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ModuleTController](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_module] [int] NOT NULL,
	[id_tController] [int] NOT NULL,
 CONSTRAINT [PK_ModuleTController] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Notification]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Notification](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_user] [int] NOT NULL,
	[id_document] [int] NOT NULL,
	[noDocument] [varchar](max) NOT NULL,
	[id_documentType] [int] NOT NULL,
	[documentType] [varchar](max) NOT NULL,
	[id_documentState] [int] NOT NULL,
	[documentState] [varchar](max) NOT NULL,
	[title] [varchar](max) NOT NULL,
	[description] [varchar](max) NOT NULL,
	[dateTime] [datetime] NOT NULL,
	[reading] [bit] NOT NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Origin]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Origin](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Origin] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentMethod]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentMethod](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](50) NOT NULL,
	[codeSRI] [int] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
	[codeProgram] [char](2) NULL,
	[descriptionEnglish] [varchar](250) NULL,
 CONSTRAINT [PK_PumentMean] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentMethodPaymentTerm]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentMethodPaymentTerm](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_paymentMethod] [int] NOT NULL,
	[id_paymentTerm] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NULL,
 CONSTRAINT [PK_PaymentMethodPaymentTerm] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PaymentTerm]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentTerm](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
	[descriptionEnglish] [varchar](250) NULL,
	[firstPaymentDays] [int] NOT NULL,
	[numberPeriods] [int] NOT NULL,
	[id_PeriodType] [int] NULL,
	[formulaReady] [bit] NOT NULL,
	[PercentAnticipation] [decimal](5, 2) NOT NULL,
	[PercentBalance] [decimal](5, 2) NOT NULL,
 CONSTRAINT [PK_PymentMethod] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PenalityClassShrimp]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PenalityClassShrimp](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[byProvider] [bit] NOT NULL,
	[id_provider] [int] NULL,
	[id_groupPersonByRol] [int] NULL,
 CONSTRAINT [PK_PenalityClassShrimp] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PenalityClassShrimpDetails]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PenalityClassShrimpDetails](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_penalityClassShrimp] [int] NOT NULL,
	[id_classShrimp] [int] NOT NULL,
	[value] [decimal](12, 4) NOT NULL,
 CONSTRAINT [PK_PenalityClassShrimpDetails] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Permission]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Permission](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Person]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Person](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_personType] [int] NOT NULL,
	[id_identificationType] [int] NOT NULL,
	[identification_number] [varchar](20) NULL,
	[fullname_businessName] [varchar](max) NULL,
	[photo] [image] NULL,
	[address] [varchar](max) NULL,
	[email] [varchar](max) NULL,
	[id_company] [int] NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
	[codeCI] [varchar](8) NULL,
	[bCC] [varchar](max) NULL,
	[tradeName] [varchar](max) NULL,
	[cellPhoneNumberPerson] [varchar](20) NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PersonRol]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonRol](
	[id_person] [int] NOT NULL,
	[id_rol] [int] NOT NULL,
 CONSTRAINT [PK_PersonRol_1] PRIMARY KEY CLUSTERED 
(
	[id_person] ASC,
	[id_rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PersonType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PersonType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PoundsRange]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PoundsRange](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[id_metricUnit] [int] NOT NULL,
	[range_ini] [int] NOT NULL,
	[range_end] [int] NOT NULL,
	[id_suggestedIceBagRange] [int] NULL,
	[isActive] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Prcarga]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Prcarga](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Size] [varchar](50) NULL,
	[Proceso] [varchar](50) NULL,
	[Clase] [varchar](50) NULL,
	[gramage] [varchar](50) NULL,
	[porcentaje] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PrcargaCola]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PrcargaCola](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Size] [varchar](50) NULL,
	[Proceso] [varchar](50) NULL,
	[Clase] [varchar](50) NULL,
	[gramage] [varchar](50) NULL,
	[porcentaje] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PriceList]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PriceList](
	[id] [int] NOT NULL,
	[name] [varchar](max) NOT NULL,
	[startDate] [date] NOT NULL,
	[endDate] [date] NOT NULL,
	[isForPurchase] [bit] NOT NULL,
	[isForSold] [bit] NOT NULL,
	[isQuotation] [bit] NOT NULL,
	[id_calendarPriceList] [int] NOT NULL,
	[byGroup] [bit] NULL,
	[id_groupPersonByRol] [int] NULL,
	[id_priceListBase] [int] NULL,
	[id_userResponsable] [int] NULL,
	[list_idInventaryLineFilter] [varchar](max) NULL,
	[list_idItemTypeFilter] [varchar](max) NULL,
	[list_idItemGroupFilter] [varchar](max) NULL,
	[list_filterShow] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[id_processtype] [int] NULL,
	[commercialDate] [datetime] NULL,
 CONSTRAINT [PK__PriceLis__3213E83F718F4CAF] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PriceListClassShrimp]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceListClassShrimp](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_priceList] [int] NOT NULL,
	[id_classShrimp] [int] NOT NULL,
	[value] [decimal](12, 4) NOT NULL,
 CONSTRAINT [PK_PriceListClassShrimp] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PriceListItemSizeDetail]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceListItemSizeDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Id_PriceList] [int] NOT NULL,
	[Id_Itemsize] [int] NOT NULL,
	[price] [numeric](12, 4) NOT NULL,
	[commission] [numeric](12, 4) NOT NULL,
 CONSTRAINT [PK_PriceListItemSizeDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PriceListPersonPersonRol]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceListPersonPersonRol](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_PriceList] [int] NOT NULL,
	[id_Person] [int] NOT NULL,
	[id_Rol] [int] NOT NULL,
 CONSTRAINT [PK_PriceListPersonPersonRol] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProcessType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](20) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[isSystem] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NULL,
 CONSTRAINT [PK_ProcessType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ProcessType] UNIQUE NONCLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductionLot]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductionLot](
	[id] [int] NOT NULL,
	[id_ProductionLotState] [int] NOT NULL,
 CONSTRAINT [PK_ProductionLot] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductionLotDetail]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductionLotDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_productionLot] [int] NOT NULL,
	[quantitydrained] [decimal](20, 6) NULL,
 CONSTRAINT [PK_ProductionLotDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductionLotState]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductionLotState](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_ProductionLotState] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Provider]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Provider](
	[id] [int] NOT NULL,
	[id_paymentTerm] [int] NULL,
	[electronicDocumentIssuance] [bit] NOT NULL,
 CONSTRAINT [PK__Provider__3213E83F3F6626A2] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderAccountingAccounts]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderAccountingAccounts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_provider] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_division] [int] NOT NULL,
	[id_branchOffice] [int] NOT NULL,
	[id_accountFor] [int] NOT NULL,
	[id_accountPlan] [int] NOT NULL,
	[id_account] [int] NOT NULL,
	[id_accountingAssistantDetailType] [int] NOT NULL,
 CONSTRAINT [PK_ProviderAccountingAccounts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderGeneralData]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProviderGeneralData](
	[id_provider] [int] NOT NULL,
	[cod_alternate] [varchar](20) NULL,
	[id_providerType] [int] NULL,
	[accountExecutive] [varchar](max) NULL,
	[observation] [varchar](max) NULL,
	[reference] [varchar](max) NULL,
	[id_origin] [int] NULL,
	[id_country] [int] NULL,
	[id_city] [int] NULL,
	[id_stateOfContry] [int] NULL,
	[noFax] [varchar](max) NULL,
	[phoneNumber1] [varchar](max) NOT NULL,
	[phoneNumber2] [varchar](max) NULL,
	[contactName] [varchar](max) NULL,
	[contactPhoneNumber] [varchar](max) NULL,
	[contactNoFax] [varchar](max) NULL,
	[electronicPayment] [bit] NOT NULL,
	[rise] [bit] NOT NULL,
	[specialTaxPayer] [bit] NOT NULL,
	[forcedToKeepAccounts] [bit] NOT NULL,
	[habitualExporter] [bit] NOT NULL,
	[taxHavenExporter] [bit] NOT NULL,
	[sponsoredLinks] [bit] NOT NULL,
	[id_typeBoxCardAndBankAD] [int] NULL,
	[id_boxCardAndBankAD] [int] NULL,
	[id_economicGroup] [int] NULL,
	[cellPhoneNumber] [varchar](30) NOT NULL,
	[isCRAFTSMAN] [bit] NOT NULL,
 CONSTRAINT [PK_ProviderGeneralData] PRIMARY KEY CLUSTERED 
(
	[id_provider] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProviderGeneralDataEP]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProviderGeneralDataEP](
	[id_provider] [int] NOT NULL,
	[id_identificationTypeEP] [int] NULL,
	[id_paymentMethodEP] [int] NULL,
	[id_bankToBelieve] [int] NULL,
	[id_accountTypeGeneralEP] [int] NULL,
	[noAccountEP] [varchar](max) NULL,
	[noRefTransfer] [varchar](max) NULL,
	[id_rtInternational] [int] NULL,
	[noRoute] [varchar](max) NULL,
	[emailEP] [varchar](max) NULL,
 CONSTRAINT [PK_ProviderGeneralDataEP] PRIMARY KEY CLUSTERED 
(
	[id_provider] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProviderGeneralDataRise]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderGeneralDataRise](
	[id_provider] [int] NOT NULL,
	[id_categoryRise] [int] NULL,
	[id_activityRise] [int] NULL,
	[invoiceAmountRise] [decimal](14, 6) NULL,
 CONSTRAINT [PK_ProviderGeneralDataRise] PRIMARY KEY CLUSTERED 
(
	[id_provider] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderMailByComDivBra]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProviderMailByComDivBra](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_provider] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_division] [int] NOT NULL,
	[id_branchOffice] [int] NOT NULL,
	[email] [varchar](max) NOT NULL,
 CONSTRAINT [PK_ProviderMailByComDivBra] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProviderPassportImportData]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderPassportImportData](
	[id_provider] [int] NOT NULL,
	[appliesDoubleTaxationAgreementOnPayment] [bit] NOT NULL,
	[subjectRetentionApplicationLegalNorm] [bit] NOT NULL,
	[relatedParty] [bit] NOT NULL,
 CONSTRAINT [PK_ProviderPassportImportData] PRIMARY KEY CLUSTERED 
(
	[id_provider] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderPaymentMethod]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderPaymentMethod](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_provider] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_division] [int] NOT NULL,
	[id_branchOffice] [int] NOT NULL,
	[id_paymentMethod] [int] NOT NULL,
	[isPredetermined] [bit] NOT NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_ProviderPaymentMethod] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderPaymentTerm]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderPaymentTerm](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_provider] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_division] [int] NOT NULL,
	[id_branchOffice] [int] NOT NULL,
	[id_paymentTerm] [int] NOT NULL,
	[isPredetermined] [bit] NOT NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_ProviderPaymentTerm] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderPaymentTermMethod]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderPaymentTermMethod](
	[id_provider] [int] NOT NULL,
	[id_paymentTerm] [int] NOT NULL,
	[id_paymentMethod] [int] NOT NULL,
 CONSTRAINT [PK_ProviderPymentMethod] PRIMARY KEY CLUSTERED 
(
	[id_provider] ASC,
	[id_paymentTerm] ASC,
	[id_paymentMethod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderPersonAuthorizedToPayTheBill]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_provider] [int] NOT NULL,
	[id_identificationType] [int] NOT NULL,
	[identification_number] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[address] [varchar](max) NOT NULL,
	[phoneNumber1] [varchar](max) NOT NULL,
	[phoneNumber2] [varchar](max) NULL,
	[typeReg] [varchar](5) NULL,
	[codeProd] [varchar](5) NULL,
	[codeEmpr] [varchar](5) NULL,
	[type] [varchar](5) NULL,
	[id_country] [int] NULL,
	[id_bank] [int] NULL,
	[id_accountType] [int] NULL,
	[noAccount] [varchar](max) NULL,
	[amount] [decimal](14, 6) NULL,
	[noPayments] [int] NULL,
	[date] [datetime] NULL,
 CONSTRAINT [PK_ProviderPersonAuthorizedToPayTheBill] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProviderRelatedCompany]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderRelatedCompany](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_provider] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_division] [int] NOT NULL,
	[id_branchOffice] [int] NOT NULL,
 CONSTRAINT [PK_ProviderRelatedCompany] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderRetention]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderRetention](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_provider] [int] NOT NULL,
	[id_retentionType] [int] NOT NULL,
	[id_retentionGroup] [int] NOT NULL,
	[id_retention] [int] NOT NULL,
	[percentRetencion] [decimal](14, 6) NOT NULL,
 CONSTRAINT [PK_ProviderRetention] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderSeriesForDocuments]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProviderSeriesForDocuments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_provider] [int] NOT NULL,
	[id_documentType] [int] NOT NULL,
	[serialNumber] [varchar](20) NOT NULL,
	[authorizationNumber] [varchar](max) NOT NULL,
	[initialNumber] [int] NOT NULL,
	[finalNumber] [int] NOT NULL,
	[dateOfExpiry] [datetime] NOT NULL,
	[currentNumber] [int] NOT NULL,
	[id_retentionSeriesForDocumentsType] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_ProviderSeriesForDocuments] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProviderType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProviderType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
	[isShrimpPerson] [bit] NOT NULL,
	[isTransportist] [bit] NOT NULL,
 CONSTRAINT [PK_ProviderType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PurchaseOrderShippingType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PurchaseOrderShippingType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](20) NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
	[isVehicleType] [bit] NOT NULL,
	[isTerrestriel] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseOrderShippingType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PurchaseReason]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PurchaseReason](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](50) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_PurchaseReason] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Rate]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Rate](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_taxType] [int] NOT NULL,
	[code] [varchar](5) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[percentage] [decimal](14, 6) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Rate] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ResultProdLotReceptionDetail]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ResultProdLotReceptionDetail](
	[idProductionLotReceptionDetail] [int] NOT NULL,
	[idDrainingTest] [int] NULL,
	[idRemissionGuide] [int] NOT NULL,
	[numberRemissionGuide] [varchar](50) NOT NULL,
	[dateArrived] [datetime] NULL,
	[poundsRemitted] [decimal](18, 6) NOT NULL,
	[drawersNumber] [int] NOT NULL,
	[numberLot] [varchar](20) NOT NULL,
	[numberLotSequential] [varchar](20) NOT NULL,
	[namePool] [varchar](50) NOT NULL,
	[nameProvider] [varchar](250) NOT NULL,
	[INPnumber] [varchar](250) NOT NULL,
	[temperature] [decimal](12, 6) NOT NULL,
	[idWarehouse] [int] NOT NULL,
	[nameWarehouse] [varchar](250) NOT NULL,
	[idWarehouseLocation] [int] NOT NULL,
	[nameWarehouseLocation] [varchar](250) NOT NULL,
	[idItem] [int] NOT NULL,
	[nameItem] [varchar](250) NOT NULL,
 CONSTRAINT [PK_ResultProdLotReceptionDetail] PRIMARY KEY CLUSTERED 
(
	[idProductionLotReceptionDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ResultReceptionDispatchMaterial]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ResultReceptionDispatchMaterial](
	[idReceptionDispatchMaterial] [int] NOT NULL,
	[idLiquidationMaterialSupplies] [int] NULL,
	[numberRemissionGuide] [varchar](100) NOT NULL,
	[dateRemissionGuide] [datetime] NOT NULL,
	[nameState] [varchar](100) NOT NULL,
	[idProvider] [int] NOT NULL,
	[nameProvider] [varchar](250) NOT NULL,
	[nameProviderShrimp] [varchar](250) NOT NULL,
	[numberRecepctionDispatchMaterials] [varchar](100) NOT NULL,
	[dateReception] [datetime] NOT NULL,
 CONSTRAINT [PK_ResultReceptionDispatchMaterial] PRIMARY KEY CLUSTERED 
(
	[idReceptionDispatchMaterial] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ResultReceptionDispatchMaterialDetail]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultReceptionDispatchMaterialDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idResultReceptionDispatchMaterial] [int] NOT NULL,
	[idItem] [int] NOT NULL,
	[idMetricUnit] [int] NOT NULL,
	[quantity] [decimal](20, 6) NOT NULL,
 CONSTRAINT [PK_ResultReceptionDispatchMaterialDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Retention]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Retention](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_retentionType] [int] NOT NULL,
	[id_retentionGroup] [int] NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[percentRetencion] [decimal](14, 6) NOT NULL,
 CONSTRAINT [PK_Retention] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RetentionGroup]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RetentionGroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[dateResolution] [datetime] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_RetentionGroup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RetentionSeriesForDocumentsType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RetentionSeriesForDocumentsType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_RetentionSeriesForDocumentsType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RetentionType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RetentionType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_RetentionType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Rol]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Rol](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK__Rol__3213E83F8507A6D3] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RtInternational]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RtInternational](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_RtlInternational] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Setting]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Setting](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[value] [varchar](50) NULL,
	[id_settingDataType] [int] NOT NULL,
	[id_module] [int] NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
	[description] [varchar](500) NOT NULL,
 CONSTRAINT [PK_Setting] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Uq_name] UNIQUE NONCLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SettingDataType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SettingDataType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](20) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_SettingDataType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SettingDetail]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SettingDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_setting] [int] NOT NULL,
	[value] [varchar](250) NOT NULL,
	[valueAux] [varchar](250) NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_SettingDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SettingNotification]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SettingNotification](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_documentType] [int] NOT NULL,
	[id_documentState] [int] NOT NULL,
	[title] [varchar](max) NOT NULL,
	[description] [varchar](max) NOT NULL,
	[sendByMail] [bit] NOT NULL,
	[addressesMails] [varchar](max) NULL,
 CONSTRAINT [PK_SettingNotification] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SettingPriceList]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SettingPriceList](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_userGroupApproval] [int] NOT NULL,
	[seeAllStates] [bit] NOT NULL,
	[readOnly] [bit] NOT NULL,
	[id_crateState] [int] NOT NULL,
	[id_reversedState] [int] NOT NULL,
	[id_aprovedState] [int] NOT NULL,
	[weight] [int] NOT NULL,
 CONSTRAINT [PK_SettingPriceList] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StateOfContry]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StateOfContry](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_country] [int] NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_StateOfContry] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TAction]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TAction](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_controller] [int] NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_TAction] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TaxType]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TaxType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](5) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_TaxType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tbsysDocumentTypeDocumentState]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbsysDocumentTypeDocumentState](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_DocumentType] [int] NOT NULL,
	[id_DocumenteState] [int] NOT NULL,
 CONSTRAINT [PK_tbsysDocumentTypeDocumentState] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TController]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TController](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_TController] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TypeBoxCardAndBank]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TypeBoxCardAndBank](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_TypeBoxCardAndBank] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TypeFiltersConfiguration]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TypeFiltersConfiguration](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](5) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_TypeFiltersConfiguration] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TypeFiltersConfigurationComparisonOperator]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeFiltersConfigurationComparisonOperator](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_typeFiltersConfiguration] [int] NOT NULL,
	[id_comparisonOperator] [int] NOT NULL,
 CONSTRAINT [PK_TypeFiltersConfigurationComparisonOperator] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TypeINP]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TypeINP](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](10) NULL,
	[name] [varchar](max) NULL,
	[description] [varchar](max) NULL,
	[isActive] [bit] NULL,
	[id_userCreate] [int] NULL,
	[dateCreate] [datetime] NULL,
	[id_userUpdate] [int] NULL,
	[dateUpdate] [datetime] NULL,
 CONSTRAINT [PK_TypeINP] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[User]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_employee] [int] NULL,
	[username] [varchar](50) NOT NULL,
	[password] [varchar](max) NOT NULL,
	[id_group] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_company] [int] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserEmissionPoint]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserEmissionPoint](
	[id_user] [int] NOT NULL,
	[id_emissionPoint] [int] NOT NULL,
 CONSTRAINT [PK_UserCompanyInformation] PRIMARY KEY CLUSTERED 
(
	[id_user] ASC,
	[id_emissionPoint] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserEmployeeInformation]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserEmployeeInformation](
	[id_user] [int] NOT NULL,
	[id_employee] [int] NOT NULL,
 CONSTRAINT [PK_UserEmployeeInformation] PRIMARY KEY CLUSTERED 
(
	[id_user] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserGroup]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserGroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserGroupMenu]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserGroupMenu](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_userGroup] [int] NOT NULL,
	[id_menu] [int] NOT NULL,
 CONSTRAINT [PK_UserGroupMenu] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserGroupMenuPermission]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserGroupMenuPermission](
	[id_userGroupMenu] [int] NOT NULL,
	[id_permission] [int] NOT NULL,
 CONSTRAINT [PK_UserGroupMenuPermission] PRIMARY KEY CLUSTERED 
(
	[id_userGroupMenu] ASC,
	[id_permission] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserMenu]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMenu](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_user] [int] NOT NULL,
	[id_menu] [int] NOT NULL,
 CONSTRAINT [PK_UserMenu] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserMenuPermission]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMenuPermission](
	[id_userMenu] [int] NOT NULL,
	[id_permission] [int] NOT NULL,
 CONSTRAINT [PK_UserMenuPermission] PRIMARY KEY CLUSTERED 
(
	[id_userMenu] ASC,
	[id_permission] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRol]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserRol](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [char](7) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[description] [varchar](max) NULL,
	[id_company] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[id_userCreate] [int] NOT NULL,
	[dateCreate] [datetime] NOT NULL,
	[id_userUpdate] [int] NOT NULL,
	[dateUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_UserRol] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_UserRol] UNIQUE NONCLUSTERED 
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRolUser]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRolUser](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_userRol] [int] NOT NULL,
	[id_User] [int] NOT NULL,
 CONSTRAINT [PK_UserRolUser] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_UserRolUser] UNIQUE NONCLUSTERED 
(
	[id_userRol] ASC,
	[id_User] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Vendor]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Vendor](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](20) NOT NULL,
	[name] [varchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VisualizationTypeData]    Script Date: 29/10/2018 12:45:55 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VisualizationTypeData](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[description] [varchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[AddressType] ADD  DEFAULT ((0)) FOR [isDefault]
GO
ALTER TABLE [dbo].[ClassShrimp] ADD  CONSTRAINT [DF_ClassShrimp_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((0)) FOR [applyIva]
GO
ALTER TABLE [dbo].[DataType] ADD  CONSTRAINT [DF_DataType_purchase_decimalPlace]  DEFAULT ((0)) FOR [purchase_decimalPlace]
GO
ALTER TABLE [dbo].[DataType] ADD  CONSTRAINT [DF_DataType_sale_decimalPlace]  DEFAULT ((0)) FOR [sale_decimalPlace]
GO
ALTER TABLE [dbo].[DataType] ADD  CONSTRAINT [DF_DataType_inventory_decimalPlace]  DEFAULT ((0)) FOR [inventory_decimalPlace]
GO
ALTER TABLE [dbo].[DocumentType] ADD  CONSTRAINT [DF_DocumentType_isElectronic]  DEFAULT ((1)) FOR [isElectronic]
GO
ALTER TABLE [dbo].[EmissionType] ADD  CONSTRAINT [DF_EmissionType_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[GeneralContactData] ADD  DEFAULT ((0)) FOR [isDefault]
GO
ALTER TABLE [dbo].[GeneralContactData] ADD  DEFAULT ((0)) FOR [isRequired]
GO
ALTER TABLE [dbo].[GeneralContactData] ADD  DEFAULT ((0)) FOR [isActive]
GO
ALTER TABLE [dbo].[ItemSize] ADD  CONSTRAINT [DF_ItemSize_orderSize]  DEFAULT ((0)) FOR [orderSize]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_code]  DEFAULT ('CODE') FOR [code]
GO
ALTER TABLE [dbo].[PaymentTerm] ADD  DEFAULT ((0)) FOR [firstPaymentDays]
GO
ALTER TABLE [dbo].[PaymentTerm] ADD  DEFAULT ((0)) FOR [numberPeriods]
GO
ALTER TABLE [dbo].[PaymentTerm] ADD  DEFAULT ((0)) FOR [formulaReady]
GO
ALTER TABLE [dbo].[PaymentTerm] ADD  DEFAULT ((0)) FOR [PercentAnticipation]
GO
ALTER TABLE [dbo].[PaymentTerm] ADD  DEFAULT ((0)) FOR [PercentBalance]
GO
ALTER TABLE [dbo].[PenalityClassShrimp] ADD  CONSTRAINT [DF_PenalityClassShrimp_byProvider_1]  DEFAULT ((0)) FOR [byProvider]
GO
ALTER TABLE [dbo].[PenalityClassShrimpDetails] ADD  CONSTRAINT [DF_PenalityClassShrimpDetails_value]  DEFAULT ((0)) FOR [value]
GO
ALTER TABLE [dbo].[PersonType] ADD  CONSTRAINT [DF_PersonType_code]  DEFAULT ('P') FOR [code]
GO
ALTER TABLE [dbo].[PoundsRange] ADD  DEFAULT ((0)) FOR [range_ini]
GO
ALTER TABLE [dbo].[PoundsRange] ADD  DEFAULT ((0)) FOR [range_end]
GO
ALTER TABLE [dbo].[PoundsRange] ADD  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[PriceList] ADD  CONSTRAINT [DF_PriceList_byGroup]  DEFAULT ((0)) FOR [byGroup]
GO
ALTER TABLE [dbo].[PriceListItemSizeDetail] ADD  CONSTRAINT [DF_PriceListItemSizeDetail_price]  DEFAULT ((0)) FOR [price]
GO
ALTER TABLE [dbo].[PriceListItemSizeDetail] ADD  CONSTRAINT [DF_PriceListItemSizeDetail_price1]  DEFAULT ((0)) FOR [commission]
GO
ALTER TABLE [dbo].[Provider] ADD  CONSTRAINT [DF_Provider_electronicDocumentIssuance]  DEFAULT ((0)) FOR [electronicDocumentIssuance]
GO
ALTER TABLE [dbo].[PurchaseOrderShippingType] ADD  DEFAULT ((1)) FOR [isVehicleType]
GO
ALTER TABLE [dbo].[PurchaseOrderShippingType] ADD  DEFAULT ((0)) FOR [isTerrestriel]
GO
ALTER TABLE [dbo].[SettingNotification] ADD  CONSTRAINT [DF_SettingNotification_sendByMail]  DEFAULT ((0)) FOR [sendByMail]
GO
ALTER TABLE [dbo].[SettingPriceList] ADD  CONSTRAINT [DF_SettingPriceList_fullAcces]  DEFAULT ((0)) FOR [seeAllStates]
GO
ALTER TABLE [dbo].[SettingPriceList] ADD  CONSTRAINT [DF_SettingPriceList_readOnly]  DEFAULT ((0)) FOR [readOnly]
GO
ALTER TABLE [dbo].[SettingPriceList] ADD  CONSTRAINT [DF_SettingPriceList_peso]  DEFAULT ((0)) FOR [weight]
GO
ALTER TABLE [dbo].[TAction] ADD  CONSTRAINT [DF_TAction_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[TController] ADD  CONSTRAINT [DF_TController_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_Account] FOREIGN KEY([id_parentAccount])
REFERENCES [dbo].[Account] ([id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_Account]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_AccountPlan1] FOREIGN KEY([id_account_plan])
REFERENCES [dbo].[AccountPlan] ([id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_AccountPlan1]
GO
ALTER TABLE [dbo].[AccountDetailAssistantType]  WITH CHECK ADD  CONSTRAINT [FK_AccountDetailAssistantType_Account] FOREIGN KEY([id_account])
REFERENCES [dbo].[Account] ([id])
GO
ALTER TABLE [dbo].[AccountDetailAssistantType] CHECK CONSTRAINT [FK_AccountDetailAssistantType_Account]
GO
ALTER TABLE [dbo].[AccountDetailAssistantType]  WITH CHECK ADD  CONSTRAINT [FK_AccountDetailAssistantType_AssistantType] FOREIGN KEY([id_assistantType])
REFERENCES [dbo].[AssistantType] ([id])
GO
ALTER TABLE [dbo].[AccountDetailAssistantType] CHECK CONSTRAINT [FK_AccountDetailAssistantType_AssistantType]
GO
ALTER TABLE [dbo].[AccountingAssistantDetailType]  WITH CHECK ADD  CONSTRAINT [FK_AccountingAssistantDetailType_AccountingAssistant] FOREIGN KEY([id_accountingAssistant])
REFERENCES [dbo].[AccountingAssistant] ([id])
GO
ALTER TABLE [dbo].[AccountingAssistantDetailType] CHECK CONSTRAINT [FK_AccountingAssistantDetailType_AccountingAssistant]
GO
ALTER TABLE [dbo].[AccountingAssistantDetailType]  WITH CHECK ADD  CONSTRAINT [FK_AccountingAssistantDetailType_AssistantType] FOREIGN KEY([id_assistantType])
REFERENCES [dbo].[AssistantType] ([id])
GO
ALTER TABLE [dbo].[AccountingAssistantDetailType] CHECK CONSTRAINT [FK_AccountingAssistantDetailType_AssistantType]
GO
ALTER TABLE [dbo].[AccountPlan]  WITH CHECK ADD  CONSTRAINT [FK_AccountPlan_Company] FOREIGN KEY([id])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[AccountPlan] CHECK CONSTRAINT [FK_AccountPlan_Company]
GO
ALTER TABLE [dbo].[BoxCardAndBank]  WITH CHECK ADD  CONSTRAINT [FK_BoxCardAndBank_TypeBoxCardAndBank] FOREIGN KEY([id_typeBoxCardAndBank])
REFERENCES [dbo].[TypeBoxCardAndBank] ([id])
GO
ALTER TABLE [dbo].[BoxCardAndBank] CHECK CONSTRAINT [FK_BoxCardAndBank_TypeBoxCardAndBank]
GO
ALTER TABLE [dbo].[BranchOffice]  WITH CHECK ADD  CONSTRAINT [FK__BranchOff__id_di__7849DB76] FOREIGN KEY([id_division])
REFERENCES [dbo].[Division] ([id])
GO
ALTER TABLE [dbo].[BranchOffice] CHECK CONSTRAINT [FK__BranchOff__id_di__7849DB76]
GO
ALTER TABLE [dbo].[CalendarPriceList]  WITH CHECK ADD  CONSTRAINT [FK_CalendarPriceList_CalendarPriceListType] FOREIGN KEY([id_calendarPriceListType])
REFERENCES [dbo].[CalendarPriceListType] ([id])
GO
ALTER TABLE [dbo].[CalendarPriceList] CHECK CONSTRAINT [FK_CalendarPriceList_CalendarPriceListType]
GO
ALTER TABLE [dbo].[CategoryActivityRise]  WITH CHECK ADD  CONSTRAINT [FK_CategoryActivityRise_ActivityRise] FOREIGN KEY([id_activityRise])
REFERENCES [dbo].[ActivityRise] ([id])
GO
ALTER TABLE [dbo].[CategoryActivityRise] CHECK CONSTRAINT [FK_CategoryActivityRise_ActivityRise]
GO
ALTER TABLE [dbo].[CategoryActivityRise]  WITH CHECK ADD  CONSTRAINT [FK_CategoryActivityRise_CategoryRise] FOREIGN KEY([id_categoryRise])
REFERENCES [dbo].[CategoryRise] ([id])
GO
ALTER TABLE [dbo].[CategoryActivityRise] CHECK CONSTRAINT [FK_CategoryActivityRise_CategoryRise]
GO
ALTER TABLE [dbo].[City]  WITH CHECK ADD  CONSTRAINT [FK_City_Country] FOREIGN KEY([id_country])
REFERENCES [dbo].[Country] ([id])
GO
ALTER TABLE [dbo].[City] CHECK CONSTRAINT [FK_City_Country]
GO
ALTER TABLE [dbo].[City]  WITH CHECK ADD  CONSTRAINT [FK_City_StateOfContry] FOREIGN KEY([id_stateOfContry])
REFERENCES [dbo].[StateOfContry] ([id])
GO
ALTER TABLE [dbo].[City] CHECK CONSTRAINT [FK_City_StateOfContry]
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD  CONSTRAINT [FK__Company__id_busi__793DFFAF] FOREIGN KEY([id_businessGroup])
REFERENCES [dbo].[BusinessGroup] ([id])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK__Company__id_busi__793DFFAF]
GO
ALTER TABLE [dbo].[Country]  WITH CHECK ADD  CONSTRAINT [FK_Country_Origin] FOREIGN KEY([id_origin])
REFERENCES [dbo].[Origin] ([id])
GO
ALTER TABLE [dbo].[Country] CHECK CONSTRAINT [FK_Country_Origin]
GO
ALTER TABLE [dbo].[Country_IdentificationType]  WITH CHECK ADD  CONSTRAINT [FK_CountryIdentificationType_country] FOREIGN KEY([id_country])
REFERENCES [dbo].[Country] ([id])
GO
ALTER TABLE [dbo].[Country_IdentificationType] CHECK CONSTRAINT [FK_CountryIdentificationType_country]
GO
ALTER TABLE [dbo].[Country_IdentificationType]  WITH CHECK ADD  CONSTRAINT [FK_CountryIdentificationType_identificationType] FOREIGN KEY([id_identificationType])
REFERENCES [dbo].[IdentificationType] ([id])
GO
ALTER TABLE [dbo].[Country_IdentificationType] CHECK CONSTRAINT [FK_CountryIdentificationType_identificationType]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_CustomerType_id] FOREIGN KEY([id_customerType])
REFERENCES [dbo].[CustomerType] ([id])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_CustomerType_id]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_EconomicGroup_id] FOREIGN KEY([id_economicGroupCusm])
REFERENCES [dbo].[EconomicGroup] ([id])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_EconomicGroup_id]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Person_id] FOREIGN KEY([id])
REFERENCES [dbo].[Person] ([id])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_Person_id]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Vendor] FOREIGN KEY([id_vendorAssigned])
REFERENCES [dbo].[Vendor] ([id])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_Vendor]
GO
ALTER TABLE [dbo].[CustomerType]  WITH CHECK ADD  CONSTRAINT [FK_CustomerType_paymentMethod_id] FOREIGN KEY([def_id_paymentMethod])
REFERENCES [dbo].[PaymentMethod] ([id])
GO
ALTER TABLE [dbo].[CustomerType] CHECK CONSTRAINT [FK_CustomerType_paymentMethod_id]
GO
ALTER TABLE [dbo].[CustomerType]  WITH CHECK ADD  CONSTRAINT [FK_CustomerType_paymentTerm_id] FOREIGN KEY([def_id_paymentTerm])
REFERENCES [dbo].[PaymentTerm] ([id])
GO
ALTER TABLE [dbo].[CustomerType] CHECK CONSTRAINT [FK_CustomerType_paymentTerm_id]
GO
ALTER TABLE [dbo].[CustomerType]  WITH CHECK ADD  CONSTRAINT [FK_CustomerType_PriceList_id] FOREIGN KEY([def_id_priceList])
REFERENCES [dbo].[PriceList] ([id])
GO
ALTER TABLE [dbo].[CustomerType] CHECK CONSTRAINT [FK_CustomerType_PriceList_id]
GO
ALTER TABLE [dbo].[Division]  WITH CHECK ADD  CONSTRAINT [FK__Division__id_com__7B264821] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[Division] CHECK CONSTRAINT [FK__Division__id_com__7B264821]
GO
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK__Document__id_doc__339FAB6E] FOREIGN KEY([id_documentType])
REFERENCES [dbo].[DocumentType] ([id])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK__Document__id_doc__339FAB6E]
GO
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK__Document__id_emi__3587F3E0] FOREIGN KEY([id_emissionPoint])
REFERENCES [dbo].[EmissionPoint] ([id])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK__Document__id_emi__3587F3E0]
GO
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Document] FOREIGN KEY([id_BaseDocument])
REFERENCES [dbo].[Document] ([id])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Document]
GO
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_DocumentState] FOREIGN KEY([id_documentState])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_DocumentState]
GO
ALTER TABLE [dbo].[DocumentDocumentState]  WITH CHECK ADD  CONSTRAINT [FK_DocumentDocumentState_DocumentState] FOREIGN KEY([id_state])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[DocumentDocumentState] CHECK CONSTRAINT [FK_DocumentDocumentState_DocumentState]
GO
ALTER TABLE [dbo].[DocumentDocumentState]  WITH CHECK ADD  CONSTRAINT [FK_DocumentDocumentState_DocumentType] FOREIGN KEY([id_documentType])
REFERENCES [dbo].[DocumentType] ([id])
GO
ALTER TABLE [dbo].[DocumentDocumentState] CHECK CONSTRAINT [FK_DocumentDocumentState_DocumentType]
GO
ALTER TABLE [dbo].[DocumentStateChange]  WITH CHECK ADD  CONSTRAINT [FK_DocumentStateChange_Document] FOREIGN KEY([id_document])
REFERENCES [dbo].[Document] ([id])
GO
ALTER TABLE [dbo].[DocumentStateChange] CHECK CONSTRAINT [FK_DocumentStateChange_Document]
GO
ALTER TABLE [dbo].[DocumentStateChange]  WITH CHECK ADD  CONSTRAINT [FK_DocumentStateChange_DocumentState] FOREIGN KEY([id_documentStateOld])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[DocumentStateChange] CHECK CONSTRAINT [FK_DocumentStateChange_DocumentState]
GO
ALTER TABLE [dbo].[DocumentStateChange]  WITH CHECK ADD  CONSTRAINT [FK_DocumentStateChange_DocumentState1] FOREIGN KEY([id_documentStateNew])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[DocumentStateChange] CHECK CONSTRAINT [FK_DocumentStateChange_DocumentState1]
GO
ALTER TABLE [dbo].[DocumentStateChange]  WITH CHECK ADD  CONSTRAINT [FK_DocumentStateChange_User] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[DocumentStateChange] CHECK CONSTRAINT [FK_DocumentStateChange_User]
GO
ALTER TABLE [dbo].[DocumentStateChange]  WITH CHECK ADD  CONSTRAINT [FK_DocumentStateChange_UserGroup] FOREIGN KEY([id_userGroup])
REFERENCES [dbo].[UserGroup] ([id])
GO
ALTER TABLE [dbo].[DocumentStateChange] CHECK CONSTRAINT [FK_DocumentStateChange_UserGroup]
GO
ALTER TABLE [dbo].[DrainingTest]  WITH CHECK ADD  CONSTRAINT [FK_DrainingTest_Document] FOREIGN KEY([id])
REFERENCES [dbo].[Document] ([id])
GO
ALTER TABLE [dbo].[DrainingTest] CHECK CONSTRAINT [FK_DrainingTest_Document]
GO
ALTER TABLE [dbo].[DrainingTest]  WITH CHECK ADD  CONSTRAINT [FK_DrainingTest_Employee] FOREIGN KEY([idAnalist])
REFERENCES [dbo].[Employee] ([id])
GO
ALTER TABLE [dbo].[DrainingTest] CHECK CONSTRAINT [FK_DrainingTest_Employee]
GO
ALTER TABLE [dbo].[DrainingTestDetail]  WITH CHECK ADD  CONSTRAINT [FK_DrainingTestDetail_DrainingTest] FOREIGN KEY([idDrainingTest])
REFERENCES [dbo].[DrainingTest] ([id])
GO
ALTER TABLE [dbo].[DrainingTestDetail] CHECK CONSTRAINT [FK_DrainingTestDetail_DrainingTest]
GO
ALTER TABLE [dbo].[DrainingTestDetail]  WITH CHECK ADD  CONSTRAINT [FK_DrainingTestDetail_MetricUnit] FOREIGN KEY([idMetricUnit])
REFERENCES [dbo].[MetricUnit] ([id])
GO
ALTER TABLE [dbo].[DrainingTestDetail] CHECK CONSTRAINT [FK_DrainingTestDetail_MetricUnit]
GO
ALTER TABLE [dbo].[EmissionPoint]  WITH CHECK ADD  CONSTRAINT [FK__EmissionP__id_br__7EF6D905] FOREIGN KEY([id_branchOffice])
REFERENCES [dbo].[BranchOffice] ([id])
GO
ALTER TABLE [dbo].[EmissionPoint] CHECK CONSTRAINT [FK__EmissionP__id_br__7EF6D905]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Department] FOREIGN KEY([id_department])
REFERENCES [dbo].[Department] ([id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Department]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Person] FOREIGN KEY([id])
REFERENCES [dbo].[Person] ([id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Person]
GO
ALTER TABLE [dbo].[Escurrido]  WITH CHECK ADD  CONSTRAINT [FK_Escurrido_Document] FOREIGN KEY([id])
REFERENCES [dbo].[Document] ([id])
GO
ALTER TABLE [dbo].[Escurrido] CHECK CONSTRAINT [FK_Escurrido_Document]
GO
ALTER TABLE [dbo].[Escurrido]  WITH CHECK ADD  CONSTRAINT [FK_Escurrido_Employee] FOREIGN KEY([id_employeAnalist])
REFERENCES [dbo].[Employee] ([id])
GO
ALTER TABLE [dbo].[Escurrido] CHECK CONSTRAINT [FK_Escurrido_Employee]
GO
ALTER TABLE [dbo].[EscurridoDetails]  WITH CHECK ADD  CONSTRAINT [FK_EscurridoDetails_Escurrido] FOREIGN KEY([id_escurrido])
REFERENCES [dbo].[Escurrido] ([id])
GO
ALTER TABLE [dbo].[EscurridoDetails] CHECK CONSTRAINT [FK_EscurridoDetails_Escurrido]
GO
ALTER TABLE [dbo].[GroupPersonByRol]  WITH CHECK ADD  CONSTRAINT [FK_GroupPersonByRol_Rol] FOREIGN KEY([id_rol])
REFERENCES [dbo].[Rol] ([id])
GO
ALTER TABLE [dbo].[GroupPersonByRol] CHECK CONSTRAINT [FK_GroupPersonByRol_Rol]
GO
ALTER TABLE [dbo].[GroupPersonByRolDetail]  WITH CHECK ADD  CONSTRAINT [FK_GroupPersonByRolDetail_GroupPersonByRol] FOREIGN KEY([id_groupPersonByRol])
REFERENCES [dbo].[GroupPersonByRol] ([id])
GO
ALTER TABLE [dbo].[GroupPersonByRolDetail] CHECK CONSTRAINT [FK_GroupPersonByRolDetail_GroupPersonByRol]
GO
ALTER TABLE [dbo].[GroupPersonByRolDetail]  WITH CHECK ADD  CONSTRAINT [FK_GroupPersonByRolDetail_Person] FOREIGN KEY([id_person])
REFERENCES [dbo].[Person] ([id])
GO
ALTER TABLE [dbo].[GroupPersonByRolDetail] CHECK CONSTRAINT [FK_GroupPersonByRolDetail_Person]
GO
ALTER TABLE [dbo].[IdentificationType]  WITH CHECK ADD  CONSTRAINT [FK_IdentificationType_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[IdentificationType] CHECK CONSTRAINT [FK_IdentificationType_Company]
GO
ALTER TABLE [dbo].[ItemSaleInformation]  WITH CHECK ADD  CONSTRAINT [FK_ItemSaleInformation_Item] FOREIGN KEY([id_item])
REFERENCES [dbo].[Item] ([id])
GO
ALTER TABLE [dbo].[ItemSaleInformation] CHECK CONSTRAINT [FK_ItemSaleInformation_Item]
GO
ALTER TABLE [dbo].[ItemSizeClass]  WITH CHECK ADD  CONSTRAINT [FK_ItemSizeClass_Class] FOREIGN KEY([id_Class])
REFERENCES [dbo].[Class] ([id])
GO
ALTER TABLE [dbo].[ItemSizeClass] CHECK CONSTRAINT [FK_ItemSizeClass_Class]
GO
ALTER TABLE [dbo].[ItemSizeClass]  WITH CHECK ADD  CONSTRAINT [FK_ItemSizeClass_ItemSize] FOREIGN KEY([id_ItemSize])
REFERENCES [dbo].[ItemSize] ([id])
GO
ALTER TABLE [dbo].[ItemSizeClass] CHECK CONSTRAINT [FK_ItemSizeClass_ItemSize]
GO
ALTER TABLE [dbo].[ItemSizeProcessPLOrder]  WITH CHECK ADD  CONSTRAINT [FK_ItemSizeProcessPLOrder_ItemSize] FOREIGN KEY([id_ItemSize])
REFERENCES [dbo].[ItemSize] ([id])
GO
ALTER TABLE [dbo].[ItemSizeProcessPLOrder] CHECK CONSTRAINT [FK_ItemSizeProcessPLOrder_ItemSize]
GO
ALTER TABLE [dbo].[ItemSizeProcessPLOrder]  WITH CHECK ADD  CONSTRAINT [FK_ItemSizeProcessPLOrder_ProcessType] FOREIGN KEY([id_ProcessType])
REFERENCES [dbo].[ProcessType] ([id])
GO
ALTER TABLE [dbo].[ItemSizeProcessPLOrder] CHECK CONSTRAINT [FK_ItemSizeProcessPLOrder_ProcessType]
GO
ALTER TABLE [dbo].[ItemSizeProcessTypePriceList]  WITH CHECK ADD  CONSTRAINT [FK_ItemSizeProcessTypePriceList_ItemSize] FOREIGN KEY([id_itemsize])
REFERENCES [dbo].[ItemSize] ([id])
GO
ALTER TABLE [dbo].[ItemSizeProcessTypePriceList] CHECK CONSTRAINT [FK_ItemSizeProcessTypePriceList_ItemSize]
GO
ALTER TABLE [dbo].[ItemSizeProcessTypePriceList]  WITH CHECK ADD  CONSTRAINT [FK_ItemSizeProcessTypePriceList_ProcessType] FOREIGN KEY([id_ProcessTypePriceList])
REFERENCES [dbo].[ProcessType] ([id])
GO
ALTER TABLE [dbo].[ItemSizeProcessTypePriceList] CHECK CONSTRAINT [FK_ItemSizeProcessTypePriceList_ProcessType]
GO
ALTER TABLE [dbo].[ItemTaxation]  WITH CHECK ADD  CONSTRAINT [FK_ItemTaxation_Item] FOREIGN KEY([id_item])
REFERENCES [dbo].[Item] ([id])
GO
ALTER TABLE [dbo].[ItemTaxation] CHECK CONSTRAINT [FK_ItemTaxation_Item]
GO
ALTER TABLE [dbo].[LiquidationMaterialSupplies]  WITH CHECK ADD  CONSTRAINT [FK_LiquidationMaterialSupplies_Document] FOREIGN KEY([id])
REFERENCES [dbo].[Document] ([id])
GO
ALTER TABLE [dbo].[LiquidationMaterialSupplies] CHECK CONSTRAINT [FK_LiquidationMaterialSupplies_Document]
GO
ALTER TABLE [dbo].[LiquidationMaterialSupplies]  WITH CHECK ADD  CONSTRAINT [FK_LiquidationMaterialSupplies_Provider] FOREIGN KEY([idProvider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[LiquidationMaterialSupplies] CHECK CONSTRAINT [FK_LiquidationMaterialSupplies_Provider]
GO
ALTER TABLE [dbo].[LiquidationMaterialSuppliesDetail]  WITH CHECK ADD  CONSTRAINT [FK_LiquidationMaterialSuppliesDetail_Item] FOREIGN KEY([idItem])
REFERENCES [dbo].[Item] ([id])
GO
ALTER TABLE [dbo].[LiquidationMaterialSuppliesDetail] CHECK CONSTRAINT [FK_LiquidationMaterialSuppliesDetail_Item]
GO
ALTER TABLE [dbo].[LiquidationMaterialSuppliesDetail]  WITH CHECK ADD  CONSTRAINT [FK_LiquidationMaterialSuppliesDetail_LiquidationMaterialSupplies] FOREIGN KEY([idLiquidationMaterialSupplies])
REFERENCES [dbo].[LiquidationMaterialSupplies] ([id])
GO
ALTER TABLE [dbo].[LiquidationMaterialSuppliesDetail] CHECK CONSTRAINT [FK_LiquidationMaterialSuppliesDetail_LiquidationMaterialSupplies]
GO
ALTER TABLE [dbo].[LoginLog]  WITH CHECK ADD  CONSTRAINT [FK_LoginLog_BranchOffice] FOREIGN KEY([id_branchoffice])
REFERENCES [dbo].[BranchOffice] ([id])
GO
ALTER TABLE [dbo].[LoginLog] CHECK CONSTRAINT [FK_LoginLog_BranchOffice]
GO
ALTER TABLE [dbo].[LoginLog]  WITH CHECK ADD  CONSTRAINT [FK_LoginLog_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[LoginLog] CHECK CONSTRAINT [FK_LoginLog_Company]
GO
ALTER TABLE [dbo].[LoginLog]  WITH CHECK ADD  CONSTRAINT [FK_LoginLog_Division] FOREIGN KEY([id_division])
REFERENCES [dbo].[Division] ([id])
GO
ALTER TABLE [dbo].[LoginLog] CHECK CONSTRAINT [FK_LoginLog_Division]
GO
ALTER TABLE [dbo].[LoginLog]  WITH CHECK ADD  CONSTRAINT [FK_LoginLog_EmissionPoint] FOREIGN KEY([id_emissionPoint])
REFERENCES [dbo].[EmissionPoint] ([id])
GO
ALTER TABLE [dbo].[LoginLog] CHECK CONSTRAINT [FK_LoginLog_EmissionPoint]
GO
ALTER TABLE [dbo].[LoginLog]  WITH CHECK ADD  CONSTRAINT [FK_LoginLog_User] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[LoginLog] CHECK CONSTRAINT [FK_LoginLog_User]
GO
ALTER TABLE [dbo].[Menu]  WITH CHECK ADD  CONSTRAINT [FK_Menu_Menu] FOREIGN KEY([id_parent])
REFERENCES [dbo].[Menu] ([id])
GO
ALTER TABLE [dbo].[Menu] CHECK CONSTRAINT [FK_Menu_Menu]
GO
ALTER TABLE [dbo].[Menu]  WITH CHECK ADD  CONSTRAINT [FK_Menu_TAction] FOREIGN KEY([id_action])
REFERENCES [dbo].[TAction] ([id])
GO
ALTER TABLE [dbo].[Menu] CHECK CONSTRAINT [FK_Menu_TAction]
GO
ALTER TABLE [dbo].[Menu]  WITH CHECK ADD  CONSTRAINT [FK_Menu_TController] FOREIGN KEY([id_controller])
REFERENCES [dbo].[TController] ([id])
GO
ALTER TABLE [dbo].[Menu] CHECK CONSTRAINT [FK_Menu_TController]
GO
ALTER TABLE [dbo].[MetricType]  WITH CHECK ADD  CONSTRAINT [FK_MetricType_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[MetricType] CHECK CONSTRAINT [FK_MetricType_Company]
GO
ALTER TABLE [dbo].[MetricType]  WITH CHECK ADD  CONSTRAINT [FK_MetricType_DataType] FOREIGN KEY([id_dataType])
REFERENCES [dbo].[DataType] ([id])
GO
ALTER TABLE [dbo].[MetricType] CHECK CONSTRAINT [FK_MetricType_DataType]
GO
ALTER TABLE [dbo].[MetricUnit]  WITH CHECK ADD  CONSTRAINT [FK__MetricUni__id_me__1B0907CE] FOREIGN KEY([id_metricType])
REFERENCES [dbo].[MetricType] ([id])
GO
ALTER TABLE [dbo].[MetricUnit] CHECK CONSTRAINT [FK__MetricUni__id_me__1B0907CE]
GO
ALTER TABLE [dbo].[MetricUnit]  WITH CHECK ADD  CONSTRAINT [FK_MetricUnit_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[MetricUnit] CHECK CONSTRAINT [FK_MetricUnit_Company]
GO
ALTER TABLE [dbo].[MetricUnitConversion]  WITH CHECK ADD  CONSTRAINT [FK__MetricUni__id_me__46E78A0C] FOREIGN KEY([id_metricOrigin])
REFERENCES [dbo].[MetricUnit] ([id])
GO
ALTER TABLE [dbo].[MetricUnitConversion] CHECK CONSTRAINT [FK__MetricUni__id_me__46E78A0C]
GO
ALTER TABLE [dbo].[MetricUnitConversion]  WITH CHECK ADD  CONSTRAINT [FK__MetricUni__id_me__47DBAE45] FOREIGN KEY([id_metricDestiny])
REFERENCES [dbo].[MetricUnit] ([id])
GO
ALTER TABLE [dbo].[MetricUnitConversion] CHECK CONSTRAINT [FK__MetricUni__id_me__47DBAE45]
GO
ALTER TABLE [dbo].[MetricUnitConversion]  WITH CHECK ADD  CONSTRAINT [FK_MetricUnitConversion_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[MetricUnitConversion] CHECK CONSTRAINT [FK_MetricUnitConversion_Company]
GO
ALTER TABLE [dbo].[ModuleTController]  WITH CHECK ADD  CONSTRAINT [FK_ModuleTController_Module] FOREIGN KEY([id_module])
REFERENCES [dbo].[Module] ([id])
GO
ALTER TABLE [dbo].[ModuleTController] CHECK CONSTRAINT [FK_ModuleTController_Module]
GO
ALTER TABLE [dbo].[ModuleTController]  WITH CHECK ADD  CONSTRAINT [FK_ModuleTController_TController] FOREIGN KEY([id_tController])
REFERENCES [dbo].[TController] ([id])
GO
ALTER TABLE [dbo].[ModuleTController] CHECK CONSTRAINT [FK_ModuleTController_TController]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_Document] FOREIGN KEY([id_document])
REFERENCES [dbo].[Document] ([id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Document]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_DocumentState] FOREIGN KEY([id_documentState])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_DocumentState]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_DocumentType] FOREIGN KEY([id_documentType])
REFERENCES [dbo].[DocumentType] ([id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_DocumentType]
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD  CONSTRAINT [FK_Notification_User] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_User]
GO
ALTER TABLE [dbo].[PaymentMethodPaymentTerm]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethodPaymentTerm_PaymentMethod] FOREIGN KEY([id_paymentMethod])
REFERENCES [dbo].[PaymentMethod] ([id])
GO
ALTER TABLE [dbo].[PaymentMethodPaymentTerm] CHECK CONSTRAINT [FK_PaymentMethodPaymentTerm_PaymentMethod]
GO
ALTER TABLE [dbo].[PaymentMethodPaymentTerm]  WITH CHECK ADD  CONSTRAINT [FK_PaymentMethodPaymentTerm_paymentTerm] FOREIGN KEY([id_paymentTerm])
REFERENCES [dbo].[PaymentTerm] ([id])
GO
ALTER TABLE [dbo].[PaymentMethodPaymentTerm] CHECK CONSTRAINT [FK_PaymentMethodPaymentTerm_paymentTerm]
GO
ALTER TABLE [dbo].[PenalityClassShrimp]  WITH CHECK ADD  CONSTRAINT [FK_PenalityClassShrimp_GroupPersonByRol] FOREIGN KEY([id_groupPersonByRol])
REFERENCES [dbo].[GroupPersonByRol] ([id])
GO
ALTER TABLE [dbo].[PenalityClassShrimp] CHECK CONSTRAINT [FK_PenalityClassShrimp_GroupPersonByRol]
GO
ALTER TABLE [dbo].[PenalityClassShrimp]  WITH CHECK ADD  CONSTRAINT [FK_PenalityClassShrimp_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[PenalityClassShrimp] CHECK CONSTRAINT [FK_PenalityClassShrimp_Provider]
GO
ALTER TABLE [dbo].[PenalityClassShrimpDetails]  WITH CHECK ADD  CONSTRAINT [FK_PenalityClassShrimpDetails_ClassShrimp] FOREIGN KEY([id_classShrimp])
REFERENCES [dbo].[ClassShrimp] ([id])
GO
ALTER TABLE [dbo].[PenalityClassShrimpDetails] CHECK CONSTRAINT [FK_PenalityClassShrimpDetails_ClassShrimp]
GO
ALTER TABLE [dbo].[PenalityClassShrimpDetails]  WITH CHECK ADD  CONSTRAINT [FK_PenalityClassShrimpDetails_PenalityClassShrimp] FOREIGN KEY([id_penalityClassShrimp])
REFERENCES [dbo].[PenalityClassShrimp] ([id])
GO
ALTER TABLE [dbo].[PenalityClassShrimpDetails] CHECK CONSTRAINT [FK_PenalityClassShrimpDetails_PenalityClassShrimp]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK__Person__id_ident__4F7CD00D] FOREIGN KEY([id_identificationType])
REFERENCES [dbo].[IdentificationType] ([id])
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK__Person__id_ident__4F7CD00D]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_Company]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_PersonType] FOREIGN KEY([id_personType])
REFERENCES [dbo].[PersonType] ([id])
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_PersonType]
GO
ALTER TABLE [dbo].[PersonRol]  WITH CHECK ADD  CONSTRAINT [FK_PersonRol_Person] FOREIGN KEY([id_person])
REFERENCES [dbo].[Person] ([id])
GO
ALTER TABLE [dbo].[PersonRol] CHECK CONSTRAINT [FK_PersonRol_Person]
GO
ALTER TABLE [dbo].[PersonRol]  WITH CHECK ADD  CONSTRAINT [FK_PersonRol_Rol] FOREIGN KEY([id_rol])
REFERENCES [dbo].[Rol] ([id])
GO
ALTER TABLE [dbo].[PersonRol] CHECK CONSTRAINT [FK_PersonRol_Rol]
GO
ALTER TABLE [dbo].[PoundsRange]  WITH CHECK ADD FOREIGN KEY([id_metricUnit])
REFERENCES [dbo].[MetricUnit] ([id])
GO
ALTER TABLE [dbo].[PriceList]  WITH CHECK ADD  CONSTRAINT [FK_PriceList_CalendarPriceList] FOREIGN KEY([id_calendarPriceList])
REFERENCES [dbo].[CalendarPriceList] ([id])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_CalendarPriceList]
GO
ALTER TABLE [dbo].[PriceList]  WITH CHECK ADD  CONSTRAINT [FK_PriceList_Document] FOREIGN KEY([id])
REFERENCES [dbo].[Document] ([id])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_Document]
GO
ALTER TABLE [dbo].[PriceList]  WITH CHECK ADD  CONSTRAINT [FK_PriceList_GroupPersonByRol] FOREIGN KEY([id_groupPersonByRol])
REFERENCES [dbo].[GroupPersonByRol] ([id])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_GroupPersonByRol]
GO
ALTER TABLE [dbo].[PriceList]  WITH CHECK ADD  CONSTRAINT [FK_PriceList_PriceList] FOREIGN KEY([id_priceListBase])
REFERENCES [dbo].[PriceList] ([id])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_PriceList]
GO
ALTER TABLE [dbo].[PriceList]  WITH CHECK ADD  CONSTRAINT [FK_PriceList_ProcessType] FOREIGN KEY([id_processtype])
REFERENCES [dbo].[ProcessType] ([id])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_ProcessType]
GO
ALTER TABLE [dbo].[PriceList]  WITH CHECK ADD  CONSTRAINT [FK_PriceList_User] FOREIGN KEY([id_userResponsable])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_User]
GO
ALTER TABLE [dbo].[PriceListClassShrimp]  WITH CHECK ADD  CONSTRAINT [FK_PriceListClassShrimp_ClassShrimp] FOREIGN KEY([id_classShrimp])
REFERENCES [dbo].[ClassShrimp] ([id])
GO
ALTER TABLE [dbo].[PriceListClassShrimp] CHECK CONSTRAINT [FK_PriceListClassShrimp_ClassShrimp]
GO
ALTER TABLE [dbo].[PriceListClassShrimp]  WITH CHECK ADD  CONSTRAINT [FK_PriceListClassShrimp_PriceList] FOREIGN KEY([id_priceList])
REFERENCES [dbo].[PriceList] ([id])
GO
ALTER TABLE [dbo].[PriceListClassShrimp] CHECK CONSTRAINT [FK_PriceListClassShrimp_PriceList]
GO
ALTER TABLE [dbo].[PriceListItemSizeDetail]  WITH CHECK ADD  CONSTRAINT [FK_PriceListItemSizeDetail_Itemsize] FOREIGN KEY([Id_Itemsize])
REFERENCES [dbo].[ItemSize] ([id])
GO
ALTER TABLE [dbo].[PriceListItemSizeDetail] CHECK CONSTRAINT [FK_PriceListItemSizeDetail_Itemsize]
GO
ALTER TABLE [dbo].[PriceListItemSizeDetail]  WITH CHECK ADD  CONSTRAINT [FK_PriceListItemSizeDetail_PriceList] FOREIGN KEY([Id_PriceList])
REFERENCES [dbo].[PriceList] ([id])
GO
ALTER TABLE [dbo].[PriceListItemSizeDetail] CHECK CONSTRAINT [FK_PriceListItemSizeDetail_PriceList]
GO
ALTER TABLE [dbo].[PriceListPersonPersonRol]  WITH CHECK ADD  CONSTRAINT [FK_PriceListPersonPersonRol_Person] FOREIGN KEY([id_Person])
REFERENCES [dbo].[Person] ([id])
GO
ALTER TABLE [dbo].[PriceListPersonPersonRol] CHECK CONSTRAINT [FK_PriceListPersonPersonRol_Person]
GO
ALTER TABLE [dbo].[PriceListPersonPersonRol]  WITH CHECK ADD  CONSTRAINT [FK_PriceListPersonPersonRol_PriceList] FOREIGN KEY([id_PriceList])
REFERENCES [dbo].[PriceList] ([id])
GO
ALTER TABLE [dbo].[PriceListPersonPersonRol] CHECK CONSTRAINT [FK_PriceListPersonPersonRol_PriceList]
GO
ALTER TABLE [dbo].[PriceListPersonPersonRol]  WITH CHECK ADD  CONSTRAINT [FK_PriceListPersonPersonRol_Rol] FOREIGN KEY([id_Rol])
REFERENCES [dbo].[Rol] ([id])
GO
ALTER TABLE [dbo].[PriceListPersonPersonRol] CHECK CONSTRAINT [FK_PriceListPersonPersonRol_Rol]
GO
ALTER TABLE [dbo].[ProductionLot]  WITH CHECK ADD  CONSTRAINT [PK_ProductionLot_ProductionLotState] FOREIGN KEY([id_ProductionLotState])
REFERENCES [dbo].[ProductionLotState] ([id])
GO
ALTER TABLE [dbo].[ProductionLot] CHECK CONSTRAINT [PK_ProductionLot_ProductionLotState]
GO
ALTER TABLE [dbo].[ProductionLotDetail]  WITH CHECK ADD  CONSTRAINT [PK_ProductionLotDetail_ProductionLot] FOREIGN KEY([id_productionLot])
REFERENCES [dbo].[ProductionLot] ([id])
GO
ALTER TABLE [dbo].[ProductionLotDetail] CHECK CONSTRAINT [PK_ProductionLotDetail_ProductionLot]
GO
ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_PaymentTerm] FOREIGN KEY([id_paymentTerm])
REFERENCES [dbo].[PaymentTerm] ([id])
GO
ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_PaymentTerm]
GO
ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_Person] FOREIGN KEY([id])
REFERENCES [dbo].[Person] ([id])
GO
ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_Person]
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProviderAccountingAccounts_Account] FOREIGN KEY([id_account])
REFERENCES [dbo].[Account] ([id])
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts] CHECK CONSTRAINT [FK_ProviderAccountingAccounts_Account]
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProviderAccountingAccounts_AccountFor] FOREIGN KEY([id_accountFor])
REFERENCES [dbo].[AccountFor] ([id])
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts] CHECK CONSTRAINT [FK_ProviderAccountingAccounts_AccountFor]
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProviderAccountingAccounts_AccountingAssistantDetailType] FOREIGN KEY([id_accountingAssistantDetailType])
REFERENCES [dbo].[AccountingAssistantDetailType] ([id])
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts] CHECK CONSTRAINT [FK_ProviderAccountingAccounts_AccountingAssistantDetailType]
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProviderAccountingAccounts_AccountPlan] FOREIGN KEY([id_accountPlan])
REFERENCES [dbo].[AccountPlan] ([id])
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts] CHECK CONSTRAINT [FK_ProviderAccountingAccounts_AccountPlan]
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProviderAccountingAccounts_BranchOffice] FOREIGN KEY([id_branchOffice])
REFERENCES [dbo].[BranchOffice] ([id])
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts] CHECK CONSTRAINT [FK_ProviderAccountingAccounts_BranchOffice]
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProviderAccountingAccounts_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts] CHECK CONSTRAINT [FK_ProviderAccountingAccounts_Company]
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProviderAccountingAccounts_Division] FOREIGN KEY([id_division])
REFERENCES [dbo].[Division] ([id])
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts] CHECK CONSTRAINT [FK_ProviderAccountingAccounts_Division]
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts]  WITH CHECK ADD  CONSTRAINT [FK_ProviderAccountingAccounts_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderAccountingAccounts] CHECK CONSTRAINT [FK_ProviderAccountingAccounts_Provider]
GO
ALTER TABLE [dbo].[ProviderGeneralData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralData_BoxCardAndBank] FOREIGN KEY([id_boxCardAndBankAD])
REFERENCES [dbo].[BoxCardAndBank] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralData] CHECK CONSTRAINT [FK_ProviderGeneralData_BoxCardAndBank]
GO
ALTER TABLE [dbo].[ProviderGeneralData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralData_City] FOREIGN KEY([id_city])
REFERENCES [dbo].[City] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralData] CHECK CONSTRAINT [FK_ProviderGeneralData_City]
GO
ALTER TABLE [dbo].[ProviderGeneralData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralData_Country] FOREIGN KEY([id_country])
REFERENCES [dbo].[Country] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralData] CHECK CONSTRAINT [FK_ProviderGeneralData_Country]
GO
ALTER TABLE [dbo].[ProviderGeneralData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralData_EconomicGroup] FOREIGN KEY([id_economicGroup])
REFERENCES [dbo].[EconomicGroup] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralData] CHECK CONSTRAINT [FK_ProviderGeneralData_EconomicGroup]
GO
ALTER TABLE [dbo].[ProviderGeneralData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralData_Origin] FOREIGN KEY([id_origin])
REFERENCES [dbo].[Origin] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralData] CHECK CONSTRAINT [FK_ProviderGeneralData_Origin]
GO
ALTER TABLE [dbo].[ProviderGeneralData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralData_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralData] CHECK CONSTRAINT [FK_ProviderGeneralData_Provider]
GO
ALTER TABLE [dbo].[ProviderGeneralData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralData_ProviderType] FOREIGN KEY([id_providerType])
REFERENCES [dbo].[ProviderType] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralData] CHECK CONSTRAINT [FK_ProviderGeneralData_ProviderType]
GO
ALTER TABLE [dbo].[ProviderGeneralData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralData_StateOfContry] FOREIGN KEY([id_stateOfContry])
REFERENCES [dbo].[StateOfContry] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralData] CHECK CONSTRAINT [FK_ProviderGeneralData_StateOfContry]
GO
ALTER TABLE [dbo].[ProviderGeneralData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralData_TypeBoxCardAndBank] FOREIGN KEY([id_typeBoxCardAndBankAD])
REFERENCES [dbo].[TypeBoxCardAndBank] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralData] CHECK CONSTRAINT [FK_ProviderGeneralData_TypeBoxCardAndBank]
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralDataEP_AccountTypeGeneral] FOREIGN KEY([id_accountTypeGeneralEP])
REFERENCES [dbo].[AccountTypeGeneral] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP] CHECK CONSTRAINT [FK_ProviderGeneralDataEP_AccountTypeGeneral]
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralDataEP_BoxCardAndBank] FOREIGN KEY([id_bankToBelieve])
REFERENCES [dbo].[BoxCardAndBank] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP] CHECK CONSTRAINT [FK_ProviderGeneralDataEP_BoxCardAndBank]
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralDataEP_IdentificationType] FOREIGN KEY([id_identificationTypeEP])
REFERENCES [dbo].[IdentificationType] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP] CHECK CONSTRAINT [FK_ProviderGeneralDataEP_IdentificationType]
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralDataEP_PaymentMethod] FOREIGN KEY([id_paymentMethodEP])
REFERENCES [dbo].[PaymentMethod] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP] CHECK CONSTRAINT [FK_ProviderGeneralDataEP_PaymentMethod]
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralDataEP_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP] CHECK CONSTRAINT [FK_ProviderGeneralDataEP_Provider]
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralDataEP_RtlInternational] FOREIGN KEY([id_rtInternational])
REFERENCES [dbo].[RtInternational] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralDataEP] CHECK CONSTRAINT [FK_ProviderGeneralDataEP_RtlInternational]
GO
ALTER TABLE [dbo].[ProviderGeneralDataRise]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralDataRise_ActivityRise] FOREIGN KEY([id_activityRise])
REFERENCES [dbo].[ActivityRise] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralDataRise] CHECK CONSTRAINT [FK_ProviderGeneralDataRise_ActivityRise]
GO
ALTER TABLE [dbo].[ProviderGeneralDataRise]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralDataRise_CategoryRise] FOREIGN KEY([id_categoryRise])
REFERENCES [dbo].[CategoryRise] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralDataRise] CHECK CONSTRAINT [FK_ProviderGeneralDataRise_CategoryRise]
GO
ALTER TABLE [dbo].[ProviderGeneralDataRise]  WITH CHECK ADD  CONSTRAINT [FK_ProviderGeneralDataRise_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderGeneralDataRise] CHECK CONSTRAINT [FK_ProviderGeneralDataRise_Provider]
GO
ALTER TABLE [dbo].[ProviderMailByComDivBra]  WITH CHECK ADD  CONSTRAINT [FK_ProviderMailByComDivBra_BranchOffice] FOREIGN KEY([id_branchOffice])
REFERENCES [dbo].[BranchOffice] ([id])
GO
ALTER TABLE [dbo].[ProviderMailByComDivBra] CHECK CONSTRAINT [FK_ProviderMailByComDivBra_BranchOffice]
GO
ALTER TABLE [dbo].[ProviderMailByComDivBra]  WITH CHECK ADD  CONSTRAINT [FK_ProviderMailByComDivBra_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[ProviderMailByComDivBra] CHECK CONSTRAINT [FK_ProviderMailByComDivBra_Company]
GO
ALTER TABLE [dbo].[ProviderMailByComDivBra]  WITH CHECK ADD  CONSTRAINT [FK_ProviderMailByComDivBra_Division] FOREIGN KEY([id_division])
REFERENCES [dbo].[Division] ([id])
GO
ALTER TABLE [dbo].[ProviderMailByComDivBra] CHECK CONSTRAINT [FK_ProviderMailByComDivBra_Division]
GO
ALTER TABLE [dbo].[ProviderMailByComDivBra]  WITH CHECK ADD  CONSTRAINT [FK_ProviderMailByComDivBra_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderMailByComDivBra] CHECK CONSTRAINT [FK_ProviderMailByComDivBra_Provider]
GO
ALTER TABLE [dbo].[ProviderPassportImportData]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPassportImportData_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderPassportImportData] CHECK CONSTRAINT [FK_ProviderPassportImportData_Provider]
GO
ALTER TABLE [dbo].[ProviderPaymentMethod]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentMethod_BranchOffice] FOREIGN KEY([id_branchOffice])
REFERENCES [dbo].[BranchOffice] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentMethod] CHECK CONSTRAINT [FK_ProviderPaymentMethod_BranchOffice]
GO
ALTER TABLE [dbo].[ProviderPaymentMethod]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentMethod_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentMethod] CHECK CONSTRAINT [FK_ProviderPaymentMethod_Company]
GO
ALTER TABLE [dbo].[ProviderPaymentMethod]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentMethod_Division] FOREIGN KEY([id_division])
REFERENCES [dbo].[Division] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentMethod] CHECK CONSTRAINT [FK_ProviderPaymentMethod_Division]
GO
ALTER TABLE [dbo].[ProviderPaymentMethod]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentMethod_PaymentMethod] FOREIGN KEY([id_paymentMethod])
REFERENCES [dbo].[PaymentMethod] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentMethod] CHECK CONSTRAINT [FK_ProviderPaymentMethod_PaymentMethod]
GO
ALTER TABLE [dbo].[ProviderPaymentMethod]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentMethod_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentMethod] CHECK CONSTRAINT [FK_ProviderPaymentMethod_Provider]
GO
ALTER TABLE [dbo].[ProviderPaymentTerm]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentTerm_BranchOffice] FOREIGN KEY([id_branchOffice])
REFERENCES [dbo].[BranchOffice] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentTerm] CHECK CONSTRAINT [FK_ProviderPaymentTerm_BranchOffice]
GO
ALTER TABLE [dbo].[ProviderPaymentTerm]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentTerm_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentTerm] CHECK CONSTRAINT [FK_ProviderPaymentTerm_Company]
GO
ALTER TABLE [dbo].[ProviderPaymentTerm]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentTerm_Division] FOREIGN KEY([id_division])
REFERENCES [dbo].[Division] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentTerm] CHECK CONSTRAINT [FK_ProviderPaymentTerm_Division]
GO
ALTER TABLE [dbo].[ProviderPaymentTerm]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentTerm_PaymentTerm] FOREIGN KEY([id_paymentTerm])
REFERENCES [dbo].[PaymentTerm] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentTerm] CHECK CONSTRAINT [FK_ProviderPaymentTerm_PaymentTerm]
GO
ALTER TABLE [dbo].[ProviderPaymentTerm]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPaymentTerm_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentTerm] CHECK CONSTRAINT [FK_ProviderPaymentTerm_Provider]
GO
ALTER TABLE [dbo].[ProviderPaymentTermMethod]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPymentMethod_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentTermMethod] CHECK CONSTRAINT [FK_ProviderPymentMethod_Provider]
GO
ALTER TABLE [dbo].[ProviderPaymentTermMethod]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPymentMethod_PumentMean] FOREIGN KEY([id_paymentMethod])
REFERENCES [dbo].[PaymentMethod] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentTermMethod] CHECK CONSTRAINT [FK_ProviderPymentMethod_PumentMean]
GO
ALTER TABLE [dbo].[ProviderPaymentTermMethod]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPymentMethod_PymentMethod] FOREIGN KEY([id_paymentTerm])
REFERENCES [dbo].[PaymentTerm] ([id])
GO
ALTER TABLE [dbo].[ProviderPaymentTermMethod] CHECK CONSTRAINT [FK_ProviderPymentMethod_PymentMethod]
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_AccountType] FOREIGN KEY([id_accountType])
REFERENCES [dbo].[AccountType] ([id])
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill] CHECK CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_AccountType]
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_BoxCardAndBank] FOREIGN KEY([id_bank])
REFERENCES [dbo].[BoxCardAndBank] ([id])
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill] CHECK CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_BoxCardAndBank]
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_Country] FOREIGN KEY([id_country])
REFERENCES [dbo].[Country] ([id])
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill] CHECK CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_Country]
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_IdentificationType] FOREIGN KEY([id_identificationType])
REFERENCES [dbo].[IdentificationType] ([id])
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill] CHECK CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_IdentificationType]
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill]  WITH CHECK ADD  CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderPersonAuthorizedToPayTheBill] CHECK CONSTRAINT [FK_ProviderPersonAuthorizedToPayTheBill_Provider]
GO
ALTER TABLE [dbo].[ProviderRelatedCompany]  WITH CHECK ADD  CONSTRAINT [FK_ProviderRelatedCompany_BranchOffice] FOREIGN KEY([id_branchOffice])
REFERENCES [dbo].[BranchOffice] ([id])
GO
ALTER TABLE [dbo].[ProviderRelatedCompany] CHECK CONSTRAINT [FK_ProviderRelatedCompany_BranchOffice]
GO
ALTER TABLE [dbo].[ProviderRelatedCompany]  WITH CHECK ADD  CONSTRAINT [FK_ProviderRelatedCompany_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[ProviderRelatedCompany] CHECK CONSTRAINT [FK_ProviderRelatedCompany_Company]
GO
ALTER TABLE [dbo].[ProviderRelatedCompany]  WITH CHECK ADD  CONSTRAINT [FK_ProviderRelatedCompany_Division] FOREIGN KEY([id_division])
REFERENCES [dbo].[Division] ([id])
GO
ALTER TABLE [dbo].[ProviderRelatedCompany] CHECK CONSTRAINT [FK_ProviderRelatedCompany_Division]
GO
ALTER TABLE [dbo].[ProviderRelatedCompany]  WITH CHECK ADD  CONSTRAINT [FK_ProviderRelatedCompany_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderRelatedCompany] CHECK CONSTRAINT [FK_ProviderRelatedCompany_Provider]
GO
ALTER TABLE [dbo].[ProviderRetention]  WITH CHECK ADD  CONSTRAINT [FK_ProviderRetention_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderRetention] CHECK CONSTRAINT [FK_ProviderRetention_Provider]
GO
ALTER TABLE [dbo].[ProviderRetention]  WITH CHECK ADD  CONSTRAINT [FK_ProviderRetention_Retention] FOREIGN KEY([id_retention])
REFERENCES [dbo].[Retention] ([id])
GO
ALTER TABLE [dbo].[ProviderRetention] CHECK CONSTRAINT [FK_ProviderRetention_Retention]
GO
ALTER TABLE [dbo].[ProviderRetention]  WITH CHECK ADD  CONSTRAINT [FK_ProviderRetention_RetentionGroup] FOREIGN KEY([id_retentionGroup])
REFERENCES [dbo].[RetentionGroup] ([id])
GO
ALTER TABLE [dbo].[ProviderRetention] CHECK CONSTRAINT [FK_ProviderRetention_RetentionGroup]
GO
ALTER TABLE [dbo].[ProviderRetention]  WITH CHECK ADD  CONSTRAINT [FK_ProviderRetention_RetentionType] FOREIGN KEY([id_retentionType])
REFERENCES [dbo].[RetentionType] ([id])
GO
ALTER TABLE [dbo].[ProviderRetention] CHECK CONSTRAINT [FK_ProviderRetention_RetentionType]
GO
ALTER TABLE [dbo].[ProviderSeriesForDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ProviderSeriesForDocuments_DocumentType] FOREIGN KEY([id_documentType])
REFERENCES [dbo].[DocumentType] ([id])
GO
ALTER TABLE [dbo].[ProviderSeriesForDocuments] CHECK CONSTRAINT [FK_ProviderSeriesForDocuments_DocumentType]
GO
ALTER TABLE [dbo].[ProviderSeriesForDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ProviderSeriesForDocuments_Provider] FOREIGN KEY([id_provider])
REFERENCES [dbo].[Provider] ([id])
GO
ALTER TABLE [dbo].[ProviderSeriesForDocuments] CHECK CONSTRAINT [FK_ProviderSeriesForDocuments_Provider]
GO
ALTER TABLE [dbo].[ProviderSeriesForDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ProviderSeriesForDocuments_RetentionSeriesForDocumentsType] FOREIGN KEY([id_retentionSeriesForDocumentsType])
REFERENCES [dbo].[RetentionSeriesForDocumentsType] ([id])
GO
ALTER TABLE [dbo].[ProviderSeriesForDocuments] CHECK CONSTRAINT [FK_ProviderSeriesForDocuments_RetentionSeriesForDocumentsType]
GO
ALTER TABLE [dbo].[Rate]  WITH CHECK ADD  CONSTRAINT [FK_Rate_TaxType] FOREIGN KEY([id_taxType])
REFERENCES [dbo].[TaxType] ([id])
GO
ALTER TABLE [dbo].[Rate] CHECK CONSTRAINT [FK_Rate_TaxType]
GO
ALTER TABLE [dbo].[ResultProdLotReceptionDetail]  WITH CHECK ADD  CONSTRAINT [FK_ResultProdLotReceptionDetail_DrainingTest] FOREIGN KEY([idDrainingTest])
REFERENCES [dbo].[DrainingTest] ([id])
GO
ALTER TABLE [dbo].[ResultProdLotReceptionDetail] CHECK CONSTRAINT [FK_ResultProdLotReceptionDetail_DrainingTest]
GO
ALTER TABLE [dbo].[ResultProdLotReceptionDetail]  WITH CHECK ADD  CONSTRAINT [FK_ResultProdLotReceptionDetail_ProductionLotDetail] FOREIGN KEY([idProductionLotReceptionDetail])
REFERENCES [dbo].[ProductionLotDetail] ([id])
GO
ALTER TABLE [dbo].[ResultProdLotReceptionDetail] CHECK CONSTRAINT [FK_ResultProdLotReceptionDetail_ProductionLotDetail]
GO
ALTER TABLE [dbo].[ResultReceptionDispatchMaterial]  WITH CHECK ADD  CONSTRAINT [FK_ResultReceptionDispatchMaterial_LiquidationMaterialSupplies] FOREIGN KEY([idLiquidationMaterialSupplies])
REFERENCES [dbo].[LiquidationMaterialSupplies] ([id])
GO
ALTER TABLE [dbo].[ResultReceptionDispatchMaterial] CHECK CONSTRAINT [FK_ResultReceptionDispatchMaterial_LiquidationMaterialSupplies]
GO
ALTER TABLE [dbo].[ResultReceptionDispatchMaterialDetail]  WITH CHECK ADD  CONSTRAINT [FK_ResultReceptionDispatchMaterialDetail_Item] FOREIGN KEY([idItem])
REFERENCES [dbo].[Item] ([id])
GO
ALTER TABLE [dbo].[ResultReceptionDispatchMaterialDetail] CHECK CONSTRAINT [FK_ResultReceptionDispatchMaterialDetail_Item]
GO
ALTER TABLE [dbo].[ResultReceptionDispatchMaterialDetail]  WITH CHECK ADD  CONSTRAINT [FK_ResultReceptionDispatchMaterialDetail_MetricUnit] FOREIGN KEY([idMetricUnit])
REFERENCES [dbo].[MetricUnit] ([id])
GO
ALTER TABLE [dbo].[ResultReceptionDispatchMaterialDetail] CHECK CONSTRAINT [FK_ResultReceptionDispatchMaterialDetail_MetricUnit]
GO
ALTER TABLE [dbo].[ResultReceptionDispatchMaterialDetail]  WITH CHECK ADD  CONSTRAINT [FK_ResultReceptionDispatchMaterialDetail_ResultReceptionDispatchMaterial] FOREIGN KEY([idResultReceptionDispatchMaterial])
REFERENCES [dbo].[ResultReceptionDispatchMaterial] ([idReceptionDispatchMaterial])
GO
ALTER TABLE [dbo].[ResultReceptionDispatchMaterialDetail] CHECK CONSTRAINT [FK_ResultReceptionDispatchMaterialDetail_ResultReceptionDispatchMaterial]
GO
ALTER TABLE [dbo].[Retention]  WITH CHECK ADD  CONSTRAINT [FK_Retention_RetentionGroup] FOREIGN KEY([id_retentionGroup])
REFERENCES [dbo].[RetentionGroup] ([id])
GO
ALTER TABLE [dbo].[Retention] CHECK CONSTRAINT [FK_Retention_RetentionGroup]
GO
ALTER TABLE [dbo].[Retention]  WITH CHECK ADD  CONSTRAINT [FK_Retention_RetentionType] FOREIGN KEY([id_retentionType])
REFERENCES [dbo].[RetentionType] ([id])
GO
ALTER TABLE [dbo].[Retention] CHECK CONSTRAINT [FK_Retention_RetentionType]
GO
ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_Setting_Module] FOREIGN KEY([id_module])
REFERENCES [dbo].[Module] ([id])
GO
ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_Setting_Module]
GO
ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_Setting_SettingDataType] FOREIGN KEY([id_settingDataType])
REFERENCES [dbo].[SettingDataType] ([id])
GO
ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_Setting_SettingDataType]
GO
ALTER TABLE [dbo].[SettingDetail]  WITH CHECK ADD  CONSTRAINT [FK_SettingDetail_Setting] FOREIGN KEY([id_setting])
REFERENCES [dbo].[Setting] ([id])
GO
ALTER TABLE [dbo].[SettingDetail] CHECK CONSTRAINT [FK_SettingDetail_Setting]
GO
ALTER TABLE [dbo].[SettingNotification]  WITH CHECK ADD  CONSTRAINT [FK_SettingNotification_DocumentState] FOREIGN KEY([id_documentState])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[SettingNotification] CHECK CONSTRAINT [FK_SettingNotification_DocumentState]
GO
ALTER TABLE [dbo].[SettingNotification]  WITH CHECK ADD  CONSTRAINT [FK_SettingNotification_DocumentType] FOREIGN KEY([id_documentType])
REFERENCES [dbo].[DocumentType] ([id])
GO
ALTER TABLE [dbo].[SettingNotification] CHECK CONSTRAINT [FK_SettingNotification_DocumentType]
GO
ALTER TABLE [dbo].[SettingPriceList]  WITH CHECK ADD  CONSTRAINT [FK_SettingPriceList_DocumentState] FOREIGN KEY([id_crateState])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[SettingPriceList] CHECK CONSTRAINT [FK_SettingPriceList_DocumentState]
GO
ALTER TABLE [dbo].[SettingPriceList]  WITH CHECK ADD  CONSTRAINT [FK_SettingPriceList_DocumentState1] FOREIGN KEY([id_aprovedState])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[SettingPriceList] CHECK CONSTRAINT [FK_SettingPriceList_DocumentState1]
GO
ALTER TABLE [dbo].[SettingPriceList]  WITH CHECK ADD  CONSTRAINT [FK_SettingPriceList_DocumentState2] FOREIGN KEY([id_reversedState])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[SettingPriceList] CHECK CONSTRAINT [FK_SettingPriceList_DocumentState2]
GO
ALTER TABLE [dbo].[SettingPriceList]  WITH CHECK ADD  CONSTRAINT [FK_SettingPriceList_UserGroup] FOREIGN KEY([id_userGroupApproval])
REFERENCES [dbo].[UserGroup] ([id])
GO
ALTER TABLE [dbo].[SettingPriceList] CHECK CONSTRAINT [FK_SettingPriceList_UserGroup]
GO
ALTER TABLE [dbo].[StateOfContry]  WITH CHECK ADD  CONSTRAINT [FK_StateOfContry_Country] FOREIGN KEY([id_country])
REFERENCES [dbo].[Country] ([id])
GO
ALTER TABLE [dbo].[StateOfContry] CHECK CONSTRAINT [FK_StateOfContry_Country]
GO
ALTER TABLE [dbo].[TAction]  WITH CHECK ADD  CONSTRAINT [FK_TAction_TController] FOREIGN KEY([id_controller])
REFERENCES [dbo].[TController] ([id])
GO
ALTER TABLE [dbo].[TAction] CHECK CONSTRAINT [FK_TAction_TController]
GO
ALTER TABLE [dbo].[tbsysDocumentTypeDocumentState]  WITH CHECK ADD  CONSTRAINT [FK_tbsysDocumentTypeDocumentState_DocumenteState] FOREIGN KEY([id_DocumenteState])
REFERENCES [dbo].[DocumentState] ([id])
GO
ALTER TABLE [dbo].[tbsysDocumentTypeDocumentState] CHECK CONSTRAINT [FK_tbsysDocumentTypeDocumentState_DocumenteState]
GO
ALTER TABLE [dbo].[tbsysDocumentTypeDocumentState]  WITH CHECK ADD  CONSTRAINT [FK_tbsysDocumentTypeDocumentState_DocumentType] FOREIGN KEY([id_DocumentType])
REFERENCES [dbo].[DocumentType] ([id])
GO
ALTER TABLE [dbo].[tbsysDocumentTypeDocumentState] CHECK CONSTRAINT [FK_tbsysDocumentTypeDocumentState_DocumentType]
GO
ALTER TABLE [dbo].[TypeFiltersConfigurationComparisonOperator]  WITH CHECK ADD  CONSTRAINT [FK_TypeFiltersConfigurationComparisonOperator_ComparisonOperator] FOREIGN KEY([id_comparisonOperator])
REFERENCES [dbo].[ComparisonOperator] ([id])
GO
ALTER TABLE [dbo].[TypeFiltersConfigurationComparisonOperator] CHECK CONSTRAINT [FK_TypeFiltersConfigurationComparisonOperator_ComparisonOperator]
GO
ALTER TABLE [dbo].[TypeFiltersConfigurationComparisonOperator]  WITH CHECK ADD  CONSTRAINT [FK_TypeFiltersConfigurationComparisonOperator_TypeFiltersConfiguration] FOREIGN KEY([id_typeFiltersConfiguration])
REFERENCES [dbo].[TypeFiltersConfiguration] ([id])
GO
ALTER TABLE [dbo].[TypeFiltersConfigurationComparisonOperator] CHECK CONSTRAINT [FK_TypeFiltersConfigurationComparisonOperator_TypeFiltersConfiguration]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Company]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Employee] FOREIGN KEY([id_employee])
REFERENCES [dbo].[Employee] ([id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Employee]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_User] FOREIGN KEY([id_userCreate])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_User]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_User1] FOREIGN KEY([id_userUpdate])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_User1]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_UserGroup] FOREIGN KEY([id_group])
REFERENCES [dbo].[UserGroup] ([id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_UserGroup]
GO
ALTER TABLE [dbo].[UserEmissionPoint]  WITH CHECK ADD  CONSTRAINT [FK_UserCompanyInformation_EmissionPoint] FOREIGN KEY([id_emissionPoint])
REFERENCES [dbo].[EmissionPoint] ([id])
GO
ALTER TABLE [dbo].[UserEmissionPoint] CHECK CONSTRAINT [FK_UserCompanyInformation_EmissionPoint]
GO
ALTER TABLE [dbo].[UserEmissionPoint]  WITH CHECK ADD  CONSTRAINT [FK_UserCompanyInformation_User] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[UserEmissionPoint] CHECK CONSTRAINT [FK_UserCompanyInformation_User]
GO
ALTER TABLE [dbo].[UserEmployeeInformation]  WITH CHECK ADD  CONSTRAINT [FK_UserEmployeeInformation_Employee] FOREIGN KEY([id_employee])
REFERENCES [dbo].[Employee] ([id])
GO
ALTER TABLE [dbo].[UserEmployeeInformation] CHECK CONSTRAINT [FK_UserEmployeeInformation_Employee]
GO
ALTER TABLE [dbo].[UserEmployeeInformation]  WITH CHECK ADD  CONSTRAINT [FK_UserEmployeeInformation_User] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[UserEmployeeInformation] CHECK CONSTRAINT [FK_UserEmployeeInformation_User]
GO
ALTER TABLE [dbo].[UserGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserGroup_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[UserGroup] CHECK CONSTRAINT [FK_UserGroup_Company]
GO
ALTER TABLE [dbo].[UserGroupMenu]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupMenu_Menu] FOREIGN KEY([id_menu])
REFERENCES [dbo].[Menu] ([id])
GO
ALTER TABLE [dbo].[UserGroupMenu] CHECK CONSTRAINT [FK_UserGroupMenu_Menu]
GO
ALTER TABLE [dbo].[UserGroupMenu]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupMenu_UserGroup] FOREIGN KEY([id_userGroup])
REFERENCES [dbo].[UserGroup] ([id])
GO
ALTER TABLE [dbo].[UserGroupMenu] CHECK CONSTRAINT [FK_UserGroupMenu_UserGroup]
GO
ALTER TABLE [dbo].[UserGroupMenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupMenuPermission_Permission] FOREIGN KEY([id_permission])
REFERENCES [dbo].[Permission] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserGroupMenuPermission] CHECK CONSTRAINT [FK_UserGroupMenuPermission_Permission]
GO
ALTER TABLE [dbo].[UserGroupMenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupMenuPermission_UserGroupMenu] FOREIGN KEY([id_userGroupMenu])
REFERENCES [dbo].[UserGroupMenu] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserGroupMenuPermission] CHECK CONSTRAINT [FK_UserGroupMenuPermission_UserGroupMenu]
GO
ALTER TABLE [dbo].[UserMenu]  WITH CHECK ADD  CONSTRAINT [FK_UserMenu_Menu] FOREIGN KEY([id_menu])
REFERENCES [dbo].[Menu] ([id])
GO
ALTER TABLE [dbo].[UserMenu] CHECK CONSTRAINT [FK_UserMenu_Menu]
GO
ALTER TABLE [dbo].[UserMenu]  WITH CHECK ADD  CONSTRAINT [FK_UserMenu_User] FOREIGN KEY([id_user])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[UserMenu] CHECK CONSTRAINT [FK_UserMenu_User]
GO
ALTER TABLE [dbo].[UserMenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_UserMenuPermission_Permission] FOREIGN KEY([id_permission])
REFERENCES [dbo].[Permission] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserMenuPermission] CHECK CONSTRAINT [FK_UserMenuPermission_Permission]
GO
ALTER TABLE [dbo].[UserMenuPermission]  WITH CHECK ADD  CONSTRAINT [FK_UserMenuPermission_UserMenu] FOREIGN KEY([id_userMenu])
REFERENCES [dbo].[UserMenu] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserMenuPermission] CHECK CONSTRAINT [FK_UserMenuPermission_UserMenu]
GO
ALTER TABLE [dbo].[UserRol]  WITH CHECK ADD  CONSTRAINT [FK_UserRol_Company] FOREIGN KEY([id_company])
REFERENCES [dbo].[Company] ([id])
GO
ALTER TABLE [dbo].[UserRol] CHECK CONSTRAINT [FK_UserRol_Company]
GO
ALTER TABLE [dbo].[UserRolUser]  WITH CHECK ADD  CONSTRAINT [FK_UserRolUser_User] FOREIGN KEY([id_User])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[UserRolUser] CHECK CONSTRAINT [FK_UserRolUser_User]
GO
ALTER TABLE [dbo].[UserRolUser]  WITH CHECK ADD  CONSTRAINT [FK_UserRolUser_UserRol] FOREIGN KEY([id_userRol])
REFERENCES [dbo].[UserRol] ([id])
GO
ALTER TABLE [dbo].[UserRolUser] CHECK CONSTRAINT [FK_UserRolUser_UserRol]
GO
