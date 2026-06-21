<template>
  <section
    class="fixed bottom-4 right-4 z-40 w-[calc(100vw-2rem)] max-w-sm overflow-hidden rounded-2xl border border-base-300 bg-base-100 shadow-2xl transition-all duration-200 sm:right-6"
    :class="aberto ? 'h-[34rem]' : 'h-16'"
    aria-label="Chat do agendamento"
  >
    <button
      type="button"
      class="flex h-16 w-full items-center justify-between gap-3 bg-base-100 px-4 text-left transition-colors hover:bg-base-200"
      :aria-expanded="aberto"
      @click="alternarChat"
    >
      <div class="flex min-w-0 items-center gap-3">
        <div
          class="relative flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-primary/10 font-bold text-primary"
        >
          {{ iniciaisParticipante }}
          <span class="absolute bottom-0 right-0 h-3 w-3 rounded-full border-2 border-base-100 bg-success" />
        </div>
        <div class="min-w-0">
          <p class="truncate text-sm font-bold">Chat do agendamento</p>
          <p class="truncate text-xs text-muted">{{ nomeParticipante }}</p>
        </div>
      </div>
      <span class="material-symbols-rounded text-muted">
        {{ aberto ? "keyboard_arrow_down" : "chat" }}
      </span>
    </button>

    <div v-show="aberto" class="flex h-[calc(34rem-4rem)] flex-col border-t border-base-300">
      <div class="bg-base-200/50 px-4 py-3">
        <p class="mt-1 text-xs text-muted">
          Você pode enviar até 3 mensagens seguidas. Depois disso, aguarde a resposta do outro participante.
        </p>
      </div>

      <div ref="listaMensagens" class="flex-1 space-y-3 overflow-y-auto px-4 py-4">
        <div v-if="mensagens.length === 0" class="flex h-full flex-col items-center justify-center text-center">
          <div class="mb-3 flex h-14 w-14 items-center justify-center rounded-full bg-primary/10 text-primary">
            <span class="material-symbols-rounded">forum</span>
          </div>
          <p class="font-semibold">Nenhuma mensagem ainda</p>
          <p class="mt-1 max-w-56 text-sm text-muted">
            Envie a primeira mensagem para combinar os detalhes do serviço.
          </p>
        </div>

        <div
          v-for="mensagem in mensagens"
          :key="mensagem.id"
          class="flex"
          :class="mensagem.enviadaPorAtual ? 'justify-end' : 'justify-start'"
        >
          <div
            class="max-w-[78%] rounded-2xl px-3 py-2 text-sm shadow-sm"
            :class="
              mensagem.enviadaPorAtual
                ? 'rounded-br-sm bg-primary text-primary-content'
                : 'rounded-bl-sm bg-base-200 text-base-content'
            "
          >
            <p class="whitespace-pre-wrap break-words">{{ mensagem.texto }}</p>
            <p
              class="mt-1 text-right text-[11px]"
              :class="mensagem.enviadaPorAtual ? 'text-primary-content/75' : 'text-muted'"
            >
              {{ formatarHorario(mensagem.dataEnvio) }}
            </p>
          </div>
        </div>
      </div>

      <form class="border-t border-base-300 bg-base-100 p-3" @submit.prevent="enviarMensagem">
        <p v-if="erro" class="mb-2 text-xs text-error">{{ erro }}</p>
        <p v-else-if="limiteConsecutivoAtingido" class="mb-2 text-xs text-warning">
          Limite de 3 mensagens seguidas atingido. Aguarde a resposta.
        </p>
        <div class="flex items-end gap-2">
          <HtTextarea
            v-model="mensagemTexto"
            class="min-w-0 flex-1"
            placeholder="Digite sua mensagem..."
            :rows="1"
            @keydown.enter.exact.prevent="enviarMensagem"
          />
          <HtButton
            class="h-12 w-12 shrink-0 rounded-full p-0"
            type="submit"
            :disabled="!podeEnviar"
            aria-label="Enviar mensagem"
          >
            <span class="material-symbols-rounded">send</span>
          </HtButton>
        </div>
      </form>
    </div>
  </section>
</template>

<script setup lang="ts">
import * as signalR from "@microsoft/signalr";
import { computed, nextTick, onBeforeUnmount, onMounted, ref } from "vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtTextarea from "@/components/ui/HtTextarea.vue";
import api, { API_BASE_URL } from "@/services/api";
import type { MensagemChat } from "@/types";

