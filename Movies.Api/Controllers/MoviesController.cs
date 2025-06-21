using Microsoft.AspNetCore.Mvc;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Application.Models;
using Movies.Contracts.Responses;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase {
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieRepository) {
        _movieService = movieRepository;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request) {
        Movie movie = request.MapToMovie();
        bool result = await _movieService.CreateAsync(movie);
        if(!result)  return BadRequest();
        MovieResponse response = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug) {

        Movie? movie = Guid.TryParse(idOrSlug, out Guid id) 
            ? await _movieService.GetByIdAsync(id)
            : await _movieService.GetBySlugAsync(idOrSlug); 
        if(movie is null) return NotFound();
        MovieResponse response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll() {
        IEnumerable<Movie> movies = await _movieService.GetAllAsync();
        MoviesResponse moviesResponse = movies.MapToResponse();
        return Ok(moviesResponse);
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateMovieRequest request) {
        Movie movie = request.MapToMovie(id);
        Movie? updatedMovie = await _movieService.UpdateAsync(movie);
        if(updatedMovie is null) return NotFound();
        MovieResponse response = updatedMovie.MapToResponse();
        return Ok(response);

    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute]Guid id) {
        bool deleted = await _movieService.DeleteByIdAsync(id);
        return deleted ? Ok() : NotFound();
    }
}

