export type ApiEnvelope<T> = {
  success: boolean;
  data?: T;
  error?: string;
};

export class ApiError extends Error {
  status: number;

  constructor(message: string, status: number) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

export async function apiFetch<T>(
  path: string,
  init: RequestInit = {},
): Promise<T> {
  const headers = new Headers(init.headers);
  headers.set("Content-Type", "application/json");
  if (typeof window !== "undefined") {
    const token = window.localStorage.getItem("bingehos.accessToken");
    if (token) headers.set("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(`/api${path}`, {
    ...init,
    headers,
  });
  const body = (await response.json().catch(() => ({}))) as ApiEnvelope<T>;

  if (response.status === 401 && typeof window !== "undefined") {
    window.localStorage.removeItem("bingehos.accessToken");
    window.localStorage.removeItem("bingehos.user");
    window.location.assign("/login");
  }

  if (!response.ok || body.success === false) {
    throw new ApiError(body.error || "İstek tamamlanamadı.", response.status);
  }

  return (body.data ?? body) as T;
}

export function queryPath(
  path: string,
  params: Record<string, string | number | boolean | undefined>,
) {
  const query = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== "") query.set(key, String(value));
  });
  const suffix = query.toString();
  return suffix ? `${path}?${suffix}` : path;
}
