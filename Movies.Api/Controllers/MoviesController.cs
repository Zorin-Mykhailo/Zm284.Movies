using Microsoft.AspNetCore.Mvc;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Application.Models;
using Movies.Contracts.Responses;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase {
    private readonly ILogger<MoviesController> _logger;
    private readonly IMovieService _movieService;

    public MoviesController(ILogger<MoviesController> logger, IMovieService movieRepository) {
        _logger = logger;
        _movieService = movieRepository;
    }

    [Authorize(AuthConstants.TrustedMemberClaimName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request, CancellationToken token) {
        Movie movie = request.MapToMovie();
        bool result = await _movieService.CreateAsync(movie);
        if(!result)  return BadRequest();
        MovieResponse response = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token) {
        _logger.LogInformation("Запит до Movies.Get");
        Movie? movie = Guid.TryParse(idOrSlug, out Guid id) 
            ? await _movieService.GetByIdAsync(id, token)
            : await _movieService.GetBySlugAsync(idOrSlug, token); 
        if(movie is null) return NotFound();
        MovieResponse response = movie.MapToResponse();
        return Ok(response);
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token) {
        IEnumerable<Movie> movies = await _movieService.GetAllAsync(token);
        MoviesResponse moviesResponse = movies.MapToResponse();
        return Ok(moviesResponse);
    }

    [Authorize(AuthConstants.TrustedMemberClaimName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateMovieRequest request, CancellationToken token) {
        Movie movie = request.MapToMovie(id);
        Movie? updatedMovie = await _movieService.UpdateAsync(movie, token);
        if(updatedMovie is null) return NotFound();
        MovieResponse response = updatedMovie.MapToResponse();
        return Ok(response);

    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken token) {
        bool deleted = await _movieService.DeleteByIdAsync(id, token);
        return deleted ? Ok() : NotFound();
    }
}

