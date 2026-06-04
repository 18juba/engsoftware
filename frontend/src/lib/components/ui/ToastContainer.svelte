<script lang="ts">
	import { toasts } from '$lib/stores/toast';
	import Toast from './Toast.svelte';
</script>

<div class="fixed top-4 right-4 z-50 flex w-96 max-w-[calc(100vw-2rem)] flex-col gap-2">
	{#each $toasts as toast (toast.id)}
		<Toast
			open={true}
			type={toast.type}
			title={toast.title}
			description={toast.description}
			onClose={() => toasts.remove(toast.id)}
		/>
	{/each}
</div>
