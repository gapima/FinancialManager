import { apiGet } from "../lib/apiClient";

export type PessoaResponseDto = {
  id: number;
  nome: string;
  idade: number;
};

export async function listarPessoas(): Promise<PessoaResponseDto[]> {
  // Ajuste aqui se sua rota for diferente
  return apiGet<PessoaResponseDto[]>("/api/pessoa");
}
