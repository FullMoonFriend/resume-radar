using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using ResumeRadar.Api.Data;
using ResumeRadar.Api.Models;
using ResumeRadar.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<IPdfExtractor, PdfExtractor>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    })
    .AddOAuth("GitHub", options =>
    {
        options.ClientId = builder.Configuration["GitHub:ClientId"]!;
        options.ClientSecret = builder.Configuration["GitHub:ClientSecret"]!;
        options.CallbackPath = "/api/auth/github-callback";
        options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        options.TokenEndpoint = "https://github.com/login/oauth/access_token";
        options.UserInformationEndpoint = "https://api.github.com/user";
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
        options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                context.RunClaimActions(json);

                // Upsert user in database
                var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var gitHubId = json.GetProperty("id").GetInt64().ToString();
                var username = json.GetProperty("login").GetString()!;
                var avatarUrl = json.GetProperty("avatar_url").GetString();

                var user = await db.Users.FirstOrDefaultAsync(u => u.GitHubId == gitHubId);
                if (user is null)
                {
                    user = new User { GitHubId = gitHubId, Username = username, AvatarUrl = avatarUrl };
                    db.Users.Add(user);
                }
                else
                {
                    user.Username = username;
                    user.AvatarUrl = avatarUrl;
                    user.UpdatedAt = DateTime.UtcNow;
                }
                await db.SaveChangesAsync();
            }
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
