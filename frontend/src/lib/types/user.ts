import type { Task, ToDoTaskStatus } from "./task";
import type { Notification } from "./notification";

export type User = {
    id: number;
    type: UserType;
    character: UserCharacter;
    name: string;
    email: string;
    whatsapp?: string;
    active: boolean;
    created_at: string;
    tasks?: Task[];
    notifications?: Notification[];
};

export type UserType = 'admin' | 'customer';

export type UserCharacter = 'anonymous' | 'bender' | 'creeper' | 'doge' | 'flameprincess' | 'freddy' | 'hellokitty' | 'homersimpson' | 'ironman' | 'jake' | 'jason' | 'joker' | 'luigi' | 'mario' | 'melody' | 'minecraft' | 'monalisa' | 'princessbubblegum' | 'scream' | 'troll';

export type UserDashboard = {
    to_do_tasks_total: number;
    to_do_tasks_analysis: ToDoTasksAnalysis[];
};

export type ToDoTasksAnalysis = {
    status: ToDoTaskStatus;
    count: number;
};