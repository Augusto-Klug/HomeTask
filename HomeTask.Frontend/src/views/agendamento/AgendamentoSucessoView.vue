<template lang="pug">
.padding.center-align(style="min-height: 60vh; display: flex; flex-direction: column; justify-content: center; align-items: center;")
  i.extra.primary check_circle
  h4 Agendamento realizado!
  p Seu agendamento foi criado com sucesso. O prestador entrará em contato em breve.
  .row.wrap.center-align.padding-top
    router-link.button.border(to="/servicos/buscar")
      i search
      span Buscar mais serviços
    router-link.button(to="/")
      i home
      span Início
</template>
