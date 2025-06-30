using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.Contracts.Requests;
public interface IPagedRequest {
    public int Page { get; init; }

    public int PageSize { get; init; }
}
