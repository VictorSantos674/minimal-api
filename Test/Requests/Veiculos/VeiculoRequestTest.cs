using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests.Veiculos;

[TestClass]
public class VeiculoRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task Deve_Criar_Veiculo_Com_Sucesso()
    {
        var veiculoDTO = new VeiculoDTO
        {
            Nome = "Civic",
            Marca = "Honda",
            Ano = 2020
        };
        var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");

        // Autenticação: obter token de admin
        var token = await ObterTokenAdmin();
        Setup.client.DefaultRequestHeaders.Clear();
        Setup.client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await Setup.client.PostAsync("/veiculos", content);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(veiculo);
        Assert.AreEqual("Civic", veiculo?.Nome);
    }

    [TestMethod]
    public async Task Deve_Listar_Veiculos()
    {
        var token = await ObterTokenAdmin();
        Setup.client.DefaultRequestHeaders.Clear();
        Setup.client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await Setup.client.GetAsync("/veiculos");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        var veiculos = JsonSerializer.Deserialize<List<Veiculo>>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(veiculos);
    }

    [TestMethod]
    public async Task Deve_Obter_Veiculo_Por_Id()
    {
        var token = await ObterTokenAdmin();
        Setup.client.DefaultRequestHeaders.Clear();
        Setup.client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await Setup.client.GetAsync("/veiculos/1");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(veiculo);
        Assert.AreEqual(1, veiculo?.Id);
    }

    private static async Task<string> ObterTokenAdmin()
    {
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com", // usuário presente no mock
            Senha = "123456"
        };
        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
        var response = await Setup.client.PostAsync("/administradores/login", content);
        var result = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(result))
            throw new Exception("Resposta vazia ao tentar obter token de admin. Verifique se o mock está registrado corretamente e o endpoint está acessível.");
        using var doc = JsonDocument.Parse(result);
        return doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
    }
}
