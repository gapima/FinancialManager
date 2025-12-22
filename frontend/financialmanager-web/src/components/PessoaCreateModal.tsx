import { useMemo, useState } from "react";
import Modal from "./ModalPessoa";
import { criarPessoa, type PessoaCreateDto } from "../app/api/pessoasApi";

type Props = {
  open: boolean;
  onClose: () => void;
  onCreated: () => void; // chama reload na page
};

export default function PessoaCreateModal({ open, onClose, onCreated }: Props) {
  const [nome, setNome] = useState("");
  const [idade, setIdade] = useState<string>("");

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const idadeNumber = useMemo(() => {
    const n = Number(idade);
    return Number.isFinite(n) ? n : NaN;
  }, [idade]);

  const canSave =
    nome.trim().length > 0 && Number.isFinite(idadeNumber) && idadeNumber >= 0 && !saving;

  async function onSubmit() {
    try {
      setSaving(true);
      setError(null);

      const payload: PessoaCreateDto = {
        nome: nome.trim(),
        idade: idadeNumber,
      };

      await criarPessoa(payload);

      // limpa e fecha
      setNome("");
      setIdade("");
      onClose();
      onCreated();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Erro ao criar pessoa");
    } finally {
      setSaving(false);
    }
  }

  function onCancel() {
    if (saving) return;
    setError(null);
    onClose();
  }

  return (
    <Modal
      open={open}
      title="Adicionar pessoa"
      onClose={onCancel}
      footer={
        <>
          <button className="btn" onClick={onCancel} disabled={saving}>
            Cancelar
          </button>
          <button className="btn primary" onClick={onSubmit} disabled={!canSave}>
            {saving ? "Salvando..." : "Salvar"}
          </button>
        </>
      }
    >
      <div style={{ display: "grid", gap: 12 }}>
        <div className="field">
          <label>Nome</label>
          <input
            value={nome}
            onChange={(e) => setNome(e.target.value)}
            placeholder="Ex: Gabriel"
            autoFocus
            disabled={saving}
          />
        </div>

        <div className="field">
          <label>Idade</label>
          <input
            value={idade}
            onChange={(e) => setIdade(e.target.value)}
            placeholder="Ex: 18"
            inputMode="numeric"
            disabled={saving}
          />
        </div>

        {error && (
          <div style={{ color: "#ffb4b4", fontSize: 13 }}>
            {error}
          </div>
        )}
      </div>
    </Modal>
  );
}
