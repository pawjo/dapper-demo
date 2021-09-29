namespace DapperDemo.Models
{
    public class Book
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }

        public int CategoryId { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
    }
}
