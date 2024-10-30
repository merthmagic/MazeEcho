using MazeEcho.Interfaces;
using MazeEcho.Silo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

//configure Orleans Silo
builder.Host.ConfigureSilo();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", static () => "Welcome to URL shortener service");

app.MapGet("/shorten", async (IGrainFactory grains, HttpRequest request, string url) =>
{
    var host = $"{request.Scheme}://{request.Host.Value}";
    if (string.IsNullOrWhiteSpace(url) ||
        Uri.IsWellFormedUriString(url, UriKind.Absolute) is false)
    {
        return Results.BadRequest($"""
                                   The URL query string is required and needs to be well formed.
                                   Consider, ${host}/shorten?url=https://www.microsoft.com.
                                   """);
    }

    var shortenedRouteSegment = Guid.NewGuid().GetHashCode().ToString("X");
    var shortenerGrain =
        grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);
    await shortenerGrain.SetUrl(url);
    var resultBuilder = new UriBuilder(host)
    {
        Path = $"/go/{shortenedRouteSegment}"
    };

    return Results.Ok(resultBuilder.Uri);
});

app.MapGet("/go/{shortenedRouteSegment:required}", async (IGrainFactory grains, string shortenedRouteSegment) =>
{
    var shortenerGrain =
        grains.GetGrain<IUrlShortenerGrain>(shortenedRouteSegment);
    var url = await shortenerGrain.GetUrl();
    var redirectBuilder = new UriBuilder(url);
    return Results.Redirect(redirectBuilder.Uri.ToString());
});

app.Run();