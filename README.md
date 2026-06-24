# EscolaApi — Sistema de Gestão Escolar

## Arquitetura
- **auth-service** (Go) — autenticação JWT
- **backend-aspnet** (C# ASP.NET 8) — API REST principal
- **frontend** (SvelteKit) — interface web
- **Turso** — banco de dados SQLite distribuído

## Pré-requisitos
- Docker e Docker Compose
- Conta no Turso (turso.tech)

## Como rodar

### 1. Clone o repositório
```bash
git clone https://github.com/18juba/engsoftware.git
cd seu_repo
```

### 2. Configure o .env
```bash
cp .env.example .env
# edite o .env com suas credenciais
```

### 3. Suba os containers
```bash
docker compose up -d
```

### 4. Acesse
| Serviço | URL |
|---|---|
| Frontend | http://localhost:3000 ou escola-frontend-production.up.railway.app |
| API Backend | http://localhost:8080/scalar ou escola-backend-production.up.railway.app/scalar |
| Auth Service | http://localhost:8000 ou escola-auth-production.up.railway.app |

## Health Checks
- `GET /health` — status geral da API
- `GET /health/ready` — verifica dependências

## Métricas
- Expostas em `GET /metrics` (formato Prometheus)
- Dashboard disponível no próprio Railway

## Deploy automatizado
Push na branch `main` dispara o pipeline GitHub Actions que:
1. Faz build das imagens Docker
2. Publica no RailWay