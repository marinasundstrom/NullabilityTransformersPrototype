using System.ComponentModel;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

namespace NullabilityTransformersPrototype.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("Test")]
    public TestDto Test()
    {
        return default!;
    }

    [HttpGet("Test2")]
    public TestDto? Test2()
    {
        return default!;
    }

    [HttpGet("Test3")]
    public KeyValuePair<int, TestDto> Test3()
    {
        return default!;
    }

    [HttpGet("Test32")]
    public KeyValuePair<int, TestDto?> Test32()
    {
        return default!;
    }

    [HttpPost("T1")]
    public void T1(TestDto testDto)
    {
    }

    [HttpPost("T2")]
    public void T2(TestDto? testDto)
    {
    }

    [HttpPost("T3")]
    public void T3(TestDto testDto, int x)
    {
    }

    [HttpPost("T4")]
    public void T4(TestDto testDto, int x = 2)
    {
    }

    [HttpPost("T5")]
    public void T5(TestDto testDto, int? x = null)
    {
    }

    [HttpGet("GetAnimal")]
    public Animal GetAnimal()
    {
        return new Dog(true);
    }

    [HttpPost("PostAnimal")]
    public void PostAnimal(Animal animal)
    {
        Console.WriteLine(animal);
    }
}

[Description("This is a test type")]
public record TestDto(string x, int? z, string? y);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "species")]
[JsonDerivedType(typeof(Dog), "Dog")]
[JsonDerivedType(typeof(Cat), "Cat")]
public abstract record Animal();

public record Dog(bool Foo) : Animal;

public record Cat(string Name) : Animal;