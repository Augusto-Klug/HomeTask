# HomeTask - Kanban Board Structure

## 📋 Colunas do Board

```
┌──────────────┬──────────────┬──────────────┬──────────────┬──────────────┬──────────────┐
│   BACKLOG    │  TO DO       │  IN PROGRESS │  CODE REVIEW │   TESTING    │    DONE      │
│              │              │              │              │              │              │
│   Priorizado │  Sprint      │  Doing       │  PR Open     │  QA Testing  │  Deployed    │
│   e Refinado │  Atual       │              │              │              │              │
└──────────────┴──────────────┴──────────────┴──────────────┴──────────────┴──────────────┘
```

---

## 🏊 Swimlanes Sugeridos

### Por Tipo de Trabalho
```
┌─ FEATURES ────────────────────────────────────────────────────────┐
│ US-001, US-002, US-007, US-010, etc.                              │
└───────────────────────────────────────────────────────────────────┘

┌─ BUGS ────────────────────────────────────────────────────────────┐
│ BUG-001, BUG-002, etc.                                            │
└───────────────────────────────────────────────────────────────────┘

┌─ TECHNICAL DEBT ──────────────────────────────────────────────────┐
│ TECH-001, REFACTOR-001, etc.                                      │
└───────────────────────────────────────────────────────────────────┘

┌─ INFRASTRUCTURE ──────────────────────────────────────────────────┐
│ INFRA-001, INFRA-002, etc.                                        │
└───────────────────────────────────────────────────────────────────┘
```

### Por Componente (Alternativa)
```
┌─ BACKEND ─────────────────────────────────────────────────────────┐
│ BACK-001, BACK-002, BACK-003, etc.                               │
└───────────────────────────────────────────────────────────────────┘

┌─ FRONTEND ────────────────────────────────────────────────────────┐
│ FRONT-001, FRONT-002, FRONT-003, etc.                            │
└───────────────────────────────────────────────────────────────────┘

┌─ FULLSTACK ───────────────────────────────────────────────────────┐
│ US-001, US-002 (que possuem subtasks de ambos)                   │
└───────────────────────────────────────────────────────────────────┘
```

---

## 🎨 Sistema de Cores

### Por Prioridade
```css
🔴 Critical/Blocker  → Vermelho (#FF0000)
🟠 High              → Laranja (#FF8C00)
🟡 Medium            → Amarelo (#FFD700)
🟢 Low               → Verde (#32CD32)
⚪ Trivial           → Cinza (#D3D3D3)
```

### Por Épico
```css
🟦 EP-001: Autenticação        → Azul (#0052CC)
🟪 EP-002: Dashboard           → Roxo (#8B00FF)
🟩 EP-003: Serviços            → Verde (#00A86B)
🟨 EP-004: Busca               → Amarelo (#FFD700)
🟧 EP-005: Agendamento         → Laranja (#FF8C00)
🟥 EP-006: Pagamentos          → Vermelho (#DC143C)
🟫 EP-007: Avaliações          → Marrom (#8B4513)
🟪 EP-008: Chat                → Magenta (#FF00FF)
⬛ EP-009: Admin               → Preto (#2C3E50)
⬜ EP-010: Relatórios          → Cinza (#95A5A6)
```

### Por Status
```css
📝 Backlog          → Cinza Claro (#ECEFF1)
📋 To Do            → Azul Claro (#E3F2FD)
⚙️ In Progress      → Amarelo (#FFF9C4)
👀 Code Review      → Laranja Claro (#FFE0B2)
🧪 Testing          → Roxo Claro (#E1BEE7)
✅ Done             → Verde (#C8E6C9)
```

---

## 🏷️ Sistema de Labels

### Labels de Tecnologia
```
backend          → .NET 10, C#
frontend         → Vue3, TypeScript
database         → MySQL, Migrations
api              → REST API, Endpoints
```

