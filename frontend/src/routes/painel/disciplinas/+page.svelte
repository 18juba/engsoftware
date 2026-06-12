<script lang="ts">
	import Icon from "$lib/components/ui/Icon.svelte";

	let minhas_disciplinas = [
		{
			id: 1,
			nome: 'Estrutura de Dados',
			codigo: 'ED',
			carga_horaria: 80,
			professor: 'Professor X'
		},
		{ id: 2, nome: 'Banco de Dados', codigo: 'BD', carga_horaria: 80, professor: 'Professor Y' }
	];

	let disciplinas = [
		{
			id: 3,
			nome: 'Visão Computacional',
			codigo: 'VC',
			carga_horaria: 80,
			professor: 'Professor Z'
		},
		{
			id: 4,
			nome: 'Sistemas Operacionais',
			codigo: 'SO',
			carga_horaria: 80,
			professor: 'Professor W'
		}
	];

	function sair(id: number) {
		minhas_disciplinas = minhas_disciplinas.filter((d) => d.id !== id);
	}

	function verProgresso(disciplina: any) {
		alert(`Progresso de ${disciplina.nome}`);
	}

	function verDetalhes(disciplina: any) {
		alert(`Detalhes de ${disciplina.nome}`);
	}

	function ingressar(id: number) {
		const disciplina = disciplinas.find((d) => d.id === id);
		if (disciplina) {
			minhas_disciplinas = [...minhas_disciplinas, disciplina];
			disciplinas = disciplinas.filter((d) => d.id !== id);
		}
	}
</script>

<div class="space-y-10 text-(--foreground)">
	<!-- Minhas Disciplinas -->
	<section>
		<h2 class="mb-4 text-2xl font-bold">Minhas Disciplinas</h2>

		{#if minhas_disciplinas.length === 0}
			<p class="opacity-80">Você não está inscrito em nenhuma disciplina.</p>
		{:else}
			<div class="grid grid-cols-1 gap-4 lg:grid-cols-3">
				{#each minhas_disciplinas as disciplina (disciplina.id)}
					<div class="flex flex-col gap-3 rounded-xl bg-(--foreground)/10 p-4 text-(--foreground)">
						<div class="flex items-center justify-between">
							<h3 class="mb-1 text-lg font-bold">
								{disciplina.nome} ({disciplina.carga_horaria}H)
							</h3>
							<span class="rounded bg-(--primary) px-2 py-0.5 text-xs font-bold">
								{disciplina.codigo}
							</span>
						</div>

						<p><span class="font-bold">Professor:</span> {disciplina.professor}</p>

						<div class="flex flex-wrap gap-4">
							<button
								onclick={() => verProgresso(disciplina)}
								class="flex-3 cursor-pointer rounded-lg bg-sky-600 px-3 py-2 text-sm font-bold transition hover:bg-sky-700"
							>
								Ver progresso
							</button>
							<button
								onclick={() => sair(disciplina.id)}
								class="flex-1 cursor-pointer rounded-lg bg-rose-600 px-3 py-2 text-sm font-bold transition hover:bg-rose-700"
							>
								Sair
							</button>
						</div>
					</div>
				{/each}
			</div>
		{/if}
	</section>

	<!-- Disciplinas Disponíveis -->
	<section>
		<h2 class="mb-4 text-2xl font-bold">Disciplinas Disponíveis</h2>
		{#if disciplinas.length === 0}
			<p class="opacity-80">Não há disciplinas disponíveis para inscrição.</p>
		{:else}
			<div class="grid grid-cols-1 gap-4 lg:grid-cols-3">
				{#each disciplinas as disciplina (disciplina.id)}
					<div class="flex flex-col gap-3 rounded-xl bg-(--foreground)/10 p-4 text-(--foreground)">
						<div class="flex items-center justify-between">
							<h3 class="mb-1 text-lg font-bold">
								{disciplina.nome} ({disciplina.carga_horaria}H)
							</h3>
							<span class="rounded bg-(--primary) px-2 py-0.5 text-xs font-bold">
								{disciplina.codigo}
							</span>
						</div>
						<p><span class="font-bold">Professor:</span> {disciplina.professor}</p>
						<div class="flex flex-wrap gap-4">
							<button
								onclick={() => verDetalhes(disciplina)}
								class="flex-3 cursor-pointer rounded-lg bg-sky-600 px-3 py-2 text-sm font-bold transition hover:bg-sky-700"
							>
								Ver detalhes
							</button>
							<button
								onclick={() => ingressar(disciplina.id)}
								class="flex-1 cursor-pointer rounded-lg bg-emerald-600 px-3 py-2 text-sm font-bold transition hover:bg-emerald-700"
							>
								Ingressar
							</button>
						</div>
					</div>
				{/each}
			</div>
		{/if}
	</section>
</div>
