# --- Base runtime image ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- Build image ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only project files first for efficient restore
COPY ["TouchpointMedical.Site/TouchpointMedical.Site.csproj", "TouchpointMedical.Site/"]
COPY ["TouchpointMedical.Integration.PointClickCare/TouchpointMedical.Integration.PointClickCare.csproj", "TouchpointMedical.Integration.PointClickCare/"]
COPY ["TouchpointMedical/TouchpointMedical.csproj", "TouchpointMedical/"]

RUN dotnet restore "TouchpointMedical.Site/TouchpointMedical.Site.csproj"

# Now copy the remainder of the source
COPY . .
WORKDIR "/src/TouchpointMedical.Site"
RUN dotnet build "TouchpointMedical.Site.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- Publish image ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TouchpointMedical.Site.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- Final runtime image ---
FROM base AS final
WORKDIR /app

# Create writable data directory for SQLite and a non-root user
# (Debian base image: use groupadd/useradd)
ARG APP_UID=1001
ARG APP_GID=1001
RUN groupadd -g ${APP_GID} appgroup \
    && useradd -u ${APP_UID} -g appgroup -m appuser \
    && mkdir -p /app/App_Data \
    && chown -R appuser:appgroup /app

# Persist the database file across container replacements
VOLUME ["/app/App_Data"]

# Bring in the published app
COPY --from=publish /app/publish .

# If you still need to bring App_Data content that isn't in publish output, uncomment:
# COPY ["TouchpointMedical.Site/App_Data", "/app/App_Data"]

# Drop privileges
USER appuser

ENTRYPOINT ["dotnet", "TouchpointMedical.Site.dll"]