using Orleans;

namespace MazeEcho.Grains;

[GenerateSerializer, Alias(nameof(UrlDetails))]
public sealed record UrlDetails
{
    [Id(0)] public string FullUrl { get; set; } = "";
    [Id(1)] public string ShortenedRouteSegment { get; set; } = "";
}