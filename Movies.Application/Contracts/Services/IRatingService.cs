namespace Movies.Application.Contracts.Services;

public interface IRatingService {
    Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default);
}
