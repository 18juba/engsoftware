export type TaskStatus = 'scheduled' | 'in_progress' | 'cancelled' | 'completed';
export type TaskPriority = 'low' | 'medium' | 'high';

export interface Task {
	id: number;
	title: string;
	description: string;
	scheduled_time: string;
	completion_time: string | null;
	status: TaskStatus;
	prioridade: TaskPriority;
    completed: boolean;
    created_at: string;
}