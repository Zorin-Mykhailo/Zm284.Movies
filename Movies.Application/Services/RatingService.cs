using Movies.Application.Contracts.Repositories;
using Movies.Application.Contracts.Services;
using FluentValidation;
using FluentValidation.Results;

namespace Movies.Application.Services;

public class RatingService : IRatingService {
    private readonly IRatingRepository _ratingRepository;
    private readonly IMovieRepository _movieRepository;

    public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository) { 
        _ratingRepository = ratingRepository; 
        _movieRepository = movieRepository;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default) {
        if(rating is <= 0 or > 5) {
            throw new ValidationException(new[] {
                new ValidationFailure {
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5"
                }
            });
        }

        bool movieExist = await _movieRepository.ExistByIdAsync(movieId);

        if(!movieExist) return false;

        return await _ratingRepository.RateMovieAsync(movieId, rating, userId, token);
    }
}
