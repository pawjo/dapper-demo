using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperDemo.Models;

namespace DapperDemo
{
    class Program
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Select mode");
            Console.WriteLine("1 - examples");
            Console.WriteLine("2 - SQL injection");

            string input = Console.ReadLine();

            Console.Clear();

            switch (input)
            {
                case "1":
                    {
                        PrintResult("Select example", await GetBooks());

                        PrintResult("Insert example", await InsertBook());

                        PrintResult("Update example", await Update("popularnonaukowe"));
                        await Update("publikacja naukowa");

                        PrintResult("Delete example", await Delete());

                        PrintResult("Count query example", await CountQuery());

                        PrintResult("Count function example", await CountFunction());

                        PrintResult("Insert stored procedure example", await InsertStoredProcedure());

                        PrintResult("Get books from view", await GetBooksWithAuthorsView());

                        break;
                    }

                case "2":
                    {
                        Console.WriteLine("Input author id for search books: ");
                        string id = Console.ReadLine();

                        PrintResult("Vulnerable select", await VulnerableSelect(id));

                        PrintResult("Secure select", await SecureSelect(id));

                        break;
                    }

                default:
                    Console.WriteLine("Invalid argument");
                    break;
            }

            Console.Write("###########################\nEnd\nPress any key to exit.");
        }

        private static async Task<Tuple<long, string>> GetBooks()
        {
            var stopwatch = new Stopwatch();
            List<Book> books;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM BOOKS";

                stopwatch.Start();
                var result = await connection.QueryAsync<Book>(sql);
                stopwatch.Stop();

                books = result.ToList();
            }

            string output = PrepareSelectBooksOutput(books);

            return Tuple.Create(stopwatch.ElapsedMilliseconds, output);
        }



        private static async Task<Tuple<long, string>> InsertBook()
        {
            int newId = await GetLastBookId() + 1;
            var book = new Book
            {
                Id = newId,
                AuthorId = 2,
                CategoryId = 1,
                Description = "Jest pierwszą z pięciu części sagi o wiedźminie tego autora.",
                Name = "Krew elfów"
            };

            var stopwatch = new Stopwatch();
            int rowsAffected;

            using (var connection = new SqlConnection(_connectionString))
            {
                stopwatch.Start();
                string sql = "INSERT INTO Books (Id, AuthorId, CategoryId, Description, Name) Values (@Id, @AuthorId, @CategoryId, @Description, @Name)";
                rowsAffected = await connection.ExecuteAsync(sql, book);
                stopwatch.Stop();
            }

            return Tuple.Create(stopwatch.ElapsedMilliseconds, $"Rows affected: {rowsAffected}");
        }

        private static async Task<Tuple<long, string>> Update(string name)
        {
            var category = new Category
            {
                Id = 1,
                Name = name
            };

            var stopwatch = new Stopwatch();
            int rowsAffected;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Categories SET Name = @Name WHERE Id = @Id";
                var parameters = new { Name = "aaaaaa", Id = 1 };

                stopwatch.Start();
                rowsAffected = await connection.ExecuteAsync(sql, parameters);
                stopwatch.Stop();
            }

            return Tuple.Create(stopwatch.ElapsedMilliseconds, $"Rows affected: {rowsAffected}");
        }

        private static async Task<Tuple<long, string>> Delete()
        {
            int id = await GetLastBookId();

            var stopwatch = new Stopwatch();
            int rowsAffected;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Books WHERE Id = @Id";
                var parameters = new { Id = id };

                stopwatch.Start();
                rowsAffected = await connection.ExecuteAsync(sql, parameters);
                stopwatch.Stop();
            }

            return Tuple.Create(stopwatch.ElapsedMilliseconds, $"Rows affected: {rowsAffected}");
        }

        private static async Task<Tuple<long, string>> CountFunction()
        {
            var stopwatch = new Stopwatch();
            int result;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT dbo.f_GetAuthorBooksCount(@AuthorId)";
                var parameter = new { AuthorId = 1 };

                stopwatch.Start();
                result = await connection.ExecuteScalarAsync<int>(sql, parameter);
                stopwatch.Stop();
            }

            return Tuple.Create(stopwatch.ElapsedMilliseconds, $"Count: {result}");
        }

        private static async Task<Tuple<long, string>> CountQuery()
        {
            var stopwatch = new Stopwatch();
            int result;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Books WHERE AuthorId = @AuthorId";
                var parameter = new { AuthorId = 1 };

                stopwatch.Start();
                result = await connection.ExecuteScalarAsync<int>(sql, parameter);
                stopwatch.Stop();
            }

            return Tuple.Create(stopwatch.ElapsedMilliseconds, $"Count: {result}");
        }

        private static async Task<Tuple<long, string>> InsertStoredProcedure()
        {
            var stopwatch = new Stopwatch();
            int rowsAffected;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "dbo.sp_InsertNewAuthor";
                var parameters = new
                {
                    FirstName = "Stephen",
                    LastName = "King",
                    Nationality = "USA"
                };

                stopwatch.Start();
                rowsAffected = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                stopwatch.Stop();
            }

            return Tuple.Create(stopwatch.ElapsedMilliseconds, $"Rows affected: {rowsAffected}");
        }

        private static async Task<Tuple<long, string>> GetBooksWithAuthorsView()
        {
            var stopwatch = new Stopwatch();
            List<object> list;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM dbo.BooksWithAuthorNames";

                stopwatch.Start();
                var result = await connection.QueryAsync<dynamic>(sql);
                stopwatch.Stop();

                list = result.ToList();
            }

            string output = "Query result";
            foreach (var item in list)
            {
                output += "\n" + item.ToString().Trim();
            }

            return Tuple.Create(stopwatch.ElapsedMilliseconds, output);
        }

        private static async Task<Tuple<long, string>> VulnerableSelect(string authorId)
        {
            var stopwatch = new Stopwatch();
            List<Book> books;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Books WHERE AuthorId =" + authorId;

                stopwatch.Start();
                var result = await connection.QueryAsync<Book>(sql);
                stopwatch.Stop();

                books = result.ToList();
            }

            var output = PrepareSelectBooksOutput(books);

            return Tuple.Create(stopwatch.ElapsedMilliseconds, output);
        }

        private static async Task<Tuple<long, string>> SecureSelect(string authorId)
        {
            var stopwatch = new Stopwatch();
            List<Book> books = null;
            string output = string.Empty;

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Books WHERE AuthorId = @AuthorId";
                var parameter = new { AuthorId = authorId };

                stopwatch.Start();
                try
                {
                    var result = await connection.QueryAsync<Book>(sql, parameter);
                    stopwatch.Stop();

                    books = result.ToList();
                }
                catch (Exception e)
                {
                    stopwatch.Stop();
                    output = e.Message;
                }
            }

            if (books != null)
            {
                output = PrepareSelectBooksOutput(books);
            }

            return Tuple.Create(stopwatch.ElapsedMilliseconds, output);
        }



        private static async Task<int> GetLastBookId()
        {
            int result;
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT MAX(Id) FROM Books";
                result = await connection.ExecuteScalarAsync<int>(sql);
            }
            return result;
        }

        private static string PrepareSelectBooksOutput(List<Book> books)
        {
            int maxLength = books.Max(x => x.Name.Length);
            int tabCount = maxLength / 9;
            string specialTab = new string('\t', tabCount);

            string output = $"BookName{specialTab}AuthorId\tDescription";
            foreach (var book in books)
            {
                output += $"\n{book.Name}{book.AuthorId}\t\t{book.Description}";
            }

            return output;
        }

        private static void PrintResult(string title, Tuple<long, string> result)
        {
            Console.WriteLine("#######################################");
            Console.WriteLine(title);
            Console.WriteLine($"\nSuccess after {result.Item1}ms\n");

            if (result.Item2 != null)
            {
                Console.WriteLine("Output:");
                Console.Write(result.Item2);
            }

            Console.Write("\n\nPress any key to continue...\n\n");
            Console.ReadKey();
        }
    }
}
