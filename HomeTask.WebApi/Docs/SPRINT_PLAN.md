# HomeTask - Sprint Planning (Jira Structure)

## Sprint 1 - Fundação e Autenticação (2 semanas)

### 📦 ÉPICO 1: Autenticação e Gerenciamento de Usuários
**Objetivo**: Implementar sistema completo de autenticação e gerenciamento de perfis

---

#### 🎯 US-001: Login de Usuários
**Como** usuário  
**Quero** fazer login no sistema  
**Para** acessar minhas funcionalidades personalizadas

**Prioridade**: Alta  
**Story Points**: 8  
**Critérios de Aceite**:
- [ ] Sistema valida e-mail e senha
- [ ] Exibe mensagens de erro claras
- [ ] Redireciona para dashboard após login bem-sucedido
- [ ] Implementa sessão/token de autenticação
- [ ] Possui opção "Lembrar-me"

**Tasks**:

**[BACK-001]** Implementar autenticação com JWT
- Criar AuthService para geração de tokens
- Implementar middleware de autenticação
- Configurar tempo de expiração e refresh token
- **Estimativa**: 5h

**[BACK-002]** Criar endpoint de login
- POST /api/auth/login
- Validar credenciais
- Retornar token JWT e dados do usuário
- **Estimativa**: 3h

**[FRONT-001]** Criar página de Login (Vue3)
- Formulário com e-mail e senha
- Validação de campos
- Integração com API de login
- **Estimativa**: 4h

**[FRONT-002]** Implementar gerenciamento de sessão
- Armazenar token no localStorage/sessionStorage
- Criar plugin de autenticação (Pinia/Vuex)
- Implementar guards de rota
- **Estimativa**: 3h

---

#### 🎯 US-002: Cadastro de Clientes e Prestadores
**Como** novo usuário  
**Quero** criar minha conta na plataforma  
**Para** utilizar os serviços como cliente ou prestador

**Prioridade**: Alta  
**Story Points**: 13  
**Critérios de Aceite**:
- [ ] Formulário diferencia cliente e prestador
- [ ] Valida CPF, e-mail e telefone
- [ ] Verifica e-mails duplicados
- [ ] Envia e-mail de confirmação
- [ ] Prestadores iniciam com status "Em Análise"

**Tasks**:

**[BACK-003]** Melhorar validações de cadastro
- Implementar validação de CPF
- Validar formato de telefone
- Verificar duplicidade de e-mail/CPF
- **Estimativa**: 4h

**[BACK-004]** Implementar envio de e-mail de confirmação
- Configurar SMTP
- Criar templates de e-mail
- Endpoint para confirmar e-mail
- **Estimativa**: 5h

**[FRONT-003]** Aprimorar página de cadastro
- Melhorar validações do formulário
- Adicionar máscaras (CPF, telefone, CEP)
- Feedback visual de validação
- **Estimativa**: 5h

**[FRONT-004]** Implementar busca de CEP (ViaCEP)
- Integração com API ViaCEP
- Preenchimento automático de endereço
- **Estimativa**: 2h

---

#### 🎯 US-003: Recuperação de Senha
**Como** usuário  
**Quero** recuperar minha senha  
**Para** ter acesso à minha conta caso esqueça

**Prioridade**: Média  
**Story Points**: 5  

**Tasks**:

**[BACK-005]** Implementar recuperação de senha
- Endpoint POST /api/auth/forgot-password
- Gerar token de recuperação
- Enviar e-mail com link
- **Estimativa**: 3h

**[BACK-006]** Endpoint de redefinição de senha
- POST /api/auth/reset-password
- Validar token de recuperação
- Atualizar senha com hash
- **Estimativa**: 2h

**[FRONT-005]** Criar página "Esqueci minha senha"
- Formulário de solicitação
- Página de redefinição de senha
- **Estimativa**: 3h

---

### 📦 ÉPICO 2: Dashboard e Perfil de Usuário

#### 🎯 US-004: Dashboard do Cliente
**Como** cliente  
**Quero** visualizar meu painel de controle  
**Para** gerenciar meus agendamentos e favoritos

**Prioridade**: Alta  
**Story Points**: 8  

**Tasks**:

**[BACK-007]** Criar endpoints de dashboard do cliente
- GET /api/cliente/dashboard
- Retornar agendamentos ativos
- Retornar histórico recente
- Estatísticas (total gasto, serviços contratados)
- **Estimativa**: 4h

