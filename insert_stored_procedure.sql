DROP PROCEDURE dbo.sp_InsertNewAuthor

GO

CREATE PROCEDURE dbo.sp_InsertNewAuthor
	@Id INT,
	@FirstName NCHAR(20),
	@LastName NCHAR(20), 
    @Nationality NCHAR(20)
AS
BEGIN

INSERT INTO Authors (Id, FirstName, LastName, Nationality)
VALUES (@Id, @FirstName, @LastName, @Nationality)

END
GO

