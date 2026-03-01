/**
 * auth.js — Gerenciamento de autenticação JWT
 * Token armazenado em sessionStorage (limpo ao fechar o navegador/aba).
 * Nunca armazene dados sensíveis (senha, chave secreta) aqui.
 */

const TOKEN_KEY = 'ht_token';
const USER_KEY  = 'ht_user';

function getToken() {
    return sessionStorage.getItem(TOKEN_KEY);
}

function setToken(token) {
    sessionStorage.setItem(TOKEN_KEY, token);
}

function getUser() {
    const raw = sessionStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
}

function setUser(user) {
    sessionStorage.setItem(USER_KEY, JSON.stringify(user));
}

function clearSession() {
    sessionStorage.removeItem(TOKEN_KEY);
    sessionStorage.removeItem(USER_KEY);
}

function isLoggedIn() {
    return !!getToken();
}

/**
 * Redireciona para login.html se não estiver autenticado.
 * Retorna false caso o redirecionamento seja acionado.
 */
function requireAuth() {
    if (!isLoggedIn()) {
        window.location.href = '/login.html';
        return false;
    }
    return true;
}
