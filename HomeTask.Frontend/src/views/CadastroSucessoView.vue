<template lang="pug">
.padding.center-align(style="min-height: 60vh; display: flex; flex-direction: column; justify-content: center; align-items: center;")
  i.extra.primary check_circle
  h4 Cadastro realizado com sucesso!
  p Sua conta foi criada. Faça login para começar.
  router-link.button.padding-top(to="/login")
    i login
    span Fazer Login
</template>
