const { createApp } = Vue;

// Protege a rota — exige autenticação
requireAuth();

const params = new URLSearchParams(window.location.search);
const servicoId  = params.get('servicoId');
const prestadorId = params.get('prestadorId');

if (!servicoId || !prestadorId) window.location.href = '/servicos/buscar.html';

createApp({
    data() {
        return {
            servico: null,
            carregando: false,
            erro: null,
            hoje: new Date().toISOString().split('T')[0],
            form: {
                data: '',
                hora: '',
                endereco: '',
                observacoes: ''
            }
        };
    },
    async mounted() {
        try {
            const { data } = await api.get('/api/ServicoOferecido/ObterServicoPorId', { params: { id: servicoId } });
            this.servico = data;
        } catch {
            this.servico = null;
        }
    },
    methods: {
        async agendar() {
            this.erro = null;
            this.carregando = true;
            try {
                const user = getUser();

                // Busca o cliente associado ao usuário logado
                const { data: cliente } = await api.get('/api/Cliente/ObterClientesPorUsuarioId',
                    { params: { usuarioId: user.userId } });

                const dataHora = new Date(`${this.form.data}T${this.form.hora}:00`).toISOString();

                await api.post('/api/Agendamento/CriarAgendamento', {
                    clienteId: cliente.id,
                    prestadorId: prestadorId,
                    servicoOferecidoId: servicoId,
                    dataHoraAgendada: dataHora,
                    enderecoServico: this.form.endereco,
                    observacoes: this.form.observacoes
                });

                window.location.href = '/agendamento/sucesso.html';
            } catch (err) {
                const msg = err.response?.data;
                this.erro = (typeof msg === 'string' && msg)
                    ? msg
                    : 'Erro ao criar agendamento. Verifique os dados e tente novamente.';
            } finally {
                this.carregando = false;
            }
        }
    }
}).mount('#app');
