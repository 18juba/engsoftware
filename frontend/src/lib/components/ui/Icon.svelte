<script lang="ts">
	interface IconProps {
		icon: string;
		color?: string;
		class?: string;
		ariaLabel?: string;
		onClick?: (e?: MouseEvent) => void;
	}

	let {
		icon,
		color = '#fff',
		class: className = 'w-6 h-6',
		onClick,
		ariaLabel
	}: IconProps = $props();

	const imageExtensions = ['png', 'jpg', 'jpeg', 'gif', 'webp', 'svg'];
	const isImage = imageExtensions.some((ext) => icon.toLowerCase().endsWith(`.${ext}`));
</script>

{#if isImage}
	{#if onClick}
		<button
			type="button"
			aria-label={ariaLabel}
			class={`cursor-pointer ${className} inline-flex items-center justify-center border-none bg-transparent p-0 select-none`}
			onclick={onClick}
		>
			<img
				src="/icons/{icon}"
				alt={ariaLabel || ''}
				class="h-full w-full object-contain"
				fetchpriority="high"
			/>
		</button>
	{:else}
		<img
			src="/icons/{icon}"
			alt={ariaLabel || ''}
			aria-label={ariaLabel}
			class={`${className} object-contain select-none`}
			role="presentation"
			aria-hidden={!ariaLabel}
			fetchpriority="high"
		/>
	{/if}
{:else if onClick}
	<button
		type="button"
		aria-label={ariaLabel}
		class={`cursor-pointer ${className} inline-flex items-center justify-center border-none bg-transparent p-0 select-none`}
		style="
        -webkit-mask: url('/icons/{icon}') no-repeat center / contain;
        mask: url('/icons/{icon}') no-repeat center / contain;
        background-color: {color};
      "
		onclick={onClick}
	></button>
{:else}
	<div
		aria-label={ariaLabel}
		class={`${className} inline-flex items-center justify-center bg-transparent p-0 select-none`}
		style="
        -webkit-mask: url('/icons/{icon}') no-repeat center / contain;
        mask: url('/icons/{icon}') no-repeat center / contain;
        background-color: {color};
      "
		role="presentation"
		aria-hidden={!ariaLabel}
	></div>
{/if}
