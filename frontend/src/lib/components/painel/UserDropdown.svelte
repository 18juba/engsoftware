<script lang="ts">
	import { auth } from '$lib/stores/auth';
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';

	type UserDropdownProps = {
		openProfile: () => void;
	};

	let { openProfile } = $props() as UserDropdownProps;

	async function logout() {
		auth.clearToken();
		auth.clearUser();
		await goto(resolve('/'));
	}
</script>

<div
	class="absolute top-full right-2 z-1 flex min-w-40 flex-col items-start gap-2 rounded-md bg-(--foreground)/30 px-4 py-2 text-(--foreground) backdrop-blur-xs"
>
	<button
		class="w-full cursor-pointer rounded-md px-2 py-1 text-start hover:bg-(--background)/10"
		onclick={openProfile}
	>
		Meu Perfil
	</button>
	<button
		class="w-full cursor-pointer rounded-md px-2 py-1 text-start hover:bg-(--background)/10"
		onclick={logout}
	>
		Sair
	</button>
</div>
