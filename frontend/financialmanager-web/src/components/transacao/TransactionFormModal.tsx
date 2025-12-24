import { useEffect, useMemo, useState } from "react";
import Modal from "../Modal/BaseModal";
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
  return raw.replace(/[^\d.,]/g, "");
}

function parseMoneyToNumber(amount: string) {
  const normalized = amount.replace(/\./g, "").replace(",", ".");
  const n = Number(normalized);
  return Number.isFinite(n) ? n : NaN;
}

function categoryAllowsType(categoryPurpose: number, type: number) {
  // type: 1=Receita, 2=Despesa
  if (categoryPurpose === 3) return true; // ambos
  if (type === 1) return categoryPurpose === 1;
  if (type === 2) return categoryPurpose === 2;
  return false;
}

export default function TransactionFormModal({ open, onClose, onCreated }: Props) {
  const [pessoas, setPessoas] = useState<PessoaResponseDto[]>([]);
  const [cats, setCats] = useState<CategoryResponseDto[]>([]);

  const [pessoaId, setPessoaId] = useState<string>("");
  const [categoryId, setCategoryId] = useState<string>("");

  const [description, setDescription] = useState("");
  const [amount, setAmount] = useState("");
  const [type, setType] = useState<string>(""); // "1" | "2"

  const [loadingRefs, setLoadingRefs] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const pessoaIdNum = useMemo(() => Number(pessoaId), [pessoaId]);
  const categoryIdNum = useMemo(() => Number(categoryId), [categoryId]);

  const selectedPessoa = useMemo(() => {
    if (!Number.isFinite(pessoaIdNum) || pessoaIdNum <= 0) return null;
    return pessoas.find((p) => p.id === pessoaIdNum) ?? null;
  }, [pessoaIdNum, pessoas]);

  const isMenor18 = (selectedPessoa?.idade ?? 999) < 18;

  const selectedCategory = useMemo(() => {
    if (!Number.isFinite(categoryIdNum) || categoryIdNum <= 0) return null;
    return cats.find((c) => c.id === categoryIdNum) ?? null;
  }, [categoryIdNum, cats]);

  // categorias exibidas conforme regra:
  const availableCategories = useMemo(() => {
    if (!isMenor18) return cats;
    // Menor de 18: somente Despesa (2) ou Ambos (3)
    return cats.filter((c) => c.purpose === 2 || c.purpose === 3);
  }, [cats, isMenor18]);

  // ======= Regra 1: menor de 18 trava Receita
  useEffect(() => {
    if (!open) return;

    // se menor, força tipo despesa
    if (isMenor18 && type !== "2") setType("2");

    // se menor e a categoria atual não serve, limpa
    if (isMenor18 && Number.isFinite(categoryIdNum) && categoryIdNum > 0) {
      const cat = cats.find((c) => c.id === categoryIdNum);
      if (cat && !(cat.purpose === 2 || cat.purpose === 3)) {
        setCategoryId("");
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isMenor18, open]);

  // ======= Regra 2: categoria define o tipo (respeitando menoridade)
  useEffect(() => {
    if (!open) return;
    if (!selectedCategory) return;

    // Menor de 18: sempre despesa
    if (isMenor18) {
      if (type !== "2") setType("2");
      return;
    }

    // Maior/igual 18: categoria receita/despesa auto-seta
    if (selectedCategory.purpose === 1) {
      if (type !== "1") setType("1");
      return;
    }
    if (selectedCategory.purpose === 2) {
      if (type !== "2") setType("2");
      return;
    }

    // purpose === 3 (Ambos): deixa escolher
    // mas se o type atual estiver inválido (ex: ""), mantém vazio e usuário escolhe.
    if (type === "1" || type === "2") return;
    // se quiser, pode deixar vazio mesmo:
    // setType("");
  }, [selectedCategory, isMenor18, open]); // não coloque type aqui pra não ficar "brigando" com o usuário

  // se o tipo mudar (adulto) garante que a categoria seja compatível
  useEffect(() => {
    if (!open) return;

    if (!Number.isFinite(categoryIdNum) || categoryIdNum <= 0) return;
    if (type !== "1" && type !== "2") return;

    const cat = cats.find((c) => c.id === categoryIdNum);
    if (!cat) return;

    // Menor de 18: já travamos despesa, então só garante aqui também
    if (isMenor18 && type === "1") {
      setType("2");
      return;
    }

    if (!categoryAllowsType(cat.purpose, Number(type))) {
      // para "Ambos" sempre passa. Para "Receita/Despesa" limpa a categoria ou ajusta tipo:
      // preferi ajustar tipo automaticamente (melhor UX):
      if (!isMenor18) {
        if (cat.purpose === 1) setType("1");
        else if (cat.purpose === 2) setType("2");
        else setCategoryId("");
      } else {
        setCategoryId("");
      }
    }
  }, [type, categoryIdNum, cats, open, isMenor18]);

  const step1Ok =
    Number.isFinite(pessoaIdNum) &&
    pessoaIdNum > 0 &&
    Number.isFinite(categoryIdNum) &&
    categoryIdNum > 0;

  const amountNumber = useMemo(() => parseMoneyToNumber(amount), [amount]);

  // ======= Tipo disponível / trava do select Tipo
  const tipoOptions = useMemo(() => {
    // se ainda não tem categoria, pode mostrar os dois (mas menor só despesa)
    if (!selectedCategory) {
      return isMenor18 ? [{ value: "2", label: "Despesa" }] : [
        { value: "1", label: "Receita" },
        { value: "2", label: "Despesa" },
      ];
    }

    // menor 18: só despesa
    if (isMenor18) return [{ value: "2", label: "Despesa" }];

    // categoria define
    if (selectedCategory.purpose === 1) return [{ value: "1", label: "Receita" }];
    if (selectedCategory.purpose === 2) return [{ value: "2", label: "Despesa" }];
    return [
      { value: "1", label: "Receita" },
      { value: "2", label: "Despesa" },
    ];
  }, [selectedCategory, isMenor18]);

  const isTipoLocked = useMemo(() => {
    if (isMenor18) return true;
    if (!selectedCategory) return false;
    return selectedCategory.purpose === 1 || selectedCategory.purpose === 2; // receita/despesa travam
  }, [isMenor18, selectedCategory]);

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
    if (!selectedPessoa) return "Selecione uma Pessoa.";
    if (!Number.isFinite(categoryIdNum) || categoryIdNum <= 0) return "Selecione uma Categoria.";

    if (isMenor18 && type === "1") {
      return "Pessoa menor de 18 anos não pode cadastrar Receita. Apenas Despesa.";
    }

    if (description.trim().length === 0) return "Descrição é obrigatória.";
    if (!Number.isFinite(amountNumber)) return "Valor inválido.";
    if (amountNumber <= 0) return "Valor precisa ser maior que zero.";
    if (type !== "1" && type !== "2") return "Selecione o tipo (Receita/Despesa).";

    const cat = cats.find((c) => c.id === categoryIdNum);
    if (!cat) return "Categoria inválida.";

    if (!categoryAllowsType(cat.purpose, Number(type))) {
      return `Categoria "${cat.description}" não é compatível com o tipo ${
        type === "1" ? "Receita" : "Despesa"
      }.`;
    }

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
        amount: normalizeMoneyInput(amount),
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
              onChange={(e) => {
                setPessoaId(e.target.value);
                setError(null);

                // reset ao trocar pessoa
                setCategoryId("");
                setType("");
              }}
              disabled={saving || loadingRefs}
            >
              <option value="">Selecione...</option>
              {pessoas.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.nome} (#{p.id}) — {p.idade} anos
                </option>
              ))}
            </select>
          </div>

          <div className="field">
            <label>Categoria</label>
            <select
              value={categoryId}
              onChange={(e) => {
                setCategoryId(e.target.value);
                setError(null);
              }}
              disabled={saving || loadingRefs || !Number.isFinite(pessoaIdNum) || pessoaIdNum <= 0}
            >
              <option value="">Selecione...</option>
              {availableCategories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.description} — {finalidadeLabel(c.purpose)} (#{c.id})
                </option>
              ))}
            </select>

            {isMenor18 && (
              <div style={{ fontSize: 12, opacity: 0.8, marginTop: 6 }}>
                ⚠️ Menor de 18: apenas categorias de despesa (ou ambos) e tipo Despesa.
              </div>
            )}
          </div>
        </div>

        <div style={{ fontSize: 12, opacity: 0.8 }}>
          {step1Ok
            ? "✅ Seleção ok. Preencha os dados da transação."
            : "⬆️ Selecione Pessoa e Categoria para liberar os campos."}
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
              <select
                value={type}
                onChange={(e) => setType(e.target.value)}
                disabled={!step1Ok || saving || isTipoLocked}
                title={
                  !step1Ok
                    ? "Selecione pessoa e categoria"
                    : isMenor18
                    ? "Menor de 18: apenas Despesa"
                    : selectedCategory?.purpose === 1
                    ? "Categoria de Receita: tipo travado"
                    : selectedCategory?.purpose === 2
                    ? "Categoria de Despesa: tipo travado"
                    : undefined
                }
              >
                <option value="">Selecione...</option>
                {tipoOptions.map((o) => (
                  <option key={o.value} value={o.value}>
                    {o.label}
                  </option>
                ))}
              </select>

              {step1Ok && isTipoLocked && (
                <div style={{ fontSize: 12, opacity: 0.8, marginTop: 6 }}>
                  ✅ Tipo travado pela regra{" "}
                  {isMenor18
                    ? "de menoridade"
                    : selectedCategory?.purpose === 1
                    ? "da categoria (Receita)"
                    : selectedCategory?.purpose === 2
                    ? "da categoria (Despesa)"
                    : ""}
                  .
                </div>
              )}

              {step1Ok && !isTipoLocked && selectedCategory?.purpose === 3 && !isMenor18 && (
                <div style={{ fontSize: 12, opacity: 0.8, marginTop: 6 }}>
                  ℹ️ Categoria “Ambos”: escolha Receita ou Despesa.
                </div>
              )}
            </div>
          </div>
        </div>

        {error && <div style={{ color: "#ffb4b4", fontSize: 13 }}>{error}</div>}
      </div>
    </Modal>
  );
}
