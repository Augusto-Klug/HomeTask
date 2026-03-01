const { createApp } = Vue;

const CATEGORIAS = [
    { id: 1,  nome: 'Faxina' },
    { id: 2,  nome: 'Jardinagem' },
    { id: 3,  nome: 'Reparos' },
    { id: 4,  nome: 'Lavanderia' },
    { id: 5,  nome: 'Passadoria' },
    { id: 6,  nome: 'Babysitter' },
    { id: 7,  nome: 'Cuidador de Idosos' },
    { id: 8,  nome: 'Pet Sitter' },
    { id: 9,  nome: 'Cozinheiro' },
    { id: 10, nome: 'Serviços Gerais' },
];

// Navbar
const navAuth = document.getElementById('nav-auth');
const user = getUser();
navAuth.innerHTML = user
    ? `<span class="navbar-text me-2">Olá, ${user.nome}</span>
       <button class="btn btn-outline-danger btn-sm" id="btn-sair">Sair</button>`
    : `<a href="/login.html" class="btn btn-outline-primary btn-sm">Entrar</a>
       <a href="/cadastro.html" class="btn btn-primary btn-sm">Cadastrar</a>`;

document.getElementById('btn-sair')?.addEventListener('click', () => {
    clearSession();
    location.reload();
});

// Lê parâmetros da URL
const params = new URLSearchParams(window.location.search);
const categoriaParam = params.get('categoria') || '';

createApp({
    data() {
        return {
            categorias: CATEGORIAS,
            filtro: {
                categoria: categoriaParam,
                cidade: '',
                precoMaximo: null,
                avaliacaoMinima: 0
            },
            servicos: [],
            carregando: false,
            buscou: false
        };
    },
    mounted() {
        this.buscar();
    },
    methods: {
        async buscar() {
            this.carregando = true;
            this.buscou = false;
            try {
                const params = {};
                if (this.filtro.categoria)    params.categoria    = this.filtro.categoria;
                if (this.filtro.cidade)       params.cidade       = this.filtro.cidade;
                if (this.filtro.precoMaximo)  params.precoMaximo  = this.filtro.precoMaximo;

                const { data } = await api.get('/api/ServicoOferecido/BuscarServicos', { params });

                this.servicos = data.filter(s =>
                    this.filtro.avaliacaoMinima === 0 || s.mediaAvaliacoes >= this.filtro.avaliacaoMinima
                );
            } catch {
                this.servicos = [];
            } finally {
                this.carregando = false;
                this.buscou = true;
            }
        },
        verDetalhes(id) {
            window.location.href = `/servicos/detalhes.html?id=${id}`;
        },
        formatarPreco(valor) {
            return Number(valor).toFixed(2).replace('.', ',');
        },
        estrelas(media) {
            const cheias = Math.floor(media);
            return '★'.repeat(cheias) + '☆'.repeat(5 - cheias);
        }
    }
}).mount('#app');
