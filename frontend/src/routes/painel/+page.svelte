<script lang="ts">
    import { onMount } from "svelte";
    import { getDashboard } from "$lib/services/userService";
    import type { UserDashboard } from "$lib/types/user";

    let dashboardData: UserDashboard | null = $state(null);
    let isLoading = $state(true);

    async function loadDashboard() {
        try {
            dashboardData = await getDashboard();
        } catch (error) {
            console.error("Error loading dashboard:", error);
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
    <p>Tarefas concluidas: {dashboardData.total_completed_tasks ?? 0}</p>
{/if}
