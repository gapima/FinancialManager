import { useEffect, useMemo, useState } from "react";
import "./TransacoesPage.css";

import { listarTransacoes, type TransactionResponseDto } from "../../api/transacoesApi";
import { listarPessoas, type PessoaResponseDto } from "../../api/pessoasApi";
import { listarCategorias, type CategoryResponseDto } from "../../api/categoryApi";

import TransactionFormModal from "../../components/transacao/TransactionFormModal";

type Tipo = 1 | 2;
function tipoLabel(type: number) {
  switch (type as Tipo) {
    case 1:
      return "Receita";
    case 2:
      return "Despesa";
    default:
      return `Tipo ${type}`;
  }
}

function formatBRL(value: number) {
  return new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(value);
}

function parseAmount(amount: string): number {
  // tenta lidar com "10", "10.50", "10,50", "1.234,56"
  const normalized = amount.replace(/\./g, "").replace(",", ".");
  const n = Number(normalized);
  return Number.isFinite(n) ? n : 0;
}

function toStartOfDay(d: Date) {
  const x = new Date(d);
  x.setHours(0, 0, 0, 0);
  return x;
}

function toEndOfDay(d: Date) {
  const x = new Date(d);
  x.setHours(23, 59, 59, 999);
  return x;
}

export default function TransacoesPage() {
  const [search, setSearch] = useState("");
  const [dateFrom, setDateFrom] = useState<string>(""); // yyyy-mm-dd
  const [dateTo, setDateTo] = useState<string>(""); // yyyy-mm-dd

  const [rows, setRows] = useState<TransactionResponseDto[]>([]);
  const [pessoas, setPessoas] = useState<PessoaResponseDto[]>([]);
  const [cats, setCats] = useState<CategoryResponseDto[]>([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [createOpen, setCreateOpen] = useState(false);

  async function load() {
    try {
      setLoading(true);
      setError(null);

      const [tx, ps, cs] = await Promise.all([listarTransacoes(), listarPessoas(), listarCategorias()]);

      setRows(tx);
      setPessoas(ps);
      setCats(cs);
    } catch (e) {
      setError(e instanceof Error ? e.message : "Erro ao carregar transações");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, []);

  const pessoaMap = useMemo(() => {
    const m = new Map<number, string>();
    for (const p of pessoas) m.set(p.id, p.nome);
    return m;
  }, [pessoas]);

  const catMap = useMemo(() => {
    const m = new Map<number, string>();
    for (const c of cats) m.set(c.id, c.description);
    return m;
  }, [cats]);

  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase();

    const from = dateFrom ? toStartOfDay(new Date(dateFrom)) : null;
    const to = dateTo ? toEndOfDay(new Date(dateTo)) : null;

    return rows.filter((t) => {
      // texto
      const okText = !q || (t.description ?? "").toLowerCase().includes(q);

      // data
      let okDate = true;
      if (from || to) {
        if (!t.createdAt) okDate = false;
        else {
          const dt = new Date(t.createdAt);
          if (from && dt < from) okDate = false;
          if (to && dt > to) okDate = false;
        }
      }

      return okText && okDate;
    });
  }, [rows, search, dateFrom, dateTo]);

  function onCreateTransaction() {
    setCreateOpen(true);
  }

  return (
    <div className="page">
      {/* Modal: Criar Transação */}
      <TransactionFormModal open={createOpen} onClose={() => setCreateOpen(false)} onCreated={load} />

      <div className="pageHeader">
        <div>
          <div className="breadcrumb">Movimento / Transações</div>
          <h1>Transações</h1>
          <p className="subtitle">Lista vindo da API (GET /api/Transaction).</p>
        </div>

        <div className="headerActions">
          <button className="btn primary" onClick={onCreateTransaction}>
            + Nova transação
          </button>
        </div>
      </div>

      <div className="card">
        <div className="toolbar">
          <div className="field">
            <label>Buscar</label>
            <input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Buscar por descrição..."
              disabled={loading}
            />
          </div>

          <div className="field">
            <label>De</label>
            <input
              type="date"
              value={dateFrom}
              onChange={(e) => setDateFrom(e.target.value)}
              disabled={loading}
            />
          </div>

          <div className="field">
            <label>Até</label>
            <input type="date" value={dateTo} onChange={(e) => setDateTo(e.target.value)} disabled={loading} />
          </div>

          <div className="toolbarRight">
            <button className="btn" onClick={load} disabled={loading}>
              Recarregar
            </button>

            <button
              className="btn"
              onClick={() => {
                setSearch("");
                setDateFrom("");
                setDateTo("");
              }}
              disabled={loading}
            >
              Limpar filtros
            </button>

            <span className="chip">{loading ? "Carregando..." : `${filtered.length} resultado(s)`}</span>
          </div>
        </div>

        {error && <div style={{ padding: 12, color: "#ffb4b4" }}>{error}</div>}

        <div className="txCards">
          {!loading &&
            !error &&
            filtered.map((t) => {
              const pessoaNome = pessoaMap.get(t.pessoaId) ?? `Pessoa #${t.pessoaId}`;
              const catNome = catMap.get(t.categoryId) ?? `Categoria #${t.categoryId}`;

              const valor = parseAmount(t.amount);
              const labelTipo = tipoLabel(t.type);

              const data = t.createdAt ? new Date(t.createdAt).toLocaleString("pt-BR") : "-";

              return (
                <div className="txCard" key={t.id}>
                  <div className="txTop">
                    <div className="txTitle">{t.description}</div>
                    <div className="txId">#{t.id}</div>
                  </div>

                  <div className="txMeta">
                    <span className="badge">{labelTipo}</span>
                    <span className="badge soft">{pessoaNome}</span>
                    <span className="badge soft">{catNome}</span>
                  </div>

                  <div className="txBottom">
                    <div className="txAmount">{formatBRL(valor)}</div>
                    <div className="txDate">{data}</div>
                  </div>
                </div>
              );
            })}

          {!loading && !error && filtered.length === 0 && <div className="empty">Nenhuma transação encontrada.</div>}

          {loading && <div className="empty">Carregando...</div>}
        </div>
      </div>
    </div>
  );
}
