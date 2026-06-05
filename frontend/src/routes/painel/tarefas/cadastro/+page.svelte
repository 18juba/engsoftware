<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/state';
	import { resolve } from '$app/paths';
	import { taskStore, taskShow, taskUpdate } from '$lib/services/taskService';
	import { toasts } from '$lib/stores/toast';
	import { onMount } from 'svelte';

	let title = $state('');
	let description = $state('');

	let isEditMode = $state(false);
	let tankId = $state<number | null>(null);
	let isLoading = $state(false);

	onMount(() => {
		const urlParams = new URLSearchParams(page.url.search);
		const id = urlParams.get('id');

		if (id) {
			tankId = parseInt(id);
			isEditMode = true;
			loadTask(tankId);
		}
	});

	async function loadTask(id: number) {
		try {
			isLoading = true;
			const task = await taskShow(String(id));
			title = task.title;
			description = task.description;
		} catch (err) {
			toasts.add({
				type: 'danger',
				title: 'Erro ao Carregar',
				description: 'Não foi possível carregar os dados da tarefa.'
			});
			setTimeout(() => goto(resolve('/painel/tarefas')), 1500);
		} finally {
			isLoading = false;
		}
	}

	let isSaving = $state(false);
	let errors = $state<Record<string, string>>({});

	function validate(): boolean {
		const newErrors: Record<string, string> = {};

		if (!title.trim()) newErrors.title = 'Titulo é obrigatório.';

		errors = newErrors;
		return Object.keys(newErrors).length === 0;
	}

	async function handleSubmit(e: Event) {
		e.preventDefault();
		if (!validate()) return;

		try {
			isSaving = true;
			const taskData = {
				title: title.trim(),
				description: description.trim(),
			};

			if (isEditMode && tankId) {
				await taskUpdate(tankId, taskData);
				toasts.add({
					type: 'success',
					title: 'Tarefa Atualizada',
					description: `A tarefa "${title}" foi atualizada com sucesso.`
				});
			} else {
				const newTask = await taskStore(taskData);
				if (newTask && newTask.id) {
					tankId = newTask.id;
				}
				toasts.add({
					type: 'success',
					title: 'Tarefa Criada',
					description: `A tarefa "${title}" foi cadastrada com sucesso.`
				});
			}

			setTimeout(() => goto(resolve('/painel/tarefas')), 1200);
		} catch (err) {
			toasts.add({
				type: 'danger',
				title: 'Erro ao Salvar',
				description: err instanceof Error ? err.message : 'Ocorreu um erro inesperado.'
			});
		} finally {
			isSaving = false;
		}
	}

	function cancelar() {
		goto(resolve('/painel/tarefas'));
	}
</script>

{#if isLoading}
	<div class="flex items-center justify-center p-8">
		<p class="text-white/60">Carregando tarefa...</p>
	</div>
{:else}
	<div class="rounded-lg bg-(--foreground)/2 p-10">
		<h2 class="mb-6 text-2xl font-bold">
			{isEditMode ? `Editar Tarefa #${tankId}` : 'Nova Tarefa'}
		</h2>

		<form onsubmit={handleSubmit} class="space-y-5">
			<!-- Titulo -->
			<div class="w-full xl:w-md">
				<label for="title" class="mb-1 block text-sm text-white/80">Titulo *</label>
				<input
					id="title"
					type="text"
					bind:value={title}
					class="w-full rounded-lg bg-(--foreground)/10 px-4 py-2 text-white placeholder-white/40 focus:ring-2 focus:ring-(--primary) focus:outline-none"
					placeholder="Ex: Tarefa Principal"
				/>
				{#if errors.title}
					<p class="mt-1 text-sm text-red-400">{errors.title}</p>
				{/if}
			</div>

			<!-- Descrição -->
			<div>
				<label for="description" class="mb-1 block text-sm text-white/80">Descrição</label>
				<textarea
					id="description"
					bind:value={description}
					rows="3"
					class="w-full rounded-lg bg-(--foreground)/10 px-4 py-2 text-white placeholder-white/40 focus:ring-2 focus:ring-(--primary) focus:outline-none"
					placeholder="Opcional: detalhes sobre a tarefa..."
				></textarea>
			</div>

			<!-- Botões -->
			<div class="flex gap-3 pt-4">
				<button
					type="submit"
					disabled={isSaving}
					class="cursor-pointer rounded-lg bg-(--primary) px-6 py-2 font-semibold text-white transition-colors hover:bg-(--primary)/90 disabled:cursor-not-allowed disabled:opacity-60"
				>
					{isSaving ? 'Salvando...' : isEditMode ? 'Atualizar' : 'Salvar'}
				</button>
				<button
					type="button"
					onclick={cancelar}
					class="cursor-pointer rounded-lg bg-(--foreground)/10 px-6 py-2 text-white transition-colors hover:bg-(--foreground)/20"
				>
					Cancelar
				</button>
			</div>
		</form>
	</div>
{/if}