using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.Contracts.Requests;
public class GetAllMoviesRequest : IPagedRequest {
    
    public required int Page { get; init; } = 1;
    
    public required int PageSize { get; init; } = 10;

    public required string? Title { get; init; }

    public required int? Year { get; init; }

    public required string? SortBy { get; init; }
}
