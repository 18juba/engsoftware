export function formatTimeInput(value: string): string {
	const digitsOnly = value.replace(/\D/g, '').slice(0, 4);

	if (digitsOnly.length <= 2) {
		return digitsOnly;
	}

	return `${digitsOnly.slice(0, 2)}:${digitsOnly.slice(2)}`;
}

export function isValidTime(value: string): boolean {
	return /^([01]\d|2[0-3]):([0-5]\d)$/.test(value);
}

export function normalizeTimeInput(value: string, fallback = '00:00'): string {
	if (isValidTime(value)) {
		return value;
	}

	const digitsOnly = value.replace(/\D/g, '').slice(0, 4);
	if (digitsOnly.length < 3) {
		return fallback;
	}

	const rawHours = Number.parseInt(digitsOnly.slice(0, 2), 10);
	const rawMinutes = Number.parseInt(digitsOnly.slice(2).padEnd(2, '0'), 10);

	if (Number.isNaN(rawHours) || Number.isNaN(rawMinutes)) {
		return fallback;
	}

	const hours = Math.min(Math.max(rawHours, 0), 23);
	const minutes = Math.min(Math.max(rawMinutes, 0), 59);

	return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}`;
}

export function parseTimeToParts(value: string): { hour: number; minute: number } {
	if (!isValidTime(value)) {
		return { hour: 0, minute: 0 };
	}

	const [hour, minute] = value.split(':').map((part) => Number.parseInt(part, 10));
	return { hour, minute };
}
