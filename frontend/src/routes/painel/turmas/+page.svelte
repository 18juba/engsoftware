<script lang="ts">
	import { onMount } from 'svelte';
	import type { Turma } from '$lib/types/turma';
	import type { Disciplina } from '$lib/types/disciplina';
	import { listarDisciplinas } from '$lib/services/disciplinaService';
	import {
		listarTurmas,
		criarTurma,
		atualizarTurma,
		deletarTurma
	} from '$lib/services/turmaService';

	// Estados de dados
	let turmas: Turma[] | null = null;
	let disciplinasCadastradas: Disciplina[] = [];
	let carregando = true;
	let turmaSelecionada: Turma | null = null;

	let exibindoFormulario = false;
	let modoEdicao = false;

	let disciplinaIdSelecionado: number = 0;

	let dadosFormulario: Turma = dadosIniciaisFormulario();

	function dadosIniciaisFormulario(): Turma {
		return {
			id: 0,
			semestre: '',
			horario: '',
			disciplina: {
				id: 0,
				nome: '',
				codigo: '',
				cargaHoraria: ''
			}
		};
	}

	async function inicializarDados() {
		carregando = true;
		try {
			const [respostaTurmas, respostaDisciplinas] = await Promise.all([
				listarTurmas(),
				listarDisciplinas()
			]);
			turmas = respostaTurmas;
			disciplinasCadastradas = respostaDisciplinas;
		} catch (error) {
			console.error('Erro ao carregar dados:', error);
			alert('Erro ao sincronizar dados com a API.');
		} finally {
			carregando = false;
		}
	}

	async function salvarTurma(e: Event) {
		e.preventDefault();

		if (disciplinaIdSelecionado === 0) {
			alert('Por favor, selecione uma disciplina válida.');
			return;
		}

		const payload: any = {
			id: dadosFormulario.id,
			semestre: dadosFormulario.semestre,
			horario: dadosFormulario.horario,
			disciplinaId: Number(disciplinaIdSelecionado),
			disciplina: {
				id: Number(disciplinaIdSelecionado)
			}
		};

		try {
			if (modoEdicao) {
				await atualizarTurma(payload.id, payload);
				alert('Turma atualizada com sucesso!');
			} else {
				await criarTurma(payload);
				alert('Turma criada com sucesso!');
			}
			fecharFormulario();

			const respostaTurmas = await listarTurmas();
			turmas = respostaTurmas;
		} catch (error) {
			console.error('Erro ao salvar turma:', error);
			alert('Erro ao salvar os dados da turma.');
		}
	}

	async function excluirTurma(id: number) {
		if (!confirm('Tem certeza que deseja deletar esta turma?')) return;

		try {
			await deletarTurma(id);
			alert('Turma removida!');
			turmaSelecionada = null;
			const respostaTurmas = await listarTurmas();
			turmas = respostaTurmas;
		} catch (error) {
			console.error('Erro ao deletar turma:', error);
			alert('Erro ao excluir a turma.');
		}
	}

	function iniciarCriacao() {
		modoEdicao = false;
		dadosFormulario = dadosIniciaisFormulario();
		disciplinaIdSelecionado = 0;
		exibindoFormulario = true;
	}

	function iniciarEdicao(turma: Turma) {
		modoEdicao = true;
		dadosFormulario = JSON.parse(JSON.stringify(turma));
		disciplinaIdSelecionado = turma.disciplina.id;
		exibindoFormulario = true;
	}

	function fecharFormulario() {
		exibindoFormulario = false;
		dadosFormulario = dadosIniciaisFormulario();
		disciplinaIdSelecionado = 0;
	}

	function abrirTurma(turma: Turma) {
		turmaSelecionada = { ...turma };
	}

	function voltar() {
		turmaSelecionada = null;
	}

	onMount(() => {
		inicializarDados();
	});
</script>

