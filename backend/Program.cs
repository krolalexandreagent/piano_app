using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<UserService>();

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
