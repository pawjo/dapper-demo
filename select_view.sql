DROP VIEW dbo.BooksWithAuthorNames;

GO

CREATE VIEW dbo.BooksWithAuthorNames AS
SELECT Books.Name as BookName, Authors.FirstName, Authors.LastName
FROM Books
LEFT JOIN Authors
ON Books.AuthorId = Authors.Id

GO

select * from BooksWithAuthorNames;
