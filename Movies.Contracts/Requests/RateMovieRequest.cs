using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.Contracts.Requests;
public class RateMovieRequest {
    public required int Rating { get; init; }
}
