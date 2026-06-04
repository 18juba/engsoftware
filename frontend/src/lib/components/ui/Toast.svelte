<script lang="ts">
	import Icon from './Icon.svelte';

	type ToastType = 'success' | 'danger' | 'info';

	interface ToastProps {
		open: boolean;
		type: ToastType;
		title: string;
		description: string;
		onClose: () => void;
		duration?: number; // em milissegundos
	}

	let {
		open = $bindable(),
		type = 'info',
		title,
		description,
		onClose,
		duration = 6000
	}: ToastProps = $props();

	$effect(() => {
		if (open && duration) {
			const timer = setTimeout(() => {
				onClose();
			}, duration);

			return () => clearTimeout(timer);
		}
	});

	const iconConfig: Record<ToastType, string> = {
		success: 'Check.png',
		danger: 'Danger.png',
		info: 'Info.png'
	};

	const colorConfig: Record<ToastType, string> = {
		success: 'border-l-4 border-emerald-700',
		danger: 'border-l-4 border-rose-700',
		info: 'border-l-4 border-sky-700'
	};

	const titleColorConfig: Record<ToastType, string> = {
		success: 'text-green-400',
		danger: 'text-red-400',
		info: 'text-blue-400'
	};
</script>

{#if open}
	<div
		class="animate-in fade-in slide-in-from-top-2 duration-300"
		role="status"
		aria-live="assertive"
		aria-atomic="true"
	>
		<div class={`w-full rounded-lg bg-slate-800 p-4 shadow-lg ${colorConfig[type]}`}>
			<div class="flex gap-3">
				<!-- Icon -->
				<div class="shrink-0 pt-0.5">
					<Icon icon={iconConfig[type]} class="h-7 w-7" ariaLabel={type} />
				</div>

				<!-- Content -->
				<div class="flex-1">
					<h3 class={`font-bold ${titleColorConfig[type]}`}>
						{title}
					</h3>
					<p class="mt-1 text-sm text-white/90">
						{description}
					</p>
				</div>

				<button
					type="button"
					class="shrink-0 cursor-pointer rounded-full p-1 transition-opacity hover:opacity-75"
					onclick={onClose}
					aria-label="Fechar notificação"
					title="Fechar"
				>
					<Icon icon="Close.png" class="h-5 w-5" ariaLabel="Fechar" />
				</button>
			</div>
		</div>
	</div>
{/if}

<style>
	@media (prefers-reduced-motion: no-preference) {
		:global(.animate-in) {
			animation: fadeInSlide 0.3s ease-out;
		}
	}

	@keyframes fadeInSlide {
		from {
			opacity: 0;
			transform: translateY(1rem);
		}
		to {
			opacity: 1;
			transform: translateY(0);
		}
	}
</style>
