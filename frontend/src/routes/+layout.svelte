<script lang="ts">
	import './global.css';
	import favicon from '$lib/assets/favicon.svg';
	import ToastContainer from '$lib/components/ui/ToastContainer.svelte';

	let { children } = $props();
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>
<main class="relative h-screen bg-(--background) text-(--foreground) antialiased">
	{@render children()}
	<ToastContainer />
</main>
