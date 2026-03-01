/**
 * api.js — Instância Axios configurada para a HomeTask API
 * O token JWT é injetado automaticamente em cada requisição.
 * Em caso de 401, limpa a sessão e redireciona para login.
 */

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: { 'Content-Type': 'application/json' }
});

// Injeta o JWT no header Authorization antes de cada requisição
api.interceptors.request.use(config => {
    const token = getToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Trata respostas de erro globalmente
api.interceptors.response.use(
    response => response,
    error => {
        if (error.response?.status === 401) {
            clearSession();
            window.location.href = '/login.html';
        }
        return Promise.reject(error);
    }
);
