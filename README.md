# Open API Tranformers

These transformers are prototypes for a potential compat shim for NSwag, and Swashbuckle. Dealing with essential things that would be a deal breaker for migrating to the new infrastructure.

## Transformer 

### Nullability transformer

Fixes nullability. Sets it wherever it applies.

### Inheritance transformer

Tries to fix inheritance to look like NSwag, and Swashbuckle. 

This is largely an issue limited by the Open API API, built by Microsoft. It doesn't align with the current libraries when handling polymorphic types.

The issue with schema naming, causing schema name of derived type being ``<BaseType><DerivedType>`` must be fixed. This is on the Open API API from Microsoft.

I suggest letting the user to choose how to do polymorphism via transformers. With this inheritance serialization, being one way.

### ApplyOperationId

Sets operation Id based on controller and method name. This is compatibility with NSwag and for the NSchema generator, allowing it to generate client classes per controller. 

On [minimal] endpoints you set the endpoint id manually.

The logic in this transfomer should be revised.

### ApplyNswagCompatTransformer

TBA - Incorporates many of the transformers.

Should also look into applying extension in aiding NJsonSchema.

### ApplyDescriptionTransformer

Extracts the description from the ``DescriptionAttribute`` of a type, and sets ``description: <value>`` on the corresponding schema.

### ApplySchemasNotDistinctByNullability

By default, the .NET Open API seems to create distinct schemas based on nullability, resulting in multiple schemas for the same .NET type.

This transformer just go through all schemas and sets ``nullable: false``.

## Issues

### Issue with NJsonSchema

NJsonSchema uses its own implementation of a converter and attributes for resolving inheritance. But that causes the discriminator to be emitted twice. The ``System.Text.Json`` de-serializer doesn't like that.

NJsonSchema should just use the ``System.Text.Json`` polymorphism attributes.

The attributes used should be:

```csharp
[JsonPolymorphic(TypeDiscriminatorPropertyName = "species")]
[JsonDerivedType(typeof(AnimalDog), "Dog")]
[JsonDerivedType(typeof(AnimalCat), "Cat")]
```