**[FRONT-006]** Criar Dashboard do Cliente
- Layout com cards de estatísticas
- Lista de agendamentos próximos
- Acesso rápido a serviços favoritos
- **Estimativa**: 6h

**[FRONT-007]** Implementar navegação principal
- Menu lateral/superior
- Rotas do cliente
- **Estimativa**: 3h

---

#### 🎯 US-005: Dashboard do Prestador
**Como** prestador  
**Quero** visualizar meu painel de controle  
**Para** gerenciar solicitações e agenda

**Prioridade**: Alta  
**Story Points**: 8  

**Tasks**:

**[BACK-008]** Criar endpoints de dashboard do prestador
- GET /api/prestador/dashboard
- Solicitações pendentes
- Agenda do dia/semana
- Estatísticas (avaliação, serviços realizados)
- **Estimativa**: 4h

**[FRONT-008]** Criar Dashboard do Prestador
- Cards com solicitações pendentes
- Calendário de agendamentos
- Indicadores de performance
- **Estimativa**: 6h

---

#### 🎯 US-006: Edição de Perfil
**Como** usuário  
**Quero** editar meu perfil  
**Para** manter minhas informações atualizadas

**Prioridade**: Média  
**Story Points**: 5  

**Tasks**:

**[BACK-009]** Endpoints de edição de perfil
- PUT /api/usuario/perfil
- PUT /api/cliente/perfil
- PUT /api/prestador/perfil
- **Estimativa**: 3h

**[FRONT-009]** Página de edição de perfil
- Formulário pré-preenchido
- Upload de foto de perfil
- **Estimativa**: 4h

---

## Sprint 2 - Catálogo de Serviços e Busca (2 semanas)

### 📦 ÉPICO 3: Gestão de Serviços Oferecidos

#### 🎯 US-007: Cadastro de Serviços pelo Prestador (RF02)
**Como** prestador  
**Quero** cadastrar os serviços que ofereço  
**Para** que clientes possam me contratar

**Prioridade**: Alta  
**Story Points**: 13  

**Tasks**:

**[BACK-010]** CRUD completo de serviços oferecidos
- POST /api/servicos
- PUT /api/servicos/{id}
- DELETE /api/servicos/{id}
- GET /api/prestador/{id}/servicos
- **Estimativa**: 5h

**[BACK-011]** Validações de serviços
- Validar categoria obrigatória
- Validar preço mínimo
- Verificar se prestador está ativo
- **Estimativa**: 2h

**[FRONT-010]** Página de gerenciamento de serviços
- Lista de serviços do prestador
- Botões de adicionar/editar/remover
- **Estimativa**: 5h

**[FRONT-011]** Modal/Formulário de serviço
- Campos: categoria, título, descrição, preço
- Validações
- **Estimativa**: 4h

---

#### 🎯 US-008: Gerenciamento de Portfólio (RF02)
**Como** prestador  
**Quero** adicionar fotos dos meus trabalhos  
**Para** mostrar minha qualidade aos clientes

**Prioridade**: Média  
**Story Points**: 8  

**Tasks**:

**[BACK-012]** Implementar upload de imagens
- Configurar armazenamento (Azure Blob/AWS S3/Local)
- Endpoint POST /api/portfolio
- Redimensionamento e otimização de imagens
- **Estimativa**: 6h

**[BACK-013]** CRUD de portfólio
- GET /api/prestador/{id}/portfolio
- DELETE /api/portfolio/{id}
- Ordenação de imagens
- **Estimativa**: 3h

**[FRONT-012]** Componente de upload de fotos
- Drag-and-drop de imagens
- Preview antes do upload
- Cropper de imagem
- **Estimativa**: 5h

**[FRONT-013]** Galeria de portfólio
- Grid de imagens
- Lightbox para visualização
- Reordenação drag-and-drop
- **Estimativa**: 4h

---

#### 🎯 US-009: Gerenciamento de Certificações (RF02)
**Como** prestador  
**Quero** adicionar minhas certificações  
**Para** aumentar minha credibilidade

**Prioridade**: Baixa  
**Story Points**: 5  

**Tasks**:

**[BACK-014]** CRUD de certificações
- POST/PUT/DELETE /api/certificacoes
- Upload de documento PDF
- **Estimativa**: 3h

