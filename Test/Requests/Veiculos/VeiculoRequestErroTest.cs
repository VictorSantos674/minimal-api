using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests.Veiculos;

[TestClass]
public class VeiculoRequestErroTest
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
    public async Task Nao_Deve_Criar_Veiculo_Com_Dados_Invalidos()
    {
        var veiculoDTO = new VeiculoDTO
        {
            Nome = "",
            Marca = "",
            Ano = 1900
        };
        var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");
        var token = await ObterTokenAdmin();
        Setup.client.DefaultRequestHeaders.Clear();
        Setup.client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await Setup.client.PostAsync("/veiculos", content);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(result.Contains("não pode ser vazio") || result.Contains("muito antigo"));
    }

    [TestMethod]
    public async Task Nao_Deve_Acessar_Endpoint_Sem_Token()
    {
    Setup.client.DefaultRequestHeaders.Clear();
    var response = await Setup.client.GetAsync("/veiculos");
    Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Nao_Deve_Acessar_Endpoint_Com_Token_Invalido()
    {
        Setup.client.DefaultRequestHeaders.Clear();
        Setup.client.DefaultRequestHeaders.Add("Authorization", "Bearer token_invalido");
        var response = await Setup.client.GetAsync("/veiculos");
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<string> ObterTokenAdmin()
    {
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };
        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
        var response = await Setup.client.PostAsync("/administradores/login", content);
        var result = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(result))
            throw new Exception("Resposta vazia ao tentar obter token de admin.");
        using var doc = JsonDocument.Parse(result);
        return doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
    }
}
