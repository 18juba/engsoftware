<script lang="ts">
    import { goto } from '$app/navigation';
    import { resolve } from '$app/paths';
    import { onMount } from 'svelte';
    import { taskIndexPaginated } from '$lib/services/taskService';
    import type { Task, TaskStatus, TaskPriority } from '$lib/types/task';

	const statusConfig: Record<TaskStatus, { label: string; class: string }> = {
		scheduled: {
			label: 'Agendada',
			class: 'bg-blue-500/20 text-blue-400 border border-blue-500/30'
		},
		in_progress: {
			label: 'Em Andamento',
			class: 'bg-amber-500/20 text-amber-400 border border-amber-500/30'
		},
		cancelled: { label: 'Cancelada', class: 'bg-red-500/20 text-red-400 border border-red-500/30' },
		completed: {
			label: 'Finalizada',
			class: 'bg-green-500/20 text-green-400 border border-green-500/30'
		}
	};

	const priorityConfig: Record<TaskPriority, { label: string; class: string }> = {
		low: { label: 'Baixa', class: 'bg-blue-500/10 text-blue-400 border border-blue-500/20' },
		medium: { label: 'Média', class: 'bg-amber-500/10 text-amber-400 border border-amber-500/20' },
		high: { label: 'Alta', class: 'bg-red-500/10 text-red-400 border border-red-500/20' }
	};

	let tarefas = $state<Task[]>([]);
	let isLoading = $state(true);
	let error = $state<string | null>(null);
	let currentPage = $state(1);
	let totalItems = $state(0);
	let totalPages = $state(1);
	const itemsPerPage = 10;

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

    function criarNovaTarefa() {
		goto(resolve('/painel/tarefas/cadastro'));
	}

    function editarTarefa(tarefa: Task) {
		goto(resolve(`/painel/tarefas/cadastro?id=${tarefa.id}`));
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
					<th class="px-4 py-2">Ações</th>
				</tr>
			</thead>

			<tbody class="divide-y divide-gray-200 text-sm">
				{#each tarefas as item (item.id)}
					<tr class="transition hover:bg-(--foreground)/5">
						<td class="flex items-center gap-4 px-4 py-2">{item.title}</td
						>
						<td class="px-4 py-2">{item.description || '-'}</td>
						<td class="px-4 py-2">
							<div class="flex items-center gap-4">

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