**[FRONT-014]** Interface de certificações
- Formulário de adição
- Lista com status de verificação
- **Estimativa**: 3h

---

### 📦 ÉPICO 4: Busca e Descoberta de Serviços

#### 🎯 US-010: Busca Avançada de Serviços (RF03, RF07)
**Como** cliente  
**Quero** buscar serviços por categoria, localização e preço  
**Para** encontrar o prestador ideal

**Prioridade**: Alta  
**Story Points**: 13  

**Tasks**:

**[BACK-015]** Aprimorar endpoint de busca
- Filtros: categoria, cidade, preço, avaliação
- Ordenação por relevância/preço/avaliação
- Paginação
- **Estimativa**: 5h

**[BACK-016]** Implementar busca por geolocalização
- Calcular distância entre prestador e cliente
- Filtro por raio de atendimento
- **Estimativa**: 4h

**[FRONT-015]** Melhorar página de busca
- Sidebar com filtros
- Cards de resultados
- Paginação/scroll infinito
- **Estimativa**: 6h

**[FRONT-016]** Implementar geolocalização
- Solicitar localização do usuário
- Exibir prestadores próximos em mapa
- Integração com Google Maps/Leaflet
- **Estimativa**: 5h

---

#### 🎯 US-011: Visualização de Perfil do Prestador (RF09)
**Como** cliente  
**Quero** visualizar o perfil completo do prestador  
**Para** decidir se quero contratá-lo

**Prioridade**: Alta  
**Story Points**: 8  

**Tasks**:

**[BACK-017]** Endpoint de perfil público do prestador
- GET /api/prestador/{id}/perfil-publico
- Incluir serviços, avaliações, portfólio, certificações
- **Estimativa**: 3h

**[FRONT-017]** Página de perfil do prestador
- Hero section com foto e dados principais
- Abas: Serviços, Portfólio, Avaliações, Sobre
- Botão de contratar/agendar
- **Estimativa**: 7h

---

## Sprint 3 - Agendamento e Pagamentos (2 semanas)

### 📦 ÉPICO 5: Sistema de Agendamento

#### 🎯 US-012: Solicitar Agendamento (RF04)
**Como** cliente  
**Quero** solicitar um agendamento de serviço  
**Para** contratar o prestador

**Prioridade**: Alta  
**Story Points**: 13  

**Tasks**:

**[BACK-018]** Validações de agendamento
- Verificar disponibilidade do prestador
- Validar data/hora futura
- Verificar conflitos de agenda
- **Estimativa**: 4h

**[BACK-019]** Criar sistema de notificações
- Notificar prestador de nova solicitação
- Notificar cliente sobre status
- **Estimativa**: 5h

**[FRONT-018]** Modal de agendamento
- Seleção de data/hora
- Calendário com disponibilidade
- Campo de observações
- Resumo do pedido
- **Estimativa**: 6h

**[FRONT-019]** Integração com calendário
- Exibir horários disponíveis
- Bloqueio de horários indisponíveis
- **Estimativa**: 4h

---

#### 🎯 US-013: Gerenciar Disponibilidade (NEG05)
**Como** prestador  
**Quero** definir meus horários de disponibilidade  
**Para** receber solicitações apenas quando posso atender

**Prioridade**: Alta  
**Story Points**: 8  

**Tasks**:

**[BACK-020]** CRUD de disponibilidade
- POST/PUT/DELETE /api/disponibilidade
- Validar horários sobrepostos
- **Estimativa**: 4h

**[FRONT-020]** Interface de gerenciamento de agenda
- Calendário semanal
- Adição de horários por dia da semana
- Marcação de indisponibilidades
- **Estimativa**: 6h

---

#### 🎯 US-014: Aceitar/Recusar Solicitações (RF10)
**Como** prestador  
**Quero** aceitar ou recusar solicitações  
**Para** controlar minha agenda

**Prioridade**: Alta  
**Story Points**: 5  

**Tasks**:

**[BACK-021]** Endpoints de resposta a agendamento
- POST /api/agendamento/{id}/aceitar
- POST /api/agendamento/{id}/recusar
- Validar status atual
- **Estimativa**: 2h

**[FRONT-021]** Interface de solicitações pendentes
- Lista de solicitações
- Botões aceitar/recusar
- Modal para motivo de recusa
- **Estimativa**: 4h

---

