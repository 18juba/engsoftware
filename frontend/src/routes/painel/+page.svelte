<script lang="ts">
	import { onMount } from 'svelte';
	import { getDashboard } from '$lib/services/userService';
	import type { UserDashboard } from '$lib/types/user';
	import type { ToDoTasksPieChartData } from '$lib/components/charts/ToDoTasksPieChart.svelte';
	import ToDoTasksPieChart from '$lib/components/charts/ToDoTasksPieChart.svelte';

	let dashboardData: UserDashboard | null = $state(null);
	let isLoading = $state(true);
	let chartData: ToDoTasksPieChartData | null = $state.raw(null);

	async function loadDashboard() {
		try {
			dashboardData = await getDashboard();

			chartData = {
				total_tasks: dashboardData.to_do_tasks_total,
				labels: ['Em andamento', 'Agendada', 'Pausada'],
				datasets: [
					{
						data: dashboardData.to_do_tasks_analysis.map((item) => item.count)
					}
				]
			};
		} catch (error) {
			console.error('Error loading dashboard:', error);
		} finally {
			isLoading = false;
		}
	}

	onMount(() => {
		loadDashboard();
	});
</script>

{#if isLoading}
	<p>Carregando dashboard...</p>
{/if}

{#if dashboardData}
	{#if chartData}
		<div class="max-w-md">
			<ToDoTasksPieChart data={chartData} total_tasks={chartData?.total_tasks} />
		</div>
	{/if}
{/if}
