import { useEffect, useMemo, useState } from "react";
import "./DashboardPage.css";
import {
  getTotaisPorPessoa,
  getTotaisPorCategoria,
  type TotaisPorPessoaDto,
  type TotaisPorCategoriaDto,
} from "../../api/dashboardApi";

type TotaisPessoaRow = {
  id: number;
  nome: string;
  receitas: number;
  despesas: number;
  saldo: number;
};

type TotaisCategoriaRow = {
  id: number;
  description: string;
  receitas: number;
  despesas: number;
  saldo: number;
};

function formatBRL(value?: number | null) {
  const v = Number(value ?? 0);
  return v.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

export default function DashboardPage() {
  // ===== filtros pessoa
  const [orderByPessoa, setOrderByPessoa] = useState<"nome" | "receitas" | "despesas" | "saldo">(
    "nome"
  );
  const [directionPessoa, setDirectionPessoa] = useState<"asc" | "desc">("asc");
  const [qPessoa, setQPessoa] = useState("");

  // ===== filtros categoria
  const [orderByCat, setOrderByCat] = useState<"description" | "receitas" | "despesas" | "saldo">(
    "description"
  );
  const [directionCat, setDirectionCat] = useState<"asc" | "desc">("asc");
  const [qCat, setQCat] = useState("");

  // ===== data pessoa
  const [pessoas, setPessoas] = useState<TotaisPessoaRow[]>([]);
  const [pessoasGeral, setPessoasGeral] = useState<{ receitas: number; despesas: number; saldo: number }>({
    receitas: 0,
    despesas: 0,
    saldo: 0,
  });

  // ===== data categoria
  const [cats, setCats] = useState<TotaisCategoriaRow[]>([]);
  const [catsGeral, setCatsGeral] = useState<{ receitas: number; despesas: number; saldo: number }>({
    receitas: 0,
    despesas: 0,
    saldo: 0,
  });

  // ===== ui
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    try {
      setLoading(true);
      setError(null);

      const [dtoPessoa, dtoCat]: [TotaisPorPessoaDto, TotaisPorCategoriaDto] = await Promise.all([
        getTotaisPorPessoa(),
        getTotaisPorCategoria(),
      ]);

      // ===== Pessoa
      const pessoaRows: TotaisPessoaRow[] = (dtoPessoa.items ?? []).map((x) => ({
        id: Number((x as any).pessoaId ?? 0),
        nome: String((x as any).pessoaNome ?? ""),
        receitas: Number((x as any).totalReceitas ?? 0),
        despesas: Number((x as any).totalDespesas ?? 0),
        saldo: Number((x as any).saldo ?? 0),
      }));

      setPessoas(pessoaRows);

      setPessoasGeral({
        receitas: Number(dtoPessoa.totalGeral?.totalReceitas ?? 0),
        despesas: Number(dtoPessoa.totalGeral?.totalDespesas ?? 0),
        saldo: Number(dtoPessoa.totalGeral?.saldo ?? 0),
      });

      // ===== Categoria
      const catRows: TotaisCategoriaRow[] = (dtoCat.items ?? []).map((x) => ({
        id: Number((x as any).categoryId ?? 0),
        description: String((x as any).categoryDescription ?? ""),
        receitas: Number((x as any).totalReceitas ?? 0),
        despesas: Number((x as any).totalDespesas ?? 0),
        saldo: Number((x as any).saldo ?? 0),
      }));

      setCats(catRows);

      setCatsGeral({
        receitas: Number(dtoCat.totalGeral?.totalReceitas ?? 0),
        despesas: Number(dtoCat.totalGeral?.totalDespesas ?? 0),
        saldo: Number(dtoCat.totalGeral?.saldo ?? 0),
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

  // ===== rows pessoa com filtro/ordenação
  const pessoaRows = useMemo(() => {
    const filtered = pessoas.filter((x) => x.nome.toLowerCase().includes(qPessoa.toLowerCase()));

    filtered.sort((a, b) => {
      const mul = directionPessoa === "asc" ? 1 : -1;

      if (orderByPessoa === "nome") return a.nome.localeCompare(b.nome) * mul;
      if (orderByPessoa === "receitas") return (a.receitas - b.receitas) * mul;
      if (orderByPessoa === "despesas") return (a.despesas - b.despesas) * mul;
      return (a.saldo - b.saldo) * mul;
    });

    return filtered;
  }, [pessoas, qPessoa, orderByPessoa, directionPessoa]);

  // ===== rows categoria com filtro/ordenação
  const catRows = useMemo(() => {
    const filtered = cats.filter((x) => x.description.toLowerCase().includes(qCat.toLowerCase()));

    filtered.sort((a, b) => {
      const mul = directionCat === "asc" ? 1 : -1;

      if (orderByCat === "description") return a.description.localeCompare(b.description) * mul;
      if (orderByCat === "receitas") return (a.receitas - b.receitas) * mul;
      if (orderByCat === "despesas") return (a.despesas - b.despesas) * mul;
      return (a.saldo - b.saldo) * mul;
    });

    return filtered;
  }, [cats, qCat, orderByCat, directionCat]);

  return (
    <div className="dash">
      <div className="dashHeader">
        <div>
          <h1>Dashboard</h1>
          <p>Totais por pessoa e por categoria.</p>
        </div>

        <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
          <button className="btn" onClick={load} disabled={loading}>
            Recarregar
          </button>
          <span className="chip">{loading ? "Carregando..." : "Atualizado"}</span>
        </div>
      </div>

      {error && <div style={{ padding: 12, color: "#ffb4b4" }}>{error}</div>}

      {/* ===== Summary cards (Top) */}
      <div className="dashSummary">
        <div className="sumCard">
          <div className="sumLabel">Receitas (geral)</div>
          <div className="sumValue">{formatBRL(pessoasGeral.receitas)}</div>
        </div>
        <div className="sumCard">
          <div className="sumLabel">Despesas (geral)</div>
          <div className="sumValue">{formatBRL(pessoasGeral.despesas)}</div>
        </div>
        <div className="sumCard">
          <div className="sumLabel">Saldo (geral)</div>
          <div className={`sumValue ${pessoasGeral.saldo < 0 ? "neg" : "pos"}`}>
            {formatBRL(pessoasGeral.saldo)}
          </div>
        </div>
      </div>

      {/* =========================
          TOTAIS POR PESSOA
      ========================= */}
      <h2 className="dashSectionTitle">Totais por pessoa</h2>

      <div className="filtersCard">
        <div className="filtersGrid">
          <div className="field">
            <label>Ordenar por</label>
            <select
              value={orderByPessoa}
              onChange={(e) => setOrderByPessoa(e.target.value as any)}
              disabled={loading}
            >
              <option value="nome">Nome</option>
              <option value="receitas">Receitas</option>
              <option value="despesas">Despesas</option>
              <option value="saldo">Saldo</option>
            </select>
          </div>

          <div className="field">
            <label>Direção</label>
            <select
              value={directionPessoa}
              onChange={(e) => setDirectionPessoa(e.target.value as any)}
              disabled={loading}
            >
              <option value="asc">Asc</option>
              <option value="desc">Desc</option>
            </select>
          </div>

          <div className="field grow">
            <label>Buscar pessoa</label>
            <input
              value={qPessoa}
              onChange={(e) => setQPessoa(e.target.value)}
              placeholder="Buscar..."
              disabled={loading}
            />
          </div>
        </div>
      </div>

      <div className="tableCard">
        <div className="tableMeta">
          Totais por pessoa — {loading ? "carregando..." : `${pessoaRows.length} registro(s)`}
        </div>

        <table className="table">
          <thead>
            <tr>
              <th>Pessoa</th>
              <th className="num">Receitas</th>
              <th className="num">Despesas</th>
              <th className="num">Saldo</th>
            </tr>
          </thead>

          <tbody>
            {!loading &&
              !error &&
              pessoaRows.map((r) => {
                const saldoClass = r.saldo < 0 ? "neg" : "pos";
                return (
                  <tr key={r.id}>
                    <td className="name">{r.nome}</td>
                    <td className="num">{formatBRL(r.receitas)}</td>
                    <td className="num">{formatBRL(r.despesas)}</td>
                    <td className={`num ${saldoClass}`}>{formatBRL(r.saldo)}</td>
                  </tr>
                );
              })}

            {!loading && !error && pessoaRows.length === 0 && (
              <tr>
                <td colSpan={4} className="empty">
                  Nenhuma pessoa encontrada.
                </td>
              </tr>
            )}

            {loading && (
              <tr>
                <td colSpan={4} className="empty">
                  Carregando...
                </td>
              </tr>
            )}

            {!loading && !error && (
              <tr className="totalRow">
                <td className="name">Total geral</td>
                <td className="num">{formatBRL(pessoasGeral.receitas)}</td>
                <td className="num">{formatBRL(pessoasGeral.despesas)}</td>
                <td className={`num ${pessoasGeral.saldo < 0 ? "neg" : "pos"}`}>
                  {formatBRL(pessoasGeral.saldo)}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <div className="dashGap" />

      {/* =========================
          TOTAIS POR CATEGORIA
      ========================= */}
      <h2 className="dashSectionTitle">Totais por categoria</h2>

      <div className="filtersCard">
        <div className="filtersGrid">
          <div className="field">
            <label>Ordenar por</label>
            <select
              value={orderByCat}
              onChange={(e) => setOrderByCat(e.target.value as any)}
              disabled={loading}
            >
              <option value="description">Descrição</option>
              <option value="receitas">Receitas</option>
              <option value="despesas">Despesas</option>
              <option value="saldo">Saldo</option>
            </select>
          </div>

          <div className="field">
            <label>Direção</label>
            <select value={directionCat} onChange={(e) => setDirectionCat(e.target.value as any)} disabled={loading}>
              <option value="asc">Asc</option>
              <option value="desc">Desc</option>
            </select>
          </div>

          <div className="field grow">
            <label>Buscar categoria</label>
            <input value={qCat} onChange={(e) => setQCat(e.target.value)} placeholder="Buscar..." disabled={loading} />
          </div>
        </div>
      </div>

      <div className="tableCard">
        <div className="tableMeta">
          Totais por categoria — {loading ? "carregando..." : `${catRows.length} registro(s)`}
        </div>

        <table className="table">
          <thead>
            <tr>
              <th>Categoria</th>
              <th className="num">Receitas</th>
              <th className="num">Despesas</th>
              <th className="num">Saldo</th>
            </tr>
          </thead>

          <tbody>
            {!loading &&
              !error &&
              catRows.map((r) => {
                const saldoClass = r.saldo < 0 ? "neg" : "pos";
                return (
                  <tr key={r.id}>
                    <td className="name">{r.description}</td>
                    <td className="num">{formatBRL(r.receitas)}</td>
                    <td className="num">{formatBRL(r.despesas)}</td>
                    <td className={`num ${saldoClass}`}>{formatBRL(r.saldo)}</td>
                  </tr>
                );
              })}

            {!loading && !error && catRows.length === 0 && (
              <tr>
                <td colSpan={4} className="empty">
                  Nenhuma categoria encontrada.
                </td>
              </tr>
            )}

            {loading && (
              <tr>
                <td colSpan={4} className="empty">
                  Carregando...
                </td>
              </tr>
            )}

            {!loading && !error && (
              <tr className="totalRow">
                <td className="name">Total geral</td>
                <td className="num">{formatBRL(catsGeral.receitas)}</td>
                <td className="num">{formatBRL(catsGeral.despesas)}</td>
                <td className={`num ${catsGeral.saldo < 0 ? "neg" : "pos"}`}>
                  {formatBRL(catsGeral.saldo)}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
