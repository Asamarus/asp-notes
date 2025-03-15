using AspNotes.Common;
using AspNotes.Common.Converters;
using AspNotes.Helpers;
using AspNotes.Interfaces;
using AspNotes.Middlewares;
using AspNotes.Models;
using AspNotes.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AllNotesSection>(builder.Configuration.GetSection("AllNotesSection"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAppCache, AppCache>();
builder.Services.AddControllers(options =>
{
    // Add global authorization filter
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new TrimmingStringJsonConverter());
});

builder.Services.AddSingleton<EntityMaterializedInterceptor>();

var connectionString = builder.Configuration.GetConnectionString("NotesDb")
    ?? throw new InvalidOperationException("Connection string 'NotesDb' not found.");

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// Add JWT authentication
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSettingsSection.Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings is not configured properly.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.SaveToken = true;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtSettings.ValidIssuer,
         ValidAudience = jwtSettings.ValidAudience,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
     };
 });

builder.Services.AddScoped<IUrlMetadataHelper, UrlMetadataHelper>();
builder.Services.AddScoped<ISectionsService, SectionsService>();
builder.Services.AddScoped<IBooksService, BooksService>();
builder.Services.AddScoped<ITagsService, TagsService>();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "AspNotes API");
    });
}

app.UseMiddleware<CustomExceptionHandlerMiddleware>();
app.UseMiddleware<JwtCookieToHeaderMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapFallbackToFile("index.html");

await app.RunAsync();
