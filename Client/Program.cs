// See https://aka.ms/new-console-template for more information
using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;

using NullabilityTransformersPrototype;

Console.WriteLine("Hello, World!");

ServiceCollection services = new ServiceCollection();

services
       .AddHttpClient(nameof(TestClient), (httpClient) =>
       {
           httpClient.BaseAddress = new Uri("http://localhost:5295/");
       })
       .AddTypedClient<ITestClient>((http, sp) => new TestClient(http));

var provider = services.BuildServiceProvider();

var testClient = provider.GetRequiredService<ITestClient>();

try
{
    var foo = await testClient.GetAnimalAsync();

    Console.WriteLine(JsonSerializer.Serialize(foo));

    await testClient.PostAnimalAsync(foo);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}