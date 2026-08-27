/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Areas/**/*.cshtml',
    './Views/**/*.cshtml',
    './wwwroot/js/**/*.js',
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Poppins', 'system-ui', 'sans-serif'],
      },
      colors: {
        cc: {
          deep: '#1a0033',
          mid: '#2e0249',
          accent: '#a78bfa',
          soft: '#c4b5fd',
        },
      },
    },
  },
  plugins: [],
};