#### 🎯 US-015: Acompanhar Status do Agendamento
**Como** cliente e prestador  
**Quero** acompanhar o status do agendamento  
**Para** saber em que etapa está

**Prioridade**: Média  
**Story Points**: 5  

**Tasks**:

**[BACK-022]** Implementar máquina de estados
- Endpoint GET /api/agendamento/{id}/historico
- Logs de mudanças de status
- **Estimativa**: 3h

**[FRONT-022]** Componente de timeline de status
- Stepper visual do status
- Histórico de mudanças
- **Estimativa**: 4h

---

### 📦 ÉPICO 6: Sistema de Pagamentos

#### 🎯 US-016: Processar Pagamento (RF05, NEG04)
**Como** cliente  
**Quero** pagar pelo serviço através da plataforma  
**Para** ter segurança na transação

**Prioridade**: Alta  
**Story Points**: 21  

**Tasks**:

**[BACK-023]** Integração com gateway de pagamento
- Configurar Stripe/PagSeguro/Mercado Pago
- Criar endpoint de iniciar pagamento
- Webhook para confirmar pagamento
- **Estimativa**: 10h

**[BACK-024]** Implementar fluxo de pagamento
- Criar pagamento ao aceitar agendamento
- Processar pagamento após conclusão do serviço
- Implementar estorno
- **Estimativa**: 6h

**[FRONT-023]** Página de pagamento
- Formulário de dados de pagamento
- Integração com gateway (SDK)
- Opções: Pix, Débito, Crédito
- **Estimativa**: 8h

**[FRONT-024]** Componente de status de pagamento
- Indicador de pagamento pendente/aprovado
- Modal de confirmação de pagamento
- **Estimativa**: 3h

---

## Sprint 4 - Avaliações e Chat (2 semanas)

### 📦 ÉPICO 7: Sistema de Avaliações

#### 🎯 US-017: Avaliar Prestador (RF06, NEG06, NEG07)
**Como** cliente  
**Quero** avaliar o prestador após o serviço  
**Para** ajudar outros clientes

**Prioridade**: Alta  
**Story Points**: 8  

**Tasks**:

**[BACK-025]** Validar elegibilidade de avaliação
- Apenas serviços concluídos
- Apenas uma avaliação por agendamento
- **Estimativa**: 2h

**[BACK-026]** Atualizar média de avaliações
- Recalcular média ao adicionar avaliação
- Atualizar contador de avaliações
- **Estimativa**: 2h

**[FRONT-025]** Modal de avaliação
- Seleção de estrelas (0-5)
- Campo de comentário
- Upload de fotos (opcional)
- **Estimativa**: 4h

**[FRONT-026]** Exibir avaliações no perfil
- Lista de avaliações com paginação
- Filtros por nota
- **Estimativa**: 3h

---

#### 🎯 US-018: Responder Avaliações
**Como** prestador  
**Quero** responder às avaliações  
**Para** dar meu feedback

**Prioridade**: Baixa  
**Story Points**: 3  

**Tasks**:

**[BACK-027]** Adicionar resposta à avaliação
- PUT /api/avaliacao/{id}/responder
- **Estimativa**: 1h

**[FRONT-027]** Interface de resposta
- Campo de texto para resposta
- Exibição da resposta
- **Estimativa**: 2h

---

### 📦 ÉPICO 8: Sistema de Chat

#### 🎯 US-019: Chat entre Cliente e Prestador (RF08, NEG09)
**Como** cliente e prestador  
**Quero** conversar antes de confirmar o agendamento  
**Para** alinhar detalhes do serviço

**Prioridade**: Média  
**Story Points**: 21  

**Tasks**:

**[BACK-028]** Implementar SignalR/WebSockets
- Configurar SignalR Hub
- Eventos de envio/recebimento de mensagens
- Persistir mensagens no banco
- **Estimativa**: 8h

**[BACK-029]** Endpoints de mensagens
- GET /api/mensagens/conversas
- GET /api/mensagens/{usuarioId}
- POST /api/mensagens/enviar
- **Estimativa**: 4h

**[FRONT-028]** Componente de chat
- Lista de conversas
- Janela de mensagens
- Input com envio por Enter
- **Estimativa**: 8h

**[FRONT-029]** Notificações em tempo real
- Badge com contador de mensagens não lidas
- Som/notificação ao receber mensagem
- **Estimativa**: 3h

