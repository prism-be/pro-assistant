/** @type {import('tailwindcss').Config} */
const colors = require('tailwindcss/colors');
module.exports = {
	content: ['./src/**/*.{astro,html,js,jsx,md,mdx,svelte,ts,tsx,vue}'],
	theme: {
		extend: {},
		colors: {
			transparent: 'transparent',
			current: 'currentColor',
			primary: '#31859c',
			secondary: '#205969',
			light: '#699eac',
			white: colors.white,
			gray: colors.gray,
			red: colors.red,
			green: colors.green,
			orange: colors.orange,
		  },
	},
	plugins: [],
}
