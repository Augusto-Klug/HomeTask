# AGENTS.md

Guia do projeto Infrastructure. Leia tambem `../AGENTS.md`.

## Regras locais

- Implementacoes de persistencia, DI e servicos externos ficam aqui.
- Nao altere contratos de dominio para acomodar detalhes de infraestrutura.
- `Data/`: configuracoes de dados devem preservar mapeamentos de enums de dominio.
- `DI/`: registre dependencias sem introduzir logica de negocio.
- `Migrations/`: migrations devem refletir o modelo atual sem edicoes manuais desnecessarias.
- `Repositories/`: repositorios implementam contratos do dominio e nao devem converter enums para strings para consumo externo.
- `Services/`: servicos de infraestrutura devem ficar isolados de regras de negocio.

## AGENTS.md relacionados

- Este projeto deve ter apenas este `AGENTS.md`; nao crie `AGENTS.md` nas subpastas diretas do projeto.
