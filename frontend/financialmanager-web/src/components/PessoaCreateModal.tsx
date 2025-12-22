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

  // erro "da API" (ex.: 500, validação no back, etc.)
  const [error, setError] = useState<string | null>(null);

  // alerta "do form" (campos não preenchidos/ inválidos)
  const [alert, setAlert] = useState<string | null>(null);

  const idadeNumber = useMemo(() => {
    const n = Number(idade);
    return Number.isFinite(n) ? n : NaN;
  }, [idade]);

  const canSave =
    nome.trim().length > 0 && Number.isFinite(idadeNumber) && idadeNumber >= 0 && !saving;

  function validate(): string | null {
    const nomeOk = nome.trim().length > 0;
    const idadeOk = idade.trim().length > 0 && Number.isFinite(idadeNumber) && idadeNumber >= 0;

    if (!nomeOk && !idadeOk) return "Preencha Nome e Idade corretamente.";
    if (!nomeOk) return "Preencha o Nome.";
    if (!idadeOk) return "Preencha a Idade com um número válido (0 ou maior).";
    return null;
  }

  async function onSubmit() {
    if (saving) return;

    // limpa mensagens anteriores
    setError(null);
    setAlert(null);

    const validationMessage = validate();
    if (validationMessage) {
      setAlert(validationMessage);
      return; // ⛔ não chama a API
    }

    try {
      setSaving(true);

      const payload: PessoaCreateDto = {
        nome: nome.trim(),
        idade: idadeNumber,
      };

      await criarPessoa(payload);

      // limpa e fecha
      setNome("");
      setIdade("");
      setAlert(null);
      setError(null);

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
    setAlert(null);
    onClose();
  }

  function handleIdadeChange(value: string) {
    // permite vazio (pra usuário apagar) ou dígitos apenas
    if (value === "" || /^\d+$/.test(value)) {
      setIdade(value);
      setAlert(null);
      setError(null);
    }
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
          {/* ✅ botão clicável pra poder mostrar alerta; só trava quando está salvando */}
          <button className="btn primary" onClick={onSubmit} disabled={saving}>
            {saving ? "Salvando..." : "Salvar"}
          </button>
        </>
      }
    >
      <div style={{ display: "grid", gap: 12 }}>
        {/* ✅ ALERTA DE VALIDAÇÃO DO FORM (ANTES DOS INPUTS) */}
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
            disabled={saving}
          />
        </div>

        <div className="field">
          <label>Idade</label>
          <input
            value={idade}
            onChange={(e) => handleIdadeChange(e.target.value)}
            inputMode="numeric"
            placeholder="Ex: 18"
            disabled={saving}
          />
        </div>

        {/* ✅ ERRO DA API (FICA EMBAIXO) */}
        {error && <div style={{ color: "#ffb4b4", fontSize: 13 }}>{error}</div>}

        {/* opcional: dica visual do canSave (se quiser) */}
        {/* {!canSave && !saving && <div style={{ color: "#aaa", fontSize: 12 }}>Preencha Nome e Idade para salvar.</div>} */}
      </div>
    </Modal>
  );
}
