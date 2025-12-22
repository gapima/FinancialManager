const BASE_URL = import.meta.env.VITE_API_URL as string | undefined;

if (!BASE_URL) {
  // Ajuda a debugar rapidão quando esquecer o .env
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
    return `Erro HTTP ${res.status}`;
  }
}

export async function apiGet<T>(path: string): Promise<T> {
  const url = `${BASE_URL}${path}`;
  const res = await fetch(url, {
    method: "GET",
    headers: { Accept: "application/json" },
  });

  if (!res.ok) {
    throw new Error(await readError(res));
  }

  return (await res.json()) as T;
}