### Labels de Funcionalidade
```
autenticacao     → Login, JWT, Sessão
cadastro         → Registro de usuário
dashboard        → Painéis de controle
busca            → Pesquisa e filtros
agendamento      → Calendário, Reservas
pagamentos       → Gateway, Transações
avaliacoes       → Ratings, Reviews
chat             → SignalR, Mensagens
admin            → Painel administrativo
```

### Labels de Tipo de Trabalho
```
mvp              → Essencial para lançamento
feature          → Nova funcionalidade
bug              → Correção de erro
refactor         → Melhoria de código
docs             → Documentação
test             → Testes
infra            → Infraestrutura
security         → Segurança
performance      → Otimização
```

### Labels de Sprint
```
sprint1          → Sprint 1 - Autenticação
sprint2          → Sprint 2 - Catálogo
sprint3          → Sprint 3 - Agendamentos
sprint4          → Sprint 4 - Avaliações
sprint5          → Sprint 5 - Admin
```

---

## 📊 Quick Filters

Configure filtros rápidos no Jira para facilitar a visualização:

### 1. **Meu Trabalho**
```jql
assignee = currentUser() AND status != Done
```

### 2. **Trabalho do Sprint Atual**
```jql
sprint in openSprints() AND status != Done
```

### 3. **Tasks de Backend**
```jql
labels = backend AND status != Done
```

### 4. **Tasks de Frontend**
```jql
labels = frontend AND status != Done
```

### 5. **MVPs Pendentes**
```jql
labels = mvp AND status != Done ORDER BY priority DESC
```

### 6. **Itens Bloqueados**
```jql
status = Blocked OR priority = Blocker
```

### 7. **Code Review Necessário**
```jql
status = "Code Review" AND reviewer is EMPTY
```

### 8. **Bugs Críticos**
```jql
type = Bug AND priority IN (Blocker, Critical, High) AND status != Done
```

### 9. **Itens Atrasados**
```jql
duedate < now() AND status != Done
```

### 10. **Sem Estimativa**
```jql
"Story Points" is EMPTY AND type = Story
```

---

## 📈 Dashboards Recomendados

### Dashboard 1: Sprint Overview
```
┌─────────────────────────────────────────────────────────────┐
│  SPRINT BURNDOWN         │  VELOCITY CHART                  │
├─────────────────────────────────────────────────────────────┤
│  SPRINT PROGRESS         │  CUMULATIVE FLOW                 │
├─────────────────────────────────────────────────────────────┤
│  WORK BREAKDOWN          │  TEAM CAPACITY                   │
└─────────────────────────────────────────────────────────────┘
```

**Gadgets:**
- Sprint Burndown Chart
- Sprint Health Gadget
- Days Remaining
- Work Breakdown (Pie Chart)
- Velocity Chart
- Cumulative Flow Diagram

### Dashboard 2: Team Performance
```
┌─────────────────────────────────────────────────────────────┐
│  ISSUES BY ASSIGNEE      │  AVG TIME IN STATUS              │
├─────────────────────────────────────────────────────────────┤
│  CODE REVIEW METRICS     │  BUG RESOLUTION TIME             │
├─────────────────────────────────────────────────────────────┤
│  CREATED VS RESOLVED     │  ISSUES BY COMPONENT             │
└─────────────────────────────────────────────────────────────┘
```

**Gadgets:**
- Assigned to Me
- Issue Statistics (By Assignee)
- Average Age Chart
- Resolution Time
- Created vs Resolved
- Pie Chart (By Component)

### Dashboard 3: Product Overview
```
┌─────────────────────────────────────────────────────────────┐
│  EPIC PROGRESS           │  EPIC BURNDOWN                   │
├─────────────────────────────────────────────────────────────┤
│  RELEASES TIMELINE       │  BUGS BY PRIORITY                │
├─────────────────────────────────────────────────────────────┤
│  ROADMAP                 │  BACKLOG STATUS                  │
└─────────────────────────────────────────────────────────────┘
```

**Gadgets:**
- Epic Report
- Epic Burndown
- Road Map
- Filter Results (Bugs)
- Two Dimensional Filter Statistics
- Version Workload

---

## 🔄 Workflow Sugerido

### Para User Stories e Tasks

