# Sistema de Gerenciamento de Endereços

Projeto desenvolvido em C# como teste prático para vaga de Desenvolvedor C#, com foco em autenticação de usuários e gerenciamento de endereços integrado à API ViaCEP.

---

## Sobre o Projeto

A aplicação permite que usuários realizem autenticação e gerenciem seus próprios endereços através de um CRUD completo. O sistema também possui integração com a API do ViaCEP para preenchimento automático de endereços a partir do CEP informado, além da funcionalidade de exportação dos endereços cadastrados em arquivo CSV.

O projeto foi desenvolvido utilizando ASP.NET Core MVC, Entity Framework Core e SQL Server, seguindo boas práticas de organização, separação de responsabilidades e segurança.

---

# Módulo Administrativo

O sistema possui um módulo administrativo exclusivo para usuários com perfil `ADMIN`, permitindo o gerenciamento centralizado dos usuários cadastrados na aplicação.

---

## Funcionalidades do Administrador

- Listagem de usuários cadastrados
- Busca de usuários por nome ou e-mail
- Visualização de detalhes do usuário
- Alteração dinâmica de permissões administrativas
- Bloqueio e desbloqueio de usuários
- Exclusão de contas de usuário
- Controle de acesso baseado em Roles (`ADMIN`)
- Interface administrativa responsiva para gerenciamento dos usuários

---

## Gerenciamento de Permissões

O administrador pode promover ou remover privilégios administrativos de qualquer usuário diretamente pela interface do sistema.

A autenticação e autorização são realizadas utilizando:

- ASP.NET Core Identity
- Roles (`ADMIN`)
- Attribute Authorization (`[Authorize(Roles = "ADMIN")]`)

---

## Controle de Bloqueio de Usuários

O sistema permite que administradores bloqueiem usuários temporariamente através do mecanismo de `Lockout` do ASP.NET Identity.

Quando bloqueado:
- O usuário não consegue autenticar no sistema
- O acesso é impedido até o desbloqueio manual pelo administrador

---

## Segurança Administrativa

As rotas administrativas são protegidas utilizando autorização baseada em perfil:

```csharp
[Authorize(Roles = "ADMIN")]
```

---

# Funcionalidades

## Usuário
- Cadastro de usuário
- Login e logout
- Validação de credenciais
- Gerenciamento de perfil
- Upload de foto de perfil
- Cadastro e exclusão de endereços

---

## CRUD de Endereços
- Cadastro manual de endereços
- Busca automática de endereço via CEP utilizando a API ViaCEP
- Listagem de endereços cadastrados
- Edição de endereços
- Exclusão de endereços
- Associação de endereços ao usuário autenticado
- Exportação dos endereços para arquivo CSV

---

## Campos do Endereço

| Campo | Obrigatório |
|---|---|
| CEP | Sim |
| Logradouro | Sim |
| Complemento | Não |
| Bairro | Sim |
| Cidade | Sim |
| UF | Sim |
| Número | Sim |

---

# Integração Externa

## ViaCEP

A aplicação utiliza a API pública do ViaCEP para consulta automática de endereços a partir do CEP informado.

### Exemplo de requisição

```http
https://viacep.com.br/ws/01001000/json/
```

---

# Tech Stack

| Camada | Tecnologia |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| ORM | Entity Framework Core 10 |
| Banco de dados | SQL Server 2022 |
| Autenticação | ASP.NET Core Identity |
| Armazenamento de imagens | MinIO (S3-compatible) |
| Frontend | Razor Views + jQuery |
| Containerização | Docker / Docker Compose |

---

# Dependências (.csproj)

```xml
Microsoft.AspNetCore.Identity.EntityFrameworkCore  10.0.5
Microsoft.AspNetCore.Identity.UI                   10.0.5
Microsoft.EntityFrameworkCore                      10.0.5
Microsoft.EntityFrameworkCore.Design               10.0.5
Microsoft.EntityFrameworkCore.SqlServer            10.0.5
Microsoft.EntityFrameworkCore.Tools                10.0.5
Minio                                              7.0.0
```

---

# Pré-requisitos

Antes de executar o projeto, é necessário possuir instalado:

- .NET 10 SDK
- Docker Desktop
- Docker Compose
- Git

---

# Instalação das Dependências

Após clonar o repositório, entre na pasta do projeto:

```bash
cd <nome-do-projeto>
```

---

## Restaurar Dependências do .NET

Execute o comando abaixo para instalar todas as dependências do projeto definidas no arquivo `.csproj`:

```bash
dotnet restore
```

---

## Verificar se as Dependências Foram Instaladas

Você pode verificar os pacotes instalados com:

```bash
dotnet list package
```

---

## Instalação Manual de Pacotes (Opcional)

Caso seja necessário instalar algum pacote manualmente:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

Exemplo para instalar o pacote do MinIO:

```bash
dotnet add package Minio --version 7.0.0
```

