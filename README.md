# Sales Project — Sales API

API REST desenvolvida em .NET para gerenciamento de usuários, produtos, filiais e vendas, com aplicação de regras de negócio para cálculo de descontos, controle de status e operações CRUD.

O projeto foi desenvolvido utilizando princípios de DDD, separando regras de negócio, casos de uso, persistência e camada de apresentação.

---

## Sobre o Projeto

A aplicação representa um sistema de vendas onde é possível gerenciar:

- Usuários
- Produtos
- Filiais
- Vendas
- Itens de venda

A principal regra de negócio está no módulo de vendas, onde o sistema calcula automaticamente descontos com base na quantidade de itens idênticos vendidos.

---

## Regras de Negócio

### Regras de desconto

O desconto é aplicado por item da venda, considerando a quantidade de produtos idênticos:

| Quantidade de itens idênticos | Desconto |
|---|---:|
| Menos de 4 itens | 0% |
| De 4 a 9 itens | 10% |
| De 10 a 20 itens | 20% |
| Acima de 20 itens | Não permitido |

### Regras adicionais

- Não é permitido vender mais de 20 unidades do mesmo produto.
- Produtos repetidos na request são agrupados pelo `ProductId`.
- O valor total da venda é recalculado automaticamente.
- O preço utilizado na venda é o preço atual do produto no momento da operação.
- Itens removidos em uma atualização de venda são marcados como cancelados/inativos.
- Operações de delete são tratadas como soft delete/desativação lógica quando aplicável.

---

## Tecnologias Utilizadas

- .NET 8
- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- AutoMapper
- MediatR
- FluentValidation
- Rebus
- RabbitMQ
- Serilog
- Docker
- Docker Compose
- Swagger/OpenAPI

---

## Arquitetura

O projeto foi desenvolvido seguindo princípios de DDD, buscando separar as responsabilidades da aplicação em camadas bem definidas.

A ideia principal é manter o domínio da aplicação mais protegido e independente, concentrando nele as entidades, regras de negócio e contratos principais. A camada de aplicação coordena os casos de uso por meio de comandos e handlers, enquanto a camada de API fica responsável por receber requisições HTTP, validar entradas e retornar respostas.

A persistência fica isolada na camada de ORM, utilizando Entity Framework Core, e as dependências são registradas por meio do projeto de IoC.

Estrutura geral:

```text
src/
├── SalesProject.WebApi
├── SalesProject.Application
├── SalesProject.Domain
├── SalesProject.ORM
├── SalesProject.IoC
└── SalesProject.Common
```

---

## Padrões Utilizados

- DDD
- Repository Pattern
- CQRS simplificado com MediatR
- DTO Pattern
- FluentValidation
- AutoMapper
- Soft Delete
- Event-driven processing com Rebus/RabbitMQ
- Dependency Injection
- Global Exception Handling
- Logging estruturado com Serilog

---

## Módulos da API

### Users

Responsável pelo gerenciamento de usuários.

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/users` | Lista usuários com paginação e filtros |
| GET | `/api/users/{id}` | Busca usuário por ID |
| POST | `/api/users` | Cria um novo usuário |
| PUT | `/api/users/{id}` | Atualiza um usuário existente |
| DELETE | `/api/users/{id}` | Remove/desativa um usuário |

### Products

Responsável pelo gerenciamento de produtos.

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/products` | Lista produtos com paginação e filtros |
| GET | `/api/products/{id}` | Busca produto por ID |
| POST | `/api/products` | Cria um novo produto |
| PUT | `/api/products/{id}` | Atualiza um produto existente |
| DELETE | `/api/products/{id}` | Remove/desativa um produto |

### Branches

Responsável pelo gerenciamento de filiais.

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/branches` | Lista filiais com paginação e filtros |
| GET | `/api/branches/{id}` | Busca filial por ID |
| POST | `/api/branches` | Cria uma nova filial |
| PUT | `/api/branches/{id}` | Atualiza uma filial existente |
| DELETE | `/api/branches/{id}` | Remove/desativa uma filial |

### Sales

Responsável pelo gerenciamento de vendas.

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/sales` | Lista vendas com paginação e filtros |
| GET | `/api/sales/{id}` | Busca venda por ID |
| POST | `/api/sales` | Cria uma nova venda |
| PUT | `/api/sales/{id}` | Atualiza uma venda existente |
| PUT | `/api/sales/{id}/reactivate` | Reativa uma venda |
| DELETE | `/api/sales/{id}` | Cancela/remove uma venda |

---

## Exemplos de Uso

### Criar produto

```http
POST /api/products
Content-Type: application/json
```

```json
{
  "name": "Notebook",
  "currentPrice": 3500.00,
  "status": 1
}
```

### Criar filial

```http
POST /api/branches
Content-Type: application/json
```

