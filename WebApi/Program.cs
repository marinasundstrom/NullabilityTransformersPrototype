using MartinCostello.OpenApi;

using NullabilityTransformersPrototype.Extensions;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerDocument(settings => settings.SchemaSettings.SchemaType = NJsonSchema.SchemaType.OpenApi3);

/*
builder.Services.AddOpenApi(options =>
            {
                // options.ApplyServersTransformer();
                // options.ApplyOperationId();
                // options.ApplyNullabilityTransformer();
                // options.ApplyDescriptionTransformer();
                // options.ApplySchemaNameTransforms(TypeExtensions.GetSchemaName);
                // options.ApplySchemasNotDistinctByNullability();
                // options.ApplyInheritanceTransformer();
            });
*/

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

////app.MapOpenApi();
//app.MapOpenApiYaml();

app.UseOpenApi(x => x.Path = "/openapi/{documentName}.yaml");

if (app.Environment.IsDevelopment())
{
    UseSwaggerUi(app);

    //MapScalar(app);
}

app.UseAuthorization();

app.MapControllers();

app.Run();

static void MapScalar(WebApplication app)
{
    app.MapScalarApiReference(x =>
    {
        x.Title = "Web API";
        x.OpenApiRoutePattern = "/openapi/{documentName}.yaml";
    });
    app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

static void UseSwaggerUi(WebApplication app)
{
    app.UseSwaggerUi(x =>
    {
        x.Path = "/openapi";
        x.DocumentPath = "/openapi/v1.yaml";
    });
}