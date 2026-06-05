<script lang="ts">
    import type { User, UserCharacter } from '$lib/types/user';
	import Icon from '../ui/Icon.svelte';
    import Modal from '../ui/Modal.svelte';
	import { CHARACTERS } from '$lib/constants/characters';

	interface UserModalProps {
        open: boolean;
        user: User | null;
		onClose: () => void;
		onChangeCharacter: (character: UserCharacter) => void;
	}

	let { open = $bindable(), onClose, user, onChangeCharacter }: UserModalProps = $props();

	let selectedCharacter: UserCharacter | null = $derived(user ? user.character : null);

	function selectCharacter(character: UserCharacter) {
		selectedCharacter = character;
		onChangeCharacter(character);
	}
</script>

<Modal {open} title="Meu Perfil" {onClose}>
	{#if user}

	<h2 class="font-semibold mb-2">Meu Avatar</h2>
      <div class="max-h-96 overflow-y-auto bg-(--foreground)/5 rounded-lg">
        <div class="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-4 2xl:grid-cols-6 p-1">
          {#each CHARACTERS as character}
            <button
              class="flex flex-col items-center rounded-lg p-2 transition-all
                {selectedCharacter === character 
                  ? 'bg-blue-100/30 ring-2 ring-sky-600' 
                  : 'hover:bg-gray-100/10 cursor-pointer'}"
              onclick={() => selectCharacter(character)}
            >
              <Icon 
                icon={`/characters/${character}.png`} 
                class="h-6 w-6 sm:h-8 sm:w-8"
              />
            </button>
          {/each}
        </div>
      </div>
	{:else}
		<p class="text-gray-600">Usuário não encontrado.</p>
	{/if}
</Modal>
