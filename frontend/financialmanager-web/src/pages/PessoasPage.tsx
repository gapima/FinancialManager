import { useEffect, useMemo, useState } from "react";
import "./PessoasPage.css";
import { listarPessoas, type PessoaResponseDto } from "../app/api/pessoasApi";
import PessoaCreateModal from "../components/PessoaCreateModal";

export default function PessoasPage() {
  const [search, setSearch] = useState("");
  const [rows, setRows] = useState<PessoaResponseDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [createOpen, setCreateOpen] = useState(false);

  async function load() {
    try {
      setLoading(true);
      setError(null);
      const data = await listarPessoas();
      setRows(data);
    } catch (e) {
      setError(e instanceof Error ? e.message : "Erro ao carregar pessoas");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, []);

  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return rows;
    return rows.filter((p) => p.nome.toLowerCase().includes(q));
  }, [rows, search]);

  function onDeletePessoa(id: number) {
    alert(`Em breve: DELETE pessoa id=${id}`);
  }

  return (
    <div className="page">
      <PessoaCreateModal
        open={createOpen}
        onClose={() => setCreateOpen(false)}
        onCreated={load}
      />

      <div className="pageHeader">
        <div>
          <div className="breadcrumb">Cadastros / Pessoas</div>
          <h1>Pessoas</h1>
          <p className="subtitle">Lista vindo da API (GET /api/Pessoa).</p>
        </div>

        <div className="headerActions">
          <button className="btn primary" onClick={() => setCreateOpen(true)}>
            + Adicionar pessoa
          </button>
        </div>
      </div>

      {/* resto igual */}
      <div className="card">
        <div className="toolbar">
          <div className="field">
            <label>Buscar</label>
            <input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Buscar pessoa..."
              disabled={loading}
            />
          </div>

          <div className="toolbarRight">
            <button className="btn" onClick={load} disabled={loading}>
              Recarregar
            </button>
            <span className="chip">
              {loading ? "Carregando..." : `${filtered.length} resultado(s)`}
            </span>
          </div>
        </div>

        {error && (
          <div style={{ padding: 12, color: "#ffb4b4" }}>
            {error}
          </div>
        )}

        <div className="tableWrap">
          <table className="table">
            <thead>
              <tr>
                <th style={{ width: 90 }}>ID</th>
                <th>Nome</th>
                <th style={{ width: 120 }}>Idade</th>
                <th style={{ width: 140, textAlign: "right" }}>Ações</th>
              </tr>
            </thead>
            <tbody>
              {!loading &&
                !error &&
                filtered.map((p) => (
                  <tr key={p.id}>
                    <td className="mono">{p.id}</td>
                    <td className="name">{p.nome}</td>
                    <td>{p.idade}</td>
                    <td style={{ textAlign: "right" }}>
                      <button className="btn danger" onClick={() => onDeletePessoa(p.id)}>
                        Excluir
                      </button>
                    </td>
                  </tr>
                ))}

              {!loading && !error && filtered.length === 0 && (
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
            </tbody>
          </table>
        </div>

        {/* <div className="footerHint">
          * Próximo passo: ligar DELETE /api/Pessoa/{id}.
        </div> */}
      </div>
    </div>
  );
}
