namespace GodzillaLocalFilmes.Movies
{
    public class Movie
    {
        public Guid MovieId { get; init; }
        public string Title { get; set; }
        public string MovieDirector { get; set; }
        public int Stock { get; set; }

        public Movie(string title, string movieDirector, int stock)
        {
            MovieId = Guid.NewGuid();
            Title = title;
            MovieDirector = movieDirector;
            Stock = stock;
        }
    }
}
