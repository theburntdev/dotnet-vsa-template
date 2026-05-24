using System.Text.Json.Serialization;

namespace BackendTemplate.Api.Common;

public record Page<T>(
    IReadOnlyList<T> Items,
    int Total,
    [property: JsonPropertyName("page")] int CurrentPage,
    int PageSize);
