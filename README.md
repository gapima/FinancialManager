# FinancialManager

Aplicação **full stack** para gerenciamento financeiro pessoal, composta por **Front-end (React + Vite)** e **Back-end (.NET 9 Web API)**.

O projeto foi estruturado com foco em **organização**, **separação clara de responsabilidades**, **boas práticas de arquitetura** e **facilidade de avaliação técnica**, com regras de negócio bem definidas e código documentado.

---

## 🧰 Tecnologias e Ferramentas Utilizadas

### Front-end
- **React**
- **Vite**
- **TypeScript**
- **Node.js v20.19.0**

### Back-end
- **.NET 9**
- **ASP.NET Web API**
- **Entity Framework Core (EF Core)**
- **AutoMapper**
- **SQLite** (banco de dados)

---

## 📁 Estrutura de Pastas – Front-end

**Caminho:** `frontend/financialmanager-web`

```text
frontend/financialmanager-web
 ├─ node_modules/
 ├─ public/
 ├─ src/
 │   ├─ api/                 # Comunicação com o back-end (HTTP / fetch)
 │   │   ├─ categoryApi.ts
 │   │   ├─ dashboardApi.ts
 │   │   ├─ pessoasApi.ts
 │   │   └─ transacoesApi.ts
 │   │
 │   ├─ app/                 # Configurações globais / providers
 │   ├─ assets/              # Assets estáticos
 │   │
 │   ├─ components/          # Componentes reutilizáveis organizados por domínio
 │   │   ├─ categoria/
 │   │   ├─ Layout/
 │   │   ├─ Modal/
 │   │   ├─ pessoa/
 │   │   └─ transacao/
 │   │
 │   ├─ lib/                 # Helpers e utilitários compartilhados
 │   │   └─ apiClient.ts
 │   │
 │   ├─ pages/               # Páginas da aplicação (feature-based)
 │   │   ├─ categorias/
 │   │   ├─ Dashboard/
 │   │   ├─ pessoas/
 │   │   └─ transacoes/
 │   │
 │   ├─ App.tsx              # Componente raiz / layout principal
 │   ├─ main.tsx             # Bootstrap da aplicação React (Vite)
 │   ├─ App.css
 │   └─ index.css
 │
 ├─ package.json
 ├─ vite.config.ts
 └─ .env
```

### 📌 Observações de Arquitetura (Front-end)

- A pasta `api/` centraliza todas as chamadas HTTP, evitando `fetch` espalhado pela UI.
- A pasta `pages/` representa as telas da aplicação.
- A pasta `components/` contém componentes reutilizáveis, organizados por domínio.
- Regras de negócio no front-end são aplicadas **apenas para UX**, nunca substituindo validações do back-end.

---

## 🚀 Passo a Passo de Uso da Aplicação

Este guia descreve o fluxo recomendado para utilização da aplicação após subir o **front-end** e o **back-end**.

---

## ▶️ Como Executar o Front-end

### 1️⃣ Acesse a pasta do front-end

```bash
cd frontend\financialmanager-web
```

### 2️⃣ Instale as dependências

```bash
npm i
```

### 3️⃣ Inicie o servidor de desenvolvimento

```bash
npm run dev
```

### ℹ️ Observações Importantes

- O front-end consome a API a partir da variável de ambiente:

```env
VITE_API_URL
```

- Caso a variável não esteja definida, a aplicação utiliza um fallback para ambiente local.
- Por padrão, o front-end ficará disponível em:

```text
http://localhost:5173
```

---

## ▶️ Como Executar o Back-end

### 1️⃣ Acesse a pasta do back-end

```bash
cd backend
```

### 2️⃣ Restaure os pacotes

```bash
dotnet restore
```

### 3️⃣ Aplique as migrations (SQLite)

```bash
dotnet ef database update
```

### 4️⃣ Execute a API

```bash
dotnet run
```

### ℹ️ Observações

- A API sobe em ambiente local com suporte a **Swagger** (em `Development`).
- O banco de dados utilizado é **SQLite**, via **EF Core**.

---

## 📁 Estrutura de Pastas – Back-end

A solução do back-end está organizada em projetos separados por responsabilidade, seguindo uma **arquitetura em camadas**.

```text
backend/
 ├─ FinancialManager.Api/
 ├─ FinancialManager.Application/
 ├─ FinancialManager.Domain/
 ├─ FinancialManager.Infrastructure/
 └─ FinancialManager.Tests/
```

---

## 🧱 Camadas do Back-end (Visão Geral)

### 🔹 FinancialManager.Api (Apresentação)
Responsável por expor os endpoints HTTP (REST), com controllers finos e pipeline configurado com Swagger, CORS e middlewares.

### 🔹 FinancialManager.Application (Regras / Casos de Uso)
Centraliza regras de negócio, validações, DTOs e orquestração.

### 🔹 FinancialManager.Domain (Núcleo do Domínio)
Entidades e enums independentes de infraestrutura.

### 🔹 FinancialManager.Infrastructure (Persistência)
EF Core, migrations SQLite e repositórios.
