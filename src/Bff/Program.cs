using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Antiforgery;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration).WriteTo.Console());
builder.Services.AddProblemDetails(); builder.Services.AddHttpContextAccessor(); builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => { o.Cookie.Name = "__Host-saas-session"; o.Cookie.HttpOnly = true; o.Cookie.SameSite = SameSiteMode.Strict; o.Cookie.SecurePolicy = CookieSecurePolicy.Always; o.Events.OnRedirectToLogin = c => { c.Response.StatusCode = 401; return Task.CompletedTask; }; });
builder.Services.AddAuthorization(); builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? []).AllowCredentials().AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build(); app.UseExceptionHandler(); app.UseSerilogRequestLogging(); app.UseHttpsRedirection(); app.UseCors(); app.UseAuthentication(); app.UseAuthorization();
app.MapGet("/bff/antiforgery", (HttpContext context, IAntiforgery antiforgery) => { var tokens = antiforgery.GetAndStoreTokens(context); return Results.Ok(new { token = tokens.RequestToken }); }).RequireAuthorization();
app.MapPost("/bff/logout", async (HttpContext context, IAntiforgery antiforgery) => { await antiforgery.ValidateRequestAsync(context); await context.SignOutAsync(); return Results.NoContent(); }).RequireAuthorization();
app.MapReverseProxy(proxy => proxy.Use(async (context, next) => { if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method) && !HttpMethods.IsOptions(context.Request.Method)) await context.RequestServices.GetRequiredService<IAntiforgery>().ValidateRequestAsync(context); await next(); })).RequireAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "bff" })); app.Run();
public partial class Program;
