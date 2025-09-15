using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using BCrypt.Net;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? BuscaPorId(int id)
    {
        return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
    }

    public Administrador Incluir(Administrador administrador)
    {
        // Hash da senha antes de salvar
        administrador.Senha = BCrypt.Net.BCrypt.HashPassword(administrador.Senha);
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.FirstOrDefault(a => a.Email == loginDTO.Email);
        if (adm != null && BCrypt.Net.BCrypt.Verify(loginDTO.Senha, adm.Senha))
            return adm;
        return null;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();
        int itensPorPagina = 10;
        if(pagina != null)
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        return query.ToList();
    }

    public void Atualizar(Administrador administrador)
    {
        var existente = _contexto.Administradores.FirstOrDefault(a => a.Id == administrador.Id);
        if (existente != null)
        {
            existente.Email = administrador.Email;
            if (!string.IsNullOrEmpty(administrador.Senha))
                existente.Senha = BCrypt.Net.BCrypt.HashPassword(administrador.Senha);
            existente.Perfil = administrador.Perfil;
            _contexto.Administradores.Update(existente);
            _contexto.SaveChanges();
        }
    }

    public void Apagar(int id)
    {
        var adm = _contexto.Administradores.FirstOrDefault(a => a.Id == id);
        if (adm != null)
        {
            _contexto.Administradores.Remove(adm);
            _contexto.SaveChanges();
        }
    }
}