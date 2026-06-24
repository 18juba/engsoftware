import { authApiFetch } from '$lib/api/authApi';
import type { UserCharacter } from '$lib/types/user';

export async function changeCharacter(character: UserCharacter) {
	return authApiFetch<UserCharacter>(`/users/change_character`, {
		method: 'PATCH',
		body: JSON.stringify({ character })
	});
}