---

# Como Executar o Projeto

## 1. Clonar o Repositório

Abra o terminal e execute:

```bash
git clone <url-do-repositorio>
```

Entre na pasta do projeto:

```bash
cd <nome-do-projeto>
```

---

## 2. Subir os Containers Docker

Execute:

```bash
docker compose up -d
```

Esse comando irá iniciar:

| Serviço | Porta |
|---|---|
| SQL Server 2022 | 1433 |
| MinIO API | 9000 |
| MinIO Console | 9001 |

---

## 3. Verificar se os Containers Estão Rodando

Execute:

```bash
docker ps
```

Os containers do SQL Server e MinIO devem aparecer como `Up`.

---

## 4. Executar a Aplicação

Execute:

```bash
dotnet run
```

---

## 5. Acessar o Sistema

Após iniciar a aplicação, ela estará disponível em:

| Ambiente | URL |
|---|---|
| HTTP | http://localhost:5067 |

---

## 6. Aplicação Automática das Migrações

Ao iniciar o sistema:
- As migrations do Entity Framework serão aplicadas automaticamente
- O banco de dados será criado automaticamente caso não exista
- O usuário administrador padrão será criado automaticamente

---

# Credenciais Iniciais

| Papel | Usuário | Senha |
|---|---|---|
| Administrador | admin@admin.com | Admin@123 |

---

# Configuração do Projeto

## appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=enderecosdb;User Id=sa;Password=Admin123!;TrustServerCertificate=True;"
  },

  "Minio": {
    "Endpoint": "http://localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "Bucket": "aec-enderecos"
  }
}
```

---

# Infraestrutura Docker

## Serviços Utilizados

| Serviço | Imagem | Porta(s) | Credenciais |
|---|---|---|---|
| SQL Server | `mcr.microsoft.com/mssql/server:2022-latest` | `1433` | `sa / Admin123!` |
| MinIO | `minio/minio` | `9000 / 9001` | `minioadmin / minioadmin` |

---

## Persistência de Dados

Os dados dos containers são persistidos através de volumes Docker locais, garantindo que as informações não sejam perdidas ao reiniciar os containers.

---

# Estrutura do Projeto

```text
AEC-TESTE-TECNICO/
├── Controllers/          # Controllers MVC
├── Models/               # Entidades do domínio
├── Views/                # Razor Views
├── ViewModels/           # Objetos utilizados entre Controller e View
├── Services/             # Regras de negócio e integrações
├── Data/                 # DbContext e Seed de dados
├── Migrations/           # Migrações do Entity Framework
├── wwwroot/              # Arquivos estáticos
├── docker-compose.yml
├── appsettings.json
└── aec-teste-tecnico.csproj
```

---

# Banco de Dados

## Tabela Usuários

| Campo | Descrição |
|---|---|
| Id | Identificador único do usuário |
| Name | Nome completo do usuário |
| UserName | Nome de usuário utilizado para login |
| Email | E-mail do usuário |
| PasswordHash | Senha criptografada pelo ASP.NET Identity |
| PhoneNumber | Telefone do usuário |
| Perfil | Perfil/permissão do usuário |
| ImageUrl | URL da foto de perfil armazenada no MinIO |

---

## Tabela Endereços

| Campo | Descrição |
|---|---|
| Id | Identificador único do endereço |
| ZipCode | CEP do endereço |
| Street | Nome da rua/logradouro |
| Number | Número do endereço |
| Complement | Complemento do endereço |
| Neighborhood | Bairro |
| City | Cidade |
| Uf | Unidade Federativa (UF) |
| UserId | Identificador do usuário proprietário do endereço |

---

# Funcionalidades Técnicas

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- Integração REST com ViaCEP
- Upload de imagens com MinIO
- Docker Compose
- Exportação CSV
- Seed automático de dados
- Aplicação automática de migrations
- Validação de formulários
- Separação por camadas
- Boas práticas de segurança

---

# Critérios Atendidos do Teste

## Tela de Login
- Autenticação de usuário
- Validação de credenciais
- Redirecionamento após login

## CRUD de Endereços
- Inserção
- Edição
- Exclusão
- Listagem
- Integração ViaCEP
- Exportação CSV

## Banco de Dados
- Tabela de usuários
- Tabela de endereços
- Relacionamento entre usuário e endereço

## Tecnologias Utilizadas
- ASP.NET MVC
- Entity Framework
- SQL Server
- HTML, CSS e JavaScript

---

# Observações

- O projeto utiliza containers Docker para simplificar a execução do ambiente
- O MinIO foi utilizado para armazenamento de imagens de perfil
- O Entity Framework Core é responsável pelas migrations e acesso ao banco de dados
- O sistema foi estruturado utilizando separação por responsabilidades para facilitar manutenção e escalabilidade

---

# Autor

## Bruno Sales Noleto de Oliveira

Acadêmico de Sistemas de Informação — UNITINS
