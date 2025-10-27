export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

export class ApiError extends Error {
    public status: number;
    public details?: unknown;

    constructor(message: string, status = 0, details?: unknown) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.details = details;
    }
}

const DEFAULT_TIMEOUT = 15000;

function buildUrl(path: string) {
    const base = import.meta.env.VITE_API_BASE_URL ?? '';

    return `${base.replace(/\/$/, '')}/${path.replace(/^\//, '')}`;
}

export type RequestOptions = {
    method?: HttpMethod;
    body?: unknown;
    signal?: AbortSignal;
    timeoutMs?: number;
    headers?: Record<string, string>;
    // hook to add auth token or other dynamic headers
    getAuthHeader?: () => Promise<Record<string, string>> | Record<string, string>;
};

export async function request<T = unknown>(path: string, opts: RequestOptions = {}): Promise<T> {
    const {
        method = 'GET',
        body,
        signal,
        timeoutMs = DEFAULT_TIMEOUT,
        headers = {},
        getAuthHeader
    } = opts;

    const controller = new AbortController();
    const timeout = setTimeout(() => controller.abort(), timeoutMs);
    const combinedSignal = signal ?? controller.signal;

    try {
        const authHeaders = getAuthHeader ? await getAuthHeader() : {};
        const res = await fetch(buildUrl(path), {
            method,
            headers: {
                'Content-Type': body != null ? 'application/json' : 'text/plain',
                Accept: 'application/json',
                ...authHeaders,
                ...headers
            },
            body: body != null ? JSON.stringify(body) : undefined,
            signal: combinedSignal
        });

        clearTimeout(timeout);

        const text = await res.text();
        const contentType = res.headers.get('content-type') ?? '';
        const data = contentType.includes('application/json') && text ? JSON.parse(text) : text;

        if (!res.ok) {
            let message = res.statusText ?? 'API error';

            if (typeof data === 'object' && data !== null) {
                if ('message' in data && typeof data.message === 'string') {
                    message = data.message;
                } else if ('error' in data && typeof data.error === 'string') {
                    message = data.error;
                }
            }

            throw new ApiError(message, res.status, data);
        }

        return data as T;
    } catch (err: unknown) {
        clearTimeout(timeout);

        if (err instanceof DOMException && err.name === 'AbortError') {
            throw new ApiError('Request aborted', 0, { aborted: true });
        }

        if (err instanceof ApiError)
            throw err;

        if (err instanceof Error) {
            throw new ApiError(err.message, 0, err);
        }

        throw new ApiError('Network error', 0, err);
    }
}
