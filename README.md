# Duplicata System

Sistema de gestão de duplicatas com integração simulada à B3 (Bolsa do Brasil), arquitetado com eventos assíncronos via Kafka e comunicação gRPC.

---

## Visão Geral

O **Duplicata System** gerencia o ciclo de vida de duplicatas (títulos financeiros), desde a criação até o registro na B3, baixa ou cancelamento. O fluxo é orientado a eventos, com workers consumindo mensagens do Kafka para simular a integração com a B3 e atualizar o status das duplicatas no banco de dados.

---

## Arquitetura

O projeto segue uma arquitetura **Clean Architecture** / **DDD** em .NET 8, organizada em camadas:

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           API Gateway (REST)                                 │
│                     https://localhost:7024 / localhost:5207                  │
└─────────────────────────────────┬───────────────────────────────────────────┘
                                  │ gRPC
                                  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                          gRPC Service                                        │
│                     https://localhost:7004 / localhost:5092                  │
└─────────────────────────────────┬───────────────────────────────────────────┘
                                  │
          ┌───────────────────────┼───────────────────────┐
          ▼                       ▼                       ▼
   ┌──────────────┐      ┌──────────────┐      ┌──────────────────┐
   │ PostgreSQL   │      │    Kafka     │      │   Application    │
   │   (Persist.) │      │   (Eventos)  │      │   Use Cases      │
   └──────────────┘      └──────┬───────┘      └──────────────────┘
                                │
          ┌─────────────────────┼─────────────────────┐
          ▼                     ▼                     ▼
   ┌──────────────┐    ┌────────────────┐    ┌──────────────────┐
   │ Worker       │    │ Worker B3Mock  │    │ Worker Status    │
   │ Consumer     │    │ (Simula B3)    │    │ (Atualiza BD)    │
   └──────────────┘    └────────────────┘    └──────────────────┘
```

---

## Projetos da Solução

| Projeto | Descrição |
|---------|-----------|
| **Duplicata.ApiGateway** | API REST (Swagger) que expõe endpoints e encaminha para o gRPC Service |
| **Duplicata.GrpcService** | Serviço gRPC com a lógica de negócio, persistência e publicação de eventos |
| **Duplicata.Domain** | Entidades, enums e regras de domínio (DDD) |
| **Duplicata.Application** | Casos de uso, DTOs, interfaces e eventos de Kafka |
| **Duplicata.Infrastructure** | Implementações: Kafka, EF Core, PostgreSQL, repositórios |
| **Duplicata.Shared.Contracts** | Contratos Proto (gRPC) compartilhados |
| **Duplicata.Worker.Consumer** | Worker que consome `duplicata.created` (exemplo/observação) |
| **Duplicata.Worker.B3Mock** | Worker que simula a B3: consome `duplicata.created` e publica status |
| **Duplicata.Worker.Status** | Worker que consome eventos de status e atualiza o banco |

---

## Mapeamento de Processos

### 1. Fluxo de Criação de Duplicata

```
Cliente → API Gateway (POST /api/duplicatas)
              → gRPC CreateDuplicata
                   → CreateDuplicataUseCase
                        → Duplicata criada (Status: ARegistrar)
                        → Persiste no PostgreSQL
                        → Publica evento em "duplicata.created"
              ← Retorna Id da duplicata
```

### 2. Fluxo de Processamento na B3 (Simulado)

```
Worker B3Mock consome "duplicata.created"
   → Simula delay de processamento (2–5 s configurável)
   → Define resultado aleatório com probabilidades:
        • 70% → "duplicata.registered" (Registrada)
        • 10% → "duplicata.paid" (Baixada)
        • 20% → "duplicata.rejected" (Cancelada)
   → Publica evento no tópico correspondente
```

### 3. Fluxo de Atualização de Status

```
Worker Status consome:
   • "duplicata.registered" → UpdateDuplicataStatusUseCase (Registrada)
   • "duplicata.paid"       → UpdateDuplicataStatusUseCase (Baixada)
   • "duplicata.rejected"   → UpdateDuplicataStatusUseCase (Cancelada)
        → Busca duplicata no repositório
        → Aplica transição de estado no domínio
        → Atualiza status no PostgreSQL
```

### 4. Diagrama de Estados (Duplicata)

```
     ┌─────────────┐
     │  ARegistrar │ ◄── Criação
     └──────┬──────┘
            │ Registrar()
            ▼
     ┌─────────────┐
     │  Registrada │
     └──────┬──────┘
            │ Baixar()
            ▼
     ┌─────────────┐
     │   Baixada   │
     └─────────────┘

     ARegistrar ──Cancelar()──► Cancelada
     Registrada ─Cancelar()──► Cancelada
