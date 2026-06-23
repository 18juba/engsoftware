import { apiFetch } from '$lib/api/api';
import type { Turma } from '$lib/types/turma';

export async function listarTurmas() {
    return apiFetch<Turma[]>('/Turmas');
}

export async function buscarTurmaPorId(id: number) {
    return apiFetch<Turma>(`/Turmas/${id}`);
}

export async function criarTurma(turma: Turma) {
    return apiFetch<Turma>('/Turmas', {
        method: 'POST',
        body: JSON.stringify(turma)
    });
}

export async function atualizarTurma(id: number, turma: Turma) {
    return apiFetch<Turma>(`/Turmas/${id}`, {
        method: 'PUT',
        body: JSON.stringify(turma)
    });
}

export async function deletarTurma(id: number) {
    return apiFetch<void>(`/Turmas/${id}`, {
        method: 'DELETE'
    });
}