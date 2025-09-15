using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Dominio.Entidades;
using Test.Mocks;

namespace Test.Domain.Servicos;

[TestClass]
public class AdministradorServicoMockTest
{
    [TestMethod]
    public void Deve_Atualizar_Administrador()
    {
        var servico = new AdministradorServicoMock();
        var adm = servico.Incluir(new Administrador { Email = "novo@teste.com", Senha = "abc", Perfil = "Editor" });
        adm.Email = "atualizado@teste.com";
        adm.Perfil = "Adm";
        servico.Atualizar(adm);
        var atualizado = servico.BuscaPorId(adm.Id);
        Assert.AreEqual("atualizado@teste.com", atualizado?.Email);
        Assert.AreEqual("Adm", atualizado?.Perfil);
    }

    [TestMethod]
    public void Deve_Apagar_Administrador()
    {
        var servico = new AdministradorServicoMock();
        var adm = servico.Incluir(new Administrador { Email = "apagar@teste.com", Senha = "abc", Perfil = "Editor" });
        servico.Apagar(adm.Id);
        var apagado = servico.BuscaPorId(adm.Id);
        Assert.IsNull(apagado);
    }
}
