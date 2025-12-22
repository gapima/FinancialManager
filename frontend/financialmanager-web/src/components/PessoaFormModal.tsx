import { useEffect, useMemo, useState } from "react";
import Modal from "./ModalPessoa";
import {
  criarPessoa,
  updatePessoa,
  getPessoaById,
  type PessoaCreateDto,
  type PessoaResponseDto,
  type PessoaUpdateDto,
} from "../app/api/pessoasApi";

type Mode = "create" | "update";

type Props = {
  open: boolean;
  mode: Mode;
  pessoaId?: number; // obrigatório no update
  initialPessoa?: PessoaResponseDto; // opcional (se você já tem na lista)
  onClose: () => void;
  onSaved: () => void; // reload
};

export default function PessoaFormModal({
  open,
  mode,
  pessoaId,
  initialPessoa,
  onClose,
  onSaved,
}: Props) {
  const [nome, setNome] = useState("");
  const [idade, setIdade] = useState<string>("");

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [alert, setAlert] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const idadeNumber = useMemo(() => {
    const n = Number(idade);
    return Number.isFinite(n) ? n : NaN;
  }, [idade]);

  function validate(): string | null {
    const nomeOk = nome.trim().length > 0;
    const idadeOk = idade.trim().length > 0 && Number.isFinite(idadeNumber) && idadeNumber >= 0;

    if (!nomeOk && !idadeOk) return "Preencha Nome e Idade corretamente.";
    if (!nomeOk) return "Preencha o Nome.";
    if (!idadeOk) return "Preencha a Idade com um número válido (0 ou maior).";
    return null;
  }

  function handleIdadeChange(value: string) {
    if (value === "" || /^\d+$/.test(value)) {
      setIdade(value);
      setAlert(null);
      setError(null);
    }
  }

  // ✅ quando abrir no modo update, preencher campos
  useEffect(() => {
    if (!open) return;

    setError(null);
    setAlert(null);

    if (mode === "create") {
      setNome("");
      setIdade("");
      return;
    }

    // update:
    const id = pessoaId ?? initialPessoa?.id;
    if (!id) {
      setError("PessoaId não informado para atualização.");
      return;
    }

    // Se já veio da lista, preenche sem chamar GET
    if (initialPessoa) {
      setNome(initialPessoa.nome ?? "");
      setIdade(String(initialPessoa.idade ?? ""));
      return;
    }

    // Opcional: buscar no backend
    (async () => {
      try {
        setLoading(true);
        const p = await getPessoaById(id);
        setNome(p.nome ?? "");
        setIdade(String(p.idade ?? ""));
      } catch (e) {
        setError(e instanceof Error ? e.message : "Erro ao carregar pessoa");
      } finally {
        setLoading(false);
      }
    })();
  }, [open, mode, pessoaId, initialPessoa]);

  async function onSubmit() {
    if (saving) return;

    setError(null);
    setAlert(null);

    const validationMessage = validate();
    if (validationMessage) {
      setAlert(validationMessage);
      return;
    }

    try {
      setSaving(true);

      if (mode === "create") {
        const payload: PessoaCreateDto = { nome: nome.trim(), idade: idadeNumber };
        await criarPessoa(payload);
      } else {
        const id = pessoaId ?? initialPessoa?.id;
        if (!id) {
          setError("PessoaId não informado para atualização.");
          return;
        }
        const payload: PessoaUpdateDto = { nome: nome.trim(), idade: idadeNumber };
        await updatePessoa(id, payload);
      }

      onClose();
      onSaved();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Erro ao salvar pessoa");
    } finally {
      setSaving(false);
    }
  }

  function onCancel() {
    if (saving) return;
    setError(null);
    setAlert(null);
    onClose();
  }

  const title = mode === "create" ? "Adicionar pessoa" : "Editar pessoa";

  return (
    <Modal
      open={open}
      title={title}
      onClose={onCancel}
      footer={
        <>
          <button className="btn" onClick={onCancel} disabled={saving}>
            Cancelar
          </button>
          <button className="btn primary" onClick={onSubmit} disabled={saving || loading}>
            {loading ? "Carregando..." : saving ? "Salvando..." : "Salvar"}
          </button>
        </>
      }
    >
      <div style={{ display: "grid", gap: 12 }}>
        {alert && (
          <div
            style={{
              background: "rgba(255, 80, 80, 0.12)",
              border: "1px solid rgba(255, 80, 80, 0.35)",
              color: "#ffb4b4",
              padding: "10px 12px",
              borderRadius: 10,
              fontSize: 13,
            }}
          >
            {alert}
          </div>
        )}

        <div className="field">
          <label>Nome</label>
          <input
            value={nome}
            onChange={(e) => {
              setNome(e.target.value);
              setAlert(null);
              setError(null);
            }}
            placeholder="Ex: Gabriel"
            autoFocus
            disabled={saving || loading}
          />
        </div>

        <div className="field">
          <label>Idade</label>
          <input
            value={idade}
            onChange={(e) => handleIdadeChange(e.target.value)}
            inputMode="numeric"
            placeholder="Ex: 18"
            disabled={saving || loading}
          />
        </div>

        {error && <div style={{ color: "#ffb4b4", fontSize: 13 }}>{error}</div>}
      </div>
    </Modal>
  );
}
