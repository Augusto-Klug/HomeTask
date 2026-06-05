# AGENTS.md

Guia do projeto Tests. Leia tambem `../AGENTS.md`.

## Regras locais

- Testes devem usar enums reais em vez de strings quando validarem dominio/DTOs.
- Preserve a intencao do teste e evite mocks que mudem contratos.
- `Helpers/`: helpers de teste devem construir objetos validos com enums reais do dominio.
- `Integration/`: testes de integracao devem validar o comportamento real de persistencia e contratos.
- `Services/`: testes de services devem cobrir regras de aplicacao sem duplicar detalhes de infraestrutura.

## AGENTS.md relacionados

- Este projeto deve ter apenas este `AGENTS.md`; nao crie `AGENTS.md` nas subpastas diretas do projeto.
