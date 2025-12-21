export type PessoaTotais = {
  pessoaId: number;
  nome: string;
  totalReceitas: number;
  totalDespesas: number;
};

export type DashboardResumo = {
  itens: PessoaTotais[];
};
