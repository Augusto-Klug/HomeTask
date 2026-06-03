# AGENTS.md

Guia do projeto WebApi. Leia tambem `../AGENTS.md`.

## Regras locais

- Controllers devem expor contratos sem normalizar enums para strings.
- Conversores JSON devem preservar o contrato combinado com frontend e DTOs.
- `Controllers/`: controllers devem ser finos e delegar regras para Application/Domain.
- `Conversores/`: conversores nao devem mascarar diferencas entre enums do backend e frontend.
- `Properties/`: mantenha configuracoes do projeto separadas de codigo de aplicacao.

## AGENTS.md relacionados

- Este projeto deve ter apenas este `AGENTS.md`; nao crie `AGENTS.md` nas subpastas diretas do projeto.
