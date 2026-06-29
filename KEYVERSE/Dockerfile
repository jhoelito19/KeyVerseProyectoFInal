# 1. Imagen base (Actualizada a .NET 10)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 2. Compilador (Actualizado a .NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["*.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# 3. Empaquetado final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
# Asegúrate de que el nombre del .dll sea exactamente igual a tu proyecto
ENTRYPOINT ["dotnet", "KeyVerse.dll"]
