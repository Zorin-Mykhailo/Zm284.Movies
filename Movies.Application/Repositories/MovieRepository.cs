using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository {
    //private readonly List<Movie> _movies = new();
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory) {
        _dbConnectionFactory = dbConnectionFactory;
    }


    public async Task<bool> CreateAsync(Movie movie) {
        //_movies.Add(movie);
        //return Task.FromResult(true);
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into movies (id, slug, title, yearofrelease)
            values (@Id, @Slug, @Title, @YearOfRelease
            """, movie));

        if(result > 0) {
            foreach(var genre in movie.Genres) {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId, name)
                    values (@MovieId, @Name)
                    """, new {MovieId = movie.Id, Name = genre}));
            }
        }

        transaction.Commit();

        return result > 0;
    }

    public Task<Movie?> GetByIdAsync(Guid id) {
        //Movie? movie = _movies.SingleOrDefault(x => x.Id == id);
        //return Task.FromResult(movie);
        throw new NotImplementedException();
    }

    

    public Task<IEnumerable<Movie>> GetAllAsync() {
        //return Task.FromResult(_movies.AsEnumerable());
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Movie movie) {
        //int movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
        //if(movieIndex == -1)
        //    return Task.FromResult(false);
        //_movies[movieIndex] = movie;
        //return Task.FromResult(true);
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByIdAsync(Guid id) {
        //int removedCount = _movies.RemoveAll(x => x.Id == id);
        //bool movieRemoved = removedCount > 0;
        //return Task.FromResult(movieRemoved);
        throw new NotImplementedException();
    }

    public Task<Movie?> GetBySlugAsync(string slug) {
        //Movie? movie = _movies.SingleOrDefault(x => x.Slug == slug);
        //return Task.FromResult(movie);
        throw new NotImplementedException();
    }

    public Task<bool> ExistByIdAsync(Guid id) {
        throw new NotImplementedException();
    }
}
