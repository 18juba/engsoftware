<script lang="ts">
    import { goto } from '$app/navigation';
    import { resolve } from '$app/paths';
    import { onMount } from 'svelte';
    import { taskIndexPaginated, taskDelete } from '$lib/services/taskService';
    import type { Task, TaskStatus, TaskPriority } from '$lib/types/task';
	import { toasts } from '$lib/stores/toast';
	import Icon from '../ui/Icon.svelte';
	import Dialog from '../ui/Dialog.svelte';
	import ModalDetalharTarefa from './ModalDetalharTarefa.svelte';

	const statusConfig: Record<TaskStatus, { label: string; class: string }> = {
		scheduled: {
			label: 'Agendada',
			class: 'bg-sky-500/20 text-sky-400'
		},
		in_progress: {
			label: 'Em Andamento',
			class: 'bg-amber-500/20 text-amber-400'
		},
		cancelled: { label: 'Cancelada', class: 'bg-red-500/20 text-red-400' },
		completed: {
			label: 'Concluída',
			class: 'bg-emerald-500/20 text-emerald-400'
		}
	};

	const priorityConfig: Record<TaskPriority, { label: string; class: string }> = {
		low: { label: 'Baixa', class: 'bg-sky-500/10 text-sky-400' },
		medium: { label: 'Média', class: 'bg-amber-500/10 text-amber-400' },
		high: { label: 'Alta', class: 'bg-red-500/10 text-red-400' }
	};

	let tarefas = $state<Task[]>([]);
	let isLoading = $state(true);
	let error = $state<string | null>(null);
	let currentPage = $state(1);
	let totalItems = $state(0);
	let totalPages = $state(1);
	const itemsPerPage = 10;
	let tarefaParaDeletar = $state<Task | null>(null);
	let tarefaParaDetalhar = $state<Task | null>(null);
	let modalDetalharOpen = $state(false);
	let dialogOpen = $state(false);

	onMount(async () => {
		await loadTarefas();
	});

    	async function loadTarefas(page = currentPage) {
		try {
			error = null;
			const response = await taskIndexPaginated(page, itemsPerPage);
			tarefas = response.data;
			totalItems = response.total;
			totalPages = Math.max(response.total_pages, 1);
			currentPage = response.page;
		} catch (err) {
			error = err instanceof Error ? err.message : 'Erro ao carregar tarefas';
			console.error('Erro ao buscar tarefas:', err);
		} finally {
			isLoading = false;
		}
	}

    	async function goToPreviousPage() {
		if (currentPage <= 1) return;
		await loadTarefas(currentPage - 1);
	}

	async function goToNextPage() {
		if (currentPage >= totalPages) return;
		await loadTarefas(currentPage + 1);
	}

	function detalharTarefa(tarefa: Task) {
		tarefaParaDetalhar = tarefa;
		modalDetalharOpen = true;
	}

	function fecharDetalhes() {
		tarefaParaDetalhar = null;
		modalDetalharOpen = false;
	}

    function criarNovaTarefa() {
		goto(resolve('/painel/tarefas/cadastro'));
	}

    function editarTarefa(tarefa: Task) {
		goto(resolve(`/painel/tarefas/cadastro?id=${tarefa.id}`));
	}

	async function deletarTarefa(tarefa: Task) {
		tarefaParaDeletar = tarefa;
		dialogOpen = true;
	}

		async function confirmDelete() {
		if (!tarefaParaDeletar) return;

		try {
			await taskDelete(tarefaParaDeletar.id);

			const targetPage = tarefas.length === 1 && currentPage > 1 ? currentPage - 1 : currentPage;
			await loadTarefas(targetPage);

			toasts.add({
				type: 'success',
				title: 'Tarefa Deletada',
				description: 'A tarefa foi deletada com sucesso.'
			});
		} catch (err) {
			error = err instanceof Error ? err.message : 'Erro ao deletar tarefa';
			console.error('Erro ao deletar tarefa:', err);
		} finally {
			dialogOpen = false;
			tarefaParaDeletar = null;
		}
	}
</script>

