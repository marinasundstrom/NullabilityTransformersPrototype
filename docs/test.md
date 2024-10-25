```yaml
components:
  schemas:
    Animal:
      required:
        - species
      type: object
      anyOf:
        - $ref: "#/components/schemas/AnimalDog"
        - $ref: "#/components/schemas/AnimalCat"
      discriminator:
        propertyName: species
        mapping:
          Dog: "#/components/schemas/AnimalDog"
          Cat: "#/components/schemas/AnimalCat"
    AnimalCat:
      required:
        - name
      allOf:
        - $ref: "#/components/schemas/Animal"
    AnimalDog:
      required:
        - foo
      allOf:
        - $ref: "#/components/schemas/Animal"
```

Desired:

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