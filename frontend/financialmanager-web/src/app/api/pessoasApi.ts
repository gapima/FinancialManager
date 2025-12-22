export type PessoaResponseDto = {
  id: number;
  nome: string;
  idade: number;
};

export type PessoaCreateDto = {
  nome: string;
  idade: number;
};

export type PessoaUpdateDto = {
  nome: string;
  idade: number;
};

const BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:7279";

async function parseError(res: Response) {
  const text = await res.text().catch(() => "");
  return text || `Erro HTTP ${res.status}`;
}

export async function listarPessoas(): Promise<PessoaResponseDto[]> {
  const res = await fetch(`${BASE_URL}/api/Pessoa`, {
    headers: { accept: "text/plain" },
  });
  if (!res.ok) throw new Error(await parseError(res));
  return res.json();
}

export async function criarPessoa(dto: PessoaCreateDto): Promise<PessoaResponseDto> {
  const res = await fetch(`${BASE_URL}/api/Pessoa`, {
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

export async function getPessoaById(id: number): Promise<PessoaResponseDto> {
  const res = await fetch(`${BASE_URL}/api/Pessoa/${id}`, {
    headers: { accept: "text/plain" },
  });
  if (!res.ok) throw new Error(await parseError(res));
  return res.json();
}

export async function updatePessoa(id: number, dto: PessoaUpdateDto): Promise<void> {
  const res = await fetch(`${BASE_URL}/api/Pessoa/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      accept: "*/*",
    },
    body: JSON.stringify(dto),
  });
  if (!res.ok) throw new Error(await parseError(res));
}
