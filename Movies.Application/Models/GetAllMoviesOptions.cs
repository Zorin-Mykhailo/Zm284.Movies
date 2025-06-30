using System.Xml.XPath;

namespace Movies.Application.Models;

public class GetAllMoviesOptions {
    public int Page { get; init; }

    public int PageSize { get; init; }


    public string? Title { get; set; }

    public int? YearOfRelease { get; set; }

    public Guid? UserId { get; set; }

    public string? SortField { get; set; }

    public SortOrder? SortOrder { get; set; }
}


public enum SortOrder {
    Unsorted,
    Ascending,
    Descending,
}
