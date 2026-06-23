import type { Disciplina } from './disciplina';

export type Turma = {
	id: number;
	disciplinaId?: number;
	semestre: string;
	horario: string;
	disciplina: Disciplina;
};
