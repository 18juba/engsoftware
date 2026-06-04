<script lang="ts">
	import Icon from './Icon.svelte';

	interface DialogProps {
		open: boolean;
		title: string;
		description: string;
		onClose: () => void;
		onConfirm?: () => void;
	}

	let { open = $bindable(), title, description, onClose, onConfirm }: DialogProps = $props();

	function handleKeydown(e: KeyboardEvent) {
		if (e.key === 'Escape' && open) {
			onClose();
		}
	}

	// Prevenir scroll do body quando modal está aberto
	$effect(() => {
		if (open) {
			document.body.style.overflow = 'hidden';
		} else {
			document.body.style.overflow = '';
		}

		return () => {
			document.body.style.overflow = '';
		};
	});
</script>

<svelte:window onkeydown={handleKeydown} />

{#if open}
	<div class="fixed inset-0 z-50 flex items-center justify-center px-2 sm:px-0" role="presentation">
		<!-- Backdrop -->
		<div
			class="absolute inset-0 bg-slate-950/30 backdrop-blur-xs"
			onclick={onClose}
			role="presentation"
			aria-hidden="true"
		></div>

		<!-- Modal Content -->
		<div
			class="relative overflow-y-scroll rounded-lg bg-(--background) p-6 md:w-md"
			onclick={(e) => e.stopPropagation()}
			role="dialog"
			aria-modal="true"
			aria-labelledby="modal-title"
			tabindex="-1"
			onkeydown={(e) => {
				if (e.key === 'Escape') {
					onClose();
				}
			}}
		>
			<!-- Header -->
			<div class="mb-4 flex items-center justify-between gap-12">
				<h1 id="modal-title" class="text-xl font-bold sm:text-2xl">
					{title}
				</h1>
				<button
					type="button"
					class="shrink-0 cursor-pointer rounded-full p-1 transition-opacity hover:opacity-75"
					onclick={onClose}
					aria-label="Fechar modal"
					title="Fechar (ESC)"
				>
					<Icon
						icon="CloseCircle.png"
						class="h-7 w-7 sm:h-8 sm:w-8"
						ariaLabel="Ícone de fechar"
						onClick={onClose}
					/>
				</button>
			</div>

			<p class="mb-6 text-sm text-white/60">{description}</p>

			<div class="flex justify-end gap-4">
				<button
					class="cursor-pointer rounded-lg bg-(--foreground)/10 px-4 py-2 text-sm text-white transition-colors hover:bg-(--foreground)/20"
					onclick={onClose}
				>
					Cancelar
				</button>
				<button
					class="cursor-pointer rounded-lg bg-(--primary) px-4 py-2 font-semibold text-white transition-colors duration-200 hover:bg-(--primary)/90"
					onclick={onConfirm}
				>
					Confirmar
				</button>
			</div>
		</div>
	</div>
{/if}
