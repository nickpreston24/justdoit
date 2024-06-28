tailwind.config = {
    theme: {
        extend: {
            animation: {
                'gradient': 'gradient 8s linear infinite',
            },
            keyframes: {
                'gradient': {
                    to: {'background-position': '200% center'},
                },
                propel: {
                    '0%': {transform: 'translateX(0)'},
                    '20%': {transform: 'translateX(25%)'},
                    '40%': {transform: 'translateX(-25%)'},
                    '60%': {transform: 'translateX(25%)'},
                    '100%': {transform: 'translateX(-25%)'},
                },
                displace: {
                    '0%': {transform: 'rotate(0deg)'},
                    '20%': {transform: 'rotate(-90deg)'},
                    '40%': {transform: 'rotate(0deg)'},
                    '60%': {transform: 'rotate(0deg)'},
                    '80%': {transform: 'rotate(90deg)'},
                    '100%': {transform: 'rotate(0deg)'},
                }
            }
        },
    },
};
