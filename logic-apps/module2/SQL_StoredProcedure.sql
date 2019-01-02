/*    ==Scripting Parameters==

    Source Database Engine Edition : Microsoft Azure SQL Database Edition
    Source Database Engine Type : Microsoft Azure SQL Database

    Target Database Engine Edition : Microsoft Azure SQL Database Edition
    Target Database Engine Type : Microsoft Azure SQL Database
*/

/****** Object:  StoredProcedure [dbo].[Query_TransactionsTotal]    Script Date: 2/20/2018 12:00:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      Stephen W Thomas
-- Create Date: 2-13-2018
-- Description: Returns the sum of of purchases for a giving reward number
-- =============================================
CREATE PROCEDURE [dbo].[Query_TransactionsTotal]
(
	@AccountNumber nvarchar(50),
	@SpendSum money OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON

	Set @SpendSum = (select sum(Amount) from Transactions where AccountNumber = @AccountNumber)

END
GO


