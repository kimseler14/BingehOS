import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./app/**/*.{ts,tsx}", "./components/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        ink: "#10233f",
        mist: "#f4f7fb",
        teal: "#0e9f9a",
      },
      boxShadow: {
        soft: "0 16px 45px rgba(15, 35, 63, 0.08)",
      },
    },
  },
  plugins: [],
};

export default config;
