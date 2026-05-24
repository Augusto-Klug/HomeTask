/**
 * api.ts — Instância Axios configurada para a HomeTask API.
 * Tokens (access + refresh) ficam nos cookies HttpOnly.
 * withCredentials envia os cookies automaticamente.
 * O interceptor de 401 tenta renovar o access token via refresh endpoint.
 * Se o refresh falhar, redireciona para o login.
 */

import axios from "axios";
import router from "@/router";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
  withCredentials: true, // envia cookies em todas as requisições
});

let isRefreshing = false;
let pendingQueue: Array<{
  resolve: (value?: unknown) => void;
  reject: (reason?: unknown) => void;
}> = [];

function processQueue(error: unknown) {
  pendingQueue.forEach(({ resolve, reject }) => {
    if (error) reject(error);
    else resolve();
  });
  pendingQueue = [];
}

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          pendingQueue.push({ resolve, reject });
        }).then(() => api(originalRequest));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        // O cookie refreshToken é enviado automaticamente porque o path
        // do cookie de refresh deve estar configurado para /api/Auth/Refresh
        await api.post("/api/Auth/Refresh");
        processQueue(null);
        return api(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError);
        router.push("/login");
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  },
);

export default api;
