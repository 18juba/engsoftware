<script lang="ts">
	import { onMount } from 'svelte';
	import {
		taskMarkAsComplete,
		taskMarkAsCancelled,
		taskStartTask,
		taskPauseTask
	} from '$lib/services/taskService';
	import { toasts } from '$lib/stores/toast';
	import Icon from '../ui/Icon.svelte';
	import type { Task, TaskStatus } from '$lib/types/task';

	let { tarefa, onStatusChange }: { tarefa: Task; onStatusChange?: (task: Task) => void } =
		$props();
	let menuOpen = $state(false);
	let menuRef: HTMLDivElement | undefined = $state();
	let buttonRef: HTMLButtonElement | undefined = $state();
	let menuPosition = $state({ top: 0, left: 0 });
	let isLoading = $state(false);

	type ActionKey = 'iniciar' | 'pausar' | 'concluir' | 'cancelar';
	interface Action {
		key: ActionKey;
		label: string;
		icon: string;
		variant?: 'danger' | 'success';
		handler: () => void;
	}

	const ACTION_MAP: Record<TaskStatus, ActionKey[]> = {
		scheduled: ['iniciar', 'cancelar'],
		in_progress: ['pausar', 'concluir', 'cancelar'],
		paused: ['iniciar', 'concluir', 'cancelar'],
		completed: [],
		cancelled: []
	};

	const ACTION_CONFIG: Record<ActionKey, Omit<Action, 'key' | 'handler'>> = {
		iniciar: { label: 'Iniciar Tarefa', icon: 'Play.png' },
		pausar: { label: 'Pausar Tarefa', icon: 'Pause.png' },
		concluir: { label: 'Marcar como Concluída', icon: 'Check.png', variant: 'success' },
		cancelar: { label: 'Cancelar Tarefa', icon: 'CloseCircle.png', variant: 'danger' }
	};

	function getHandler(key: ActionKey): () => void {
		switch (key) {
			case 'iniciar':
				return iniciarTarefa;
			case 'pausar':
				return pausarTarefa;
			case 'concluir':
				return concluirTarefa;
			case 'cancelar':
				return cancelarTarefa;
			default:
				return () => {};
		}
	}

	const acoes = $derived.by(() => {
		const actionKeys = ACTION_MAP[tarefa.status] ?? [];
		return actionKeys.map((key) => ({
			key,
			...ACTION_CONFIG[key],
			handler: getHandler(key)
		}));
	});

	function toggleMenu() {
		if (!menuOpen && buttonRef) {
			const rect = buttonRef.getBoundingClientRect();
			menuPosition = {
				top: rect.bottom + 8,
				left: rect.left - 160
			};
		}
		menuOpen = !menuOpen;
	}

	function closeMenu() {
		menuOpen = false;
	}

	function handleClickOutside(event: MouseEvent) {
		if (menuRef && !menuRef.contains(event.target as Node)) {
			closeMenu();
		}
	}

	onMount(() => {
		document.addEventListener('click', handleClickOutside);
		return () => document.removeEventListener('click', handleClickOutside);
	});

	async function executeAction(
		promise: Promise<any>,
		newStatus: TaskStatus,
		title: string,
		description: string
	) {
		if (isLoading) return;

		try {
			isLoading = true;
			await promise;
			tarefa.status = newStatus;
			onStatusChange?.(tarefa);
			toasts.add({ type: 'success', title, description });
			closeMenu();
		} catch (err) {
			console.error('Erro ao executar ação:', err);
			toasts.add({
				type: 'danger',
				title: 'Erro',
				description: 'Não foi possível executar a ação.'
			});
		} finally {
			isLoading = false; // ✅ Sempre reseta
		}
	}

	async function iniciarTarefa() {
		await executeAction(
			taskStartTask(tarefa.id),
			'in_progress',
			'Tarefa Iniciada',
			'A tarefa foi iniciada com sucesso.'
		);
	}

	async function pausarTarefa() {
		await executeAction(
			taskPauseTask(tarefa.id),
			'paused',
			'Tarefa Pausada',
			'A tarefa foi pausada com sucesso.'
		);
	}

	async function concluirTarefa() {
		await executeAction(
			taskMarkAsComplete(tarefa.id),
			'completed',
			'Tarefa Concluída',
			'A tarefa foi marcada como concluída.'
		);
	}

	async function cancelarTarefa() {
		await executeAction(
			taskMarkAsCancelled(tarefa.id),
			'cancelled',
			'Tarefa Cancelada',
			'A tarefa foi cancelada.'
		);
	}
</script>

<div bind:this={menuRef}>
	<button
		bind:this={buttonRef}
		onclick={toggleMenu}
		class="flex h-8 w-8 cursor-pointer items-center justify-center rounded-full transition-colors hover:bg-white/10"
		aria-label="Ações extras"
	>
		<Icon icon="Menu.png" class="h-5 w-5" />
	</button>

	{#if menuOpen}
		<div
			class="fixed z-50 mt-2 min-w-40 rounded-md bg-(--background) shadow-lg ring-1 ring-(--foreground)/10"
			style="top: {menuPosition.top}px; left: {menuPosition.left}px;"
		>
			<div class="p-1">
				{#each acoes as acao (acao.key)}
					<button
						onclick={acao.handler}
						disabled={isLoading}
						class="flex w-full cursor-pointer items-center gap-2 rounded-lg px-4 py-2 text-left text-sm text-(--foreground) transition-colors hover:bg-(--foreground)/10 disabled:cursor-not-allowed disabled:opacity-50"
						class:text-emerald-400={acao.variant === 'success'}
						class:text-red-400={acao.variant === 'danger'}
					>
						<Icon icon={acao.icon} class="h-4 w-4 shrink-0" />
						<span>{acao.label}</span>
					</button>
				{/each}

				{#if acoes.length === 0}
					<div class="px-4 py-2 text-sm text-white/50">Nenhuma ação disponível</div>
				{/if}
			</div>
		</div>
	{/if}
</div>
