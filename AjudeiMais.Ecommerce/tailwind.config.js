/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './**/*.cshtml', // <- adiciona isso se estiver usando Layout em outras pastas também
        './wwwroot/js/**/*.js'
    ],
    theme: {
        extend: {
            colors: {
                primary: '#FF6B00',
                secondary: '#2E2E2E',
                background: '#F8F9FA',
            },
            fontFamily: {
                sans: ['Poppins', 'sans-serif'],
            }
        },
    },
    plugins: [],
}

