# Estágio base (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar os arquivos de projeto de todas as camadas para aproveitamento de cache do Docker
COPY ["HomeTask.WebApi/HomeTask.WebApi.csproj", "HomeTask.WebApi/"]
COPY ["HomeTask.Application/HomeTask.Application.csproj", "HomeTask.Application/"]
COPY ["HomeTask.Domain/HomeTask.Domain.csproj", "HomeTask.Domain/"]
COPY ["HomeTask.Infrastructure/HomeTask.Infrastructure.csproj", "HomeTask.Infrastructure/"]

# Restaurar pacotes usando o projeto principal (o restore resolve as dependências das outras camadas)
RUN dotnet restore "HomeTask.WebApi/HomeTask.WebApi.csproj"

# Copiar o restante do código da solução
COPY . .
WORKDIR "/src/HomeTask.WebApi"
RUN dotnet build "HomeTask.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Estágio de publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HomeTask.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Estágio final (Montagem da imagem)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeTask.WebApi.dll"]
