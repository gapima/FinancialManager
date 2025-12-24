import { useEffect, useMemo, useState } from "react";
import "./DashboardPage.css";
import { getTotaisPorPessoa, type TotaisPorPessoaDto } from "../../api/dashboardApi";

type TotaisPessoaRow = {
  id: number;
  nome: string;
  receitas: number;
  despesas: number;
  saldo: number;
};

function formatBRL(value?: number | null) {
  const v = Number(value ?? 0);
  return v.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}


export default function DashboardPage() {
  const [orderBy, setOrderBy] = useState<"nome" | "receitas" | "despesas" | "saldo">("nome");
  const [direction, setDirection] = useState<"asc" | "desc">("asc");
  const [q, setQ] = useState("");

  const [data, setData] = useState<TotaisPessoaRow[]>([]);
  const [geral, setGeral] = useState<{ receitas: number; despesas: number; saldo: number }>({
    receitas: 0,
    despesas: 0,
    saldo: 0,
  });

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    try {
      setLoading(true);
      setError(null);

      const dto: TotaisPorPessoaDto = await getTotaisPorPessoa();

      // adapta o shape do back pro shape do front
    const rows: TotaisPessoaRow[] = (dto.items ?? []).map((x) => ({
      id: Number(x.pessoaId ?? 0),
      nome: String(x.pessoaNome ?? ""),
      receitas: Number(x.totalReceitas ?? 0),
      despesas: Number(x.totalDespesas ?? 0),
      saldo: Number(x.saldo ?? 0),
    }));

    setData(rows);

    setGeral({
      receitas: Number(dto.totalReceitas ?? 0),
      despesas: Number(dto.totalDespesas ?? 0),
      saldo: Number(dto.saldoLiquido ?? 0),
    });
    } catch (e) {
      setError(e instanceof Error ? e.message : "Erro ao carregar dashboard");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, []);

  const rows = useMemo(() => {
    const filtered = data.filter((x) => x.nome.toLowerCase().includes(q.toLowerCase()));

    filtered.sort((a, b) => {
      const mul = direction === "asc" ? 1 : -1;

      if (orderBy === "nome") return a.nome.localeCompare(b.nome) * mul;
      if (orderBy === "receitas") return (a.receitas - b.receitas) * mul;
      if (orderBy === "despesas") return (a.despesas - b.despesas) * mul;
      return (a.saldo - b.saldo) * mul;
    });

    return filtered;
  }, [data, q, orderBy, direction]);

  const totals = useMemo(() => {
    const receitas = rows.reduce((acc, x) => acc + x.receitas, 0);
    const despesas = rows.reduce((acc, x) => acc + x.despesas, 0);

    return {
      receitas,
      despesas,
      saldo: receitas - despesas,
    };
  }, [rows]);

  return (
    <div className="dash">
      <div className="dashHeader">
        <div>
          <h1>Totais por pessoa</h1>
          <p>Lista receitas, despesas e saldo por pessoa, com total geral ao final.</p>
        </div>

        <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
          <button className="btn" onClick={load} disabled={loading}>
            Recarregar
          </button>
          <span className="chip">{loading ? "Carregando..." : `${rows.length} resultado(s)`}</span>
        </div>
      </div>

      <div className="filtersCard">
        <div className="filtersGrid">
          <div className="field">
            <label>Ordenar por</label>
            <select value={orderBy} onChange={(e) => setOrderBy(e.target.value as any)} disabled={loading}>
              <option value="nome">Nome</option>
              <option value="receitas">Receitas</option>
              <option value="despesas">Despesas</option>
              <option value="saldo">Saldo</option>
            </select>
          </div>

          <div className="field">
            <label>Direção</label>
            <select value={direction} onChange={(e) => setDirection(e.target.value as any)} disabled={loading}>
              <option value="asc">Asc</option>
              <option value="desc">Desc</option>
            </select>
          </div>

          <div className="field grow">
            <label>Buscar pessoa</label>
            <input
              value={q}
              onChange={(e) => setQ(e.target.value)}
              placeholder="Buscar..."
              disabled={loading}
            />
          </div>
        </div>
      </div>

      <div className="tableCard">
        <div className="tableMeta">
          Showing {rows.length} of {data.length}
        </div>

        {error && <div style={{ padding: 12, color: "#ffb4b4" }}>{error}</div>}

        <table className="table">
          <thead>
            <tr>
              <th>Pessoa</th>
              <th className="num">Receitas</th>
              <th className="num">Despesas</th>
              <th className="num">Saldo</th>
              <th className="actions">Ações</th>
            </tr>
          </thead>

          <tbody>
            {!loading &&
              !error &&
              rows.map((r) => {
                const saldoClass = r.saldo < 0 ? "neg" : "pos";

                return (
                  <tr key={r.id}>
                    <td className="name">{r.nome}</td>
                    <td className="num">{formatBRL(r.receitas)}</td>
                    <td className="num">{formatBRL(r.despesas)}</td>
                    <td className={`num ${saldoClass}`}>{formatBRL(r.saldo)}</td>
                    <td className="actions">
                      <button className="kebab" title="Ações">
                        ⋮
                      </button>
                    </td>
                  </tr>
                );
              })}

            {!loading && !error && rows.length === 0 && (
              <tr>
                <td colSpan={5} className="empty">
                  Nenhuma pessoa encontrada.
                </td>
              </tr>
            )}

            {loading && (
              <tr>
                <td colSpan={5} className="empty">
                  Carregando...
                </td>
              </tr>
            )}

            {!loading && !error && (
              <tr className="totalRow">
                <td className="name">Total geral</td>
                <td className="num">{formatBRL(totals.receitas)}</td>
                <td className="num">{formatBRL(totals.despesas)}</td>
                <td className={`num ${totals.saldo < 0 ? "neg" : "pos"}`}>
                  {formatBRL(totals.saldo)}
                </td>
                <td />
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
