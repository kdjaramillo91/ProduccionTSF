
DROP TABLE IF EXISTS AccountingFreight
CREATE TABLE AccountingFreight (Id int Primary Key Identity(1,1) NOT NULL,
								id_processPlant INT NOT NULL ,
								liquidation_type CHAR(1) NOT NULL,
								id_userCreate INT NOT NULL, 
								dateCreate Datetime NOT NULL, 
								id_userUpdate INT,
								dateUpdate Datetime,
								isActive BIT NOT NULL
								)

DROP TABLE IF EXISTS AccountingFreightDetails
CREATE TABLE AccountingFreightDetails (Id int Primary Key Identity(1,1) NOT NULL,
								id_accountingFreight int NOT NULL, 
								accountingAccountCode nvarchar(20),
								isAuxiliar BIT,
								code_Auxiliar NVARCHAR(8),
								idAuxContable NVARCHAR(8),
								id_costCenter INT,
								id_subCostCenter INT,
								accountType Char(1) NOT NULL,
								id_userCreate INT NOT NULL, 
								dateCreate Datetime NOT NULL, 
								id_userUpdate INT,
								dateUpdate Datetime,
								isActive BIT NOT NULL
								)

ALTER TABLE [dbo].[AccountingFreightDetails]  WITH CHECK ADD  CONSTRAINT [FK_AccountingFreightDetails_AccountingFreight_id_accountingFreight] FOREIGN KEY([id_accountingFreight])
REFERENCES [dbo].[AccountingFreight] ([Id])




