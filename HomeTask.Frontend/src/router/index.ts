import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/stores/auth";

const router = createRouter({
  history: createWebHistory(),
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
    // Agendamento
    {
      path: "/agendamento/novo",
      name: "agendamento-novo",
      // servicoId e prestadorId chegam via query: ?servicoId=x&prestadorId=y
      component: () => import("@/views/agendamento/NovoAgendamentoView.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/agendamento/sucesso",
      name: "agendamento-sucesso",
      component: () => import("@/views/agendamento/AgendamentoSucessoView.vue"),
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
