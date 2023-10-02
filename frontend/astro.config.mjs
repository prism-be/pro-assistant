import { defineConfig } from 'astro/config';
import tailwind from "@astrojs/tailwind";
import react from "@astrojs/react";
import astroI18next from "astro-i18next";

// https://astro.build/config
export default defineConfig({
  site: "https://web.pro-assistant.eu",
  integrations: [tailwind(), react(), astroI18next()],
  vite: {
    server: {
      proxy: {
        "/api": {
          target: "http://localhost:7099",
          changeOrigin: true,
          secure: false
        }
      }
    }
  },
});