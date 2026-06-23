<script lang="ts">
    import { onMount } from 'svelte';
    import type { Aluno } from '$lib/types/aluno';
    import { 
        listarAlunos, 
        criarAluno, 
        atualizarAluno, 
        deletarAluno 
    } from '$lib/services/alunoService';

    let alunos: Aluno[] | null = null;
    let carregando = true;
    let exibindoFormulario = false;
    let modoEdicao = false;

    let dadosFormulario: Aluno = dadosIniciaisFormulario();

    function dadosIniciaisFormulario(): Aluno {
        return {
            id: 0,
            nome: '',
            email: '',
            matricula: '',
            curso: ''
        };
    }

    async function carregarAlunos() {
        carregando = true;
        try {
            alunos = await listarAlunos();
        } catch (error) {
            console.error("Erro ao listar alunos:", error);
            alert("Erro ao carregar a lista de alunos.");
        } finally {
            carregando = false;
        }
    }

    async function salvarAluno(e: Event) {
        e.preventDefault();
        try {
            if (modoEdicao) {
                await atualizarAluno(dadosFormulario.id, dadosFormulario);
                alert("Aluno atualizado com sucesso!");
            } else {
                await criarAluno(dadosFormulario);
                alert("Aluno cadastrado com sucesso!");
            }
            fecharFormulario();
            await carregarAlunos();
        } catch (error) {
            console.error("Erro ao salvar aluno:", error);
            alert("Erro ao salvar os dados do aluno.");
        }
    }

    async function excluirAluno(id: number) {
        if (!confirm("Tem certeza que deseja remover este aluno do sistema?")) return;
        
        try {
            await deletarAluno(id);
            alert("Aluno removido com sucesso!");
            await carregarAlunos();
        } catch (error) {
            console.error("Erro ao deletar aluno:", error);
            alert("Erro ao excluir o aluno. Certifique-se de que ele não possui vínculos ativos.");
        }
    }

    function iniciarCriacao() {
        modoEdicao = false;
        dadosFormulario = dadosIniciaisFormulario();
        exibindoFormulario = true;
    }

    function iniciarEdicao(aluno: Aluno) {
        modoEdicao = true;
        dadosFormulario = { ...aluno };
        exibindoFormulario = true;
    }

    function fecharFormulario() {
        exibindoFormulario = false;
        dadosFormulario = dadosIniciaisFormulario();
    }

    onMount(() => {
        carregarAlunos();
    });
</script>

<div class="space-y-10 text-(--foreground)">
    
    {#if exibindoFormulario}
        <section class="max-w-2xl mx-auto bg-(--foreground)/5 border border-(--foreground)/10 rounded-xl p-6">
            <h2 class="text-2xl font-bold mb-6">
                {modoEdicao ? 'Editar Dados do Aluno' : 'Cadastrar Novo Aluno'}
            </h2>

            <form onsubmit={salvarAluno} class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div class="md:col-span-2">
                        <label class="block text-sm font-semibold mb-1" for="nome">Nome Completo</label>
                        <input id="nome" type="text" placeholder="Ex: João Silva" bind:value={dadosFormulario.nome} required
                            class="w-full bg-(--foreground)/10 border border-(--foreground)/20 rounded-lg p-2.5 text-sm focus:outline-none focus:border-sky-500" />
                    </div>
                </div>

                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <label class="block text-sm font-semibold mb-1" for="matricula">Matrícula</label>
                        <input id="matricula" type="text" placeholder="Ex: 202610012" bind:value={dadosFormulario.matricula} required
                            class="w-full bg-(--foreground)/10 border border-(--foreground)/20 rounded-lg p-2.5 text-sm focus:outline-none focus:border-sky-500" />
                    </div>
                    <div>
                        <label class="block text-sm font-semibold mb-1" for="curso">Curso</label>
                        <input id="curso" type="text" placeholder="Ex: Ciência da Computação" bind:value={dadosFormulario.curso} required
                            class="w-full bg-(--foreground)/10 border border-(--foreground)/20 rounded-lg p-2.5 text-sm focus:outline-none focus:border-sky-500" />
                    </div>
                </div>

                <div>
                    <label class="block text-sm font-semibold mb-1" for="email">E-mail Institucional</label>
                    <input id="email" type="email" placeholder="Ex: joao.silva@universidade.com" bind:value={dadosFormulario.email} required
                        class="w-full bg-(--foreground)/10 border border-(--foreground)/20 rounded-lg p-2.5 text-sm focus:outline-none focus:border-sky-500" />
                </div>

                <div class="flex justify-end gap-3 pt-4">
                    <button type="button" onclick={fecharFormulario}
                        class="cursor-pointer rounded-lg bg-(--foreground)/10 px-4 py-2 text-sm font-bold transition hover:bg-(--foreground)/20">
                        Cancelar
                    </button>
                    <button type="submit"
                        class="cursor-pointer rounded-lg bg-sky-600 px-5 py-2 text-sm font-bold text-white transition hover:bg-sky-700">
                        Salvar Aluno
                    </button>
                </div>
            </form>
        </section>

    {:else}
        <section>
            <div class="mb-6 flex items-center justify-between gap-4">
                <div>
                    <h2 class="text-2xl font-bold">Base de Alunos</h2>
                    <p class="text-sm opacity-60">Gerencie o cadastro de estudantes aptos para enturmação.</p>
                </div>
                <button onclick={iniciarCriacao}
                    class="cursor-pointer rounded-lg bg-emerald-600 px-4 py-2 text-sm font-bold text-white transition hover:bg-emerald-700 whitespace-nowrap">
                    + Cadastrar Aluno
                </button>
            </div>

            {#if carregando}
                <p class="opacity-60 animate-pulse">Carregando dados dos alunos...</p>
            {:else}
                {#if !alunos || alunos.length === 0}
                    <div class="rounded-xl border border-dashed border-(--foreground)/20 p-8 text-center">
                        <p class="opacity-80">Nenhum aluno cadastrado no sistema.</p>
                    </div>
                {:else}
                    <div class="grid grid-cols-1 gap-4 lg:grid-cols-3">
                        {#each alunos as aluno (aluno.id)}
                            <div class="flex flex-col gap-3 rounded-xl bg-(--foreground)/10 border border-(--foreground)/5 p-4 text-(--foreground)">
                                <div class="flex items-start justify-between gap-2">
                                    <div>
                                        <h3 class="text-lg font-bold leading-tight">{aluno.nome}</h3>
                                        <p class="text-xs font-semibold opacity-60 tracking-wide mt-0.5">{aluno.curso}</p>
                                    </div>
                                    <span class="shrink-0 rounded bg-(--primary) px-2 py-0.5 text-xs font-mono font-bold">
                                        #{aluno.matricula}
                                    </span>
                                </div>

                                <div class="text-sm opacity-90 space-y-0.5">
                                    <p class="truncate"><span class="font-semibold opacity-70">E-mail:</span> {aluno.email}</p>
                                </div>

                                <div class="mt-auto flex gap-2 pt-3 border-t border-(--foreground)/5">
                                    <button onclick={() => iniciarEdicao(aluno)}
                                        class="flex-1 cursor-pointer rounded-lg bg-sky-600/20 text-sky-500 hover:bg-sky-600 hover:text-white px-3 py-1.5 text-xs font-bold transition">
                                        Editar
                                    </button>
                                    <button onclick={() => excluirAluno(aluno.id)}
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