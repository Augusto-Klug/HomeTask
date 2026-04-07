<template>
  <DialogRoot :open="open" @update:open="emit('update:open', $event)">
    <DialogPortal>
      <!-- Overlay -->
      <DialogOverlay
        class="fixed inset-0 z-40 bg-black/40 backdrop-blur-sm
               data-[state=open]:animate-in data-[state=closed]:animate-out
               data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0"
        @click="emit('update:open', false)"
      />

      <!-- Painel lateral -->
      <DialogContent
        class="fixed left-0 top-0 z-50 h-full w-[var(--ht-sidebar-w)] max-w-[85vw]
               bg-card border-r border-border flex flex-col shadow-xl
               data-[state=open]:animate-in data-[state=closed]:animate-out
               data-[state=closed]:slide-out-to-left data-[state=open]:slide-in-from-left
               duration-300 ease-in-out focus:outline-none"
        :aria-describedby="undefined"
      >
        <!-- Header do sidenav -->
        <div class="flex items-center justify-between px-4 h-14 border-b border-border shrink-0">
          <DialogTitle class="text-title font-semibold text-foreground">
            <slot name="title">Menu</slot>
          </DialogTitle>
          <DialogClose
            class="p-1.5 rounded-lg text-muted hover:text-foreground hover:bg-surface transition-colors"
            @click="emit('update:open', false)"
          >
            <span class="material-symbols-rounded text-xl">close</span>
          </DialogClose>
        </div>

        <!-- Conteúdo (links, seções, etc.) -->
        <div class="flex-1 overflow-y-auto py-2">
          <slot />
        </div>

        <!-- Rodapé opcional -->
        <div v-if="$slots.footer" class="border-t border-border py-3 px-4 shrink-0">
          <slot name="footer" />
        </div>
      </DialogContent>
    </DialogPortal>
  </DialogRoot>
</template>

<script setup lang="ts">
import {
  DialogRoot,
  DialogPortal,
  DialogOverlay,
  DialogContent,
  DialogClose,
  DialogTitle,
} from 'reka-ui'

defineProps<{ open: boolean }>()

const emit = defineEmits<{ 'update:open': [value: boolean] }>()
</script>
