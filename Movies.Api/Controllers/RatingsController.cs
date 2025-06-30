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
public class RatingsController(IRatingService ratingService) : ControllerBase {

    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken token = default) {
        Guid? userId = HttpContext.GetUserId();
        bool result = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);
        return result ? Ok() : NotFound();
    }



    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token = default) {
        Guid? userId = HttpContext.GetUserId();
        bool result = await ratingService.DeleteRatingAsync(id, userId!.Value, token);
        return result ? Ok() : NotFound();
    }



    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> GetUserRatings(CancellationToken token = default) {
        Guid? userId = HttpContext.GetUserId();
        IEnumerable<MovieRating> ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
        IEnumerable<MovieRatingResponse> ratingsResponse = ratings.MapToResponse();
        return Ok(ratingsResponse);
    }
}
