export type TransactionResponseDto = {
  id: number;
  description: string;
  amount: string;
  type: number;
  categoryId: number;
  pessoaId: number;
  createdAt: string;
};

export type TransactionCreateDto = {
  description: string;
  amount: string; // backend espera string
  type: number; // 1 receita / 2 despesa
  categoryId: number;
  pessoaId: number;
};

const BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:7279";

async function parseError(res: Response) {
  const text = await res.text().catch(() => "");
  return text || `Erro HTTP ${res.status}`;
}

function safeJsonParseLenient<T>(text: string): T {
  try {
    return JSON.parse(text) as T;
  } catch {
    const fixed = text.replace(/\]\s*\[/g, ",");
    const wrapped = `[${fixed}]`;
    const parsed = JSON.parse(wrapped) as unknown;

    if (Array.isArray(parsed) && parsed.every(Array.isArray)) {
      return (parsed as unknown[]).flat() as T;
    }
    return parsed as T;
  }
}

export async function listarTransacoes(): Promise<TransactionResponseDto[]> {
  const res = await fetch(`${BASE_URL}/api/Transaction`, {
    headers: { accept: "text/plain" },
  });
  if (!res.ok) throw new Error(await parseError(res));

  const text = await res.text();
  const data = safeJsonParseLenient<TransactionResponseDto[]>(text);

  const uniq = new Map<number, TransactionResponseDto>();
  for (const t of data) uniq.set(t.id, t);
  return Array.from(uniq.values());
}

export async function criarTransacao(dto: TransactionCreateDto): Promise<TransactionResponseDto> {
  const res = await fetch(`${BASE_URL}/api/Transaction`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      accept: "text/plain",
    },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await parseError(res));
  return res.json();
}
