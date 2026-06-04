<script lang="ts">
	import { parseTimeToParts } from '$lib/utils/timeInput';

	interface DatePickerProps {
		open: boolean;
		value: string;
		onSelect: (time: string) => void;
		onClose: () => void;
	}

	let { open = $bindable(), value, onSelect, onClose }: DatePickerProps = $props();

	let selectedHour = $state(0);
	let selectedMinute = $state(0);
	let pickerElement = $state<HTMLDivElement | null>(null);

	$effect(() => {
		if (!open) return;
		const { hour, minute } = parseTimeToParts(value);
		selectedHour = hour;
		selectedMinute = minute;
	});

	$effect(() => {
		if (!open || typeof document === 'undefined') return;

		const closeOnOutsideClick = (event: PointerEvent) => {
			const target = event.target as Node | null;
			if (!target || pickerElement?.contains(target)) return;
			onClose();
		};

		const closeOnEscape = (event: KeyboardEvent) => {
			if (event.key === 'Escape') {
				onClose();
			}
		};

		document.addEventListener('pointerdown', closeOnOutsideClick);
		document.addEventListener('keydown', closeOnEscape);

		return () => {
			document.removeEventListener('pointerdown', closeOnOutsideClick);
			document.removeEventListener('keydown', closeOnEscape);
		};
	});

	function formatTwoDigits(numberValue: number): string {
		return String(numberValue).padStart(2, '0');
	}

	function adjustHour(delta: number) {
		selectedHour = (selectedHour + delta + 24) % 24;
	}

	function adjustMinute(delta: number) {
		selectedMinute = (selectedMinute + delta + 60) % 60;
	}

	function applySelectedTime() {
		onSelect(`${formatTwoDigits(selectedHour)}:${formatTwoDigits(selectedMinute)}`);
		onClose();
	}

	function setBrazilNow() {
		const nowInBrazil = new Date(
			new Date().toLocaleString('en-US', { timeZone: 'America/Sao_Paulo' })
		);
		selectedHour = nowInBrazil.getHours();
		selectedMinute = nowInBrazil.getMinutes();
		applySelectedTime();
	}
</script>

{#if open}
	<div
		bind:this={pickerElement}
		class="absolute right-0 bottom-full z-9999 max-h-[calc(100vh-1rem)] w-64 max-w-[calc(100vw-1rem)] overflow-y-auto rounded-lg border border-(--foreground)/15 bg-(--background) p-3 shadow-2xl"
	>
		<div class="mb-3 flex items-center justify-between">
			<p class="text-sm font-semibold">Selecionar horário</p>
			<button
				type="button"
				class="cursor-pointer rounded bg-(--foreground)/10 px-2 py-1 text-xs transition-colors hover:bg-(--foreground)/20"
				onclick={onClose}
			>
				Fechar
			</button>
		</div>

		<div class="mb-3 grid grid-cols-2 gap-3">
			<div class="rounded-lg bg-(--foreground)/5 p-2">
				<p class="mb-2 text-center text-xs text-(--foreground)/70">Hora</p>
				<div class="flex items-center justify-between">
					<button
						type="button"
						class="h-8 w-8 cursor-pointer rounded-md bg-(--foreground)/10 transition-colors hover:bg-(--foreground)/20"
						onclick={() => adjustHour(-1)}
						aria-label="Diminuir hora"
					>
						-
					</button>
					<span class="text-lg font-bold">{formatTwoDigits(selectedHour)}</span>
					<button
						type="button"
						class="h-8 w-8 cursor-pointer rounded-md bg-(--foreground)/10 transition-colors hover:bg-(--foreground)/20"
						onclick={() => adjustHour(1)}
						aria-label="Aumentar hora"
					>
						+
					</button>
				</div>
			</div>

			<div class="rounded-lg bg-(--foreground)/5 p-2">
				<p class="mb-2 text-center text-xs text-(--foreground)/70">Minuto</p>
				<div class="flex items-center justify-between">
					<button
						type="button"
						class="h-8 w-8 cursor-pointer rounded-md bg-(--foreground)/10 transition-colors hover:bg-(--foreground)/20"
						onclick={() => adjustMinute(-5)}
						aria-label="Diminuir minuto"
					>
						-
					</button>
					<span class="text-lg font-bold">{formatTwoDigits(selectedMinute)}</span>
					<button
						type="button"
						class="h-8 w-8 cursor-pointer rounded-md bg-(--foreground)/10 transition-colors hover:bg-(--foreground)/20"
						onclick={() => adjustMinute(5)}
						aria-label="Aumentar minuto"
					>
						+
					</button>
				</div>
			</div>
		</div>

		<div class="flex gap-2">
			<button
				type="button"
				class="flex-1 cursor-pointer rounded-md bg-(--foreground)/10 px-3 py-2 text-sm transition-colors hover:bg-(--foreground)/20"
				onclick={setBrazilNow}
			>
				Agora
			</button>
			<button
				type="button"
				class="flex-1 cursor-pointer rounded-md bg-(--primary) px-3 py-2 text-sm font-semibold text-white transition-colors hover:bg-(--primary)/90"
				onclick={applySelectedTime}
			>
				Aplicar
			</button>
		</div>
	</div>
{/if}
