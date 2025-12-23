import { useEffect, useMemo, useState } from "react";
import Modal from "../Modal/BaseModal"; // ajuste pro seu modal base
import { listarPessoas, type PessoaResponseDto } from "../../api/pessoasApi";
import { listarCategorias, type CategoryResponseDto } from "../../api/categoryApi";
import { criarTransacao, type TransactionCreateDto } from "../../api/transacoesApi";

type Props = {
  open: boolean;
  onClose: () => void;
  onCreated: () => void;
};

function finalidadeLabel(purpose: number) {
  switch (purpose) {
    case 1:
      return "Receita";
    case 2:
      return "Despesa";
    case 3:
      return "Ambos";
    default:
      return `(${purpose})`;
  }
}

function normalizeMoneyInput(raw: string) {
  // deixa só números, vírgula e ponto
  return raw.replace(/[^\d.,]/g, "");
}

function parseMoneyToNumber(amount: string) {
  // "1.234,56" -> "1234.56" | "1234.56" fica
  const normalized = amount.replace(/\./g, "").replace(",", ".");
  const n = Number(normalized);
  return Number.isFinite(n) ? n : NaN;
}

export default function TransactionFormModal({ open, onClose, onCreated }: Props) {
  const [pessoas, setPessoas] = useState<PessoaResponseDto[]>([]);
  const [cats, setCats] = useState<CategoryResponseDto[]>([]);

  const [pessoaId, setPessoaId] = useState<string>("");
  const [categoryId, setCategoryId] = useState<string>("");

  const [description, setDescription] = useState("");
  const [amount, setAmount] = useState(""); // string
  const [type, setType] = useState<string>(""); // "1" | "2"

  const [loadingRefs, setLoadingRefs] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const pessoaIdNum = useMemo(() => Number(pessoaId), [pessoaId]);
  const categoryIdNum = useMemo(() => Number(categoryId), [categoryId]);

  const step1Ok =
    Number.isFinite(pessoaIdNum) &&
    pessoaIdNum > 0 &&
    Number.isFinite(categoryIdNum) &&
    categoryIdNum > 0;

  const amountNumber = useMemo(() => parseMoneyToNumber(amount), [amount]);

  const canSave =
    step1Ok &&
    description.trim().length > 0 &&
    Number.isFinite(amountNumber) &&
    amountNumber > 0 &&
    (type === "1" || type === "2") &&
    !saving;

  useEffect(() => {
    if (!open) return;

    async function loadRefs() {
      try {
        setLoadingRefs(true);
        setError(null);

        const [ps, cs] = await Promise.all([listarPessoas(), listarCategorias()]);
        setPessoas(ps);
        setCats(cs);
      } catch (e) {
        setError(e instanceof Error ? e.message : "Erro ao carregar pessoas/categorias");
      } finally {
        setLoadingRefs(false);
      }
    }

    void loadRefs();
  }, [open]);

  function resetForm() {
    setPessoaId("");
    setCategoryId("");
    setDescription("");
    setAmount("");
    setType("");
    setError(null);
    setSaving(false);
  }

  function onCancel() {
    if (saving) return;
    resetForm();
    onClose();
  }

  function validateAndGetError(): string | null {
    if (!step1Ok) return "Selecione uma Pessoa e uma Categoria.";
    if (description.trim().length === 0) return "Descrição é obrigatória.";
    if (!Number.isFinite(amountNumber)) return "Valor inválido.";
    if (amountNumber <= 0) return "Valor precisa ser maior que zero.";
    if (type !== "1" && type !== "2") return "Selecione o tipo (Receita/Despesa).";
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

      const payload: TransactionCreateDto = {
        description: description.trim(),
        amount: normalizeMoneyInput(amount), // envia string
        type: Number(type),
        categoryId: categoryIdNum,
        pessoaId: pessoaIdNum,
      };

      await criarTransacao(payload);

      resetForm();
      onClose();
      onCreated();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Erro ao criar transação");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Modal
      open={open}
      title="Nova transação"
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
        {/* STEP 1 */}
        <div style={{ display: "grid", gap: 12, gridTemplateColumns: "1fr 1fr" }}>
          <div className="field">
            <label>Pessoa</label>
            <select
              value={pessoaId}
              onChange={(e) => setPessoaId(e.target.value)}
              disabled={saving || loadingRefs}
            >
              <option value="">Selecione...</option>
              {pessoas.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.nome} (#{p.id})
                </option>
              ))}
            </select>
          </div>

          <div className="field">
            <label>Categoria</label>
            <select
              value={categoryId}
              onChange={(e) => setCategoryId(e.target.value)}
              disabled={saving || loadingRefs}
            >
              <option value="">Selecione...</option>
              {cats.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.description} — {finalidadeLabel(c.purpose)} (#{c.id})
                </option>
              ))}
            </select>
          </div>
        </div>

        <div style={{ fontSize: 12, opacity: 0.8 }}>
          {step1Ok ? "✅ Seleção ok. Preencha os dados da transação." : "⬆️ Selecione Pessoa e Categoria para liberar os campos."}
        </div>

        {/* STEP 2 */}
        <div style={{ display: "grid", gap: 12, opacity: step1Ok ? 1 : 0.5 }}>
          <div className="field">
            <label>Descrição</label>
            <input
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder='Ex: "Salário", "Aluguel", "Mercado"...'
              disabled={!step1Ok || saving}
            />
          </div>

          <div style={{ display: "grid", gap: 12, gridTemplateColumns: "1fr 1fr" }}>
            <div className="field">
              <label>Valor</label>
              <input
                value={amount}
                onChange={(e) => setAmount(normalizeMoneyInput(e.target.value))}
                placeholder="Ex: 1000,00"
                inputMode="decimal"
                disabled={!step1Ok || saving}
              />
              <div style={{ fontSize: 12, opacity: 0.8, marginTop: 6 }}>
                * Aceita vírgula ou ponto. Enviamos como string pro backend.
              </div>
            </div>

            <div className="field">
              <label>Tipo</label>
              <select value={type} onChange={(e) => setType(e.target.value)} disabled={!step1Ok || saving}>
                <option value="">Selecione...</option>
                <option value="1">Receita</option>
                <option value="2">Despesa</option>
              </select>
            </div>
          </div>
        </div>

        {error && <div style={{ color: "#ffb4b4", fontSize: 13 }}>{error}</div>}
      </div>
    </Modal>
  );
}
