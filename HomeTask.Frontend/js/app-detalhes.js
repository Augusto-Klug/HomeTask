const { createApp } = Vue;

const servicoId = new URLSearchParams(window.location.search).get('id');

if (!servicoId) window.location.href = '/servicos/buscar.html';

createApp({
    data() {
        return {
            servico: null,
            avaliacoes: [],
            carregando: true
        };
    },
    computed: {
        podeAgendar() {
            return isLoggedIn();
        }
    },
    async mounted() {
        try {
            const [servicoRes, avaliacoesRes] = await Promise.all([
                api.get('/api/ServicoOferecido/ObterServicoPorId', { params: { id: servicoId } }),
                api.get('/api/Avaliacao/ObterAvaliacoesPorPrestador', { params: { prestadorId: null } })
                    .catch(() => ({ data: [] }))
            ]);

            this.servico = servicoRes.data;

            // Busca avaliações com o prestadorId do serviço
            if (this.servico?.prestadorId) {
                try {
                    const av = await api.get('/api/Avaliacao/ObterAvaliacoesPorPrestador',
                        { params: { prestadorId: this.servico.prestadorId } });
                    this.avaliacoes = av.data ?? [];
                } catch { this.avaliacoes = []; }
            }
        } catch {
            this.servico = null;
        } finally {
            this.carregando = false;
        }
    },
    methods: {
        agendar() {
            window.location.href = `/agendamento/novo.html?servicoId=${servicoId}&prestadorId=${this.servico.prestadorId}`;
        },
        formatarPreco(valor) {
            return Number(valor).toFixed(2).replace('.', ',');
        },
        estrelas(media) {
            const cheias = Math.floor(media ?? 0);
            return '★'.repeat(cheias) + '☆'.repeat(5 - cheias);
        },
        formatarData(dataStr) {
            return new Date(dataStr).toLocaleDateString('pt-BR');
        }
    }
}).mount('#app');
