namespace LMDb.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string? Title { get; set; } // Nullable string
        public string? Director { get; set; } // Nullable string
        public int Year { get; set; }

        // Override ToString for easier display
        public override string ToString()
        {
            return $"ID: {Id}, Title: \"{Title}\", Director: {Director}, Year: {Year}";
        }
    }

}