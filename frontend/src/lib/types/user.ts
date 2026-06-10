export type User = {
	id: number;
	type: UserType;
	character: UserCharacter;
	name: string;
	email: string;
	whatsapp?: string;
	active: boolean;
	created_at: string;
};

export type UserType = 'admin' | 'teacher' | 'student';

export type UserTeacher = User & {
	department: string;	
};

export type UserStudent = User & {
	registry: string;
};

export type UserCharacter =
	| 'anonymous'
	| 'bender'
	| 'creeper'
	| 'doge'
	| 'flameprincess'
	| 'freddy'
	| 'hellokitty'
	| 'homersimpson'
	| 'ironman'
	| 'jake'
	| 'jason'
	| 'joker'
	| 'luigi'
	| 'mario'
	| 'melody'
	| 'minecraft'
	| 'monalisa'
	| 'princessbubblegum'
	| 'scream'
	| 'troll';