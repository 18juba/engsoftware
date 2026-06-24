<script lang="ts">
	import { onMount } from 'svelte';
	import type { Disciplina } from '$lib/types/disciplina';
	import {
		listarDisciplinas,
		criarDisciplina,
		atualizarDisciplina,
		deletarDisciplina
	} from '$lib/services/disciplinaService';

	let disciplinas: Disciplina[] | null = null;
	let carregando = true;
	let exibindoFormulario = false;
	let modoEdicao = false;

	let dadosFormulario: Disciplina = dadosIniciaisFormulario();

	function dadosIniciaisFormulario(): Disciplina {
		return {
			id: 0,
			nome: '',
			codigo: '',
			cargaHoraria: ''
		};
	}

	async function carregarDisciplinas() {
		carregando = true;
		try {
			disciplinas = await listarDisciplinas();
		} catch (error) {
			console.error('Erro ao listar disciplinas:', error);
			alert('Erro ao carregar a lista de disciplinas.');
		} finally {
			carregando = false;
		}
	}

	async function salvarDisciplina(e: Event) {
		e.preventDefault();
		try {
			if (modoEdicao) {
				await atualizarDisciplina(dadosFormulario.id, dadosFormulario);
				alert('Disciplina atualizada com sucesso!');
			} else {
				await criarDisciplina(dadosFormulario);
				alert('Disciplina criada com sucesso!');
			}
			fecharFormulario();
			await carregarDisciplinas();
		} catch (error) {
			console.error('Erro ao salvar disciplina:', error);
			alert('Erro ao salvar os dados da disciplina.');
		}
	}

	async function excluirDisciplina(id: number) {
		if (
			!confirm(
				'Tem certeza que deseja deletar esta disciplina? Turmas vinculadas a ela podem ser afetadas.'
			)
		)
			return;

		try {
			await deletarDisciplina(id);
			alert('Disciplina removida com sucesso!');
			await carregarDisciplinas();
		} catch (error) {
			console.error('Erro ao deletar disciplina:', error);
			alert('Erro ao excluir a disciplina. Verifique se ela não está vinculada a alguma turma.');
		}
	}

	function iniciarCriacao() {
		modoEdicao = false;
		dadosFormulario = dadosIniciaisFormulario();
		exibindoFormulario = true;
	}

	function iniciarEdicao(disciplina: Disciplina) {
		modoEdicao = true;
		dadosFormulario = { ...disciplina };
		exibindoFormulario = true;
	}

	function fecharFormulario() {
		exibindoFormulario = false;
		dadosFormulario = dadosIniciaisFormulario();
	}

	onMount(() => {
		carregarDisciplinas();
	});
</script>

<div class="space-y-10 text-(--foreground)">
	{#if exibindoFormulario}
		<section
			class="mx-auto max-w-xl rounded-xl border border-(--foreground)/10 bg-(--foreground)/5 p-6"
		>
			<h2 class="mb-6 text-2xl font-bold">
				{modoEdicao ? 'Editar Disciplina' : 'Nova Disciplina'}
			</h2>

			<form onsubmit={salvarDisciplina} class="space-y-4">
				<div>
					<label class="mb-1 block text-sm font-semibold" for="nome">Nome da Disciplina</label>
					<input
						id="nome"
						type="text"
						placeholder="Ex: Estrutura de Dados"
						bind:value={dadosFormulario.nome}
						required
						class="w-full rounded-lg border border-(--foreground)/20 bg-(--foreground)/10 p-2.5 text-sm focus:border-sky-500 focus:outline-none"
					/>
				</div>

				<div class="grid grid-cols-1 gap-4 md:grid-cols-2">
					<div>
						<label class="mb-1 block text-sm font-semibold" for="codigo">Código</label>
						<input
							id="codigo"
							type="text"
							placeholder="Ex: ED01"
							bind:value={dadosFormulario.codigo}
							required
							class="w-full rounded-lg border border-(--foreground)/20 bg-(--foreground)/10 p-2.5 text-sm focus:border-sky-500 focus:outline-none"
						/>
					</div>
					<div>
						<label class="mb-1 block text-sm font-semibold" for="cargaHoraria">Carga Horária</label>
						<input
							id="cargaHoraria"
							type="text"
							placeholder="Ex: 80h"
							bind:value={dadosFormulario.cargaHoraria}
							required
							class="w-full rounded-lg border border-(--foreground)/20 bg-(--foreground)/10 p-2.5 text-sm focus:border-sky-500 focus:outline-none"
						/>
					</div>
				</div>

				<div class="flex justify-end gap-3 pt-4">
					<button
						type="button"
						onclick={fecharFormulario}
						class="cursor-pointer rounded-lg bg-(--foreground)/10 px-4 py-2 text-sm font-bold transition hover:bg-(--foreground)/20"
					>
						Cancelar
					</button>
					<button
						type="submit"
						class="cursor-pointer rounded-lg bg-sky-600 px-5 py-2 text-sm font-bold text-white transition hover:bg-sky-700"
					>
						Salvar Disciplina
					</button>
				</div>
			</form>
		</section>
	{:else}
		<section>
			<div class="mb-6 flex items-center justify-between">
				<div>
					<h2 class="text-2xl font-bold">Gerenciamento de Disciplinas</h2>
					<p class="text-sm opacity-60">
						Cadastre disciplinas para poder vinculá-las às suas turmas posteriormente.
					</p>
				</div>
				<button
					onclick={iniciarCriacao}
					class="cursor-pointer rounded-lg bg-emerald-600 px-4 py-2 text-sm font-bold whitespace-nowrap text-white transition hover:bg-emerald-700"
				>
					+ Nova Disciplina
				</button>
			</div>

			{#if carregando}
				<p class="animate-pulse opacity-60">Carregando disciplinas...</p>
			{:else if !disciplinas || disciplinas.length === 0}
				<div class="rounded-xl border border-dashed border-(--foreground)/20 p-8 text-center">
					<p class="opacity-80">Nenhuma disciplina cadastrada no sistema.</p>
				</div>
			{:else}
				<div class="grid grid-cols-1 gap-4 lg:grid-cols-3">
					{#each disciplinas as disciplina (disciplina.id)}
						<div
							class="flex flex-col gap-3 rounded-xl border border-(--foreground)/5 bg-(--foreground)/10 p-4 text-(--foreground)"
						>
							<div class="flex items-start justify-between gap-2">
								<h3 class="text-lg leading-tight font-bold">{disciplina.nome}</h3>
								<span class="shrink-0 rounded bg-(--primary) px-2 py-0.5 text-xs font-bold">
									{disciplina.codigo}
								</span>
							</div>

							<div class="text-sm opacity-90">
								<p>
									<span class="text-xs font-semibold tracking-wider uppercase opacity-60"
										>Carga Horária:</span
									>
									{disciplina.cargaHoraria}
								</p>
							</div>

							<div class="mt-auto flex gap-2 border-t border-(--foreground)/5 pt-3">
								<button
									onclick={() => iniciarEdicao(disciplina)}
									class="flex-1 cursor-pointer rounded-lg bg-sky-500 px-3 py-1.5 text-xs font-bold transition hover:bg-sky-600"
								>
									Editar
								</button>
								<button
									onclick={() => excluirDisciplina(disciplina.id)}
									class="cursor-pointer rounded-lg bg-rose-500 px-3 py-1.5 text-xs font-bold transition hover:bg-rose-600"
								>
									Excluir
								</button>
							</div>
						</div>
					{/each}
				</div>
			{/if}
		</section>
	{/if}
</div>
