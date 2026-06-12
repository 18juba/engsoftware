<script lang="ts">
	import { page } from '$app/state';
	import Icon from '../ui/Icon.svelte';
	import { LINKS } from './Links';

	let { userType } = $props();

	function isActive(href: string): boolean {
		const currentPath = page.url.pathname;

		if (href === '/painel') {
			return currentPath === href;
		}

		return currentPath.startsWith(href);
	}
</script>

<nav
	class="
		fixed
		right-0
		bottom-0
		left-0
		z-50
		rounded-t-lg
		border-t-2
		border-(--foreground)
        pb-[env(safe-area-inset-bottom)]
		backdrop-blur-xs
		sm:hidden
	"
>
	<ul class="flex items-center justify-evenly py-2">
		{#each LINKS as item (item.id)}
			{#if item.userTypes.includes(userType)}
				<li>
					<a
						href={item.href}
						class="
						flex flex-col items-center gap-1
						rounded-b
						px-1
                        pb-0.5
                        text-xs
                        transition-all
						duration-100
						{isActive(item.href) ? 'border-b border-b-(--foreground) opacity-100 bg-linear-to-t from-(--foreground)/10 to-transparent' : 'border-b-0 border-b-transparent bg-transparent hover:bg-(--foreground)/10 opacity-70'}
					"
					>
						<Icon icon={item.icon} class="h-6 w-6" />
						<span class="text-xs">{item.label}</span>
					</a>
				</li>
			{/if}
		{/each}
	</ul>
</nav>