interface ChatMensagem {
  id: string;
  texto: string;
  dataEnvio: Date;
  remetenteId: string;
  enviadaPorAtual: boolean;
}

const props = defineProps<{
  agendamentoId: string;
  usuarioAtualId: string;
  nomeParticipante: string;
  tituloServico: string;
}>();

const aberto = defineModel<boolean>("aberto", { default: false });
const mensagemTexto = ref("");
const mensagens = ref<ChatMensagem[]>([]);
const listaMensagens = ref<HTMLElement | null>(null);
const conexao = ref<signalR.HubConnection | null>(null);
const erro = ref<string | null>(null);
const conectando = ref(false);

const limiteConsecutivoAtingido = computed(() => {
  const ultimas = mensagens.value.slice(-3);
  return ultimas.length === 3 && ultimas.every((mensagem) => mensagem.enviadaPorAtual);
});

const podeEnviar = computed(
  () =>
    mensagemTexto.value.trim().length > 0 &&
    !limiteConsecutivoAtingido.value &&
    conexao.value?.state === signalR.HubConnectionState.Connected,
);

const iniciaisParticipante = computed(() => {
  const partesNome = props.nomeParticipante.trim().split(/\s+/).filter(Boolean);
  const iniciais = partesNome
    .slice(0, 2)
    .map((parte) => parte.charAt(0).toUpperCase())
    .join("");

  return iniciais || "HT";
});

onMounted(async () => {
  await carregarHistorico();
  await conectarChat();
});

onBeforeUnmount(async () => {
  await conexao.value?.stop();
});

function alternarChat() {
  aberto.value = !aberto.value;

  if (aberto.value) {
    rolarParaFim();
  }
}

async function enviarMensagem() {
  const texto = mensagemTexto.value.trim();
  if (!texto || !conexao.value || limiteConsecutivoAtingido.value) return;

  try {
    erro.value = null;
    await conexao.value.invoke("EnviarMensagem", props.agendamentoId, texto);
    mensagemTexto.value = "";
  } catch (error) {
    erro.value = error instanceof Error ? error.message : "Não foi possível enviar a mensagem.";
  }
}

async function carregarHistorico() {
  try {
    erro.value = null;
    const { data } = await api.get<MensagemChat[]>("/api/Mensagem/ObterPorAgendamento", {
      params: { agendamentoId: props.agendamentoId },
    });
    mensagens.value = data.map(mapearMensagem);
    await rolarParaFim();
  } catch {
    erro.value = "Não foi possível carregar as mensagens.";
  }
}

async function conectarChat() {
  if (conectando.value || conexao.value) return;

  conectando.value = true;
  const novaConexao = new signalR.HubConnectionBuilder()
    .withUrl(`${API_BASE_URL}/hubs/agendamento-chat`, { withCredentials: true })
    .withAutomaticReconnect()
    .build();

  novaConexao.on("MensagemRecebida", async (mensagem: MensagemChat) => {
    adicionarOuAtualizarMensagem(mapearMensagem(mensagem));
    await rolarParaFim();
  });

  novaConexao.onreconnected(async () => {
    await novaConexao.invoke("EntrarNoAgendamento", props.agendamentoId);
  });

  try {
    await novaConexao.start();
    await novaConexao.invoke("EntrarNoAgendamento", props.agendamentoId);
    conexao.value = novaConexao;
  } catch {
    erro.value = "Não foi possível conectar ao chat em tempo real.";
    await novaConexao.stop();
  } finally {
    conectando.value = false;
  }
}

function adicionarOuAtualizarMensagem(mensagem: ChatMensagem) {
  const indice = mensagens.value.findIndex((item) => item.id === mensagem.id);
  if (indice >= 0) {
    mensagens.value[indice] = mensagem;
    return;
  }

  mensagens.value.push(mensagem);
}

function mapearMensagem(mensagem: MensagemChat): ChatMensagem {
  return {
    id: mensagem.id,
    texto: mensagem.conteudo,
    dataEnvio: new Date(mensagem.dataEnvio),
    remetenteId: mensagem.remetenteId,
    enviadaPorAtual: mensagem.remetenteId === props.usuarioAtualId,
  };
}

async function rolarParaFim() {
  await nextTick();
  if (!listaMensagens.value) return;

  listaMensagens.value.scrollTop = listaMensagens.value.scrollHeight;
}

function formatarHorario(data: Date) {
  return new Intl.DateTimeFormat("pt-BR", {
    hour: "2-digit",
    minute: "2-digit",
  }).format(data);
}
</script>
