import { writable } from 'svelte/store';

type Toast = {
	id: number;
	type: 'success' | 'danger' | 'info';
	title: string;
	description: string;
};

function createToastStore() {
	const { subscribe, update } = writable<Toast[]>([]);
	return {
		subscribe,
		add(toast: Omit<Toast, 'id'>) {
			update((toasts) => [...toasts, { ...toast, id: Date.now() }]);
		},
		remove(id: number) {
			update((toasts) => toasts.filter((t) => t.id !== id));
		}
	};
}

export const toasts = createToastStore();
