# HomeTask - Guia de Execução (Docker & Kubernetes)

Guia rapido para subir o ambiente local com Docker e testar o fluxo de pagamento do Mercado Pago.

## Pre-requisitos

- Docker Desktop instalado e em execucao
- `cloudflared` instalado para expor a API local publicamente durante os testes de pagamento

## Subindo o ambiente local

1. Copie o arquivo de exemplo:
```bash
cp .env.example .env
```

2. Preencha as variaveis obrigatorias no `.env`.

3. Suba os containers:
```bash
docker compose up -d --build
```

## Portas locais

- Frontend: `http://localhost:8080`
- API / Swagger: `http://localhost:5000/swagger`
- MySQL: `localhost:3307`

## Variaveis do `.env`

### Aplicacao

- `DB_PASSWORD`: senha do MySQL local
- `CONNECTION_STRING`: string de conexao usada pela API dentro do Docker
- `JWT_SECRET_KEY`: chave do JWT
- `VITE_API_BASE_URL`: URL base da API consumida pelo frontend local. Em desenvolvimento: `http://localhost:5000`

### Mercado Pago

- `MERCADOPAGO_PUBLIC_KEY`: public key da aplicacao
- `MERCADOPAGO_ACCESS_TOKEN`: access token da aplicacao
- `MERCADOPAGO_APP_ID`: numero da aplicacao
- `MERCADOPAGO_USER_ID`: user id da conta dona da integracao
- `MERCADOPAGO_TEST_USER`: usuario da conta compradora de teste
- `MERCADOPAGO_TEST_PASSWORD`: senha da conta compradora de teste
- `MERCADOPAGO_TEST_VERIFICATION_CODE`: codigo de verificacao da conta compradora de teste
- `MERCADOPAGO_API_BASE_URL`: URL publica da API usada pelo Mercado Pago para webhook e retorno do checkout
- `MERCADOPAGO_FRONTEND_BASE_URL`: URL local do frontend para onde a API redireciona o navegador apos o checkout
- `MERCADOPAGO_WEBHOOK_PATH`: caminho do webhook publico. Padrao atual: `/api/Pagamento/WebhookMercadoPago`

## Onde obter as credenciais do Mercado Pago

No painel do Mercado Pago Developers:

- Credenciais da aplicacao:
  `Suas integracoes > sua aplicacao > Credenciais`
- Conta de teste:
  `Suas integracoes > sua aplicacao > Testes > Contas de teste`
- Cartoes de teste:
  `Suas integracoes > sua aplicacao > Testes > Cartoes de teste`

## Configurando o tunel para testes locais

O Mercado Pago precisa acessar a API local publicamente para:

- `POST /api/Pagamento/WebhookMercadoPago`
- `GET /api/Pagamento/RetornoCheckout/{agendamentoId}`

O tunel deve apontar para a API, nao para o frontend.

1. Abra o tunel:
```bash
cloudflared tunnel --url http://localhost:5000
```

2. Copie a URL gerada, por exemplo:
```text
https://seu-tunel.trycloudflare.com
```

3. Atualize o `.env`:
```env
MERCADOPAGO_API_BASE_URL=https://seu-tunel.trycloudflare.com
MERCADOPAGO_FRONTEND_BASE_URL=http://localhost:8080
MERCADOPAGO_WEBHOOK_PATH=/api/Pagamento/WebhookMercadoPago
```

4. Rebuild da API:
```bash
docker compose up -d --build api
```

5. No Mercado Pago, configure o webhook publico para:
```text
https://seu-tunel.trycloudflare.com/api/Pagamento/WebhookMercadoPago
```

## Fluxo de retorno do checkout

- O Mercado Pago chama a URL publica da API
- A API trata o retorno em `/api/Pagamento/RetornoCheckout/{agendamentoId}`
- A API redireciona o navegador para:
  `http://localhost:8080/agendamento/detalhes/{agendamentoId}`
- A confirmacao oficial continua vindo do webhook e da reconciliacao do pagamento

## Como testar pagamento com sucesso

1. Faca login no checkout com a conta compradora de teste.
2. Escolha pagamento com cartao.
3. Use um cartao de teste, por exemplo:
   - Visa: `4235 6477 2802 5682`
   - CVV: `123`
   - validade: `11/30`
4. Para aprovar, use:
   - nome do titular: `APRO`
   - documento: `12345678909`

Resultado esperado:

- webhook recebido com sucesso
- pagamento aprovado
- agendamento concluido

## Como testar pagamento recusado

Use o mesmo cartao de teste, alterando o nome do titular:

- `OTHE`: recusado por erro geral
- `FUND`: recusado por saldo insuficiente
- `SECU`: recusado por codigo de seguranca invalido
- `EXPI`: recusado por problema de validade

Para `OTHE`, use tambem:

- documento: `12345678909`

Resultado esperado:

- retorno ao HomeTask com mensagem de falha
- pagamento nao concluido
- acao `Tentar novamente` disponivel

## Comandos uteis

Subir tudo:
```bash
docker compose up -d --build
```

Rebuild so da API:
```bash
docker compose up -d --build api
```

Rebuild so do frontend:
```bash
docker compose up -d --build frontend
```

Ver logs da API:
```bash
docker compose logs api --tail 200
```
