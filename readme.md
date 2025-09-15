# Minimal API - Gestão de Veículos

Projeto exemplo de uma API minimalista em .NET 7 para cadastro e autenticação de administradores e veículos, com autenticação JWT, Entity Framework Core, testes automatizados e arquitetura em camadas.

## Funcionalidades
- Cadastro, login e listagem de administradores
- Cadastro, listagem, atualização e remoção de veículos
- Autenticação JWT com roles (Adm, Editor)
- Validação de dados e tratamento de erros
- Testes automatizados (MSTest) com mocks e banco in-memory
- Documentação automática via Swagger

## Tecnologias Utilizadas
- .NET 7
- Entity Framework Core (MySQL e InMemory)
- Autenticação JWT
- MSTest
- Swagger (Swashbuckle)

## Como rodar o projeto

### Pré-requisitos
- .NET 7 SDK
- MySQL (opcional, apenas para rodar com banco real)

### Configuração
1. Clone o repositório:
   ```bash
   git clone <url-do-repo>
   cd minimal-api
   ```
2. Configure a string de conexão no arquivo `Api/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "MySql": "Server=localhost;Database=minimal_api;Uid=root;Pwd=root;"
   }
   ```
3. (Opcional) Execute as migrations para criar o banco:
   ```bash
   dotnet ef database update --project Api
   ```

### Executando a API
```bash
cd Api
# Para ambiente de desenvolvimento
DOTNET_ENVIRONMENT=Development dotnet run
```
Acesse a documentação Swagger em: [http://localhost:5004/swagger](http://localhost:5004/swagger)

### Executando os testes
```bash
cd Test
# Todos os testes (usando banco in-memory)
dotnet test
```

## Exemplos de uso

### Login de Administrador
```http
POST /administradores/login
{
  "email": "adm@teste.com",
  "senha": "123456"
}
```

### Cadastro de Veículo
```http
POST /veiculos
Authorization: Bearer <token>
{
  "nome": "Civic",
  "marca": "Honda",
  "ano": 2020
}
```

## Estrutura do Projeto
- `Api/` - Código da API principal
- `Test/` - Testes automatizados (unitários e integração)
- `Dominio/` - Entidades, DTOs, interfaces, serviços
- `Infraestrutura/` - DbContext e Migrations

## Usuários de exemplo
- **Administrador:**
  - Email: `adm@teste.com`
  - Senha: `123456`
  - Perfil: `Adm`
- **Editor:**
  - Email: `editor@teste.com`
  - Senha: `123456`
  - Perfil: `Editor`

## Observações
- Senhas não estão criptografadas (apenas para fins didáticos)
- O projeto está pronto para extensão e customização

---

Desenvolvido por VictorSantos674
