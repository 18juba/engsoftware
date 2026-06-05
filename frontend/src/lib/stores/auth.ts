import { browser } from '$app/environment';
import type { User } from '$lib/types/user';
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
		},

		setUser(user: User | null) {
			if (browser) localStorage.setItem('user', JSON.stringify(user));
		},

		getUser(): User | null {
			if (!browser) return null;
			const userJson = localStorage.getItem('user');
			return userJson ? JSON.parse(userJson) : null;
		},

		clearUser() {
			if (browser) localStorage.removeItem('user');
		}
	};
}

export const auth = createAuthStore();
