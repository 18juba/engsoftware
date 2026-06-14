<script lang="ts">
	import { resolve } from '$app/paths';
	import Icon from '$lib/components/ui/Icon.svelte';
	import { goto } from '$app/navigation';
	import { env } from '$env/dynamic/public';
	import { auth } from '$lib/stores/auth';
	import { toasts } from '$lib/stores/toast';

	let showPassword = $state(false);
	let email = $state('');
	let password = $state('');
	let isSubmitting = $state(false);

	async function login(event: SubmitEvent) {
		event.preventDefault();

		if (!env.PUBLIC_API_URL) {
			toasts.add({
				type: 'danger',
				title: 'Configuração inválida',
				description: 'Erro interno do servidor'
			});
			return;
		}
		if (!email.trim() || !password) {
			toasts.add({
				type: 'danger',
				title: 'Campos obrigatórios',
				description: 'Informe e-mail e senha para continuar.'
			});
			return;
		}
		if (password.length < 8) {
			toasts.add({
				type: 'danger',
				title: 'Senha inválida',
				description: 'A senha deve ter no mínimo 8 caracteres.'
			});
			return;
		}

		try {
			isSubmitting = true;

			const response = await fetch(`${env.PUBLIC_AUTHAPI_URL}/auth/login`, {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({
					email,
					password
				})
			});

			if (response.status === 200) {
				const data = await response.json();
				auth.setToken(data.token);
				auth.setUser(data.user);
				toasts.add({
					type: 'success',
					title: 'Login realizado',
					description: 'Autenticação concluída com sucesso.'
				});
				await goto(resolve('/painel'));
				return;
			}

			if (response.status === 400) {
				toasts.add({
					type: 'danger',
					title: 'Dados inválidos',
					description: 'Revise os dados e tente novamente.'
				});
				return;
			}

			if (response.status === 401) {
				toasts.add({
					type: 'danger',
					title: 'Credenciais inválidas',
					description: 'Email ou senha estão incorretos.'
				});
				return;
			}

			toasts.add({
				type: 'danger',
				title: 'Erro no login',
				description: 'Não foi possível autenticar no momento.'
			});
		} catch {
			toasts.add({
				type: 'danger',
				title: 'Erro de conexão',
				description: 'Não foi possível conectar ao servidor.'
			});
		} finally {
			isSubmitting = false;
		}
	}
</script>

<div class="flex h-full items-center justify-center p-4">
	<form
		onsubmit={login}
		class="flex w-full max-w-sm flex-col gap-6 rounded-lg bg-(--foreground)/5 p-6 backdrop-blur-xs"
	>
		<h1 class="text-center text-2xl font-bold">Entrar</h1>

		<div class="flex flex-col gap-2">
			<label for="email" class="text-sm font-medium">Email</label>
			<input
				id="email"
				type="email"
				placeholder="seu@email.com"
				bind:value={email}
				class="rounded-lg bg-(--foreground)/10 px-4 py-2 transition-colors duration-200 focus:bg-(--foreground)/20 focus:outline-none"
			/>
		</div>

		<div class="flex flex-col gap-2">
			<label for="senha" class="text-sm font-medium">Senha</label>
			<div class="relative">
				<input
					id="senha"
					type={showPassword ? 'text' : 'password'}
					placeholder="Sua senha"
					bind:value={password}
					class="w-full rounded-lg bg-(--foreground)/10 px-4 py-2 pr-10 transition-colors duration-200 focus:bg-(--foreground)/20 focus:outline-none"
				/>
				<button
					type="button"
					onclick={() => (showPassword = !showPassword)}
					class="absolute top-1/2 right-3 h-5 w-5 -translate-y-1/2 opacity-80 transition-opacity hover:opacity-100"
					aria-label={showPassword ? 'Ocultar senha' : 'Mostrar senha'}
				>
					{#if showPassword}
						<Icon icon="Eye.png" class="h-5 w-5 cursor-pointer" />
					{:else}
						<Icon icon="EyeClosed.png" class="h-5 w-5 cursor-pointer" />
					{/if}
				</button>
			</div>
		</div>

		<a
			href={resolve('/esqueci-senha')}
			class="text-right text-sm text-(--primary) transition-colors hover:text-(--primary)/80"
		>
			Esqueci minha senha
		</a>

		<button
			type="submit"
			disabled={isSubmitting}
			class="cursor-pointer rounded-lg bg-(--primary) px-4 py-2 font-semibold text-white transition-colors duration-200 hover:bg-(--primary)/90 disabled:cursor-not-allowed disabled:opacity-50"
		>
			{isSubmitting ? 'Entrando...' : 'Entrar'}
		</button>

		<a href={resolve('/registro')} class="mb-4 text-center text-sm">
			Não tem conta? <span class="font-bold text-(--primary)">Criar conta</span>
		</a>

		<a href={resolve('/')} class="text-center text-xs"> Voltar </a>
	</form>
</div>