```json
{
  "name": "Recife Branch",
  "address": "Av. Boa Viagem, 1000 - Recife/PE",
  "status": 1
}
```

### Criar venda

```http
POST /api/sales
Content-Type: application/json
```

```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "branchId": "22222222-2222-2222-2222-222222222222",
  "items": [
    {
      "productId": "33333333-3333-3333-3333-333333333333",
      "quantity": 5
    }
  ]
}
```

Nesse exemplo, como a venda possui 5 unidades do mesmo produto, o sistema aplica 10% de desconto para esse item.

### Atualizar venda

```http
PUT /api/sales/{id}
Content-Type: application/json
```

```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "branchId": "22222222-2222-2222-2222-222222222222",
  "items": [
    {
      "productId": "33333333-3333-3333-3333-333333333333",
      "quantity": 11
    }
  ]
}
```

Nesse exemplo, como a venda possui 11 unidades do mesmo produto, o sistema aplica 20% de desconto para esse item.

Itens que existiam anteriormente na venda e não forem enviados na nova request são marcados como cancelados/inativos.

---

## Paginação e Filtros

Os endpoints de listagem seguem o padrão de paginação por query string.

Exemplo:

```http
GET /api/products?pageNumber=1&pageSize=10
```

Exemplo com filtros:

```http
GET /api/products?pageNumber=1&pageSize=10&name=notebook&status=1
```

Exemplo para vendas:

```http
GET /api/sales?pageNumber=1&pageSize=10&status=1&startDate=2026-01-01&endDate=2026-12-31
```

Resposta paginada:

```json
{
  "success": true,
  "data": [
    {
      "id": "e6dd0d5c-b1d2-40a7-a6b0-f9d82a0dd001",
      "name": "Notebook",
      "currentPrice": 3500.00,
      "status": 1
    }
  ],
  "currentPage": 1,
  "totalPages": 1,
  "totalCount": 1
}
```

---

## Mensageria com Rebus e RabbitMQ

O projeto utiliza Rebus com RabbitMQ para publicar eventos de aplicação.

Atualmente, o principal evento utilizado é:

```text
SaleCreatedEvent
```

Quando uma venda é criada com sucesso, a aplicação publica um evento informando que a venda foi registrada.

Fluxo simplificado:

```text
CreateSaleHandler
    ↓
Cria a venda no banco
    ↓
Publica SaleCreatedEvent
    ↓
Rebus envia a mensagem para o RabbitMQ
    ↓
SaleCreatedEventHandler consome o evento
    ↓
A aplicação registra um log da venda criada
```

A abstração de mensageria fica na camada de aplicação, enquanto a implementação concreta com Rebus fica no projeto de IoC.

A Aplicação também possui outros eventos:

```text
SaleCancelledEvent
SaleModifiedEvent
SaleItemsCancelledEvent
SaleReactivatedEvent

ProductCreatedEvent
ProductDeletedEvent
ProductModifiedEvent

BranchCreatedEvent
BranchDeletedEvent
BranchModifiedEvent

UserCreatedEvent
UserDeletedEvent
UserModifiedEvent
```

Todos possuem seus logs bem definidos.

---

## Logging

O projeto utiliza Serilog para logs estruturados.

Os logs podem ser exibidos no console, Docker logs ou arquivos locais, dependendo da forma de execução da aplicação.

Exemplo de log gerado ao processar uma venda criada:

```text
Sale created event received and processed. SaleId: ..., SaleNumber: ..., UserId: ..., BranchId: ..., TotalAmount: ...
```

---

## Tratamento de Erros

A API utiliza tratamento global de exceções e respostas padronizadas.

Exemplos de cenários tratados:

| Cenário | Status HTTP |
|---|---:|
| Entidade não encontrada | 404 |
| Request inválida | 400 |
| Erro de validação | 400 |
| Erro interno inesperado | 500 |

---

## Como Executar o Projeto

### Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- .NET 8 SDK
- Docker
- Docker Compose
- Git

Opcional:

- Visual Studio 2022
- DBeaver, pgAdmin ou outro cliente PostgreSQL

Localmente (Caso não use o docker):
- PostgreSQL 16
- Rabbitmq:3

---

## Executando com Docker Compose (RECOMENDADO)

Na pasta onde está o arquivo `docker-compose.yml`, execute:

```bash
docker compose up --build
```

Para executar em background:

```bash
docker compose up -d --build
```

Para parar os containers:

```bash
docker compose down
```

Para parar os containers e remover volumes:

```bash
docker compose down -v
```

Use `-v` somente se quiser apagar também os dados persistidos dos volumes.

**Atenção:** No caso do docker as migrations são aplicadas automaticamente caso a flag APPLY_MIGRATIONS esteja true.

---

## Connection Strings

### API rodando localmente e serviços em Docker

Exemplo para RabbitMQ:

