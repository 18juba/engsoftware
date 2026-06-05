<script lang="ts">
	import { page } from '$app/state';
	import Icon from '../ui/Icon.svelte';
	import { LINKS } from './Links';

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
        backdrop-blur-xs
		pb-[env(safe-area-inset-bottom)]
		sm:hidden
	"
>
	<ul class="flex items-center justify-evenly py-2">
		{#each LINKS as item (item.id)}
			<li>
				<a
					href={item.href}
					class="
						flex flex-col items-center gap-1
						text-xs
						transition-all
                        rounded-b
                        px-1
                        pb-0.5
						{isActive(item.href) ? 'border-b-2 opacity-100' : 'opacity-70'}
					"
				>
					<Icon icon={item.icon} class="h-6 w-6" />
					<span class="text-xs">{item.label}</span>
				</a>
			</li>
		{/each}
	</ul>
</nav>
