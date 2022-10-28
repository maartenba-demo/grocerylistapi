using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GroceryListApi.Endpoints.Schemas;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniValidation;

namespace GroceryListApi.Endpoints;

public static class AuthenticationEndpoints
{
    public static WebApplication MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/token",
                async Task<Results<Ok<ApiToken>, Unauthorized>>(
                ApiUser apiUser,
                GroceryListDb db,
                IConfiguration configuration) =>
            {
                if (!MiniValidator.TryValidate(apiUser, out _))
                {
                    return Results.Extensions.UnauthorizedTypedResult();
                }

                var user = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == apiUser.Username);
                if (user == null)
                {
                    return Results.Extensions.UnauthorizedTypedResult();
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                    new Claim(JwtRegisteredClaimNames.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                };

                var token = new JwtSecurityToken
                (
                    issuer: configuration["Issuer"],
                    audience: configuration["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(60),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SigningKey"] ??
                                                                        throw new NullReferenceException(
                                                                            "The value for 'SigningKey' must be specified in appsettings.json"))),
                        SecurityAlgorithms.HmacSha256)
                );

                return TypedResults.Ok(
                    new ApiToken(new JwtSecurityTokenHandler().WriteToken(token)));
            })
            .AllowAnonymous()
            .WithTags("Authentication");

        return app;
    }
}