using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository {
    Task<bool> CreateAsync(Movie movie);

    //TODO: Task<MovieDto?> GetByIdAsync(Guid id);
    Task<Movie?> GetByIdAsync(Guid id);

    //TODO: Task<MovieDto?> GetBySlugAsync(string slug);
    Task<Movie?> GetBySlugAsync(string slug);

    //TODO: Task<IEnumerable<MovieDto>> GetAllAsync();
    Task<IEnumerable<Movie>> GetAllAsync();

    Task<bool> UpdateAsync(Movie movie);

    Task<bool> DeleteByIdAsync(Guid id);

    Task<bool> ExistByIdAsync(Guid id);
}
