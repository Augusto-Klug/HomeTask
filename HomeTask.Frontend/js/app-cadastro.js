const { createApp } = Vue;

createApp({
    data() {
        return {
            carregando: false,
            erro: null,
            form: {
                tipoUsuario: '1',
                nome: '',
                email: '',
                cpf: '',
                telefone: '',
                senha: '',
                // campos do prestador
                cidade: '',
                estado: '',
                raioAtendimentoKm: 10,
                descricao: ''
            }
        };
    },
    methods: {
        async cadastrar() {
            this.erro = null;
            this.carregando = true;
            try {
                // 1. Criar usuário
                const usuarioPayload = {
                    nome: this.form.nome,
                    email: this.form.email,
                    cpf: this.form.cpf,
                    telefone: this.form.telefone,
                    senha: this.form.senha,
                    tipo: parseInt(this.form.tipoUsuario)
                };

                const { data: usuario } = await api.post('/api/Usuario/CriarUsuario', usuarioPayload);

                // 2. Criar perfil(s) conforme o tipo escolhido
                const tipo = this.form.tipoUsuario;

                // Cria perfil de Cliente (tipo 1 = Cliente, tipo 3 = Ambos)
                if (tipo === '1' || tipo === '3') {
                    await api.post('/api/Cliente/CriarCliente', { usuarioId: usuario.id });
                }

                // Cria perfil de Prestador (tipo 2 = Prestador, tipo 3 = Ambos)
                if (tipo === '2' || tipo === '3') {
                    await api.post('/api/Prestador/CriarPrestador', {
                        usuarioId: usuario.id,
                        cidade: this.form.cidade,
                        estado: this.form.estado,
                        raioAtendimentoKm: this.form.raioAtendimentoKm,
                        descricao: this.form.descricao
                    });
                }

                window.location.href = '/cadastro-sucesso.html';
            } catch (err) {
                const status = err.response?.status;
                const msg = err.response?.data;

                if (status === 409) {
                    // E-mail ou CPF já cadastrado — mensagem clara
                    this.erro = typeof msg === 'string' ? msg : 'E-mail ou CPF já cadastrado.';
                } else if (status === 400) {
                    this.erro = typeof msg === 'string' ? msg : 'Dados inválidos. Verifique o formulário.';
                } else {
                    this.erro = 'Erro ao cadastrar. Tente novamente em instantes.';
                }
            } finally {
                this.carregando = false;
            }
        }
    }
}).mount('#app');
