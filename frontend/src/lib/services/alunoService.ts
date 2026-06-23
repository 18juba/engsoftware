import { apiFetch } from '$lib/api/api';
import type { Aluno } from '$lib/types/aluno';

export async function listarAlunos() {
    return apiFetch<Aluno[]>('/Alunos');
}

export async function buscarAlunoPorId(id: number) {
    return apiFetch<Aluno>(`/Alunos/${id}`);
}

export async function criarAluno(aluno: Aluno) {
    return apiFetch<Aluno>('/Alunos', {
        method: 'POST',
        body: JSON.stringify(aluno)
    });
}

export async function atualizarAluno(id: number, aluno: Aluno) {
    return apiFetch<Aluno>(`/Alunos/${id}`, {
        method: 'PUT',
        body: JSON.stringify(aluno)
    });
}

export async function deletarAluno(id: number) {
    return apiFetch<void>(`/Alunos/${id}`, {
        method: 'DELETE'
    });
}