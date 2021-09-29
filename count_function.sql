DROP FUNCTION dbo.f_GetAuthorBooksCount
GO

CREATE FUNCTION dbo.f_GetAuthorBooksCount(@AuthorId INT)
RETURNS INT
AS 
BEGIN 
    DECLARE @returnvalue INT;

    SELECT @returnvalue = COUNT(*) 
    FROM Books
    WHERE AuthorID = @AuthorId

    RETURN(@returnvalue);
END