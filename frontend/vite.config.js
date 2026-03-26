import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import path from "node:path";

const cwd = process.cwd();
const rootDir = cwd.endsWith("frontend") ? cwd : path.resolve(cwd, "frontend");

export default defineConfig({
  root: rootDir,
  plugins: [react()],
  server: {
    port: 5173
  }
});
