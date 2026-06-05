<script lang="ts">
	import { onMount } from 'svelte';
	import { Chart } from 'chart.js/auto';

	export type ToDoTasksPieChartData = {
        total_tasks: number;
		labels: string[];
		datasets: {
			data: number[];
		}[];
	};

	let { data, total_tasks }: { data: ToDoTasksPieChartData; total_tasks: number } = $props();

	let canvas: HTMLCanvasElement;
	let chart: Chart;

	onMount(() => {
		const chartData = {
			labels: [...data.labels],
			datasets: data.datasets.map((dataset) => ({
				...dataset,
				data: [...dataset.data],

backgroundColor: [
	'rgba(14, 165, 233, 0.8)',  // Em andamento
	'rgba(115, 115, 115, 0.8)', // Agendada
	'rgba(100, 116, 139, 0.8)'  // Pausada
],

borderColor: [
	'rgb(14, 165, 233)',
	'rgb(115, 115, 115)',
	'rgb(100, 116, 139)'
],

borderWidth: 2,
hoverBorderWidth: 3
			}))
		};

		chart = new Chart(canvas, {
			type: 'pie',
			data: chartData,
			options: {
				responsive: true,
				maintainAspectRatio: false,

				plugins: {
					legend: {
						position: 'bottom',
						labels: {
							padding: 20,
							usePointStyle: true,
							pointStyle: 'circle'
						}
					},

					tooltip: {
						callbacks: {
							label(context) {
								const total = context.dataset.data.reduce(
									(acc, value) => acc + Number(value),
									0
								);

								const value = Number(context.raw);
								const percentage = (
									(value / total) *
									100
								).toFixed(1);

								return `${context.label}: ${value} (${percentage}%)`;
							}
						}
					}
				}
			}
		});

		return () => chart?.destroy();
	});
</script>

<div
	class="bg-white dark:bg-zinc-900 rounded-xl border border-zinc-200 dark:border-zinc-800 p-6 shadow-sm"
>
	<h3 class="text-lg font-semibold">
		Distribuição das tarefas
	</h3>
    <p class="mb-4">Total: {total_tasks}</p>

	<div class="w-full max-w-sm h-72 mx-auto">
		<canvas bind:this={canvas}></canvas>
	</div>
</div>