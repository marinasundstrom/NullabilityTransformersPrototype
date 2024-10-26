using System.ComponentModel;
using System.Reflection;

using MartinCostello.OpenApi.Transformers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace NullabilityTransformersPrototype.Extensions;

public static class Transformers
{
    public static OpenApiOptions ApplyServersTransformer(this OpenApiOptions options)
    {
        options.AddDocumentTransformer<AddServersTransformer>();
        return options;
    }

    public static OpenApiOptions ApplyNullabilityTransformer(this OpenApiOptions options)
    {
        options.AddSchemaTransformer<SchemaNullabilityTransformer>();
        options.AddOperationTransformer<OperationNullabilityTransformer>();
        return options;
    }

    public static OpenApiOptions ApplyInheritanceTransformer(this OpenApiOptions options)
    {
        options.AddSchemaTransformer((schema, context, ct) =>
        {
            const string SchemaId = "x-schema-id";

            if (schema.Discriminator is not null)
            {
                var baseSchema = schema;

                if (context.JsonTypeInfo.Type.IsAbstract)
                {
                    baseSchema.Extensions.Add("x-abstract", new OpenApiBoolean(true));
                }

                baseSchema.AdditionalPropertiesAllowed = false;

                var discriminatorPropertyName = baseSchema.Discriminator.PropertyName;

                var baseSchemaName = baseSchema.Annotations[SchemaId].ToString()!;

                var subSchemas = baseSchema.AnyOf.ToArray();

                foreach (var subSchema in subSchemas)
                {
                    // INFO: This doesn't work since the names are modified after the transformers have run.

                    /*
                    var subSchemaName = subSchema.Annotations[SchemaId].ToString()!;
                    var newName = subSchemaName.Replace(baseSchemaName, string.Empty);
                    subSchema.Annotations[SchemaId] = newName;
                    */

                    //Console.WriteLine($"Change name form {subSchemaName} to {newName}");

                    var allOfSchema = new OpenApiSchema(subSchema);

                    allOfSchema.Type = "object";
                    allOfSchema.Properties.Remove(discriminatorPropertyName);
                    allOfSchema.AdditionalPropertiesAllowed = false;

                    var refSchema = new OpenApiSchema();

                    refSchema.Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = baseSchema.Annotations[SchemaId].ToString()
                    };

                    subSchema.AllOf.Add(refSchema);
                    subSchema.AllOf.Add(allOfSchema);

                    subSchema.Required.Clear();
                    subSchema.Properties.Clear();
                }

                // INFO: If you do this, which you logically would have done. The sub-schemas disappear.

                //schema.AnyOf.Clear();

                /*
                // Re-mapping schemas in binding

                foreach (var (mappingKey, schemaRef) in baseSchema.Discriminator.Mapping.ToList())
                {
                    var newName = schemaRef.Replace(baseSchemaName, string.Empty);

                    baseSchema.Discriminator.Mapping.Remove(mappingKey);
                    baseSchema.Discriminator.Mapping.Add(mappingKey, newName);
                }
                */
            }

            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions ApplyOperationId(this OpenApiOptions options)
    {
        options.AddOperationTransformer((operation, context, ct) =>
        {
            var methodInfo = (context.Description.ActionDescriptor as ControllerActionDescriptor)!.MethodInfo;

            operation.OperationId = $"{methodInfo.DeclaringType!.Name.Replace("Controller", string.Empty)}_{methodInfo.Name}";

            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions ApplyNswagCompatTransformer(this OpenApiOptions options)
    {
        return options;
    }

    public static OpenApiOptions ApplySchemaNameTransforms(this OpenApiOptions options, Func<Type, string> transformer)
    {
        options.AddSchemaTransformer((schema, context, ct) =>
        {
            const string SchemaId = "x-schema-id";

            if (schema.Annotations?.TryGetValue(SchemaId, out var referenceIdObject) == true
                 && referenceIdObject is string newSchemaId)
            {
                var clrType = context.JsonTypeInfo.Type;
                newSchemaId = transformer(clrType);
                schema.Annotations[SchemaId] = newSchemaId;
            }

            return Task.CompletedTask;
        });

        return options;
    }

    public static OpenApiOptions ApplyDescriptionTransformer(this OpenApiOptions options)
    {
        options.AddSchemaTransformer((schema, context, ct) =>
        {
            var clrType = context.JsonTypeInfo.Type;

            schema.Description = clrType.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description;

            return Task.CompletedTask;
        });

        return options;
    }
}