<div class="space-y-10 text-(--foreground)">
	{#if exibindoFormulario}
		<section
			class="mx-auto max-w-2xl rounded-xl border border-(--foreground)/10 bg-(--foreground)/5 p-6"
		>
			<h2 class="mb-6 text-2xl font-bold">
				{modoEdicao ? 'Editar Turma' : 'Nova Turma'}
			</h2>

			<form onsubmit={salvarTurma} class="space-y-6">
				<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
					<div>
						<label class="mb-1 block text-sm font-semibold" for="semestre">Semestre</label>
						<input
							id="semestre"
							type="text"
							placeholder="Ex: 2026.1"
							bind:value={dadosFormulario.semestre}
							required
							class="w-full rounded-lg border border-(--foreground)/20 bg-(--foreground)/10 p-2 text-sm focus:border-sky-500 focus:outline-none"
						/>
					</div>
					<div>
						<label class="mb-1 block text-sm font-semibold" for="horario">Horário</label>
						<input
							id="horario"
							type="text"
							placeholder="Ex: Ter/Qui 08:00"
							bind:value={dadosFormulario.horario}
							required
							class="w-full rounded-lg border border-(--foreground)/20 bg-(--foreground)/10 p-2 text-sm focus:border-sky-500 focus:outline-none"
						/>
					</div>
				</div>

				<div class="border-t border-(--foreground)/10 pt-4">
					<label class="mb-2 block text-sm font-bold" for="select-disciplina"
						>Selecione a Disciplina</label
					>

					{#if disciplinasCadastradas.length === 0}
						<p
							class="rounded-lg border border-amber-500/20 bg-amber-500/10 p-3 text-sm text-amber-500"
						>
							⚠️ Nenhuma disciplina encontrada. Cadastre uma disciplina antes de criar a turma.
						</p>
					{:else}
						<select
							id="select-disciplina"
							bind:value={disciplinaIdSelecionado}
							required
							class="w-full rounded-lg border border-(--foreground)/20 bg-(--foreground)/10 p-2.5 text-sm text-(--foreground) focus:border-sky-500 focus:outline-none"
						>
							<option value={0} disabled selected>-- Escolha uma Disciplina --</option>
							{#each disciplinasCadastradas as disc (disc.id)}
								<option value={disc.id}>
									{disc.nome} ({disc.codigo})
								</option>
							{/each}
						</select>
						<p class="mt-1 text-xs opacity-60">
							O identificador numérico interno da disciplina será enviado para a API.
						</p>
					{/if}
				</div>

				<div class="flex justify-end gap-3 pt-2">
					<button
						type="button"
						onclick={fecharFormulario}
						class="cursor-pointer rounded-lg bg-(--foreground)/10 px-4 py-2 text-sm font-bold transition hover:bg-(--foreground)/20"
					>
						Cancelar
					</button>
					<button
						type="submit"
						disabled={disciplinasCadastradas.length === 0}
						class="cursor-pointer rounded-lg bg-sky-600 px-5 py-2 text-sm font-bold text-white transition hover:bg-sky-700 disabled:cursor-not-allowed disabled:opacity-50"
					>
						Salvar Turma
					</button>
				</div>
			</form>
		</section>
	{:else if !turmaSelecionada}
		<section>
			<div class="mb-6 flex items-center justify-between">
				<div>
					<h2 class="text-2xl font-bold">Minhas Turmas</h2>
					<p class="text-sm opacity-60">Gerencie turmas vinculadas às disciplinas do sistema.</p>
				</div>
				<button
					onclick={iniciarCriacao}
					class="cursor-pointer rounded-lg bg-emerald-600 px-4 py-2 text-sm font-bold text-white transition hover:bg-emerald-700"
				>
					+ Criar Turma
				</button>
			</div>

			{#if carregando}
				<p class="animate-pulse opacity-60">Carregando turmas...</p>
			{:else if !turmas || turmas.length === 0}
				<p class="opacity-80">Você não possui turmas cadastradas.</p>
			{:else}
				<div class="grid grid-cols-1 gap-4 lg:grid-cols-3">
					{#each turmas as turma (turma.id)}
						<div
							class="flex flex-col gap-3 rounded-xl border border-(--foreground)/5 bg-(--foreground)/10 p-4"
						>
							<div class="flex items-start justify-between gap-2">
								<h3 class="text-lg leading-tight font-bold">{turma.disciplina.nome}</h3>
								<span class="shrink-0 rounded bg-(--primary) px-2 py-0.5 text-xs font-bold">
									{turma.semestre}
								</span>
							</div>

							<div class="space-y-1 text-sm opacity-90">
								<p><span class="font-semibold">Código:</span> {turma.disciplina.codigo}</p>
								<p><span class="font-semibold">Horário:</span> {turma.horario}</p>
							</div>

							<div class="mt-auto flex gap-2 pt-2">
								<button
									onclick={() => abrirTurma(turma)}
									class="flex-1 cursor-pointer rounded-lg bg-sky-600 px-3 py-1.5 text-xs font-bold text-white transition hover:bg-sky-700"
								>
									Gerenciar
								</button>
								<button
									onclick={() => iniciarEdicao(turma)}
									class="cursor-pointer rounded-lg bg-(--foreground)/10 px-3 py-1.5 text-xs font-bold transition hover:bg-(--foreground)/20"
								>
									Editar
								</button>
								<button
									onclick={() => excluirTurma(turma.id)}
									class="cursor-pointer rounded-lg bg-rose-600 px-3 py-1.5 text-xs font-bold text-white transition hover:bg-rose-700"
								>
									Excluir
								</button>
							</div>
						</div>
					{/each}
				</div>
			{/if}
		</section>
	{:else}
		<section>
			<div class="mb-6 flex items-start justify-between gap-4">
				<div>
					<h2 class="text-2xl font-bold">{turmaSelecionada.disciplina.nome}</h2>
					<p class="mt-1 text-sm opacity-80">
						{turmaSelecionada.disciplina.codigo} • Semestre {turmaSelecionada.semestre} • Horário: {turmaSelecionada.horario}
					</p>
				</div>
				<div class="flex gap-2">
					<button
						onclick={() => iniciarEdicao(turmaSelecionada!)}
						class="cursor-pointer rounded-lg bg-sky-600 px-4 py-2 text-sm font-bold text-white transition hover:bg-sky-700"
					>
						Editar Turma
					</button>
					<button
						onclick={voltar}
						class="cursor-lg rounded-lg bg-(--foreground)/10 px-4 py-2 text-sm font-bold transition hover:bg-(--foreground)/20"
					>
						Voltar
					</button>
				</div>
			</div>

			<div class="grid grid-cols-1 gap-4 lg:grid-cols-2">
				<div
					class="flex flex-col gap-3 rounded-xl border border-(--foreground)/10 bg-(--foreground)/5 p-5"
				>
					<h3 class="text-lg font-bold">📝 Atividades</h3>
					<p class="text-sm opacity-80">Gerencie as atividades e crie novas entregas.</p>
					<div class="mt-auto pt-4">
						<button
							onclick={() => alert('Nova atividade')}
							class="w-full cursor-pointer rounded-lg bg-emerald-600 px-3 py-2 text-sm font-bold text-white transition hover:bg-emerald-700"
						>
							Nova atividade
						</button>
					</div>
				</div>

				<div class="flex flex-col gap-3 rounded-xl border border-rose-500/20 bg-rose-500/10 p-5">
					<h3 class="text-lg font-bold text-rose-500">Excluir turma</h3>
					<p class="text-sm opacity-80">
						A exclusão da turma apagará permanentemente todos os registros vinculados.
					</p>
					<div class="mt-auto pt-4">
						<button
							onclick={() => excluirTurma(turmaSelecionada!.id)}
							class="w-full cursor-pointer rounded-lg bg-rose-600 px-3 py-2 text-sm font-bold text-white transition hover:bg-rose-700"
						>
							Excluir Turma Permanentemente
						</button>
					</div>
				</div>
			</div>
		</section>
	{/if}
</div>
