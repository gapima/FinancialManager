# 📊 FinancialManager API

API REST para gerenciamento financeiro pessoal, permitindo o controle de **pessoas**, **categorias**, **transações financeiras** e a visualização de **dashboards consolidados** (receitas, despesas e saldo).

O projeto foi desenvolvido com foco em **organização de camadas**, **boas práticas**, **legibilidade** e **facilidade de manutenção**, priorizando regras de negócio no back-end.

---

## 🧱 Arquitetura

O projeto segue uma arquitetura em camadas bem definida:

FinancialManager.Api
FinancialManager.Application
FinancialManager.Domain
FinancialManager.Infrastructure
FinancialManager.Tests



### 🔹 Api
Responsável por:
- Expor endpoints REST
- Traduzir resultados do Service em respostas HTTP
- Configuração do pipeline (middlewares, CORS, Swagger)

Controllers são **finos**, delegando regras para a camada Application.

---

### 🔹 Application
Camada responsável por:
- Regras de negócio
- Validações de entrada
- Orquestração de casos de uso
- Definição de contratos (Interfaces)
- DTOs de entrada e saída
- Mapeamentos (AutoMapper)

> Nenhuma regra de negócio fica no controller ou no front-end.

---

### 🔹 Domain
Contém:
- Entidades centrais (`Pessoa`, `Category`, `Transaction`)
- Enums de domínio (`TransactionType`, `CategoryPurpose`, etc.)

Essa camada é **agnóstica de infraestrutura**.

---

### 🔹 Infrastructure
Responsável por:
- Persistência de dados (Entity Framework Core + SQLite)
- Implementação dos repositórios
- Consultas agregadas para o Dashboard

Consultas de leitura utilizam `AsNoTracking()` para melhor performance.

---

## 🗄️ Banco de Dados

- **SQLite**
- Mapeamento via Entity Framework Core
- Relacionamentos:
  - Pessoa → Transactions (**Cascade**)
  - Category → Transactions (**Restrict**)

> Categorias não podem ser removidas se existirem transações vinculadas, garantindo integridade referencial.

---

## 🔄 Funcionalidades

### ✔ Pessoas
- CRUD completo
- Validações de dados no back-end

### ✔ Categorias
- CRUD completo
- Validação de enums de propósito
- Proteção contra exclusão indevida (Restrict)

### ✔ Transações
- CRUD completo
- Validação de:
  - Campos obrigatórios
  - Existência de Pessoa e Categoria
- `CreatedAt` controlado exclusivamente pelo servidor (UTC)

### ✔ Dashboard
Consultas agregadas:
- Totais por Pessoa
- Totais por Categoria
- Total geral consolidado (Receitas, Despesas e Saldo)

> Todos os cálculos são feitos no servidor para evitar inconsistências no front-end.

---

## 📈 Dashboard – Decisões Técnicas

- Agregações feitas no banco usando `GroupBy` e projeção direta para DTOs
- Uso de `LEFT JOIN` para incluir Pessoas/Categorias sem transações
- `Sum` com nullable (`decimal?`) para evitar erros quando não há registros
- Saldo calculado como:  
  **Receitas − Despesas**

---

## 🧪 Tratamento Global de Erros

O projeto possui um **middleware global de exceções**:

- `ArgumentException` → **400 Bad Request**
- `KeyNotFoundException` → **404 Not Found**
- Outras exceções → **500 Internal Server Error**

Formato de erro padronizado usando **ProblemDetails (RFC 7807)**, incluindo `traceId` para facilitar debug.

---

## 🌐 CORS

CORS configurado para permitir comunicação com o front-end em ambiente de desenvolvimento:

http://localhost:5173


---

## 📚 Swagger

Swagger disponível em ambiente de desenvolvimento para exploração e testes dos endpoints.

---

## 🚀 Execução do Projeto

```bash
dotnet restore
dotnet ef database update
dotnet run
