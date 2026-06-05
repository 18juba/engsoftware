<script lang="ts">
	import { auth } from '$lib/stores/auth';
	import { goto } from '$app/navigation';
    import { resolve } from '$app/paths';

	type UserDropdownProps = {
		openProfile: () => void;
	}

	let { openProfile } = $props() as UserDropdownProps;

	async function logout() {
		auth.clearToken();
		auth.clearUser();
		await goto(resolve('/'));
	}
</script>

<div class="bg-(--foreground)/30 text-(--foreground) absolute min-w-40 top-full right-2 px-4 py-2 rounded-md flex flex-col items-start gap-2 z-1 backdrop-blur-xs"
>
    <button class="w-full rounded-md text-start cursor-pointer hover:bg-(--background)/10 px-2 py-1" onclick={openProfile}>
        Meu Perfil
    </button>
    <button class="w-full rounded-md text-start cursor-pointer hover:bg-(--background)/10 px-2 py-1" onclick={logout}>
        Sair
    </button>
</div>