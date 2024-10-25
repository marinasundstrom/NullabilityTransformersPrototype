    ```csharp
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "species")]
    [JsonDerivedType(typeof(AnimalDog), "Dog")]
    [JsonDerivedType(typeof(AnimalCat), "Cat")]
    ```