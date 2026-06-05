import { apiFetch } from '$lib/api/api';
import type { PaginatedResponse } from '$lib/types/pagination';
import type { Task } from '$lib/types/task';

export async function taskIndexPaginated(page = 1, limit = 20) {
	const params = new URLSearchParams({
		page: String(page),
		limit: String(limit)
	});

	return apiFetch<PaginatedResponse<Task>>(`/tasks?${params.toString()}`);
}

export async function taskIndex(page = 1, limit = 100) {
	const response = await taskIndexPaginated(page, limit);
	return response.data;
}

export async function taskShow(id: string) {
	return apiFetch<Task>(`/tasks/${id}`);
}

export async function taskStore(task?: Partial<Task>) {
	return apiFetch<Task | null>('/tasks', {
		method: 'POST',
		body: JSON.stringify(task ?? {})
	});
}

export async function taskUpdate(id: number, task: Partial<Task>) {
	return apiFetch<Task | null>(`/tasks/${id}`, {
		method: 'PUT',
		body: JSON.stringify(task)
	});
}

export async function taskDelete(id: number) {
	return apiFetch<Task | null>(`/tasks/${id}`, {
		method: 'DELETE'
	});
}

export async function taskToggleActive(id: number) {
	return apiFetch<Task>(`/tasks/toggle/${id}`, {
		method: 'PATCH'
	});
}