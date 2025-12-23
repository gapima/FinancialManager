export type CategoryResponseDto = {
  id: number;
  description: string;
  purpose: number;
};

export type CategoryCreateDto = {
  description: string;
  purpose: number;
};

export type CategoryUpdateDto = {
  description: string;
  purpose: number;
};

const BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:7279";

async function parseError(res: Response) {
  const text = await res.text().catch(() => "");
  return text || `Erro HTTP ${res.status}`;
}

/* ============================
 * GET ALL
 * ============================ */
export async function listarCategorias(): Promise<CategoryResponseDto[]> {
  const res = await fetch(`${BASE_URL}/api/Category`, {
    headers: { accept: "text/plain" },
  });

  if (!res.ok) throw new Error(await parseError(res));
  return res.json();
}

/* ============================
 * CREATE
 * ============================ */
export async function criarCategoria(
  dto: CategoryCreateDto
): Promise<CategoryResponseDto> {
  const res = await fetch(`${BASE_URL}/api/Category`, {
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

/* ============================
 * GET BY ID
 * ============================ */
export async function getCategoriaById(
  id: number
): Promise<CategoryResponseDto> {
  const res = await fetch(`${BASE_URL}/api/Category/${id}`, {
    headers: { accept: "text/plain" },
  });

  if (!res.ok) throw new Error(await parseError(res));
  return res.json();
}

/* ============================
 * UPDATE
 * ============================ */
export async function updateCategoria(
  id: number,
  dto: CategoryUpdateDto
): Promise<void> {
  const res = await fetch(`${BASE_URL}/api/Category/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      accept: "*/*",
    },
    body: JSON.stringify(dto),
  });

  if (!res.ok) throw new Error(await parseError(res));
}

/* ============================
 * DELETE
 * ============================ */
export async function deletarCategoria(id: number): Promise<void> {
  const res = await fetch(`${BASE_URL}/api/Category/${id}`, {
    method: "DELETE",
    headers: {
      accept: "*/*",
    },
  });

  if (!res.ok) {
    const txt = await res.text().catch(() => "");
    throw new Error(txt || `Erro ao excluir categoria (HTTP ${res.status})`);
  }
}
