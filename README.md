# HomeTask - Guia de Execução (Docker & Kubernetes)

Este repositório agora está totalmente conteinerizado! Isso significa que você não precisa mais instalar o MySQL separadamente na sua máquina ou configurar o ambiente manualmente para rodar a API.

Abaixo estão as instruções de como rodar a aplicação no seu dia a dia.

---

## Pré-requisitos
1. **[Docker Desktop](https://www.docker.com/products/docker-desktop/)** instalado e rodando.
2. (Opcional, mas recomendado) **Kubernetes** ativado *dentro do próprio Docker Desktop* (Vá em Configurações > Kubernetes > *Enable Kubernetes*).

---

## Opção 1: Docker Compose (Recomendado para o Dia a Dia)
Ideal para desenvolvimento local rápido. Ele sobe a API e o Banco de Dados em containers e gerencia a conexão entre eles.

### Como rodar:
No terminal, na raiz do projeto (onde está o arquivo `docker-compose.yml`), execute:
```bash
docker compose up --build -d
```
*(O `--build` garante que o Docker recompile seu código mais recente, e o `-d` libera seu terminal)*.

### Acessos:
- **Swagger / API:** [http://localhost:8080/swagger](http://localhost:8080/swagger)
- **Banco de Dados (MySQL):** 
  - **Host:** `localhost`
  - **Porta:** `3307` *(Usamos a 3307 externa para não conflitar com nenhum MySQL que você já tenha na porta 3306)*
  - **User:** `root`
  - **Password:** `123456789`

> **Nota sobre o Banco:** O repositório já está configurado para **rodar as Migrations automaticamente** quando a API sobe (`Program.cs`). Você não precisa rodar comando de `update-database`. Os dados ficam salvos de forma segura em um volume do Docker na sua máquina.

### Como parar:
```bash
docker compose down
```

---

## Opção 2: Kubernetes (Simulando Produção/Orquestração)
Se você quer testar a aplicação em um ambiente orquestrado com **Pods**, balanceamento de carga e *self-healing*, use os manifestos da pasta `/k8s`.

### Como rodar:
1. Primeiro, construa a imagem Docker (para o K8s usar a versão mais atual do seu código):
```bash
docker build -t hometaskwebapi:latest .
```
2. Aplique todos os manifestos de uma vez:
```bash
kubectl apply -f k8s/
```
3. Verifique se os Pods estão rodando:
```bash
kubectl get pods
```

### Acessos:
A API através do Kubernetes também ficará disponível na mesma porta:
- **Swagger / API:** [http://localhost:8080/swagger](http://localhost:8080/swagger)

### Como parar o Cluster local:
```bash
kubectl delete -f k8s/
```

---

## CI/CD Automatizado
Temos um workflow configurado no GitHub Actions (`.github/workflows/ci-cd.yml`).
Sempre que um **Push** ou **Pull Request** for aberto em qualquer branch, o GitHub irá:
1. Fazer o Build e validar toda a Solução (.NET 10).
2. Fazer o Build da Imagem Docker.
3. Publicar automaticamente a imagem no **GitHub Container Registry (GHCR)**.
