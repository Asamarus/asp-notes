using AspNotes.Core.Book;
using AspNotes.Core.Common;
using AspNotes.Core.Note;
using AspNotes.Core.Section;
using AspNotes.Core.Tag;
using AspNotes.Core.User;
using AspNotes.Web.Middlewares;
using AspNotes.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SqlKata.Compilers;
using SqlKata.Execution;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Text.Json;

namespace AspNotes.Web;

public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddSingleton<AppCache>();

        var connectionString = configuration.GetConnectionString("NotesDb")
            ?? throw new InvalidOperationException("Connection string 'NotesDb' not found.");

        services.AddDbContext<NotesDbContext>(options =>
                   options.UseSqlite(connectionString, options => options.MigrationsAssembly("AspNotes.Web")));

        services.AddScoped(sp =>
        {
            var dbContext = sp.GetRequiredService<NotesDbContext>();
            var connection = dbContext.GetDbConnection();
            var compiler = new SqliteCompiler();

            var queryFactory = new QueryFactory(connection, compiler);

#if DEBUG
            queryFactory.Logger = compiled =>
            {
                System.Diagnostics.Debug.WriteLine(compiled.ToString());
            };
#endif

            return queryFactory;
        });

        services.AddControllersWithViews(config =>
        {
            var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();

            config.Filters.Add(new AuthorizeFilter(policy));
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

#if DEBUG
        //Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNotes API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            c.OperationFilter<AuthOperationFilter>();

            var filePath = Path.Combine(AppContext.BaseDirectory, "AspNotes.Web.xml");
            c.IncludeXmlComments(filePath);
        });
#endif

        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<ISectionsService, SectionsService>();
        services.AddScoped<INotesService, NotesService>();
        services.AddScoped<IBookService, BooksService>();
        services.AddScoped<ITagsService, TagsService>();

        var jwtSettingsSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettingsSection);

        var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                if (jwtSettings != null)
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.ValidIssuer,
                        ValidAudience = jwtSettings.ValidAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                    };
                }
                else
                {
                    throw new Exception("JwtSettings is null. Please check your appsettings.json");
                }
            });
    }

    public void Configure(IApplicationBuilder app, NotesDbContext dbContext)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseMiddleware<JwtCookieToHeaderMiddleware>();

        if (environment.IsDevelopment())
        {
            dbContext.Database.Migrate();

            app.UseSwagger();// /swagger/v1/swagger.json
            app.UseSwaggerUI();// /swagger
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{*url}",
                new { controller = "Home", action = "Index" },
                constraints: new { ignoreApiAndFiles = new IgnoreApiAndFileExtensionRoutesConstraint() }
            );
        });
    }

    private class IgnoreApiAndFileExtensionRoutesConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var path = httpContext?.Request.Path.Value;

            // Ignore routes that start with /api and have a file extension
            if (path != null && (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) || Path.HasExtension(path)))
            {
                return false;
            }

            return true;
        }
    }

    private class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var allowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>()
                .Any();

            if (allowAnonymous)
                return;

            operation.Security =
            [
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                }
            ];
        }
    }
}
