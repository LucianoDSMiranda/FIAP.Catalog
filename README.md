# FIAP.Catalog

API de catálogo de jogos com autenticação JWT, SQLite e integração com RabbitMQ via MassTransit.

## ✅ Funcionalidades

- CRUD básico de jogos
- Compra de jogo (publica evento `OrderPlacedEvent` via RabbitMQ)
- Autenticação JWT (Bearer)
- Swagger para testes em desenvolvimento
- Banco de dados SQLite local (`catalog.db`)

## 📁 Estrutura

- `Program.cs` - configuração do app, DbContext, auth, MassTransit e endpoints.
- `Controllers/GamesController.cs` - controller de jogos e compra.
- `Data/CatalogDbContext.cs` - contexto EF Core.
- `Consumers/PaymentProcessedEventConsumer.cs` - consumer de evento de pagamento.
- `Messages/` - eventos compartilhados (OrderPlacedEvent, PaymentProcessedEvent, etc.).

## 🚀 Como rodar

1. Abra no terminal na pasta do projeto:

```bash
cd c:\projetos\Facul\FIAP.Catalog
```

2. Configure as credenciais JWT em `appsettings.json` ou `appsettings.Development.json` (Issuer, Audience, SecretKey).

3. Inicie o RabbitMQ local (ex: Docker):

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

4. Execute a API:

```bash
dotnet run
```

5. Acesse Swagger (se estiver em `Development`):

```
https://localhost:5001/swagger
```

## 🔐 Endpoints principais

Todos os endpoints de `GamesController` requerem autenticação JWT (`[Authorize]`).

- `GET /api/games` - lista todos os jogos
- `GET /api/games/{id}` - busca jogo por id
- `POST /api/games` - cria um jogo
- `POST /api/games/purchase` - compra um jogo (envia evento `OrderPlacedEvent`)

### Modelo `PurchaseRequest`

```json
{
  "userId": "string",
  "gameId": 1,
  "email": "usuario@email.com"
}
```

## 🐘 Notas importantes

- O banco SQLite é criado automaticamente via `EnsureCreated()`.
- A fila de eventos é configurada em `Program.cs`:
  - Exchange `OrderPlacedEvent`
  - Endpoint `payment-processed-queue`
- Verifique se o `JwtSettings` no `appsettings` está presente com `Issuer`, `Audience`, `SecretKey`.

## 🔧 Dicas de desenvolvimento

- Use uma ferramenta de JWT (ex: jwt.io) para gerar tokens se você ainda não tiver um provider.
- Para debug rápido no Swagger, clique em "Authorize" e cole `Bearer <token>`.
- Se precisar resetar dados, delete `catalog.db` e reinicie a API.

---