```
┌─────────┐
│ BACKLOG │
└────┬────┘
     │ Prioritize & Refine
     ↓
┌─────────┐
│ TO DO   │ ← Sprint Planning
└────┬────┘
     │ Start Work
     ↓
┌──────────────┐
│ IN PROGRESS  │
└────┬─────────┘
     │ Create PR
     ↓
┌──────────────┐
│ CODE REVIEW  │
└────┬─────────┘
     │ Approve PR
     ↓
┌──────────┐
│ TESTING  │
└────┬─────┘
     │ QA Pass
     ↓
┌──────┐
│ DONE │
└──────┘
```

### Status Transitions

| De | Para | Condição | Quem Pode |
|---|---|---|---|
| Backlog | To Do | Sprint Planning | Scrum Master |
| To Do | In Progress | Start Development | Developer |
| In Progress | Code Review | PR Created | Developer |
| Code Review | In Progress | Changes Requested | Reviewer |
| Code Review | Testing | PR Approved & Merged | Reviewer |
| Testing | In Progress | Test Failed | QA |
| Testing | Done | All Tests Pass | QA/PO |
| In Progress | Blocked | Impediment | Developer |
| Blocked | In Progress | Impediment Resolved | Developer |

---

## 🎯 Definition of Done (DoD) Checklist

Use como subtasks ou checklist nos cards:

### Para Backend Tasks
```markdown
- [ ] Código implementado seguindo padrões do projeto
- [ ] Testes unitários escritos e passando (>70% cobertura)
- [ ] Testes de integração criados (se aplicável)
- [ ] Documentação da API atualizada (Swagger)
- [ ] Code review aprovado por pelo menos 1 dev
- [ ] Sem warnings de compilação
- [ ] Migrations criadas (se banco de dados)
- [ ] Logs apropriados adicionados
- [ ] Tratamento de erros implementado
- [ ] Merge na branch develop realizado
```

### Para Frontend Tasks
```markdown
- [ ] Componente implementado e funcionando
- [ ] Responsivo (Mobile, Tablet, Desktop)
- [ ] Sem erros no console do navegador
- [ ] Acessibilidade básica (WCAG 2.1 AA)
- [ ] Loading/Error states implementados
- [ ] Validações de formulário (se aplicável)
- [ ] Integração com API testada
- [ ] Code review aprovado por pelo menos 1 dev
- [ ] Tradução/i18n configurada (se aplicável)
- [ ] Merge na branch develop realizado
```

### Para User Stories
```markdown
- [ ] Todos os critérios de aceite atendidos
- [ ] Todas as subtasks concluídas
- [ ] Testado manualmente pelo PO
- [ ] Deploy em ambiente de staging realizado
- [ ] Documentação do usuário criada/atualizada
- [ ] Demo realizada na Sprint Review
- [ ] PO aprovou a funcionalidade
```

---

## 📅 Cerimônias do Scrum

### Sprint Planning (Início da Sprint)
**Duração:** 4h (para sprint de 2 semanas)
**Participantes:** Todo o time
**Artefatos:**
- Product Backlog refinado
- Velocity da equipe
- Capacity do time

**Resultado:**
- Sprint Backlog definido
- Sprint Goal estabelecido
- Tasks estimadas

### Daily Standup (Diário)
**Duração:** 15min
**Participantes:** Time de desenvolvimento
**Formato:**
1. O que fiz ontem?
2. O que farei hoje?
3. Há algum impedimento?

### Sprint Review (Final da Sprint)
**Duração:** 2h
**Participantes:** Time + Stakeholders
**Agenda:**
- Demo das funcionalidades
- Feedback dos stakeholders
- Atualização do Product Backlog

### Sprint Retrospective (Final da Sprint)
**Duração:** 1.5h
**Participantes:** Time de desenvolvimento
**Formato:**
- O que funcionou bem?
- O que pode melhorar?
- Ações para a próxima sprint

### Backlog Refinement (Durante a Sprint)
**Duração:** 1h (semanal)
**Participantes:** PO + Time
**Objetivo:**
- Refinar próximas user stories
- Adicionar critérios de aceite
- Estimar story points

