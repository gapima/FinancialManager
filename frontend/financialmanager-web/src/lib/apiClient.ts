const BASE_URL = import.meta.env.VITE_API_URL as string | undefined;

if (!BASE_URL) {
  // eslint-disable-next-line no-console
  console.warn("VITE_API_URL não definido. Crie o arquivo .env com VITE_API_URL=...");
}

type ApiError = {
  title?: string;
  detail?: string;
  status?: number;
};

async function readError(res: Response) {
  try {
    const data = (await res.json()) as ApiError;
    return data?.detail || data?.title || `Erro HTTP ${res.status}`;
  } catch {
    // às vezes a API devolve text/plain
    try {
      const txt = await res.text();
      return txt || `Erro HTTP ${res.status}`;
    } catch {
      return `Erro HTTP ${res.status}`;
    }
  }
}

export async function apiGet<T>(path: string): Promise<T> {
  const url = `${BASE_URL}${path}`;
  const res = await fetch(url, {
    method: "GET",
    headers: { Accept: "application/json" },
  });

  if (!res.ok) throw new Error(await readError(res));
  return (await res.json()) as T;
}

export async function apiPost<TResponse, TBody>(
  path: string,
  body: TBody
): Promise<TResponse> {
  const url = `${BASE_URL}${path}`;
  const res = await fetch(url, {
    method: "POST",
    headers: {
      Accept: "text/plain, application/json",
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  });

  if (!res.ok) throw new Error(await readError(res));

  // Pode ser que a API retorne text/plain
  const contentType = res.headers.get("content-type") || "";
  if (contentType.includes("application/json")) {
    return (await res.json()) as TResponse;
  }

  // fallback: tenta parsear JSON mesmo se header veio errado
  const txt = await res.text();
  try {
    return JSON.parse(txt) as TResponse;
  } catch {
    // @ts-expect-error fallback quando API não retorna objeto
    return txt as TResponse;
  }
}
