using Movies.Application.Contracts.Repositories;
using Movies.Application.Contracts.Services;
using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;

namespace Movies.Application.Services;

public class RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository) : IRatingService {
    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default) {
        if(rating is <= 0 or > 5) {
            throw new ValidationException(new[] {
                new ValidationFailure {
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5"
                }
            });
        }

        bool movieExist = await movieRepository.ExistByIdAsync(movieId);

        if(!movieExist) return false;

        return await ratingRepository.RateMovieAsync(movieId, rating, userId, token);
    }


    public Task<bool> DeleteRatingAsync(Guid movieID, Guid userId, CancellationToken token = default) {
        return ratingRepository.DeleteRatingAsync(movieID, userId, token);
    }

    public Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default) {
        return ratingRepository.GetRatingsForUserAsync(userId, token);
    }
}
