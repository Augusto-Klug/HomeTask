<template>
  <Teleport to="body">
    <!-- Overlay -->
    <Transition name="ht-fade">
      <div
        v-if="open"
        class="fixed inset-0 z-40 bg-black/40 backdrop-blur-sm"
        @click="emit('update:open', false)"
      />
    </Transition>

    <!-- Painel lateral -->
    <Transition name="ht-slide">
      <aside
        v-if="open"
        class="fixed left-0 top-0 z-50 h-full flex flex-col shadow-xl"
        :style="{ width: 'var(--ht-sidebar-w, 260px)', maxWidth: '85vw' }"
      >
        <!-- Header -->
        <div class="flex items-center justify-between px-4 h-14 border-b border-base-300 bg-base-100 shrink-0">
          <span class="text-base font-semibold text-base-content">
            <slot name="title">Menu</slot>
          </span>
          <button
            type="button"
            class="btn btn-ghost btn-sm btn-square"
            @click="emit('update:open', false)"
          >
            <span class="material-symbols-rounded text-xl">close</span>
          </button>
        </div>

        <!-- Conteúdo -->
        <div class="flex-1 overflow-y-auto py-2 bg-base-100">
          <slot />
        </div>

        <!-- Rodapé opcional -->
        <div
          v-if="$slots.footer"
          class="border-t border-base-300 py-3 px-4 shrink-0 bg-base-100"
        >
          <slot name="footer" />
        </div>
      </aside>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
defineProps<{ open: boolean }>()

const emit = defineEmits<{ 'update:open': [value: boolean] }>()
</script>
