<script lang="ts">
	import Modal from '$lib/components/ui/Modal.svelte';
	import type { Task } from '$lib/types/task';

	interface DetalharTarefaModalProps {
		open: boolean;
		tarefa: Task | null;
		onClose: () => void;
	}

	let { open = $bindable(), onClose, tarefa }: DetalharTarefaModalProps = $props();
</script>

<Modal {open} title="Detalhes da Tarefa" {onClose}>
	{#if tarefa}
		<h2 class="mb-4 text-lg font-bold">{tarefa.title}</h2>
		<p class="opacity-90">{tarefa.description}</p>
	{:else}
		<p class="text-gray-600">Tarefa não encontrada.</p>
	{/if}
</Modal>
