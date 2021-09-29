DROP TABLE Books;
DROP TABLE Authors;
DROP TABLE Categories;


CREATE TABLE Authors
(
	Id INT NOT NULL PRIMARY KEY, 
    FirstName NCHAR(20) NOT NULL, 
    LastName NCHAR(20) NOT NULL, 
    Nationality NCHAR(20) NULL
);

INSERT INTO Authors VALUES (1, 'Andrzej', 'Sapkowski', 'Polska');
INSERT INTO Authors VALUES (2, 'Stephen', 'Hawking', 'Wielka brytania');


CREATE TABLE Categories
(
	Id INT NOT NULL PRIMARY KEY, 
    Name NCHAR(40) NOT NULL 
);

INSERT INTO Categories VALUES (1, 'publikacja popularnonaukowa');
INSERT INTO Categories VALUES (2, 'fantasy');


CREATE TABLE Books (
    Id          INT        NOT NULL,
    AuthorId    INT        NOT NULL,
    CategoryId  INT        NULL,
    Description NCHAR (100) NULL,
    Name        NCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

ALTER TABLE Books
ADD CONSTRAINT FK_BookAuthor
FOREIGN KEY (AuthorId) REFERENCES Authors(Id);

ALTER TABLE Books
ADD CONSTRAINT FK_BookCategory
FOREIGN KEY (CategoryId) REFERENCES Categories(Id);

INSERT INTO Books VALUES (1, 1, 2, 'Seria Wiedźmin', 'Chrzest ognia');
INSERT INTO Books VALUES (2, 1, 2, 'Seria Wiedźmin', 'Miecz przeznaczenia');
INSERT INTO Books VALUES (3, 1, 2, 'Seria Wiedźmin', 'Sezon burz');
INSERT INTO Books VALUES (4, 2, 1, '', 'Teoria wszystkiego: powstanie i losy Wszechświata');
INSERT INTO Books VALUES (5, 2, 1, '', 'Sezon burz');