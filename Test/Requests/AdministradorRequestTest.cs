using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class AdministradorRequestTest
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
    public async Task TestarGetSetPropriedades()
    {
        // Arrange
        var loginDTO = new LoginDTO{
            Email = "adm@teste.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8,  "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/administradores/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado?.Email ?? "");
        Assert.IsNotNull(admLogado?.Perfil ?? "");
        Assert.IsNotNull(admLogado?.Token ?? "");

        Console.WriteLine(admLogado?.Token);
    }

    [TestMethod]
    public async Task Deve_Atualizar_Administrador_Com_Sucesso()
    {
        // Autenticação: obter token de admin
        var token = await Setup.ObterTokenAdmin();
        Setup.client.DefaultRequestHeaders.Clear();
        Setup.client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        // Cria um novo admin para atualizar
        var novoAdm = new AdministradorDTO { Email = "atualizar@teste.com", Senha = "123456", Perfil = MinimalApi.Dominio.Enuns.Perfil.Editor };
        var content = new StringContent(JsonSerializer.Serialize(novoAdm), Encoding.UTF8, "application/json");
        var response = await Setup.client.PostAsync("/administradores", content);
        var result = await response.Content.ReadAsStringAsync();
        var criado = JsonSerializer.Deserialize<AdministradorModelView>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(criado);

        // Atualiza
        var updateDTO = new AdministradorDTO { Email = "atualizado@teste.com", Senha = "", Perfil = MinimalApi.Dominio.Enuns.Perfil.Adm };
        var updateContent = new StringContent(JsonSerializer.Serialize(updateDTO), Encoding.UTF8, "application/json");
        var putResponse = await Setup.client.PutAsync($"/administradores/{criado.Id}", updateContent);
        Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
        var putResult = await putResponse.Content.ReadAsStringAsync();
        var atualizado = JsonSerializer.Deserialize<AdministradorModelView>(putResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.AreEqual("atualizado@teste.com", atualizado?.Email);
        Assert.AreEqual("Adm", atualizado?.Perfil);
    }

    [TestMethod]
    public async Task Deve_Deletar_Administrador_Com_Sucesso()
    {
        // Autenticação: obter token de admin
        var token = await Setup.ObterTokenAdmin();
        Setup.client.DefaultRequestHeaders.Clear();
        Setup.client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        // Cria um novo admin para deletar
        var novoAdm = new AdministradorDTO { Email = "deletar@teste.com", Senha = "123456", Perfil = MinimalApi.Dominio.Enuns.Perfil.Editor };
        var content = new StringContent(JsonSerializer.Serialize(novoAdm), Encoding.UTF8, "application/json");
        var response = await Setup.client.PostAsync("/administradores", content);
        var result = await response.Content.ReadAsStringAsync();
        var criado = JsonSerializer.Deserialize<AdministradorModelView>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(criado);

        // Deleta
        var deleteResponse = await Setup.client.DeleteAsync($"/administradores/{criado.Id}");
        Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        // Confirma que não existe mais
        var getResponse = await Setup.client.GetAsync($"/Administradores/{criado.Id}");
        Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}