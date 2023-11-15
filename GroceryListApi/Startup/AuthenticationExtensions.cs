using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GroceryListApi.Startup;

public static class AuthenticationExtensions
{
    public static WebApplicationBuilder AddAuthenticationServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(GroceryClaimTypes.UserId)
                .Build();
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Issuer"],
                    ValidAudience = builder.Configuration["Audience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        if (context.Principal == null) return;
                        
                        var db = context.HttpContext.RequestServices.GetRequiredService<GroceryListDb>();
                        
                        var user = await db.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Username == context.Principal.GetClaimValue(ClaimTypes.NameIdentifier));

                        if (user == null)
                        {
                            context.Fail("No matching user was found");
                            return;
                        }
                        
                        var appIdentity = new ClaimsIdentity(new []
                        {
                            new Claim(GroceryClaimTypes.UserId, user.Id.ToString())
                        });

                        context.Principal.AddIdentity(appIdentity);
                    }
                };
            });

        return builder;
    }
}