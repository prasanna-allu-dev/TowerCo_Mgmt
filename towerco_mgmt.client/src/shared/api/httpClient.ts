import { API_BASE_URL, API_ERROR_FALLBACK } from '@/shared/constants/api';
import { ApiError } from './ApiError';

interface ProblemDetails {
    title?: string;
    detail?: string;
}

async function toApiError(response: Response): Promise<ApiError> {
    const problem: ProblemDetails = await response.json().catch(() => ({}));
    return new ApiError(problem.detail ?? problem.title ?? API_ERROR_FALLBACK, response.status);
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${path}`, {
        headers: { 'Content-Type': 'application/json' },
        ...init,
    });
    if (!response.ok) throw await toApiError(response);
    return response.json() as Promise<T>;
}

export const httpClient = {
    get: <T>(path: string) => request<T>(path),
    post: <T>(path: string, body: unknown) =>
        request<T>(path, { method: 'POST', body: JSON.stringify(body) }),
    patch: <T>(path: string, body: unknown) =>
        request<T>(path, { method: 'PATCH', body: JSON.stringify(body) }),
};
