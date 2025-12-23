import { useEffect, useMemo, useState } from "react";
import Modal from "../Modal/BaseModal";
import {
  criarCategoria,
  updateCategoria,
  type CategoryCreateDto,
  type CategoryResponseDto,
  type CategoryUpdateDto,
} from "../../api/categoryApi";

type Mode = "create" | "edit";

type Props = {
  open: boolean;
  onClose: () => void;
  onSaved: () => void; // reload na page
  mode: Mode;
  initialCategory?: CategoryResponseDto | null;
};

type FinalidadeOption = { value: 1 | 2 | 3; label: string };

const FINALIDADES: FinalidadeOption[] = [
  { value: 1, label: "Receita" },
  { value: 2, label: "Despesa" },
  { value: 3, label: "Ambos" },
];

export default function CategoryFormModal({
  open,
  onClose,
  onSaved,
  mode,
  initialCategory,
}: Props) {
  const [description, setDescription] = useState("");
  const [finalidade, setFinalidade] = useState<"1" | "2" | "3">("1");

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const title = mode === "edit" ? "Editar categoria" : "Adicionar categoria";

  // quando abrir em modo edit, preenche o form
  useEffect(() => {
    if (!open) return;

    setError(null);

    if (mode === "edit" && initialCategory) {
      setDescription(initialCategory.description ?? "");
      const p = initialCategory.purpose;
      setFinalidade(p === 2 ? "2" : p === 3 ? "3" : "1");
    } else {
      // create
      setDescription("");
      setFinalidade("1");
    }
  }, [open, mode, initialCategory]);

  const purposeNumber = useMemo(() => Number(finalidade), [finalidade]);

  const canSave = description.trim().length > 0 && !saving;

  function validateAndGetError(): string | null {
    if (description.trim().length === 0) return "Descrição é obrigatória.";
    if (![1, 2, 3].includes(purposeNumber)) return "Finalidade inválida.";
    if (mode === "edit" && (!initialCategory || initialCategory.id <= 0))
      return "Categoria inválida para edição.";
    return null;
  }

  async function onSubmit() {
    const validationError = validateAndGetError();
    if (validationError) {
      setError(validationError);
      return;
    }

    try {
      setSaving(true);
      setError(null);

      const payloadCreate: CategoryCreateDto = {
        description: description.trim(),
        purpose: purposeNumber,
      };

      if (mode === "create") {
        await criarCategoria(payloadCreate);
      } else {
        const id = initialCategory!.id;
        const payloadUpdate: CategoryUpdateDto = payloadCreate;
        await updateCategoria(id, payloadUpdate);
      }

      onClose();
      onSaved();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Erro ao salvar categoria");
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
      title={title}
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
          <label>Descrição</label>
          <input
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder='Ex: "Cartão de crédito"'
            autoFocus
            disabled={saving}
          />
        </div>

        <div className="field">
          <label>Finalidade</label>
          <select
            value={finalidade}
            onChange={(e) => setFinalidade(e.target.value as "1" | "2" | "3")}
            disabled={saving}
          >
            {FINALIDADES.map((f) => (
              <option key={f.value} value={String(f.value)}>
                {f.label}
              </option>
            ))}
          </select>
        </div>

        {error && <div style={{ color: "#ffb4b4", fontSize: 13 }}>{error}</div>}
      </div>
    </Modal>
  );
}
