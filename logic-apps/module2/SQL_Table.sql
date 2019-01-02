/*    ==Scripting Parameters==

    Source Database Engine Edition : Microsoft Azure SQL Database Edition
    Source Database Engine Type : Microsoft Azure SQL Database

    Target Database Engine Edition : Microsoft Azure SQL Database Edition
    Target Database Engine Type : Microsoft Azure SQL Database
*/

/****** Object:  Table [dbo].[Transactions]    Script Date: 2/20/2018 12:01:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Transactions](
	[Id] [int] NULL,
	[AccountNumber] [nvarchar](50) NULL,
	[Amount] [money] NULL,
	[LastModified] [datetime] NOT NULL
)
GO

ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_LastModified]  DEFAULT (getdate()) FOR [LastModified]
GO


