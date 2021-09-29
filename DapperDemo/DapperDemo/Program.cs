using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
            await CountFunction();
            Console.WriteLine("Success");
        }

        private static async Task InsertBook()
        {
            var book = new Book
            {
                Id = 1025,
                AuthorId = 2,
                CategoryId = 1,
                Description = "Jest pierwszą z pięciu części sagi o wiedźminie tego autora.",
                Name = "Krew elfów"
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Books (Id, AuthorId, CategoryId, Description, Name) Values (@Id, @AuthorId, @CategoryId, @Description, @Name)";
                int rowsAffected = await connection.ExecuteAsync(sql, book);
            }

            var category = new Category
            {
                Id = 3,
                Name = "kryminał"
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Categories (Id, Name) Values (@Id, @Name)";
                int rowsAffected = await connection.ExecuteAsync(sql, category);
            }
        }

        private static async Task GetBooks()
        {
            List<Book> books;
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM BOOKS";
                var result = await connection.QueryAsync<Book>(sql);
                books = result.ToList();
            }

            Console.WriteLine("BookName\tAuthorId\tDescription");
            foreach (var book in books)
            {
                Console.WriteLine($"{book.Name}\t{book.AuthorId}\t{book.Description}");
            }
        }

        private static async Task Update()
        {
            var category = new Category
            {
                Id = 1,
                Name = "popularnonaukowe"
            };
            var parameters = new { Name = "aaaaaa", Id = 1 };
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Categories SET Name = @Name WHERE Id = @Id";
                int rowsAffected = await connection.ExecuteAsync(sql, parameters);
            }
        }

        private static async Task Delete()
        {
            var parameters = new { Id = 1 };
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Categories WHERE Id = @Id";
                int rowsAffected = await connection.ExecuteAsync(sql, parameters);
            }
        }

        private static async Task CountFunction()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT dbo.f_GetAuthorBooksCount(@AuthorId)";
                var parameter = new { AuthorId = 1 };
                int result = await connection.ExecuteScalarAsync<int>(sql, parameter);
                Console.WriteLine($"Count: {result}");
            }
        }
    }
}
