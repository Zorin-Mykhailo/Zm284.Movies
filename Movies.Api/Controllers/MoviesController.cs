using Microsoft.AspNetCore.Mvc;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Application.Models;
using Movies.Contracts.Responses;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Movies.Api.Controllers;

[Authorize]
[ApiController]
public class MoviesController : ControllerBase {
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieRepository) {
        _movieService = movieRepository;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request, CancellationToken token) {
        Movie movie = request.MapToMovie();
        bool result = await _movieService.CreateAsync(movie);
        if(!result)  return BadRequest();
        MovieResponse response = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token) {

        Movie? movie = Guid.TryParse(idOrSlug, out Guid id) 
            ? await _movieService.GetByIdAsync(id, token)
            : await _movieService.GetBySlugAsync(idOrSlug, token); 
        if(movie is null) return NotFound();
        MovieResponse response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token) {
        IEnumerable<Movie> movies = await _movieService.GetAllAsync(token);
        MoviesResponse moviesResponse = movies.MapToResponse();
        return Ok(moviesResponse);
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateMovieRequest request, CancellationToken token) {
        Movie movie = request.MapToMovie(id);
        Movie? updatedMovie = await _movieService.UpdateAsync(movie, token);
        if(updatedMovie is null) return NotFound();
        MovieResponse response = updatedMovie.MapToResponse();
        return Ok(response);

    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken token) {
        bool deleted = await _movieService.DeleteByIdAsync(id, token);
        return deleted ? Ok() : NotFound();
    }
}

