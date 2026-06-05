# AGENTS.md

Guia principal do repositorio. Leia este arquivo antes dos AGENTS.md mais especificos.

## Regras gerais

- Preserve o menor diff correto e nao reverta alteracoes de terceiros.
- Em componentes Vue, todo objeto mutavel deve usar `reactive`; use `ref` para primitivos, arrays simples e referencias nulas quando fizer sentido.
- Enums do frontend devem seguir o backend em nome e valor. Nao converta enum para string no fluxo de dados; converta apenas no ponto de exibicao de texto.
- Componentes `.vue` devem receber enums nas props quando a prop representa um conjunto fechado de opcoes.
- Em arquivos `.vue`, o bloco `<template>` deve vir antes do bloco `<script>`.
- Em `HomeTask.Frontend/src/types`, mantenha enums em `enums.ts`, interfaces/types em `interfaces.ts` e reexports em `index.ts`.
- Para badges, use `HtBadgeVariant` de `HomeTask.Frontend/src/components/ui/HtBadge.vue`; converta status para variant com funcao local no ponto de uso.
- Em componentes `.vue`, constantes e mapeamentos usados apenas para exibicao no frontend, como labels e conversoes para badge variant, devem ficar no final do arquivo.

## AGENTS.md relacionados

- Backend/projetos .NET: cada projeto com `csproj` deve ter apenas um `AGENTS.md` na raiz do projeto. Veja `HomeTask.Application/AGENTS.md`, `HomeTask.Domain/AGENTS.md`, `HomeTask.Infrastructure/AGENTS.md`, `HomeTask.WebApi/AGENTS.md` e `HomeTask.Tests/AGENTS.md`.
- Frontend: veja `HomeTask.Frontend/AGENTS.md` e `HomeTask.Frontend/src/AGENTS.md` para regras Vue/TypeScript mais detalhadas.
- Pastas geradas ou de dependencia (`bin`, `obj`, `dist`, `node_modules`) nao recebem AGENTS.md.
