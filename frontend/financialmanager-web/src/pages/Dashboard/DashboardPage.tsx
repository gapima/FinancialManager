import { useMemo, useState } from "react";
import "./DashboardPage.css";

type TotaisPessoaRow = {
  id: number;
  nome: string;
  receitas: number;
  despesas: number;
};

function formatBRL(value: number) {
  return value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

export default function DashboardPage() {
  const [orderBy, setOrderBy] = useState<"nome" | "receitas" | "despesas" | "saldo">("nome");
  const [direction, setDirection] = useState<"asc" | "desc">("asc");
  const [q, setQ] = useState("");

  const data: TotaisPessoaRow[] = [
    { id: 1, nome: "Gabriel", receitas: 5200, despesas: 3100 },
    { id: 2, nome: "Gabriela", receitas: 4300, despesas: 2500 },
    { id: 3, nome: "Noah", receitas: 0, despesas: 800 },
  ];

  const rows = useMemo(() => {
    const filtered = data.filter((x) => x.nome.toLowerCase().includes(q.toLowerCase()));
    const withSaldo = filtered.map((x) => ({ ...x, saldo: x.receitas - x.despesas }));

    withSaldo.sort((a, b) => {
      const mul = direction === "asc" ? 1 : -1;

      if (orderBy === "nome") return a.nome.localeCompare(b.nome) * mul;
      if (orderBy === "receitas") return (a.receitas - b.receitas) * mul;
      if (orderBy === "despesas") return (a.despesas - b.despesas) * mul;
      return (a.saldo - b.saldo) * mul;
    });

    return withSaldo;
  }, [q, orderBy, direction]);

  const totals = useMemo(() => {
    const receitas = rows.reduce((acc, x) => acc + x.receitas, 0);
    const despesas = rows.reduce((acc, x) => acc + x.despesas, 0);
    return { receitas, despesas, saldo: receitas - despesas };
  }, [rows]);

  return (
    <div className="dash">
      <div className="dashHeader">
        <h1>Totais por pessoa</h1>
        <p>Lista receitas, despesas e saldo por pessoa, com total geral ao final.</p>
      </div>

      <div className="filtersCard">
        <div className="filtersGrid">
          <div className="field">
            <label>Ordenar por</label>
            <select value={orderBy} onChange={(e) => setOrderBy(e.target.value as any)}>
              <option value="nome">Nome</option>
              <option value="receitas">Receitas</option>
              <option value="despesas">Despesas</option>
              <option value="saldo">Saldo</option>
            </select>
          </div>

          <div className="field">
            <label>Direção</label>
            <select value={direction} onChange={(e) => setDirection(e.target.value as any)}>
              <option value="asc">Asc</option>
              <option value="desc">Desc</option>
            </select>
          </div>

          <div className="field grow">
            <label>Buscar pessoa</label>
            <input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Buscar..." />
          </div>
        </div>
      </div>

      <div className="tableCard">
        <div className="tableMeta">Showing {rows.length} of {data.length}</div>

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
            {rows.map((r) => {
              const saldo = r.receitas - r.despesas;
              const saldoClass = saldo < 0 ? "neg" : "pos";

              return (
                <tr key={r.id}>
                  <td className="name">{r.nome}</td>
                  <td className="num">{formatBRL(r.receitas)}</td>
                  <td className="num">{formatBRL(r.despesas)}</td>
                  <td className={`num ${saldoClass}`}>{formatBRL(saldo)}</td>
                  <td className="actions">
                    <button className="kebab" title="Ações">⋮</button>
                  </td>
                </tr>
              );
            })}

            <tr className="totalRow">
              <td className="name">Total geral</td>
              <td className="num">{formatBRL(totals.receitas)}</td>
              <td className="num">{formatBRL(totals.despesas)}</td>
              <td className={`num ${totals.saldo < 0 ? "neg" : "pos"}`}>{formatBRL(totals.saldo)}</td>
              <td />
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  );
}
