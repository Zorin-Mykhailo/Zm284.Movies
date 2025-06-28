using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository {
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);

    //TODO: Task<MovieDto?> GetByIdAsync(Guid id);
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);

    //TODO: Task<MovieDto?> GetBySlugAsync(string slug);
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);

    //TODO: Task<IEnumerable<MovieDto>> GetAllAsync();
    Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken token = default);

    Task<bool> UpdateAsync(Movie movie, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<bool> ExistByIdAsync(Guid id, CancellationToken token = default);
}
