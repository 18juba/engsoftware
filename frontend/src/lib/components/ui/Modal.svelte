<script lang="ts">
	import type { Snippet } from 'svelte';
	import Icon from './Icon.svelte';

	interface ModalProps {
		open: boolean;
		title: string;
		onClose: () => void;
		children: Snippet;
	}

	let { open = $bindable(), title, onClose, children }: ModalProps = $props();

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
		<div
			class="absolute inset-0 bg-slate-950/30 backdrop-blur-xs"
			onclick={onClose}
			role="presentation"
			aria-hidden="true"
		></div>

		<div
			class="relative max-h-[90vh] overflow-y-scroll rounded-lg bg-(--background) p-6 md:min-w-4xl"
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

			{@render children()}
		</div>
	</div>
{/if}
