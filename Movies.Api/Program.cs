using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application;
using Movies.Application.Database;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ConfigurationManager config = builder.Configuration;

builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.TokenValidationParameters = new TokenValidationParameters {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true,
    };
});

builder.Services.AddAuthorization(x => {
    x.AddPolicy(AuthConstants.AdminUserPolicyName, p => p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));
    x.AddPolicy(AuthConstants.TrustedMemberPolicyName, p => p.RequireAssertion(c => 
        c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true"})
        || c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })

        ));
});

builder.Services.AddControllers();
//builder.Services.AddOpenApi();
builder.Services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options => { options
        .WithTitle("Zm.Movies")
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.RestSharp);

        options.Authentication = new ScalarAuthenticationOptions { PreferredSecuritySchemes = ["Bearer"] };
        options.Servers = Array.Empty<ScalarServer>();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

DbInitializer dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();


internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer {
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken) {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        string authSchemeName = "Bearer";

        if(authenticationSchemes.Any(authScheme => authScheme.Name == authSchemeName)) {

            OpenApiSecurityScheme bearerSecurityScheme = new () {
                Type = SecuritySchemeType.Http,
                Scheme = authSchemeName,
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token"
            };

            (document.Components ??= new OpenApiComponents()).SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme> { [authSchemeName] = bearerSecurityScheme };
        }
    }
}