export type TaskStatus = 'scheduled' | 'in_progress' | 'cancelled' | 'completed' | 'paused';
export type TaskPriority = 'low' | 'medium' | 'high';

export interface Task {
	id: number;
	title: string;
	description: string;
	scheduled_time: string;
	completion_time: string | null;
	status: TaskStatus;
	priority: TaskPriority;
    created_at: string;
}