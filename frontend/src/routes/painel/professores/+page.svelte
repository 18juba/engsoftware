<script lang="ts">
    import { onMount } from 'svelte';
    import type { Professor } from '$lib/types/professor';
    import { 
        listarProfessores, 
        criarProfessor, 
        atualizarProfessor, 
        deletarProfessor 
    } from '$lib/services/professorService';

    let professores: Professor[] | null = null;
    let carregando = true;
    let exibindoFormulario = false;
    let modoEdicao = false;

    let dadosFormulario: Professor = dadosIniciaisFormulario();

    function dadosIniciaisFormulario(): Professor {
        return {
            id: 0,
            nome: '',
            email: '',
            siape: '',
            departamento: ''
        };
    }

    async function carregarProfessores() {
        carregando = true;
        try {
            professores = await listarProfessores();
        } catch (error) {
            console.error("Erro ao listar professores:", error);
            alert("Erro ao carregar a lista de professores.");
        } finally {
            carregando = false;
        }
    }

    async function salvarProfessor(e: Event) {
        e.preventDefault();
        try {
            if (modoEdicao) {
                await atualizarProfessor(dadosFormulario.id, dadosFormulario);
                alert("Professor atualizado com sucesso!");
            } else {
                await criarProfessor(dadosFormulario);
                alert("Professor cadastrado com sucesso!");
            }
            fecharFormulario();
            await carregarProfessores();
        } catch (error) {
            console.error("Erro ao salvar professor:", error);
            alert("Erro ao salvar os dados do professor.");
        }
    }

    async function excluirProfessor(id: number) {
        if (!confirm("Tem certeza que deseja remover este professor do sistema?")) return;
        
        try {
            await deletarProfessor(id);
            alert("Professor removido com sucesso!");
            await carregarProfessores();
        } catch (error) {
            console.error("Erro ao deletar professor:", error);
            alert("Erro ao excluir o professor. Verifique se ele não possui vínculos com turmas vigentes.");
        }
    }

    function iniciarCriacao() {
        modoEdicao = false;
        dadosFormulario = dadosIniciaisFormulario();
        exibindoFormulario = true;
    }

    function iniciarEdicao(professor: Professor) {
        modoEdicao = true;
        dadosFormulario = { ...professor };
        exibindoFormulario = true;
    }

    function fecharFormulario() {
        exibindoFormulario = false;
        dadosFormulario = dadosIniciaisFormulario();
    }

    onMount(() => {
        carregarProfessores();
    });
</script>

