import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/stores/auth";

const router = createRouter({
  history: createWebHistory(),
  scrollBehavior: () => ({ top: 0 }),
  routes: [
    // Rota raiz → Home
    {
      path: "/",
      name: "home",
      component: () => import("@/views/HomeView.vue"),
    },
    // Autenticação
    {
      path: "/login",
      name: "login",
      component: () => import("@/views/LoginView.vue"),
      meta: { guestOnly: true },
    },
    {
      path: "/cadastro",
      name: "cadastro",
      component: () => import("@/views/CadastroView.vue"),
      meta: { guestOnly: true },
    },
    {
      path: "/cadastro-sucesso",
      name: "cadastro-sucesso",
      component: () => import("@/views/CadastroSucessoView.vue"),
    },
    {
      path: "/esqueci-senha",
      name: "esqueci-senha",
      component: () => import("@/views/EsqueciSenhaView.vue"),
      meta: { guestOnly: true },
    },
    {
      path: "/redefinir-senha",
      name: "redefinir-senha",
      // token vem via query string: /redefinir-senha?token=xxx
      component: () => import("@/views/RedefinirSenhaView.vue"),
      meta: { guestOnly: true },
    },
    // Serviços
    {
      path: "/servicos/buscar",
      name: "servicos-buscar",
      component: () => import("@/views/servicos/BuscarServicosView.vue"),
    },
    {
      path: "/servicos/detalhes/:id",
      name: "servicos-detalhes",
      component: () => import("@/views/servicos/DetalhesServicoView.vue"),
      props: true,
    },
    {
      path: "/servicos/novo-cliente",
      name: "servicos-novo-cliente",
      component: () =>
        import("@/views/servicos/CadastrarServicoClienteView.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/servicos/novo-prestador",
      name: "servicos-novo-prestador",
      component: () =>
        import("@/views/servicos/CadastrarServicoPrestadorView.vue"),
      // meta: { requiresAuth: false },
    },
    // Agendamento
    {
      path: "/agendamento/novo/:servicoId",
      name: "agendamento-novo",
      // servicoId no path; prestadorId chega via query: ?prestadorId=y
      component: () => import("@/views/agendamento/NovoAgendamentoView.vue"),
      props: true,
      // meta: { requiresAuth: true },
    },
    {
      path: "/agendamento/sucesso",
      name: "agendamento-sucesso",
      component: () => import("@/views/agendamento/AgendamentoSucessoView.vue"),
    },
    {
      path: "/agendamento/detalhes/:id",
      name: "agendamento-detalhes",
      component: () => import("@/views/agendamento/DetalhesAgendamentoView.vue"),
    },
    // Perfil
    {
      path: "/perfil/agendamentos",
      name: "perfil-agendamentos",
      component: () => import("@/views/perfil/MeusAgendamentosView.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/perfil/minha-conta",
      name: "perfil-minha-conta",
      component: () => import("@/views/perfil/MinhaContaView.vue"),
      meta: { requiresAuth: true },
    },
    // Fallback
    {
      path: "/:pathMatch(.*)*",
      redirect: "/",
    },
  ],
});

// Navigation guard
router.beforeEach((to) => {
  const auth = useAuthStore();

  if (to.meta.requiresAuth && !auth.isLoggedIn) {
    return { name: "login", query: { redirect: to.fullPath } };
  }

  if (to.meta.guestOnly && auth.isLoggedIn) {
    return { name: "home" };
  }
});

export default router;