```json
{
  "ConnectionStrings": {
    "RabbitMq": "amqp://guest:guest@localhost:5672"
  }
}
```

Exemplo para PostgreSQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=salesproject;Username=postgres;Password=Pass@word"
  }
}
```

### API rodando dentro do Docker Compose

Quando a API também está dentro do Docker Compose, o host deve ser o nome do serviço.

Exemplo:

```json
{
  "ConnectionStrings": {
    "RabbitMq": "amqp://guest:guest@rabbitmq:5672"
  }
}
```

---

## Executando localmente sem Docker

### 1. Clonar o repositório

```bash
git clone <repository-url>
cd <repository-folder>
```

*A pasta que você deve ir com change directory é a backend que tem a SalesProject.sln*

### 2. Restaurar dependências

```bash
dotnet restore .\SalesProject.sln
```

Caso o nome da solution seja diferente, ajuste o comando conforme o arquivo .sln existente no projeto.

### 3. Compilar a solução

```bash
dotnet build .\SalesProject.sln
```

### 4. Aplicar migrations

Ajuste o comando conforme o nome real dos projetos da solução.

Exemplo:

```bash
dotnet ef database update \
  --project src/SalesProject.ORM \
  --startup-project src/SalesProject.WebApi
```

No Windows PowerShell, você também pode usar em uma linha:

```bash
dotnet ef database update --project .\src\SalesProject.ORM --startup-project .\src\SalesProject.WebApi
```

### 5. Executar a API

```bash
dotnet run --project .\src\SalesProject.WebApi
```

### Você pode executar também usando Visual Studio, selecionando a solution

Caso queira executar pelo Visual Studio essas são as 2 images separadas do postgreSQL e do rabbitMQ

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16
    container_name: salesproject_postgres
    environment:
      POSTGRES_DB: salesproject
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: Pass@word
    ports:
      - "5432:5432"
    volumes:
      - salesproject_postgres_data_test:/var/lib/postgresql/data

  rabbitmq:
    image: rabbitmq:3-management
    container_name: salesproject_rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest

volumes:
  salesproject_postgres_data_test:
```

### Não se esqueça de executar as migrations, se preferir você pode acessar o painel console gerenciador de pacotes e usar o seguinte comando apontando para o projeto ORM:

```bash
Update-Database
```

## Executando os Testes

O projeto possui testes automatizados para validar os principais casos de uso da aplicação, incluindo handlers, validators e regras de negócio.

Execute os comandos abaixo na pasta raiz do projeto, onde está o arquivo `.sln`.

### Executando todos os testes

Para executar todos os testes da solução:

```bash
dotnet test .\SalesProject.sln
```

### Executando um projeto de testes específico

Para executar apenas os testes unitários:

```bash
dotnet test .\tests\SalesProject.Unit\
```

Ou informe diretamente o arquivo `.csproj` do projeto de testes:

```bash
dotnet test .\tests\SalesProject.Unit\SalesProject.Unit.csproj
```

### Executando testes com mais detalhes

Para exibir mais informações durante a execução dos testes:

```bash
dotnet test .\tests\SalesProject.Unit\ --verbosity normal
```

### Executando testes por filtro

Para executar apenas testes que contenham determinado nome na classe, namespace ou método:

```bash
dotnet test .\tests\SalesProject.Unit\ --filter "FullyQualifiedName~CreateSaleHandlerTests"
```

Exemplo para executar testes relacionados a produtos:

```bash
dotnet test .\tests\SalesProject.Unit\ --filter "FullyQualifiedName~Product"
```

Exemplo para executar testes relacionados a vendas:

```bash
dotnet test .\tests\SalesProject.Unit\ --filter "FullyQualifiedName~Sale"
```

### Observação

Use sempre `dotnet test` para executar testes e informe a solução ou o projeto de testes quando houver mais de um `.sln` ou `.csproj` na mesma pasta.

Exemplo executando o projeto de testes unitários:

```bash
dotnet test .\tests\SalesProject.Unit\
```
---

---

## Swagger

Com a API em execução, acesse:


```text
http://localhost:8080/swagger
```

O Swagger permite testar os endpoints diretamente pelo navegador.

---

## Observações sobre Docker

- O Docker Compose executa os serviços localmente.
- Para limpar os containers:

```bash
docker compose down
```

- Para limpar containers e volumes:

```bash
docker compose down -v
```

---

## Status do Projeto

Funcionalidades implementadas:

- CRUD de usuários
- CRUD de produtos
- CRUD de filiais
- CRUD de vendas
- Paginação e filtros em listagens
- Validação com FluentValidation
- Mapeamentos com AutoMapper
- Casos de uso com MediatR
- Persistência com EF Core e PostgreSQL
- Mensageria com Rebus e RabbitMQ
- Logs com Serilog
- Docker Compose para ambiente local

---

## Autor

Desenvolvido por Kawhan Laurindo de Lima.