<div class="space-y-10 text-(--foreground)">
    
    {#if exibindoFormulario}
        <section class="max-w-2xl mx-auto bg-(--foreground)/5 border border-(--foreground)/10 rounded-xl p-6">
            <h2 class="text-2xl font-bold mb-6">
                {modoEdicao ? 'Editar Dados do Professor' : 'Cadastrar Novo Professor'}
            </h2>

            <form onsubmit={salvarProfessor} class="space-y-4">
                <div>
                    <label class="block text-sm font-semibold mb-1" for="nome">Nome Completo</label>
                    <input id="nome" type="text" placeholder="Ex: Dr. Alan Turing" bind:value={dadosFormulario.nome} required
                        class="w-full bg-(--foreground)/10 border border-(--foreground)/20 rounded-lg p-2.5 text-sm focus:outline-none focus:border-sky-500" />
                </div>

                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <label class="block text-sm font-semibold mb-1" for="siape">SIAPE</label>
                        <input id="siape" type="text" placeholder="Ex: 1234567" bind:value={dadosFormulario.siape} required
                            class="w-full bg-(--foreground)/10 border border-(--foreground)/20 rounded-lg p-2.5 text-sm focus:outline-none focus:border-sky-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-semibold mb-1" for="departamento">Departamento</label>
                        <input id="departamento" type="text" placeholder="Ex: DC - Departamento de Computação" bind:value={dadosFormulario.departamento} required
                            class="w-full bg-(--foreground)/10 border border-(--foreground)/20 rounded-lg p-2.5 text-sm focus:outline-none focus:border-sky-500" />
                    </div>
                </div>

                <div>
                    <label class="block text-sm font-semibold mb-1" for="email">E-mail Institucional</label>
                    <input id="email" type="email" placeholder="Ex: professor.alan@universidade.com" bind:value={dadosFormulario.email} required
                        class="w-full bg-(--foreground)/10 border border-(--foreground)/20 rounded-lg p-2.5 text-sm focus:outline-none focus:border-sky-500" />
                </div>

                <div class="flex justify-end gap-3 pt-4">
                    <button type="button" onclick={fecharFormulario}
                        class="cursor-pointer rounded-lg bg-(--foreground)/10 px-4 py-2 text-sm font-bold transition hover:bg-(--foreground)/20">
                        Cancelar
                    </button>
                    <button type="submit"
                        class="cursor-pointer rounded-lg bg-sky-600 px-5 py-2 text-sm font-bold text-white transition hover:bg-sky-700">
                        Salvar Professor
                    </button>
                </div>
            </form>
        </section>

    {:else}
        <section>
            <div class="mb-6 flex items-center justify-between gap-4">
                <div>
                    <h2 class="text-2xl font-bold">Corpo Docente</h2>
                    <p class="text-sm opacity-60">Gerencie o cadastro de professores e seus respectivos departamentos.</p>
                </div>
                <button onclick={iniciarCriacao}
                    class="cursor-pointer rounded-lg bg-emerald-600 px-4 py-2 text-sm font-bold text-white transition hover:bg-emerald-700 whitespace-nowrap">
                    + Cadastrar Professor
                </button>
            </div>

            {#if carregando}
                <p class="opacity-60 animate-pulse">Carregando dados dos professores...</p>
            {:else}
                {#if !professores || professores.length === 0}
                    <div class="rounded-xl border border-dashed border-(--foreground)/20 p-8 text-center">
                        <p class="opacity-80">Nenhum professor cadastrado no sistema.</p>
                    </div>
                {:else}
                    <div class="grid grid-cols-1 gap-4 lg:grid-cols-3">
                        {#each professores as professor (professor.id)}
                            <div class="flex flex-col gap-3 rounded-xl bg-(--foreground)/10 border border-(--foreground)/5 p-4 text-(--foreground)">
                                <div class="flex items-start justify-between gap-2">
                                    <div>
                                        <h3 class="text-lg font-bold leading-tight">{professor.nome}</h3>
                                        <p class="text-xs font-semibold opacity-60 tracking-wide mt-0.5">{professor.departamento}</p>
                                    </div>
                                    <span class="shrink-0 rounded bg-(--primary) px-2 py-0.5 text-xs font-mono font-bold">
                                        SIAPE {professor.siape}
                                    </span>
                                </div>

                                <div class="text-sm opacity-90 space-y-0.5">
                                    <p class="truncate"><span class="font-semibold opacity-70">E-mail:</span> {professor.email}</p>
                                </div>

                                <div class="mt-auto flex gap-2 pt-3 border-t border-(--foreground)/5">
                                    <button onclick={() => iniciarEdicao(professor)}
                                        class="flex-1 cursor-pointer rounded-lg bg-sky-600/20 text-sky-500 hover:bg-sky-600 hover:text-white px-3 py-1.5 text-xs font-bold transition">
                                        Editar
                                    </button>
                                    <button onclick={() => excluirProfessor(professor.id)}
                                        class="cursor-pointer rounded-lg bg-rose-600/10 text-rose-500 hover:bg-rose-600 hover:text-white px-3 py-1.5 text-xs font-bold transition">
                                        Excluir
                                    </button>
                                </div>
                            </div>
                        {/each}
                    </div>
                {/if}
            {/if}
        </section>
    {/if}
</div>