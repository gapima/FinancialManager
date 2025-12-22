import { apiGet, apiPost } from "../lib/apiClient";

export type PessoaResponseDto = {
  id: number;
  nome: string;
  idade: number;
};

export type PessoaCreateDto = {
  nome: string;
  idade: number;
};

export async function listarPessoas(): Promise<PessoaResponseDto[]> {
  return apiGet<PessoaResponseDto[]>("/api/Pessoa");
}

export async function criarPessoa(dto: PessoaCreateDto): Promise<PessoaResponseDto> {
  return apiPost<PessoaResponseDto, PessoaCreateDto>("/api/Pessoa", dto);
}