---

## Sprint 5 - Relatórios e Admin (2 semanas)

### 📦 ÉPICO 9: Painel Administrativo

#### 🎯 US-020: Dashboard Administrativo
**Como** administrador  
**Quero** visualizar métricas da plataforma  
**Para** monitorar o negócio

**Prioridade**: Média  
**Story Points**: 13  

**Tasks**:

**[BACK-030]** Endpoints de métricas administrativas
- Total de usuários, prestadores, clientes
- Total de agendamentos por status
- Receita total e comissões
- **Estimativa**: 5h

**[BACK-031]** Relatórios exportáveis
- Endpoint para exportar CSV/Excel
- **Estimativa**: 3h

**[FRONT-030]** Dashboard administrativo
- Gráficos de crescimento
- Tabelas de dados
- Filtros por período
- **Estimativa**: 8h

---

#### 🎯 US-021: Verificação de Prestadores (OB02)
**Como** administrador  
**Quero** verificar documentação dos prestadores  
**Para** garantir segurança da plataforma

**Prioridade**: Alta  
**Story Points**: 8  

**Tasks**:

**[BACK-032]** Sistema de verificação de prestadores
- Endpoint para listar prestadores pendentes
- Aprovar/reprovar documentos
- Alterar status do prestador
- **Estimativa**: 4h

**[FRONT-031]** Interface de verificação
- Lista de prestadores em análise
- Visualizar documentos enviados
- Botões aprovar/reprovar com motivo
- **Estimativa**: 5h

---

#### 🎯 US-022: Suspensão Automática por Avaliações (NEG08)
**Como** sistema  
**Quero** suspender prestadores com avaliações baixas  
**Para** manter qualidade da plataforma

**Prioridade**: Média  
**Story Points**: 5  

**Tasks**:

**[BACK-033]** Implementar job de monitoramento
- Background job para verificar avaliações
- Lógica de suspensão automática
- Notificação ao prestador
- **Estimativa**: 4h

**[FRONT-032]** Notificação de suspensão
- Banner de alerta no dashboard do prestador
- Instruções para regularização
- **Estimativa**: 2h

---

### 📦 ÉPICO 10: Histórico e Relatórios

#### 🎯 US-023: Histórico de Serviços do Cliente (RF11)
**Como** cliente  
**Quero** visualizar histórico de serviços contratados  
**Para** acompanhar minhas contratações

**Prioridade**: Média  
**Story Points**: 5  

**Tasks**:

**[BACK-034]** Endpoint de histórico
- GET /api/cliente/historico
- Filtros por período e status
- **Estimativa**: 2h

**[FRONT-033]** Página de histórico
- Lista de agendamentos passados
- Detalhes de cada serviço
- Link para avaliar (se não avaliado)
- **Estimativa**: 4h

---

#### 🎯 US-024: Histórico de Serviços do Prestador (RF12)
**Como** prestador  
**Quero** visualizar histórico de serviços realizados  
**Para** acompanhar meu desempenho

**Prioridade**: Média  
**Story Points**: 5  

**Tasks**:

**[BACK-035]** Endpoint de histórico do prestador
- GET /api/prestador/historico
- Incluir valores recebidos
- **Estimativa**: 2h

**[FRONT-034]** Página de histórico do prestador
- Lista de serviços realizados
- Estatísticas (total ganho, média de avaliação)
- **Estimativa**: 4h

---

## 🔧 Tasks Técnicas Adicionais

### Infraestrutura e DevOps

**[INFRA-001]** Configurar CI/CD
- GitHub Actions ou Azure DevOps
- Build, testes e deploy automatizados
- **Estimativa**: 8h

**[INFRA-002]** Configurar banco de dados MySQL
- Migrations iniciais
- Scripts de seed para desenvolvimento
- **Estimativa**: 4h

**[INFRA-003]** Configurar ambiente de homologação
- Deploy em servidor de staging
- **Estimativa**: 6h

**[INFRA-004]** Implementar logging e monitoramento
- Application Insights ou Sentry
- Logs estruturados
- **Estimativa**: 4h

### Testes

**[TEST-001]** Testes unitários do backend
- Cobertura mínima de 70%
- Testes de serviços críticos
- **Estimativa**: 20h (distribuído nas sprints)

**[TEST-002]** Testes de integração
- Testes de endpoints da API
- **Estimativa**: 15h (distribuído nas sprints)

