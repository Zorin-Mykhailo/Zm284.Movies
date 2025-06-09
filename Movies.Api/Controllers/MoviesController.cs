using Microsoft.AspNetCore.Mvc;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Application.Models;
using Movies.Contracts.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Movies.Api.Mapping;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase {
    private readonly IMovieRepository _movieRepository;

    public MoviesController(IMovieRepository movieRepository) {
        _movieRepository = movieRepository;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request) {
        Movie movie = request.MapToMovie();
        bool result = await _movieRepository.CreateAsync(movie);
        return Created($"/{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id) {
        Movie? movie = await _movieRepository.GetByIdAsync(id);
        if(movie is null) return NotFound();
        MovieResponse response = movie.MapToResponse();
        return Ok(response);
    }
}

