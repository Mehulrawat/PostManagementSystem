
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace PostManagement.Api.Security;
public class BasicAuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasBasicAuthorize =
            context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>()
                .Any(a => a.AuthenticationSchemes?.Contains("Basic") ?? false)
            ||
            (context.MethodInfo.DeclaringType != null &&
             context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>()
                .Any(a => a.AuthenticationSchemes?.Contains("Basic") ?? false));

        if (hasBasicAuthorize)
        {
            operation.Security ??= new System.Collections.Generic.List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" }
                    },
                    System.Array.Empty<string>()
                }
            });
        }
    }
}
