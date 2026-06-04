import { browser } from '$app/environment';
import { writable } from 'svelte/store';

const TOKEN_KEY = 'auth_token';

function createAuthStore() {
	const initial = browser ? localStorage.getItem(TOKEN_KEY) : null;
	const { subscribe, set } = writable<string | null>(initial);

	return {
		subscribe,

		setToken(token: string) {
			if (browser) localStorage.setItem(TOKEN_KEY, token);
			set(token);
		},

		clearToken() {
			if (browser) localStorage.removeItem(TOKEN_KEY);
			set(null);
		},

		getToken(): string | null {
			return browser ? localStorage.getItem(TOKEN_KEY) : null;
		}
	};
}

export const auth = createAuthStore();
