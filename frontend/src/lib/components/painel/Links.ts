export const LINKS = [
	{
		id: 1,
		label: 'Dashboard',
		href: '/painel',
		icon: 'Dashboard.png',
		userTypes: ['admin', 'student', 'teacher']
	},
	{
		id: 2,
		label: 'Alunos',
		href: '/painel/alunos',
		icon: 'Student.png',
		userTypes: ['admin', 'teacher', 'student']
	},
	{
		id: 3,
		label: 'Professores',
		href: '/painel/professores',
		icon: 'Teacher.png',
		userTypes: ['admin', 'teacher', 'student']
	},
	{
		id: 4,
		label: 'Disciplinas',
		href: '/painel/disciplinas',
		icon: 'Books.png',
		userTypes: ['admin', 'student', 'teacher']
	},
	{
		id: 5,
		label: 'Turmas',
		href: '/painel/turmas',
		icon: 'Classroom.png',
		userTypes: ['admin', 'teacher', 'student']
	}
];
