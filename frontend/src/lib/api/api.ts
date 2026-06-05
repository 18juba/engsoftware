import { browser } from '$app/environment';
import { goto } from '$app/navigation';
import { resolve } from '$app/paths';
import { env } from '$env/dynamic/public';
import { auth } from '$lib/stores/auth';

export class ApiError extends Error {
	constructor(
		message: string,
		public statusCode: number
	) {
		super(message);
	}
}

export async function apiFetch<T>(url: string, options?: RequestInit): Promise<T> {
	const apiUrl = env.PUBLIC_API_URL;

	if (!apiUrl) {
		throw new Error('PUBLIC_API_URL não está configurada');
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

	const res = await fetch(`${apiUrl}${url}`, {
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
			throw new ApiError('Sessão expirada', 401);
		}

		throw new ApiError(message, res.status);
	}

	if (res.status === 204) {
		return null as T;
	}

	return res.json();
}
