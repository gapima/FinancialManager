export type TotaisPorPessoaItemDto = {
  pessoaId: number;
  pessoaNome: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
};

export type TotaisPorPessoaDto = {
  items: TotaisPorPessoaItemDto[];
  totalReceitas: number;
  totalDespesas: number;
  saldoLiquido: number;
};

const BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:7279";

async function parseError(res: Response) {
  const text = await res.text().catch(() => "");
  return text || `Erro HTTP ${res.status}`;
}

export async function getTotaisPorPessoa(): Promise<TotaisPorPessoaDto> {
  const res = await fetch(`${BASE_URL}/api/Dashboard/totais-por-pessoa`, {
    headers: { accept: "text/plain" },
  });

  if (!res.ok) throw new Error(await parseError(res));
  return res.json();
}
