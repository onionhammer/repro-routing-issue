
var builder = WebApplication.CreateBuilder(args);

// Configure route
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap["partner"] = typeof(PartnerRouteConstraint);
});

var app = builder.Build();

// Create an '/api' route
var api = app.MapGroup("/api");

api.Map("/tiles/{**path}", async (HttpContext context, string path) =>
{
    var tile = new {
        Path = path,
        Zoom = 10,
        X = 10,
        Y = 10
    };

    await context.Response.WriteAsJsonAsync(tile);
});

api.MapGet("/{partner:partner}/{form}.js", (string partner, string form) =>
{
    return Results.Text($"console.log('Hello from {partner}/{form}.js');", contentType: "application/javascript");
});

app.Run();


class PartnerRouteConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var value) && value is string str)
        {
            if (str == "partner1" || str == "partner2")
            {
                return true;
            }

            throw new Exception("Invalid partner, thrown for demonstration purposes");
        }

        return false;
    }
}