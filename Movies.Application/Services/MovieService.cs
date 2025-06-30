using FluentValidation;
using Movies.Application.Contracts.Repositories;
using Movies.Application.Contracts.Services;
using Movies.Application.Models;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository movieRepository, IRatingRepository ratingRepository, IValidator<Movie> movieValidator, IValidator<GetAllMoviesOptions> optionsValidator) : IMovieService {
    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default) {
        await optionsValidator.ValidateAndThrowAsync(options, token);
        return await movieRepository.GetAllAsync(options, token);
    }
    
    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default) {
        return movieRepository.GetByIdAsync(id, userId, token);
    }
    
    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default) {
        return movieRepository.GetBySlugAsync(slug, userId, token);
    }
    
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default) {
        await movieValidator.ValidateAndThrowAsync(movie, token);
        return await movieRepository.CreateAsync(movie, token);
    }
    
    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default) {
        await movieValidator.ValidateAndThrowAsync(movie, token);
        bool movieExist = await movieRepository.ExistByIdAsync(movie.Id, token);
        if(!movieExist) return null;
        await movieRepository.UpdateAsync(movie, token);

        if(!userId.HasValue) {
            float? rating = await ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }

        var ratings = await ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default) {
        return movieRepository.DeleteByIdAsync(id, token);
    }
    
}