**[TEST-003]** Testes E2E no frontend
- Cypress ou Playwright
- Fluxos principais
- **Estimativa**: 12h (Sprint 5)

### Documentação

**[DOC-001]** Documentação da API (Swagger)
- Endpoints documentados
- Exemplos de request/response
- **Estimativa**: 8h (Sprint 2)

**[DOC-002]** README e guias de setup
- Instruções de instalação
- Guia de contribuição
- **Estimativa**: 4h

---

## 📊 Resumo por Sprint

| Sprint | Story Points | Duração | Foco Principal |
|--------|--------------|---------|----------------|
| Sprint 1 | 36 | 2 semanas | Autenticação e Dashboards |
| Sprint 2 | 47 | 2 semanas | Catálogo e Busca |
| Sprint 3 | 52 | 2 semanas | Agendamento e Pagamentos |
| Sprint 4 | 32 | 2 semanas | Avaliações e Chat |
| Sprint 5 | 36 | 2 semanas | Admin e Relatórios |
| **TOTAL** | **203** | **10 semanas** | |

---

## 🎯 Priorização

### Must Have (Lançamento MVP)
- ✅ Autenticação (US-001, US-002)
- ✅ Cadastro de Serviços (US-007)
- ✅ Busca de Serviços (US-010)
- ✅ Perfil do Prestador (US-011)
- ✅ Agendamento (US-012, US-013, US-014)
- ✅ Pagamento (US-016)
- ✅ Avaliações (US-017)
- ✅ Verificação de Prestadores (US-021)

### Should Have (Pós-MVP)
- Chat (US-019)
- Portfólio (US-008)
- Histórico (US-023, US-024)
- Dashboard Admin (US-020)

### Could Have (Melhorias futuras)
- Certificações (US-009)
- Resposta a Avaliações (US-018)
- Geolocalização avançada

### Won't Have (Fora do escopo inicial)
- Integração com redes sociais
- Sistema de fidelidade/pontos
- Marketplace de produtos

---

## 📋 Definições de Pronto (DoD)

### Backend
- [ ] Código revisado por pelo menos 1 desenvolvedor
- [ ] Testes unitários implementados e passando
- [ ] Documentação da API atualizada (Swagger)
- [ ] Sem warnings de compilação
- [ ] Migrations criadas (se aplicável)

### Frontend
- [ ] Código revisado por pelo menos 1 desenvolvedor
- [ ] Componente responsivo (mobile/tablet/desktop)
- [ ] Sem erros no console
- [ ] Acessibilidade básica (WCAG 2.1 AA)
- [ ] Loading states implementados

### Ambos
- [ ] Critérios de aceite atendidos
- [ ] Testado manualmente pelo PO
- [ ] Deploy em ambiente de staging
- [ ] Documentação atualizada

---

## 🚀 Como Importar para o Jira

### Estrutura Sugerida:

1. **Criar Épicos** (9 épicos)
   - Use os títulos dos épicos acima
   - Defina cores distintas para cada épico

2. **Criar User Stories**
   - Link com o épico correspondente
   - Adicione story points
   - Defina prioridade

3. **Criar Tasks** dentro das User Stories
   - Prefixe com [BACK] ou [FRONT]
   - Adicione estimativa em horas
   - Atribua para desenvolvedores específicos

4. **Criar Sprints**
   - Agrupar stories por sprint sugerida
   - Ajustar capacidade da equipe

### Script de Importação CSV (Exemplo)

```csv
Issue Type,Summary,Epic Link,Story Points,Priority,Estimate
Epic,Autenticação e Gerenciamento de Usuários,,,Alta,
Story,Login de Usuários,Autenticação e Gerenciamento de Usuários,8,Alta,
Task,[BACK-001] Implementar autenticação com JWT,Login de Usuários,,Alta,5h
Task,[FRONT-001] Criar página de Login,Login de Usuários,,Alta,4h
...
```

---

## 📞 Contatos e Responsabilidades

- **Product Owner**: [Nome]
- **Scrum Master**: [Nome]
- **Tech Lead Backend**: [Nome]
- **Tech Lead Frontend**: [Nome]
- **Devs Backend**: [Nomes]
- **Devs Frontend**: [Nomes]

---

**Última atualização**: Dezembro 2024  
**Versão do documento**: 1.0
