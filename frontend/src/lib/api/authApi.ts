import { browser } from '$app/environment';
import { goto } from '$app/navigation';
import { resolve } from '$app/paths';
import { env } from '$env/dynamic/public';
import { auth } from '$lib/stores/auth';

export class AuthApiError extends Error {
    constructor(
        message: string,
        public statusCode: number
    ) {
        super(message);
    }
}

export async function authApiFetch<T>(url: string, options?: RequestInit): Promise<T> {
    const authApiUrl = env.PUBLIC_AUTHAPI_URL;

    if (!authApiUrl) {
        throw new Error('PUBLIC_AUTHAPI_URL não está configurada');
    }

    const headers: Record<string, string> = {
        'Content-Type': 'application/json',
        ...(options?.headers as Record<string, string>)
    };

    if (options?.body instanceof FormData) {
        delete headers['Content-Type'];
    }

    if (browser) {
        const token = auth.getToken();
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
    }

    const res = await fetch(`${authApiUrl}${url}`, {
        ...options,
        headers
    });

    if (!res.ok) {
        let message = 'Erro na requisição';
        try {
            const errorBody = await res.json();
            if (typeof errorBody?.message === 'string' && errorBody.message.trim() !== '') {
                message = errorBody.message;
            } else if (typeof errorBody?.Message === 'string' && errorBody.Message.trim() !== '') {
                message = errorBody.Message;
            }
        } catch {
            if (res.statusText) {
                message = res.statusText;
            }
        }

        if (res.status === 401) {
            auth.clearToken();
            goto(resolve('/'));
            throw new AuthApiError('Sessão expirada', 401);
        }

        throw new AuthApiError(message, res.status);
    }

    if (res.status === 204) {
        return null as T;
    }

    return res.json();
}
