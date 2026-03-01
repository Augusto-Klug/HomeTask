# HomeTask.Frontend

Frontend estático do HomeTask, construído com **Vue 3 (CDN)**, **Bootstrap 5**, **Axios** e HTML/CSS puro — sem build step.

## Estrutura

```
HomeTask.Frontend/
├── index.html                  ← Home
├── login.html                  ← Login
├── cadastro.html               ← Cadastro de usuário
├── cadastro-sucesso.html       ← Confirmação de cadastro
├── servicos/
│   ├── buscar.html             ← Busca de serviços com filtros
│   └── detalhes.html           ← Detalhes do serviço + avaliações
├── agendamento/
│   ├── novo.html               ← Formulário de agendamento (requer login)
│   └── sucesso.html            ← Confirmação de agendamento
├── css/
│   └── main.css                ← Estilos globais
└── js/
    ├── config.js               ← URL base da API
    ├── auth.js                 ← Gerenciamento de sessão (sessionStorage)
    ├── api.js                  ← Instância Axios + interceptor JWT
    ├── app-home.js
    ├── app-login.js
    ├── app-cadastro.js
    ├── app-buscar.js
    ├── app-detalhes.js
    └── app-agendamento.js
```

## Como rodar

### 1. Backend (API)

```bash
cd HomeTask.WebApi
dotnet run
# Rodando em http://localhost:5000
```

### 2. Frontend

Use o **Live Server** do VS Code (extensão recomendada) ou qualquer servidor HTTP estático.

**Com VS Code Live Server:**
1. Abra a pasta `HomeTask.Frontend` no VS Code
2. Clique com botão direito em `index.html` → **Open with Live Server**
3. O frontend ficará em `http://localhost:5500`

**Com Node.js:**
```bash
cd HomeTask.Frontend
npx serve .
# ou: npx http-server . -p 5500
```

## Segurança

| Prática | Implementação |
|---|---|
| JWT | Gerado e validado **exclusivamente no backend** |
| Armazenamento do token | `sessionStorage` — limpo ao fechar o navegador |
| Dados sensíveis | Nenhuma chave secreta ou string de conexão no frontend |
| CORS | API permite apenas `localhost:5500` / `127.0.0.1:5500` |
| Rotas protegidas | `requireAuth()` redireciona para `/login.html` se não autenticado |

## Configuração

Para mudar a URL da API, edite `js/config.js`:

```js
const API_BASE_URL = 'http://localhost:5000'; // ajuste conforme necessário
```

Para produção, atualize também `HomeTask.WebApi/appsettings.json`:

```json
"Cors": {
  "AllowedOrigins": [ "https://seu-dominio.com" ]
}
```
