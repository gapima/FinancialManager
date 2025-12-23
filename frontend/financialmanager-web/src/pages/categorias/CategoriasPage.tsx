import { useEffect, useMemo, useState } from "react";
import "./CategoriasPage.css";
import {
  deletarCategoria,
  listarCategorias,
  type CategoryResponseDto,
} from "../../api/categoryApi";
import CategoryFormModal from "../../components/categoria/CategoryFormModal";

type Finalidade = 1 | 2 | 3;

function finalidadeLabel(purpose: number): string {
  switch (purpose as Finalidade) {
    case 1:
      return "Receita";
    case 2:
      return "Despesa";
    case 3:
      return "Ambos";
    default:
      return `Desconhecida (${purpose})`;
  }
}

export default function CategoriasPage() {
  const [search, setSearch] = useState("");
  const [rows, setRows] = useState<CategoryResponseDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [createOpen, setCreateOpen] = useState(false);

  const [editOpen, setEditOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<CategoryResponseDto | null>(null);

  async function load() {
    try {
      setLoading(true);
      setError(null);
      const data = await listarCategorias();
      setRows(data);
    } catch (e) {
      setError(e instanceof Error ? e.message : "Erro ao carregar categorias");
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
    return rows.filter((c) => c.description.toLowerCase().includes(q));
  }, [rows, search]);

  function openEditModal(cat: CategoryResponseDto) {
    setEditingCategory(cat);
    setEditOpen(true);
  }

  async function onDeleteCategory(cat: CategoryResponseDto) {
    const ok = window.confirm(
      `Tem certeza que deseja excluir a categoria "${cat.description}"?`
    );
    if (!ok) return;

    try {
      setError(null);
      await deletarCategoria(cat.id);
      await load();
    } catch (e) {
      // mantém padrão do app (mensagem na tela) e também alerta rápido
      const msg = e instanceof Error ? e.message : "Erro ao excluir categoria";
      setError(msg);
      alert(msg);
    }
  }

  return (
    <div className="page">
      {/* Create */}
      <CategoryFormModal
        open={createOpen}
        mode="create"
        onClose={() => setCreateOpen(false)}
        onSaved={load}
      />

      {/* Edit */}
      <CategoryFormModal
        open={editOpen}
        mode="edit"
        initialCategory={editingCategory}
        onClose={() => setEditOpen(false)}
        onSaved={load}
      />

      <div className="pageHeader">
        <div>
          <div className="breadcrumb">Cadastros / Categorias</div>
          <h1>Categorias</h1>
          <p className="subtitle">Lista vindo da API (GET /api/Category).</p>
        </div>

        <div className="headerActions">
          <button className="btn primary" onClick={() => setCreateOpen(true)}>
            + Adicionar categoria
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
              placeholder="Buscar categoria..."
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

        {error && <div style={{ padding: 12, color: "#ffb4b4" }}>{error}</div>}

        <div className="cards">
          {!loading &&
            !error &&
            filtered.map((c) => {
              const label = finalidadeLabel(c.purpose);

              return (
                <div
                  className="catCard selectable"
                  key={c.id}
                  role="button"
                  tabIndex={0}
                  onClick={() => openEditModal(c)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter" || e.key === " ") {
                      e.preventDefault();
                      openEditModal(c);
                    }
                  }}
                  title="Clique para editar"
                >
                  <div className="catLeft">
                    <div className="catTitle">{c.description}</div>
                    <div className="catId">#{c.id}</div>
                  </div>

                  <div className="catRight">
                    <div className="catMeta">
                      <span className="badge">
                        Finalidade: {label}
                        <span style={{ opacity: 0.75 }}> ({c.purpose})</span>
                      </span>
                    </div>

                    <div className="catActions">
                      <button
                        className="btn danger"
                        onClick={(e) => {
                          e.stopPropagation(); // não abre modal de edição
                          void onDeleteCategory(c);
                        }}
                      >
                        Excluir
                      </button>
                    </div>
                  </div>
                </div>
              );
            })}

          {!loading && !error && filtered.length === 0 && (
            <div className="empty">Nenhuma categoria encontrada.</div>
          )}

          {loading && <div className="empty">Carregando...</div>}
        </div>
      </div>
    </div>
  );
}
