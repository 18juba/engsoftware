import { browser } from '$app/environment';
import type { User } from '$lib/types/user';
import { writable } from 'svelte/store';

const TOKEN_KEY = 'auth_token';
const USER_KEY = 'user';

function createAuthStore() {
	const initialToken = browser ? localStorage.getItem(TOKEN_KEY) : null;
	const initialUser = browser ? JSON.parse(localStorage.getItem(USER_KEY) || 'null') : null;

	const { subscribe, set } = writable<string | null>(initialToken);
	const userStore = writable<User | null>(initialUser);

	return {
		subscribe,
		user: userStore, // 👈 expõe a store diretamente
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
			if (browser) localStorage.setItem(USER_KEY, JSON.stringify(user));
			userStore.set(user);
		},
		getUser(): User | null {
			if (!browser) return null;
			const userJson = localStorage.getItem(USER_KEY);
			return userJson ? JSON.parse(userJson) : null;
		},
		clearUser() {
			if (browser) localStorage.removeItem(USER_KEY);
			userStore.set(null);
		}
	};
}

export const auth = createAuthStore();
