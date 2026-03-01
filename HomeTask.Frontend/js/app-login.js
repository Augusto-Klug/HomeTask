const { createApp } = Vue;

// Redireciona quem já está logado
if (isLoggedIn()) window.location.href = '/index.html';

createApp({
    data() {
        return {
            email: '',
            senha: '',
            mostrarSenha: false,
            carregando: false,
            erro: null
        };
    },
    methods: {
        async login() {
            this.erro = null;
            this.carregando = true;
            try {
                const { data } = await api.post('/api/Auth/Login', {
                    email: this.email,
                    senha: this.senha
                });

                setToken(data.token);
                setUser({ userId: data.userId, nome: data.nome, email: data.email, tipo: data.tipo });

                window.location.href = '/index.html';
            } catch (err) {
                if (err.response?.status === 401) {
                    this.erro = 'E-mail ou senha inválidos.';
                } else {
                    this.erro = 'Erro ao conectar com o servidor. Tente novamente.';
                }
            } finally {
                this.carregando = false;
            }
        }
    }
}).mount('#app');
