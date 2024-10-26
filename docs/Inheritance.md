This is to highlight the difference between the new OpenAPI generator and NSwag.

This is what I get from inheritance as is:

(YAML is generated with an extension by Martin Costello)

```yaml
components:
  schemas:
    Animal:
      required:
        - species
      type: object
      anyOf:
        - $ref: '#/components/schemas/AnimalDog'
        - $ref: '#/components/schemas/AnimalCat'
      discriminator:
        propertyName: species
        mapping:
          Dog: '#/components/schemas/AnimalDog'
          Cat: '#/components/schemas/AnimalCat'
    AnimalCat:
      required:
        - name
      properties:
        species:
          enum:
            - Cat
          type: string
        name:
          type: string
    AnimalDog:
      required:
        - foo
      properties:
        species:
          enum:
            - Dog
          type: string
        foo:
          type: boolean
```

This is what NSwag would have [roughly] produced:

(It is a reconstruction)

```yaml
components:
  schemas:
    Animal:
      required:
        - species
      type: object
      discriminator:
        propertyName: species
        mapping:
          Dog: "#/components/schemas/Dog"
          Cat: "#/components/schemas/Cat"
      x-abstract: true
      additionalProperties: false
    Cat:
      allOf:
        - $ref: "#/components/schemas/Animal"
        - type: object
          additionalProperties: false
          properties:
            name:
              type: string
    Dog:
      allOf:
        - $ref: "#/components/schemas/Animal"
        - type: object
          additionalProperties: false
          properties:
            foo:
              type: boolean
```

The most notable thing here is the use of ``allOf`` to indicate that a subtype inherits a schema.

This might be an opinionated approach. But this should be taken into account. Without it people won't move over to use this API.

There are also extension like ``x-abstract`` that add some extra information for generators like NSwag.