<script lang="ts">
	import { resolve } from '$app/paths';
	import Icon from '$lib/components/ui/Icon.svelte';
	import { goto } from '$app/navigation';
	import { env } from '$env/dynamic/public';
	import { auth } from '$lib/stores/auth';
	import { toasts } from '$lib/stores/toast';
	import { authApiFetch, AuthApiError } from '$lib/api/authApi';
	import type { User } from '$lib/types/user';

	type LoginResponse = { token: string; user: User };

	let showPassword = $state(false);
	let name = $state('');
	let email = $state('');
	let password = $state('');
	let passwordConfirmation = $state('');
	let whatsapp = $state('');
	let isSubmitting = $state(false);

	async function register(event: SubmitEvent) {
		event.preventDefault();

		if (!env.PUBLIC_AUTHAPI_URL) {
			toasts.add({
				type: 'danger',
				title: 'Configuração inválida',
				description: 'Erro interno do servidor'
			});
			return;
		}

		if (!name.trim() || !email.trim() || !password || !passwordConfirmation) {
			toasts.add({
				type: 'danger',
				title: 'Campos obrigatórios',
				description: 'Preencha todos os campos para continuar.'
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

		if (password !== passwordConfirmation) {
			toasts.add({
				type: 'danger',
				title: 'Confirmação inválida',
				description: 'A confirmação de senha não confere.'
			});
			return;
		}

		try {
			isSubmitting = true;

			await authApiFetch<void>(`/auth/register`, {
				method: 'POST',
				body: JSON.stringify({
					name,
					email,
					password,
					passwordConfirmation,
					whatsapp: whatsapp.trim() || null
				})
			});

			toasts.add({
				type: 'success',
				title: 'Cadastro realizado',
				description: 'Conta criada com sucesso.'
			});

			try {
				const data = await authApiFetch<LoginResponse>(`/auth/login`, {
					method: 'POST',
					body: JSON.stringify({ email, password })
				});

				auth.setToken(data.token);
				auth.setUser(data.user);
				await goto(resolve('/painel'));
			} catch {
				toasts.add({
					type: 'info',
					title: 'Conta criada',
					description: 'Faça login para continuar.'
				});
				await goto(resolve('/'));
			}
		} catch (error) {
			if (error instanceof AuthApiError) {
				if (error.statusCode === 400) {
					toasts.add({
						type: 'danger',
						title: 'Dados inválidos',
						description: 'Revise os dados e tente novamente.'
					});
					return;
				}
				if (error.statusCode === 409) {
					toasts.add({
						type: 'danger',
						title: 'E-mail em uso',
						description: 'Esse e-mail já possui cadastro.'
					});
					return;
				}
				toasts.add({
					type: 'danger',
					title: 'Erro no cadastro',
					description: 'Não foi possível concluir o cadastro.'
				});
				return;
			}
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
		onsubmit={register}
		class="flex w-full max-w-sm flex-col gap-6 rounded-lg bg-(--foreground)/5 p-6 backdrop-blur-xs"
	>
		<h1 class="text-center text-2xl font-bold">Registrar</h1>

		<div class="flex flex-col gap-2">
			<label for="nome" class="text-sm font-medium">Nome</label>
			<input
				id="nome"
				type="text"
				placeholder="Seu nome completo"
				bind:value={name}
				class="rounded-lg bg-(--foreground)/10 px-4 py-2 transition-colors duration-200 focus:bg-(--foreground)/20 focus:outline-none"
			/>
		</div>

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

		<div class="flex flex-col gap-2">
			<label for="confirmar-senha" class="text-sm font-medium">Confirmar Senha</label>
			<div class="relative">
				<input
					id="confirmar-senha"
					type={showPassword ? 'text' : 'password'}
					placeholder="Confirme sua senha"
					bind:value={passwordConfirmation}
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

		<div class="flex flex-col gap-2">
			<label for="whatsapp" class="flex items-center gap-2 text-sm font-medium">
				<Icon icon="Whatsapp.png" class="h-4 w-4" />
				WhatsApp (opcional)
			</label>
			<input
				id="whatsapp"
				type="text"
				placeholder="+55 11 99999-9999"
				bind:value={whatsapp}
				class="rounded-lg bg-(--foreground)/10 px-4 py-2 transition-colors duration-200 focus:bg-(--foreground)/20 focus:outline-none"
			/>
		</div>

		<button
			type="submit"
			disabled={isSubmitting}
			class="cursor-pointer rounded-lg bg-(--primary) px-4 py-2 font-semibold text-white transition-colors duration-200 hover:bg-(--primary)/90 disabled:cursor-not-allowed disabled:opacity-50"
		>
			{isSubmitting ? 'Registrando...' : 'Registrar'}
		</button>

		<a href={resolve('/')} class="text-center text-sm">
			Já tem conta? <span class="font-bold text-(--primary)">Fazer login</span>
		</a>
	</form>
</div>
