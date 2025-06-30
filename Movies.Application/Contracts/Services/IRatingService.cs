using Movies.Application.Models;

namespace Movies.Application.Contracts.Services;

public interface IRatingService {
    Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default);

    Task<bool> DeleteRatingAsync(Guid movieID, Guid userId, CancellationToken token = default);

    Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
}
