<script lang="ts">
    let atividades = [
        {
            id: 1,
            titulo: 'Atividade 1',
            descricao: 'Descrição da atividade 1',
            disciplina: 'Disciplina A',
            turma: '2026.1',
            prazo: '2026-07-01'
        },
        {
            id: 2,
            titulo: 'Atividade 2',
            descricao: 'Descrição da atividade 2',
            disciplina: 'Disciplina B',
            turma: '2026.1',
            prazo: '2026-07-15'
        },
        {
            id: 3,
            titulo: 'Atividade 3',
            descricao: 'Descrição da atividade 3',
            disciplina: 'Disciplina C',
            turma: '2026.1',
            prazo: '2026-08-01'
        },
        {
            id: 4,
            titulo: 'Atividade 4',
            descricao: 'Descrição da atividade 4',
            disciplina: 'Disciplina D',
            turma: '2026.1',
            prazo: '2026-08-15'
        }
    ];
    let entregas = [
        {
            id: 1,
            atividadeId: 1,
            dataEntrega: '12/06/2026',
            nota: 9.5
        },
        {
            id: 2,
            atividadeId: 2,
            dataEntrega: '13/06/2026',
            nota: null
        },
        {
            id: 3,
            atividadeId: 3,
            dataEntrega: null,
            nota: null
        }
    ];

    function getEntrega(atividadeId: number) {
        return entregas.find((e) => e.atividadeId === atividadeId);
    }

    function formatPrazo(prazo: string) {
        const [ano, mes, dia] = prazo.split('-');
        return `${dia}/${mes}/${ano}`;
    }

    function statusInfo(atividadeId: number) {
        const entrega = getEntrega(atividadeId);

        if (!entrega || !entrega.dataEntrega) {
            return { label: 'Pendente', classe: 'bg-amber-500/20 text-amber-500' };
        }
        if (entrega.nota !== null) {
            return { label: 'Corrigido', classe: 'bg-emerald-500/20 text-emerald-500' };
        }
        return { label: 'Entregue', classe: 'bg-sky-500/20 text-sky-500' };
    }

    function entregar(atividadeId: number) {
        alert(`Entregar atividade ${atividadeId}`);
    }
</script>

<div class="space-y-10 text-(--foreground)">
    <section>
        <h2 class="mb-4 text-2xl font-bold">Atividades e Entregas</h2>

        {#if atividades.length === 0}
            <p class="opacity-80">Não há atividades cadastradas.</p>
        {:else}
            <div class="grid grid-cols-1 gap-4 lg:grid-cols-3">
                {#each atividades as atividade (atividade.id)}
                    {@const entrega = getEntrega(atividade.id)}
                    {@const status = statusInfo(atividade.id)}
                    <div class="flex flex-col gap-3 rounded-xl bg-(--foreground)/10 p-4 text-(--foreground)">
                        <div class="flex items-center justify-between">
                            <h3 class="text-lg font-bold">{atividade.titulo}</h3>
                            <span class="rounded px-2 py-0.5 text-xs font-bold {status.classe}">
                                {status.label}
                            </span>
                        </div>

                        <p class="text-sm opacity-80">{atividade.descricao}</p>

                        <div class="space-y-1 text-sm">
                            <p><span class="font-bold">Disciplina:</span> {atividade.disciplina}</p>
                            <p><span class="font-bold">Turma:</span> {atividade.turma}</p>
                            <p><span class="font-bold">Prazo:</span> {formatPrazo(atividade.prazo)}</p>
                            {#if entrega?.dataEntrega}
                                <p><span class="font-bold">Entregue em:</span> {entrega.dataEntrega}</p>
                            {/if}
                            {#if entrega?.nota !== null && entrega?.nota !== undefined}
                                <p><span class="font-bold">Nota:</span> {entrega.nota}</p>
                            {/if}
                        </div>

                        <div class="mt-auto flex gap-4 pt-2">
                            {#if !entrega?.dataEntrega}
                                <button
                                    onclick={() => entregar(atividade.id)}
                                    class="flex-1 cursor-pointer rounded-lg bg-emerald-600 px-3 py-2 text-sm font-bold transition hover:bg-emerald-700"
                                >
                                    Entregar
                                </button>
                            {:else}
                                <button
                                    onclick={() => entregar(atividade.id)}
                                    class="flex-1 cursor-pointer rounded-lg bg-sky-600 px-3 py-2 text-sm font-bold transition hover:bg-sky-700"
                                >
                                    Ver entrega
                                </button>
                            {/if}
                        </div>
                    </div>
                {/each}
            </div>
        {/if}
    </section>
</div>