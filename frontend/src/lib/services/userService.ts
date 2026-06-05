import { apiFetch } from '$lib/api/api';
import type { UserCharacter, UserDashboard } from '$lib/types/user';

export async function changeCharacter(character: UserCharacter) {
    return apiFetch<UserCharacter>(`/users/change_character`, {
        method: 'PATCH',
        body: JSON.stringify({ character })
    });
}

export async function getDashboard() {
    return apiFetch<UserDashboard>(`/users/dashboard`);
}