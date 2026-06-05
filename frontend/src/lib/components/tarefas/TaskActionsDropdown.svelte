<script lang="ts">
    import { onMount } from 'svelte';
    import { taskMarkAsComplete, taskMarkAsCancelled, taskStartTask, taskPauseTask } from '$lib/services/taskService';
    import { toasts } from '$lib/stores/toast';
    import Icon from '../ui/Icon.svelte';
    import type { Task } from '$lib/types/task';

  let { tarefa, onStatusChange }: { tarefa: Task; onStatusChange?: (task: Task) => void } = $props();
  let menuOpen = $state(false);
  let menuRef: HTMLDivElement | undefined = $state();
  let buttonRef: HTMLButtonElement | undefined = $state();
  let menuPosition = $state({ top: 0, left: 0 });


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

    	async function iniciarTarefa() {
	    try {
	        await taskStartTask(tarefa.id);
	        tarefa.status = 'in_progress';
	        toasts.add({
	            type: 'success',
	            title: 'Tarefa Iniciada',
	            description: 'A tarefa foi iniciada com sucesso.'
	        });
	    } catch (err) {
            console.error(err);
            toasts.add({
                type: 'danger',
                title: 'Erro',
                description: 'Não foi possível iniciar a tarefa.'
            });
	    }
	}

    async function marcarComoConcluida() {
        try {
            await taskMarkAsComplete(tarefa.id);
            tarefa.status = 'completed';
            if (onStatusChange) onStatusChange(tarefa);
            toasts.add({
                type: 'success',
                title: 'Tarefa Concluída',
                description: `"${tarefa.title}" foi concluída.`
            });
            closeMenu();
        } catch (err) {
            console.error(err);
            toasts.add({
                type: 'danger',
                title: 'Erro',
                description: 'Não foi possível concluir a tarefa.'
            });
        }
    }

    	async function cancelarTarefa() {
	    try {
	        await taskMarkAsCancelled(tarefa.id);
	        tarefa.status = 'cancelled';
	        toasts.add({
	            type: 'success',
	            title: 'Tarefa Cancelada',
	            description: 'A tarefa foi cancelada com sucesso.'
	        });
	    } catch (err) {
            console.error(err);
            toasts.add({
                type: 'danger',
                title: 'Erro',
                description: 'Não foi possível cancelar a tarefa.'
            });
	    }
	}

      	async function pausarTarefa() {
	    try {
	        await taskPauseTask(tarefa.id);
	        tarefa.status = 'paused';
	        toasts.add({
	            type: 'success',
	            title: 'Tarefa Pausada',
	            description: 'A tarefa foi pausada com sucesso.'
	        });
	    } catch (err) {
            console.error(err);
            toasts.add({
                type: 'danger',
                title: 'Erro',
                description: 'Não foi possível pausar a tarefa.'
            });
	    }
	}
</script>

<div bind:this={menuRef}>
  <button
    bind:this={buttonRef}
    onclick={toggleMenu}
    class="flex h-8 w-8 items-center justify-center rounded-full transition-colors hover:bg-white/10 cursor-pointer"
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
          <button
          onclick={iniciarTarefa}
          class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-(--foreground) transition-colors hover:bg-(--foreground)/10 rounded-lg"
        >
          <Icon icon="Play.png" class="h-4 w-4" />
          Iniciar Tarefa
        </button>
          <button
            onclick={pausarTarefa}
            class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-(--foreground) transition-colors hover:bg-(--foreground)/10 rounded-lg"
          >
            <Icon icon="Pause.png" class="h-4 w-4" />
            Pausar Tarefa
          </button>
        <button
          onclick={marcarComoConcluida}
          class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-(--foreground) transition-colors hover:bg-(--foreground)/10 rounded-lg"
        >
          <Icon icon="Check.png" class="h-4 w-4" />
          Marcar como Concluída
        </button>
        <button
          onclick={cancelarTarefa}
          class="flex w-full items-center gap-2 px-4 py-2 text-left text-sm text-(--foreground) transition-colors hover:bg-(--foreground)/10 rounded-lg"
        >
          <Icon icon="CloseCircle.png" class="h-4 w-4" />
          Cancelar Tarefa
        </button>
      </div>
    </div>
  {/if}
</div>