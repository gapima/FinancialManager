import { apiPost, apiGet } from "../lib/apiClient";

export type PessoaResponseDto = { id: number; nome: string; idade: number };
export type PessoaCreateDto = { nome: string; idade: number };

export async function listarPessoas(): Promise<PessoaResponseDto[]> {
  return apiGet("/api/Pessoa");
}

export async function criarPessoa(input: PessoaCreateDto): Promise<PessoaResponseDto> {
  return apiPost("/api/Pessoa", input);
}
