/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './**/*.cshtml',
        './wwwroot/js/**/*.js'
    ],
    theme: {
        extend: {
            colors: {
                primary: '#6A994E', // Verde suave e acolhedor
                secondary: '#3D405B', // Azul acinzentado para equilíbrio
                background: '#F2E9E4', // Bege claro aconchegante
                highlight: '#A7C957', // Verde claro para chamar atenção
                accent: '#DDBEA9', // Tom pastel quente para suavidade
                text: '#2E2E2E', // Cinza escuro para legibilidade
                hover: '#5A853E', // Cor de hover mais escura
            },
            fontFamily: {
                sans: ['Poppins', 'sans-serif'],
            }
        },
    },
    plugins: [],
};
