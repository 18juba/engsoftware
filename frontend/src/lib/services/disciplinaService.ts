import { apiFetch } from '$lib/api/api';
import type { Disciplina } from '$lib/types/disciplina';

export async function listarDisciplinas() {
    return apiFetch<Disciplina[]>('/Disciplinas');
}

export async function buscarDisciplinaPorId(id: number) {
    return apiFetch<Disciplina>(`/Disciplinas/${id}`);
}

export async function criarDisciplina(disciplina: Disciplina) {
    return apiFetch<Disciplina>('/Disciplinas', {
        method: 'POST',
        body: JSON.stringify(disciplina)
    });
}

export async function atualizarDisciplina(id: number, disciplina: Disciplina) {
    return apiFetch<Disciplina>(`/Disciplinas/${id}`, {
        method: 'PUT',
        body: JSON.stringify(disciplina)
    });
}

export async function deletarDisciplina(id: number) {
    return apiFetch<void>(`/Disciplinas/${id}`, {
        method: 'DELETE'
    });
}