# AGENTS.md

Guia do projeto Application. Leia tambem `../AGENTS.md`.

## Regras locais

- Mantenha regras de aplicacao, DTOs, mapeamentos e contratos sem depender de infraestrutura.
- Quando expor enums em DTOs, preserve os enums do dominio sem alterar nomes ou valores.
- `Dtos/`: DTOs devem refletir contratos de entrada/saida sem converter enums de dominio para strings.
- `Interfaces/`: interfaces descrevem contratos de aplicacao sem acoplar detalhes de infraestrutura.
- `Mappings/`: mapeamentos devem preservar tipos de dominio, especialmente enums, sem conversoes para string.
- `Services/`: services concentram casos de uso e devem manter regras de dominio no dominio quando possivel.

## AGENTS.md relacionados

- Este projeto deve ter apenas este `AGENTS.md`; nao crie `AGENTS.md` nas subpastas diretas do projeto.
