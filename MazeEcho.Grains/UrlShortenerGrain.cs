using MazeEcho.Interfaces;

namespace MazeEcho.Grains;

using Orleans;

public sealed class UrlShortenerGrain(
    [PersistentState(stateName: "url", storageName: "urls")]
    IPersistentState<UrlDetails> state)
    : Grain, IUrlShortenerGrain
{
    public Task<string> GetUrl() => Task.FromResult(state.State.FullUrl);


    public async Task SetUrl(string url)
    {
        state.State = new UrlDetails()
        {
            FullUrl = url,
            ShortenedRouteSegment = this.GetPrimaryKeyString()
        };
        await state.WriteStateAsync();
    }
}