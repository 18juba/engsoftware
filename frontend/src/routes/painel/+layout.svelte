<script lang="ts">
	import NavbarPainel from '$lib/components/painel/NavbarPainel.svelte';

	let { children } = $props();
</script>

<svelte:head><title>Meu Painel</title></svelte:head>
<main class="relative h-screen bg-(--background) text-(--foreground) antialiased">
	<NavbarPainel>
		{@render children()}
	</NavbarPainel>
</main>
