<script lang="ts">
	import { auth } from '$lib/stores/auth';
	import type { UserCharacter } from '$lib/types/user';
	import { changeCharacter } from '$lib/services/userService';
	import Icon from '../ui/Icon.svelte';
	import { page } from '$app/state';
	import BottombarPainel from './BottombarPainel.svelte';
	import { LINKS } from './Links';
	import UserDropdown from './UserDropdown.svelte';
	import UserModal from '../modals/UserModal.svelte';

	let { children } = $props();

	const userStore = auth.user;

	let user = $derived($userStore);

	let isSidebarOpen = $state(true);
	let isUserDropdownOpen = $state(false);
	let isUserModalOpen = $state(false);

	function toggleUserDropdown() {
		isUserDropdownOpen = !isUserDropdownOpen;
	}

	function toggleSidebar() {
		isSidebarOpen = !isSidebarOpen;
	}

	function isActive(href: string): boolean {
		const currentPath = page.url.pathname;

		if (href === '/painel') {
			return currentPath === href;
		}

		return currentPath.startsWith(href);
	}

	function openProfile(): void {
		isUserModalOpen = true;
		isUserDropdownOpen = false;
	}

	function closeProfile(): void {
		isUserModalOpen = false;
	}

	function handleChangeCharacter(character: UserCharacter): void {
		changeCharacter(character)
			.then((updatedUserCharacter) => {
				let newUser = user ? { ...user, character: updatedUserCharacter } : null;
				auth.setUser(newUser);
				user = newUser;
			})
			.catch((error) => {
				console.error('Erro ao alterar personagem:', error);
			});
	}
</script>

<header
	class="relative flex max-h-18 w-full bg-(--primary)/30 py-2 sm:h-20 sm:max-h-20 sm:py-0 sm:pr-4"
>
	<button
		class={`hidden h-full cursor-pointer items-center justify-center border-r-2 shadow-lg transition-all duration-300 ease-in-out sm:flex
    ${isSidebarOpen ? 'w-52 px-0' : 'w-16 px-4'}`}
		onclick={toggleSidebar}
	>
		<Icon
			icon={isSidebarOpen ? 'Menu.png' : 'Menu2.png'}
			class={`transition-all duration-300 ease-in-out ${isSidebarOpen ? 'h-12 w-12' : 'h-8 w-8'}`}
		/>
	</button>
	<div class="flex w-full items-center justify-between px-2 sm:px-4">
		<address class="hidden items-center justify-center gap-2 not-italic sm:flex">
			<div class="flex flex-col">
				<h2 class="text-lg font-bold">Marca</h2>
				<p class="text-sm">Descrição da marca</p>
			</div>
		</address>

		<ul class="flex gap-6 sm:gap-8">
			<!-- <Icon icon="/Whatsapp.png" class="h-6 w-6 hover:scale-105 sm:h-8 sm:w-8" /> -->
		</ul>

		<address class="relative flex items-center justify-center gap-2 not-italic sm:gap-3">
			{#if user}
				<Icon icon={`/characters/${user?.character}.png`} class="h-8 w-8 sm:h-10 sm:w-10" />
			{:else}
				<Icon icon={`/characters/anonymous.png`} class="h-8 w-8 sm:h-10 sm:w-10" />
			{/if}

			<div class="flex flex-col">
				<h2 class="text-xs font-bold sm:text-lg">{user?.name || 'Usuário Anônimo'}</h2>
				<p class="text-[10px] sm:text-sm">{user?.email}</p>
			</div>
			<Icon
				icon={isUserDropdownOpen ? '/ArrowUp.png' : '/ArrowDown.png'}
				class="h-8 w-8 cursor-pointer hover:scale-105 sm:ml-4"
				onClick={toggleUserDropdown}
			/>{' '}
			{#if isUserDropdownOpen}
				<UserDropdown {openProfile} />
			{/if}
		</address>
	</div>
</header>

<div class="flex">
	<aside
		class={`hidden min-h-screen bg-(--foreground)/5 transition-all duration-300 ease-in-out
        sm:block
			${isSidebarOpen ? 'w-52' : 'w-16'}`}
	>
		<nav class="px-2 py-6">
			<ul class="font2 flex flex-col gap-6">
				{#each LINKS as item (item.id)}
					<a href={item.href}>
						<li
							class={`flex items-center justify-start gap-1 pl-1 font-bold transition-all duration-200
								${
									isActive(item.href)
										? 'border-l-2 border-l-(--text) bg-linear-to-r from-(--text)/20 to-transparent'
										: 'border-l-0 border-transparent bg-transparent hover:bg-(--text)/10'
								}`}
						>
							<Icon icon={item.icon} class="h-10 w-10 shrink-0" />

							<span
								class={`ml-2 overflow-hidden whitespace-nowrap transition-all duration-300 ease-in-out
									${isSidebarOpen ? 'max-w-xs translate-x-0 opacity-100' : 'max-w-0 -translate-x-2 opacity-0'}`}
							>
								{item.label}
							</span>
						</li>
					</a>
				{/each}
			</ul>
		</nav>
	</aside>

	<main class="font2 w-full p-4 pb-20 sm:p-8 sm:pb-8">
		{@render children()}
	</main>

	<BottombarPainel />
</div>

<UserModal
	open={isUserModalOpen}
	{user}
	onClose={closeProfile}
	onChangeCharacter={handleChangeCharacter}
/>
