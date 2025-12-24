const BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:7279";

async function parseError(res: Response) {
  const text = await res.text().catch(() => "");
  return text || `Erro HTTP ${res.status}`;
}

/** ===== Totais por Pessoa ===== */
export type TotaisPorPessoaItemDto = {
  pessoaId: number;
  pessoaNome: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
};

export type TotaisPorPessoaDto = {
  items: TotaisPorPessoaItemDto[];
  totalGeral: { totalReceitas: number; totalDespesas: number; saldo: number };

  totalReceitas?: number;
  totalDespesas?: number;
  saldoLiquido?: number;
};

export async function getTotaisPorPessoa(): Promise<TotaisPorPessoaDto> {
  const res = await fetch(`${BASE_URL}/api/Dashboard/totais-por-pessoa`, {
    headers: { accept: "*/*" },
  });
  if (!res.ok) throw new Error(await parseError(res));
  return res.json();
}

/** ===== Totais por Categoria ===== */
export type TotaisPorCategoriaItemDto = {
  categoryId: number;
  categoryDescription: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
};

export type TotaisPorCategoriaDto = {
  items: TotaisPorCategoriaItemDto[];
  totalGeral: { totalReceitas: number; totalDespesas: number; saldo: number };
};

export async function getTotaisPorCategoria(): Promise<TotaisPorCategoriaDto> {
  const res = await fetch(`${BASE_URL}/api/Dashboard/totais-por-categoria`, {
    headers: { accept: "*/*" },
  });
  if (!res.ok) throw new Error(await parseError(res));
  return res.json();
}
