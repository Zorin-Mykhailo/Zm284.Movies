using Movies.Application.Models;

namespace Movies.Application.Contracts.Repositories;

public interface IMovieRepository {
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);

    //TODO: Task<MovieDto?> GetByIdAsync(Guid id);
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);

    //TODO: Task<MovieDto?> GetBySlugAsync(string slug);
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);

    //TODO: Task<IEnumerable<MovieDto>> GetAllAsync();
    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default);

    Task<bool> UpdateAsync(Movie movie, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<bool> ExistByIdAsync(Guid id, CancellationToken token = default);

    Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default);
}
