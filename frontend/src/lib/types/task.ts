export type TaskStatus = 'scheduled' | 'in_progress' | 'cancelled' | 'completed' | 'paused';
export type ToDoTaskStatus = 'in_progress' | 'scheduled' | 'paused';

export interface Task {
	id: number;
	title: string;
	description: string;
	scheduled_time: string;
	completion_time: string | null;
	status: TaskStatus;
	created_at: string;
}