<div class="space-y-4">
    <div class="flex items-center justify-between mb-2">
	<h2 class="text-2xl font-bold">Minhas Tarefas</h2>
	<button
		onclick={criarNovaTarefa}
		class="w-fit cursor-pointer rounded-lg bg-(--primary) px-4 py-2 font-semibold text-(--foreground) transition-colors duration-200 hover:bg-(--primary)/90"
	>
		Criar Tarefa
	</button>
    </div>

	{#if isLoading}
		<div class="flex items-center justify-center p-8">
			<p class="text-white/60">Carregando tarefas...</p>
		</div>
	{:else if tarefas.length === 0}
		<div class="flex items-center justify-center rounded-lg bg-(--foreground)/5 p-8">
			<p class="text-white/60">Nenhuma tarefa encontrada</p>
		</div>
	{:else}
	<div class="overflow-x-auto">
		<table class="min-w-full overflow-hidden rounded-lg">
			<thead class="bg-(--foreground)/5 text-left text-sm">
				<tr>
					<th class="px-4 py-2">Tarefa</th>
					<th class="px-4 py-2">Descrição</th>
					<th class="px-4 py-2">Prioridade</th>
					<th class="px-4 py-2">Status</th>
					<th class="px-4 py-2">Ações</th>
				</tr>
			</thead>

			<tbody class="divide-y divide-gray-200 text-sm">
				{#each tarefas as item (item.id)}
					<tr class="transition hover:bg-(--foreground)/5">
						<td class="px-4 py-2 truncate">{item.title}</td>
						<td class="px-4 py-2 truncate">{item.description || '-'}</td>
						<td class="px-4 py-2">
							<span class="inline-block rounded-full px-4 py-1 text-xs font-medium {priorityConfig[item.priority]?.class}">
								{priorityConfig[item.priority]?.label || item.priority}
							</span>
						</td>
						<td class="px-4 py-2">
							<span class="inline-block rounded-full px-4 py-1 text-xs font-medium {statusConfig[item.status]?.class}">
								{statusConfig[item.status]?.label || item.status}
							</span>
						</td>
						<td class="px-4 py-2">
							<div class="flex items-center gap-4">
								<Icon
									icon="Eye.png"
									class="h-7 w-7 cursor-pointer"
									onClick={() => detalharTarefa(item)}
								/>
								<Icon
									icon="Edit.png"
									class="h-7 w-7 cursor-pointer"
									onClick={() => editarTarefa(item)}
								/>
								<Icon
									icon="Delete.png"
									class="h-7 w-7 cursor-pointer"
									onClick={() => deletarTarefa(item)}
								/>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>

	<nav class="mt-4 flex items-center justify-between">
		<span class="text-sm text-white/60">
			Mostrando {totalItems === 0 ? 0 : (currentPage - 1) * itemsPerPage + 1} a {Math.min(
				(currentPage - 1) * itemsPerPage + tarefas.length,
				totalItems
			)} de {totalItems}
		</span>
		<div class="flex gap-2">
			<button
				disabled={currentPage === 1}
				onclick={goToPreviousPage}
				class="cursor-pointer rounded-lg bg-(--foreground)/10 px-3 py-2 text-sm text-white transition-colors hover:bg-(--foreground)/20 disabled:cursor-not-allowed disabled:opacity-50"
			>
				Anterior
			</button>
			<span class="px-3 py-2 text-sm text-white/60">
				{currentPage} / {totalPages}
			</span>
			<button
				disabled={currentPage === totalPages}
				onclick={goToNextPage}
				class="cursor-pointer rounded-lg bg-(--foreground)/10 px-3 py-2 text-sm text-white transition-colors hover:bg-(--foreground)/20 disabled:cursor-not-allowed disabled:opacity-50"
			>
				Próxima
			</button>
		</div>
	</nav>
	{/if}
</div>

<Dialog
	bind:open={dialogOpen}
	title="Deletar Tarefa"
	description={tarefaParaDeletar ? `Deseja mesmo deletar a tarefa #${tarefaParaDeletar.id} ?` : ''}
	onClose={() => {
		dialogOpen = false;
		tarefaParaDeletar = null;
	}}
	onConfirm={confirmDelete}
/>

<ModalDetalharTarefa bind:open={modalDetalharOpen} tarefa={tarefaParaDetalhar} onClose={fecharDetalhes} />