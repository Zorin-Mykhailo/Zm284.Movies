using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Contracts.Services;
using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(ILogger<MoviesController> logger, IMovieService movieRepository) : ControllerBase {
    [Authorize]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token) {
        logger.LogInformation("Запит до Movies.Get");
        Guid? userId = HttpContext.GetUserId();
        Movie? movie = Guid.TryParse(idOrSlug, out Guid id)
            ? await movieRepository.GetByIdAsync(id, userId, token)
            : await movieRepository.GetBySlugAsync(idOrSlug, userId, token);
        if(movie is null) return NotFound();
        MovieResponse response = movie.MapToResponse();
        return Ok(response);
    }



    [Authorize]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token) {
        Guid? userId = HttpContext.GetUserId();
        GetAllMoviesOptions options = request.MapToOptions()
            .WithUser(userId);
        IEnumerable<Movie> movies = await movieRepository.GetAllAsync(options, token);
        MoviesResponse moviesResponse = movies.MapToResponse();
        return Ok(moviesResponse);
    }



    [Authorize(AuthConstants.TrustedMemberClaimName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token) {
        Movie movie = request.MapToMovie();
        bool result = await movieRepository.CreateAsync(movie);
        if(!result) return BadRequest();
        MovieResponse response = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
    }



    [Authorize(AuthConstants.TrustedMemberClaimName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token) {
        Movie movie = request.MapToMovie(id);
        Guid? userId = HttpContext.GetUserId();
        Movie? updatedMovie = await movieRepository.UpdateAsync(movie, userId, token);
        if(updatedMovie is null) return NotFound();
        MovieResponse response = updatedMovie.MapToResponse();
        return Ok(response);

    }



    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token) {
        bool deleted = await movieRepository.DeleteByIdAsync(id, token);
        return deleted ? Ok() : NotFound();
    }
}

