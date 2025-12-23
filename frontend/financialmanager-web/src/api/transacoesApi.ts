export type TransactionResponseDto = {
  id: number;
  description: string;
  amount: string; // vem string do backend
  type: number; // 1,2 etc
  categoryId: number;
  pessoaId: number;
  createdAt: string; // ISO
};

const BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:7279";

async function parseError(res: Response) {
  const text = await res.text().catch(() => "");
  return text || `Erro HTTP ${res.status}`;
}

/**
 * Caso a API venha bugada com "[...][...]" (JSON inválido),
 * tenta consertar e dar flatten.
 */
function safeJsonParseLenient<T>(text: string): T {
  try {
    return JSON.parse(text) as T;
  } catch {
    // tenta corrigir o caso: [..][..]
    const fixed = text.replace(/\]\s*\[/g, ",");
    // se virou ",", vamos envolver como array único
    // Ex: [a][b] -> [a,b] (mas a e b são arrays), então vira [[...],[...]]
    const wrapped = `[${fixed}]`;
    const parsed = JSON.parse(wrapped) as unknown;

    // se vier como array de arrays, flatten
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

  // se duplicou, remove ids repetidos (opcional, mas ajuda)
  const uniq = new Map<number, TransactionResponseDto>();
  for (const t of data) uniq.set(t.id, t);
  return Array.from(uniq.values());
}
