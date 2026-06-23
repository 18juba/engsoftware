<script lang="ts">
	import { apiFetch } from '$lib/api/api';
	import { onMount } from 'svelte';

	let turmas: {
		id: number;
		nome: string;
		semestre: string;
		disciplina: {
			id: number,
			nome: string,
			codigo: string,
			cargaHoraria: string
		};
		horario: string;
		alunos: string[];
	}[] = [];

	async function buscarTurmas() {
		const response = await apiFetch('/Turmas');

		turmas = await response.json();
		console.log("turmas: ", turmas)
	}

	let turmaSelecionada: (typeof turmas)[0] | null = null;

	function abrirTurma(turma: (typeof turmas)[0]) {
		turmaSelecionada = turma;
	}

	function voltar() {
		turmaSelecionada = null;
	}

	function novaAtividade(turmaId: number) {
		alert(`Nova atividade para a turma ${turmaId}`);
	}

	function corrigirAtividades(turmaId: number) {
		alert(`Corrigir atividades da turma ${turmaId}`);
	}

	onMount(() => {
		buscarTurmas();
	});
</script>

<div class="space-y-10 text-(--foreground)">
	{#if !turmaSelecionada}
		<section>
			<h2 class="mb-4 text-2xl font-bold">Minhas Turmas</h2>

			{#if turmas.length === 0}
				<p class="opacity-80">Você não possui turmas cadastradas.</p>
			{:else}
				<div class="grid grid-cols-1 gap-4 lg:grid-cols-3">
					{#each turmas as turma (turma.id)}
						<div
							class="flex flex-col gap-3 rounded-xl bg-(--foreground)/10 p-4 text-(--foreground)"
						>
							<div class="flex items-center justify-between">
								<h3 class="text-lg font-bold">{turma.nome}</h3>
								<span class="rounded bg-(--primary) px-2 py-0.5 text-xs font-bold">
									{turma.semestre}
								</span>
							</div>

							<div class="space-y-1 text-sm">
								<p><span class="font-bold">Disciplina:</span> {turma.disciplina.nome}</p>
							</div>

							<div class="mt-auto flex flex-wrap gap-4 pt-2">
								<button
									onclick={() => abrirTurma(turma)}
									class="flex-3 cursor-pointer rounded-lg bg-sky-600 px-3 py-2 text-sm font-bold transition hover:bg-sky-700"
								>
									Gerenciar
								</button>
								<button
									onclick={() => novaAtividade(turma.id)}
									class="flex-1 cursor-pointer rounded-lg bg-emerald-600 px-3 py-2 text-sm font-bold transition hover:bg-emerald-700"
								>
									+ Atividade
								</button>
							</div>
						</div>
					{/each}
				</div>
			{/if}
		</section>
	{:else}
		<section>
			<div class="mb-4 flex items-center justify-between">
				<div>
					<h2 class="text-2xl font-bold">{turmaSelecionada.nome}</h2>
					<p class="text-sm opacity-80">
						{turmaSelecionada.disciplina.nome} • {turmaSelecionada.curso} • {turmaSelecionada.semestre}
					</p>
				</div>
				<button
					onclick={voltar}
					class="cursor-pointer rounded-lg bg-(--foreground)/10 px-3 py-2 text-sm font-bold transition hover:bg-(--foreground)/20"
				>
					Voltar
				</button>
			</div>

			<div class="grid grid-cols-1 gap-4 lg:grid-cols-3">
				<div class="flex flex-col gap-3 rounded-xl bg-(--foreground)/10 p-4">
					<h3 class="text-lg font-bold">Atividades</h3>
					<p class="text-sm opacity-80">
						Gerencie as atividades desta turma e crie novas entregas.
					</p>
					<div class="mt-auto flex gap-4 pt-2">
						<button
							onclick={() => novaAtividade(turmaSelecionada!.id)}
							class="flex-1 cursor-pointer rounded-lg bg-emerald-600 px-3 py-2 text-sm font-bold transition hover:bg-emerald-700"
						>
							Nova atividade
						</button>
					</div>
				</div>

				<div class="flex flex-col gap-3 rounded-xl bg-(--foreground)/10 p-4">
					<h3 class="text-lg font-bold">Correção</h3>
					<p class="text-sm opacity-80">Veja as entregas dos alunos e atribua notas.</p>
					<div class="mt-auto flex gap-4 pt-2">
						<button
							onclick={() => corrigirAtividades(turmaSelecionada!.id)}
							class="flex-1 cursor-pointer rounded-lg bg-sky-600 px-3 py-2 text-sm font-bold transition hover:bg-sky-700"
						>
							Corrigir entregas
						</button>
					</div>
				</div>

				<div class="flex flex-col gap-3 rounded-xl bg-(--foreground)/10 p-4">
					<h3 class="text-lg font-bold">Alunos ({turmaSelecionada.alunos?.length})</h3>
					<ul class="space-y-1 text-sm">
						{#each turmaSelecionada.alunos as aluno}
							<li class="rounded bg-(--foreground)/10 px-2 py-1">{aluno}</li>
						{/each}
					</ul>
				</div>
			</div>
		</section>
	{/if}
</div>