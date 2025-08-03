using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register services. UserService manages user data; PianoService manages piano locations.
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<PianoService>();

var app = builder.Build();

// Sign-up endpoint
app.MapPost("/signup", (SignUpRequest req, UserService userService) =>
{
    if (!IsValidEmail(req.Email))
    {
        return Results.BadRequest("Invalid email.");
    }

    bool created = userService.AddUser(req.Username, req.Email, req.Password);
    return created ? Results.Ok("User created successfully.") : Results.BadRequest("Username or email already exists.");
});

// Login endpoint
app.MapPost("/login", (LoginRequest req, UserService userService) =>
{
    bool valid = userService.VerifyUser(req.Username, req.Password);
    return valid ? Results.Ok("Login successful.") : Results.Unauthorized();
});

// Pianos endpoints
// Returns all pianos currently known to the system. The PianoService caches values from a CSV file.
app.MapGet("/pianos", (PianoService pianoService) =>
{
    var pianos = pianoService.GetPianos();
    return Results.Ok(pianos);
});

// Adds a new piano. Expects a Piano payload containing latitude, longitude and name. ID is ignored.
app.MapPost("/pianos", (Piano piano, PianoService pianoService) =>
{
    pianoService.AddPiano(piano.Latitude, piano.Longitude, piano.Name);
    return Results.Ok("Piano added successfully.");
});

// Refreshes the piano cache from disk. This can be called after external changes to the CSV file.
app.MapPost("/pianos/refresh", (PianoService pianoService) =>
{
    pianoService.Refresh();
    return Results.Ok("Piano list refreshed.");
});

app.Run();

static bool IsValidEmail(string email)
{
    try
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
    catch
    {
        return false;
    }
}
