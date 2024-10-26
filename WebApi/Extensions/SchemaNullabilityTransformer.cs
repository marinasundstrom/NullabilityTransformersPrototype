using System.Reflection;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace NullabilityTransformersPrototype.Extensions;

public class SchemaNullabilityTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        // The schema itself should not be nullable.
        schema.Nullable = false;

        // Remove required
        schema.Required.Clear();

        var clrType = context.JsonTypeInfo.Type;

        foreach (var (name, propertySchema) in schema.Properties)
        {
            var propertyInfo = clrType.GetProperties().FirstOrDefault(p => p.Name == name);

            if (propertyInfo is null) continue;

            NullabilityInfoContext nullabilityInfoContext = new NullabilityInfoContext();
            var nullabilityContext = nullabilityInfoContext.Create(propertyInfo);

            propertySchema.Nullable = nullabilityContext.ReadState == NullabilityState.Nullable;

            Console.WriteLine("{0}: {1}", name, propertySchema.Nullable);

            // Is propertySchema.Nullable overridden later later?
        }

        return Task.CompletedTask;
    }
}
