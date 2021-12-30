using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GroceryListApi.Authentication;

[UsedImplicitly]
public class AddAuthorizationHeaderOperationHeader : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var isAuthorized = actionMetadata.Any(metadataItem => metadataItem is AuthorizeAttribute);
        var allowAnonymous = actionMetadata.Any(metadataItem => metadataItem is AllowAnonymousAttribute);

        if (!isAuthorized || allowAnonymous)
        {
            return;
        }

        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Security = new List<OpenApiSecurityRequirement>();

        // Add JWT bearer type
        operation.Security.Add(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            }
        );
    }
}