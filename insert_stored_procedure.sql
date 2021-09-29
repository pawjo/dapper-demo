DROP PROCEDURE dbo.sp_InsertNewAuthor

GO

CREATE PROCEDURE dbo.sp_InsertNewAuthor
	@FirstName NCHAR(20),
	@LastName NCHAR(20), 
    @Nationality NCHAR(20)
AS
BEGIN

DECLARE @Id INT;

SET @Id = (SELECT MAX(Id) FROM Authors) + 1

INSERT INTO Authors (Id, FirstName, LastName, Nationality)
VALUES (@Id, @FirstName, @LastName, @Nationality)

END
GO

