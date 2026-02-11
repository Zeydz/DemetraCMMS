export default {
    content: [
        "./Views/**/*.cshtml",
        "./Pages/**/*.cshtml",
        "./wwwroot/**/*.js"
    ],

    theme: {
        extend: {

            /* 🎨 Brand colors */
            colors: {
                primary: "#02f1b4",
                secondary: "#041f41",

                /* Status / CMMS colors */
                critical: "#dc2626",
                high: "#f97316",
                medium: "#eab308",
                low: "#22c55e",

                surface: {
                    DEFAULT: "#0f172a",   // cards / panels
                    light: "#1e293b"
                }
            },

            /* 🔤 Typography */
            fontFamily: {
                brand: ["Saira", "sans-serif"],
                heading: ["Saira", "sans-serif"],
                mono: ["ui-monospace"]
            },

            /* ✨ UI polish */
            borderRadius: {
                xl: "0.9rem"
            },

            boxShadow: {
                soft: "0 4px 20px rgba(0,0,0,0.25)"
            }
        }
    }
};