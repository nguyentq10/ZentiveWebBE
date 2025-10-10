# =========================
# === Build Stage ========
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ solution vào image
COPY . .

# Chuyển vào thư mục project API
WORKDIR /src/ZentiveAPI

# Restore & publish
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# =========================
# === Runtime Stage =======
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy file đã build vào container runtime
COPY --from=build /app/out .

# Mở cổng 80 (HTTP)
EXPOSE 8080

ENTRYPOINT ["dotnet", "ZentiveAPI.dll"]