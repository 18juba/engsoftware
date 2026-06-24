import { apiFetch } from '$lib/api/api';
import type { Professor } from '$lib/types/professor';

export async function listarProfessores() {
	return apiFetch<Professor[]>('/Professores');
}

export async function buscarProfessorPorId(id: number) {
	return apiFetch<Professor>(`/Professores/${id}`);
}

export async function criarProfessor(professor: Professor) {
	return apiFetch<Professor>('/Professores', {
		method: 'POST',
		body: JSON.stringify(professor)
	});
}

export async function atualizarProfessor(id: number, professor: Professor) {
	return apiFetch<Professor>(`/Professores/${id}`, {
		method: 'PUT',
		body: JSON.stringify(professor)
	});
}

export async function deletarProfessor(id: number) {
	return apiFetch<void>(`/Professores/${id}`, {
		method: 'DELETE'
	});
}
