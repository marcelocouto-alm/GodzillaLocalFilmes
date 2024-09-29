using GodzillaLocalFilmes.Data;
using Microsoft.EntityFrameworkCore;

namespace GodzillaLocalFilmes.Movies
{
    public static class MovieRoutes
    {
        public static void AddMovieRoutes(this WebApplication app)
        {
            // Endpoint para inserir um novo filme
            app.MapPost("locadora", async (AddMovieRequest request, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    bool movieExist = await context.Movies.AnyAsync(movie => movie.Title == request.Title, ct);
                    if (movieExist)
                        return Results.Conflict("Já existe um filme com este título.");

                    var newMovie = new Movie(request.Title, request.MovieDirector, request.Stock);
                    await context.Movies.AddAsync(newMovie, ct);
                    await context.SaveChangesAsync(ct);

                    return Results.Created($"/locadora/{newMovie.MovieId}", newMovie);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Ocorreu um erro: {ex.Message}");
                }
            });

            // Endpoint para buscar todos os filmes disponíveis
            app.MapGet("locadora", async (AppDbContext context, CancellationToken ct, int page = 1, int pageSize = 10) =>
            {
                try
                {
                    var movies = await context.Movies
                        .Where(m => m.Stock > 0)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync(ct);

                    return Results.Ok(movies);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Ocorreu um erro: {ex.Message}");
                }
            });

            // Endpoint para buscar filmes pelo título
            app.MapGet("locadora/godzilla", async (string title, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(title))
                        return Results.BadRequest("O parâmetro 'title' deve ser informado.");

                    var movies = await context.Movies
                        .Where(movie => movie.Title.Contains(title))
                        .ToListAsync(ct);

                    if (!movies.Any())
                        return Results.NotFound("Nenhum filme encontrado com o título especificado.");

                    return Results.Ok(movies);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Ocorreu um erro: {ex.Message}");
                }
            });

            // Endpoint para alugar um filme pelo ID
            app.MapPost("godzilla", async (Guid id, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    var movie = await context.Movies.FindAsync(id);

                    if (movie == null)
                        return Results.NotFound("Filme não encontrado.");

                    if (movie.Stock <= 0)
                        return Results.StatusCode(403);

                    movie.Stock--; // Diminui o estoque em 1
                    await context.SaveChangesAsync(ct);

                    return Results.Ok("Filme alugado com sucesso.");
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Ocorreu um erro: {ex.Message}");
                }
            });
        }
    }
}
