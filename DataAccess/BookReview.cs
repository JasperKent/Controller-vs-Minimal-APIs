namespace DataAccess
{
    public class BookReview
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required double Rating { get; set; }
    }
}