---

## 🏃 Squad Sugerido

### Composição do Time

```
┌─────────────────────────────────────────────────────┐
│                  PRODUCT OWNER                      │
│            (Define o que fazer)                     │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────┴──────────────────────────────┐
│                 SCRUM MASTER                        │
│           (Remove impedimentos)                     │
└──────────────────────┬──────────────────────────────┘
                       │
       ┌───────────────┼───────────────┐
       │               │               │
┌──────┴──────┐ ┌─────┴──────┐ ┌─────┴──────┐
│   BACKEND   │ │  FRONTEND  │ │   DEVOPS   │
│             │ │            │ │            │
│ • Dev 1     │ │ • Dev 1    │ │ • Eng 1    │
│ • Dev 2     │ │ • Dev 2    │ └────────────┘
│ • Dev 3     │ │ • Dev 3    │
└─────────────┘ └────────────┘
       │               │
       └───────┬───────┘
               │
        ┌──────┴──────┐
        │     QA      │
        │   Tester    │
        └─────────────┘
```

**Capacidade Total (exemplo):**
- 6 Desenvolvedores × 6h/dia × 10 dias = 360h
- Overhead (meetings, etc) -20% = 288h úteis

---

## 📊 Métricas a Acompanhar

### Métricas de Sprint
```
┌──────────────────────────┬──────────────┐
│ Métrica                  │ Meta         │
├──────────────────────────┼──────────────┤
│ Velocity                 │ ~40-50 SP    │
│ Commitment Reliability   │ >85%         │
│ Sprint Goal Success      │ 100%         │
│ Code Review Time         │ <24h         │
│ Bug Escape Rate          │ <5%          │
│ Technical Debt Ratio     │ <20%         │
└──────────────────────────┴──────────────┘
```

### Métricas de Qualidade
```
┌──────────────────────────┬──────────────┐
│ Métrica                  │ Meta         │
├──────────────────────────┼──────────────┤
│ Code Coverage            │ >70%         │
│ Bugs per Story Point     │ <0.5         │
│ Mean Time to Recovery    │ <4h          │
│ Defect Density           │ <2 per KLOC  │
└──────────────────────────┴──────────────┘
```

---

## 🎁 Templates de Issues

### Template: Bug Report
```markdown
## 🐛 Descrição do Bug
[Descreva o bug de forma clara e concisa]

## 📋 Passos para Reproduzir
1. Vá para '...'
2. Clique em '...'
3. Role até '...'
4. Veja o erro

## ✅ Comportamento Esperado
[Descreva o que deveria acontecer]

## ❌ Comportamento Atual
[Descreva o que está acontecendo]

## 📸 Screenshots
[Se aplicável, adicione screenshots]

## 🖥️ Ambiente
- OS: [e.g. Windows 10]
- Browser: [e.g. Chrome 120]
- Versão: [e.g. 1.0.0]

## 📝 Informações Adicionais
[Qualquer contexto adicional]

## 🔗 Relacionado a
[Links para issues/PRs relacionados]
```

### Template: User Story
```markdown
## 👤 Como [tipo de usuário]
## 🎯 Quero [ação/funcionalidade]
## 💡 Para [benefício/valor]

## 📝 Critérios de Aceite
- [ ] Critério 1
- [ ] Critério 2
- [ ] Critério 3

## 📋 Definição de Pronto (DoD)
- [ ] Código implementado
- [ ] Testes criados e passando
- [ ] Code review aprovado
- [ ] Documentação atualizada
- [ ] Deploy em staging
- [ ] PO aprovou

## 🎨 Design/Mockups
[Links ou attachments]

## 🔗 Dependências
- Depende de: #123
- Bloqueia: #456

## 📊 Story Points
[A ser definido na Planning]

## 📝 Notas Técnicas
[Considerações de implementação]
```

---

**Última atualização:** Dezembro 2024  
**Versão:** 1.0  
**Mantido por:** Time HomeTask
