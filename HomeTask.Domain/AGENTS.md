# AGENTS.md

Guia do projeto Domain. Leia tambem `../AGENTS.md`.

## Regras locais

- O dominio e a fonte de verdade para entidades, enums e regras de negocio.
- Alteracoes em enums do dominio exigem atualizacao equivalente no frontend em `HomeTask.Frontend/src/types/enums.ts`.
- `Common/`: codigo comum do dominio nao deve depender de Application, Infrastructure ou WebApi.
- `Contratos/`: contratos do dominio devem manter linguagem ubiqua e tipos fortes.
- `Entidades/`: entidades concentram invariantes e transicoes de estado; evite espalhar regras fora daqui.
- `Enums/`: enums daqui sao a fonte de verdade. Se mudar nome ou valor, atualize os enums equivalentes no frontend.
- `Repositories/`: contratos de repositorio pertencem ao dominio; implementacoes ficam em Infrastructure.
- `ViewModel/`: ViewModels devem preservar tipos fortes e nao transformar enums em strings.

## AGENTS.md relacionados

- Este projeto deve ter apenas este `AGENTS.md`; nao crie `AGENTS.md` nas subpastas diretas do projeto.
