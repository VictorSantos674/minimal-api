using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MinimalApi.Dominio.Interfaces;
using Test.Mocks;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Helpers;

public class Setup
{
    public const string PORT = "5001";
    public static TestContext testContext = default!;
    public static WebApplicationFactory<Startup> http = default!;
    public static HttpClient client = default!;

    public static void ClassInit(TestContext testContext)
    {
        Setup.testContext = testContext;
        Setup.http = new WebApplicationFactory<Startup>();

        Setup.http = Setup.http.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Development");
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdministradorServico, AdministradorServicoMock>();
                services.AddScoped<IVeiculoServico, VeiculoServicoMock>();
            });
        });

        Setup.client = Setup.http.CreateClient();
    }

    public static void ClassCleanup()
    {
        Setup.http.Dispose();
    }

    public static async Task<string> ObterTokenAdmin()
    {
        var loginDTO = new MinimalApi.DTOs.LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(loginDTO), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/administradores/login", content);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        var admLogado = System.Text.Json.JsonSerializer.Deserialize<MinimalApi.Dominio.ModelViews.AdministradorLogado>(result, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return admLogado?.Token ?? string.Empty;
    }
}