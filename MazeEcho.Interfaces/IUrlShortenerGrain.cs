namespace MazeEcho.Interfaces;

using Orleans;

public interface IUrlShortenerGrain : IGrainWithStringKey
{
    Task<string> GetUrl();
    Task SetUrl(string url);
}