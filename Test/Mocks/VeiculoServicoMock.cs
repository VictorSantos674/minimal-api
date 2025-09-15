using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;

namespace Test.Mocks;

public class VeiculoServicoMock : IVeiculoServico
{
    private static List<Veiculo> veiculos = new List<Veiculo>(){
        new Veiculo{
            Id = 1,
            Nome = "Fiesta 2.0",
            Marca = "Ford",
            Ano = 2013
        },
        new Veiculo{
            Id = 2,
            Nome = "X6",
            Marca = "BMW",
            Ano = 2022
        }
    };

    public void Apagar(Veiculo veiculo)
    {
        veiculos.RemoveAll(v => v.Id == veiculo.Id);
    }

    public void Atualizar(Veiculo veiculo)
    {
        var idx = veiculos.FindIndex(v => v.Id == veiculo.Id);
        if (idx >= 0)
            veiculos[idx] = veiculo;
    }

    public Veiculo? BuscaPorId(int id)
    {
        return veiculos.FirstOrDefault(v => v.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        veiculo.Id = veiculos.Count > 0 ? veiculos.Max(v => v.Id) + 1 : 1;
        veiculos.Add(veiculo);
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        return veiculos;
    }
}
