# HomeTask

## Como rodar o projeto

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- MySQL (qualquer versão) — via **Docker** ou instalado diretamente

---

### Opção 1 — MySQL via Docker (recomendado)

**1. Instale o [Docker Desktop](https://www.docker.com/products/docker-desktop/)**

**2. Clone o repositório**
```bash
git clone https://github.com/Augusto-Klug/HomeTask
cd HomeTask
```

**3. Suba o banco de dados**
```bash
docker compose up -d
```

**4. Rode o projeto**
```bash
dotnet run --project HomeTask.Web/HomeTask.Web.csproj
```

---

### Opção 2 — MySQL instalado na máquina

**1. Instale o [MySQL Community Server](https://dev.mysql.com/downloads/mysql/)** (qualquer versão)

**2. Durante a instalação, defina a senha do root como:**
```
123456789
```

**3. Clone o repositório**
```bash
git clone https://github.com/Augusto-Klug/HomeTask
cd HomeTask
```

**4. Rode o projeto**
```bash
dotnet run --project HomeTask.Web/HomeTask.Web.csproj
```
Ou abra a solução no **Visual Studio**, clique com botão direito em **HomeTask.Web** → **Set as Startup Project** e pressione **F5**.

---

> As tabelas são criadas automaticamente na primeira execução. Não é necessário rodar `dotnet ef` manualmente.
