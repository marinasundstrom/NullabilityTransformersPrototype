using MartinCostello.OpenApi;

using NullabilityTransformersPrototype.Extensions;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddOpenApi(options =>
            {
                options.ApplyServersTransformer();
                options.ApplyOperationId();
                options.ApplyNullabilityTransformer();
                options.ApplyDescriptionTransformer();
                options.ApplySchemaNameTransforms(TypeExtensions.GetSchemaName);
                options.ApplySchemasNotDistinctByNullability();
                options.ApplyInheritanceTransformer();
            });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

//app.MapOpenApi();
app.MapOpenApiYaml();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(x =>
    {
        x.Title = "Web API";
        x.OpenApiRoutePattern = "/openapi/{documentName}.yaml";
    });
    app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.UseAuthorization();

app.MapControllers();

app.Run();