```

---

## Tópicos Kafka

| Tópico | Produtor | Consumidores | Descrição |
|--------|----------|--------------|-----------|
| `duplicata.created` | GrpcService (CreateDuplicataUseCase) | Worker.Consumer, Worker.B3Mock | Evento disparado ao criar uma duplicata |
| `duplicata.registered` | Worker.B3Mock | Worker.Status | B3 aprovou o registro |
| `duplicata.paid` | Worker.B3Mock | Worker.Status | B3 indicou pagamento/baixa |
| `duplicata.rejected` | Worker.B3Mock | Worker.Status | B3 rejeitou a duplicata |

---

## Especificações Técnicas

### Tecnologias

- **.NET 8**
- **ASP.NET Core** (API REST + gRPC)
- **Entity Framework Core** + **PostgreSQL**
- **Kafka** (Confluent.Kafka)
- **Protocol Buffers** (gRPC)

### Requisitos

- .NET 8 SDK
- Docker e Docker Compose (Kafka, Zookeeper, Kafka UI)
- PostgreSQL (ou uso do connection string configurado)

### Portas Padrão

| Serviço | HTTPS | HTTP |
|---------|-------|------|
| ApiGateway | 7024 | 5207 |
| GrpcService | 7004 | 5092 |
| Kafka | - | 29092 (externo) |
| Kafka UI | - | 8080 |

---

## Como Executar

### 1. Subir a infraestrutura (Kafka + Zookeeper)

```bash
docker-compose up -d
```

### 2. Criar o banco de dados PostgreSQL

```sql
CREATE DATABASE duplicata_db;
CREATE USER duplicata WITH PASSWORD 'duplicata123';
GRANT ALL PRIVILEGES ON DATABASE duplicata_db TO duplicata;
```

### 3. Aplicar migrations (se houver)

```bash
dotnet ef database update --project Duplicata.Infrastructure --startup-project Duplicata.GrpcService
```

### 4. Executar os projetos

**Terminal 1 – GrpcService**
```bash
dotnet run --project Duplicata.GrpcService
```

**Terminal 2 – ApiGateway**
```bash
dotnet run --project Duplicata.ApiGateway
```

**Terminal 3 – Worker B3Mock**
```bash
dotnet run --project Duplicata.Worker.B3Mock
```

**Terminal 4 – Worker Status**
```bash
dotnet run --project Duplicata.Worker.Status
```

**Opcional – Worker Consumer** (observa `duplicata.created`)
```bash
dotnet run --project Duplicata.Worker.Consumer
```

---

## API REST (ApiGateway)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/duplicatas` | Cria uma duplicata |
| GET | `/api/duplicatas` | Lista todas as duplicatas |
| GET | `/api/duplicatas/by-number?numero=XXX` | Busca duplicata pelo número |

### Exemplo de criação

```json
POST /api/duplicatas
{
  "numero": "DUP-001",
  "valor": 1500.50,
  "vencimento": "2025-03-15"
}
```

**Swagger:** `https://localhost:7024/swagger` (ou `http://localhost:5207/swagger`)

---

## Configuração

### Kafka (`appsettings.json`)

```json
"Kafka": {
  "BootstrapServers": "localhost:29092"
}
```

### B3Mock (probabilidades e delay)

```json
"B3Mock": {
  "MinDelayMs": 2000,
  "MaxDelayMs": 5000,
  "RegisteredProbability": 0.7,
  "PaidProbability": 0.1
}
```

- `RegisteredProbability`: chance de `duplicata.registered` (70%)
- `PaidProbability`: chance de `duplicata.paid` (10%)
- Restante: `duplicata.rejected` (20%)

### Banco de Dados

```json
"ConnectionStrings": {
  "DuplicataDb": "Host=localhost;Port=5432;Database=duplicata_db;Username=duplicata;Password=duplicata123"
}
```

---

## Estrutura de Pastas

```
DuplicataSystem/
├── Duplicata.ApiGateway/        # REST API
├── Duplicata.GrpcService/       # Serviço gRPC
├── Duplicata.Domain/            # Entidades e enums
├── Duplicata.Application/       # Use cases, DTOs, interfaces
├── Duplicata.Infrastructure/    # Kafka, EF Core, repositórios
├── Duplicata.Shared.Contracts/  # Proto files
├── Duplicata.Worker.Consumer/   # Consumer duplicata.created
├── Duplicata.Worker.B3Mock/     # Simulação B3
├── Duplicata.Worker.Status/     # Atualização de status
├── docker-compose.yml           # Kafka, Zookeeper, Kafka UI
└── DuplicataSystem.sln
```

---

## Licença

Projeto de estudos.
