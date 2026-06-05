export const CHARACTERS = [
	'anonymous',
	'bender',
	'creeper',
	'doge',
	'flameprincess',
	'freddy',
	'hellokitty',
	'homersimpson',
	'ironman',
	'jake',
	'jason',
	'joker',
	'luigi',
	'mario',
	'melody',
	'minecraft',
	'monalisa',
	'princessbubblegum',
	'scream',
	'troll'
] as const;

export type UserCharacter = (typeof CHARACTERS)[number];
