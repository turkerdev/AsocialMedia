FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AsocialMedia.Worker/AsocialMedia.Worker.csproj", "AsocialMedia.Worker/"]
RUN dotnet restore "AsocialMedia.Worker/AsocialMedia.Worker.csproj"
COPY . .
WORKDIR "/src/AsocialMedia.Worker"
RUN dotnet build "AsocialMedia.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AsocialMedia.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV RabbitMQ__URL=RabbitMQ__URL
ENV Google__ClientId=Google__ClientId
ENV Google__ClientSecret=Google__ClientSecret
ENTRYPOINT ["dotnet", "AsocialMedia.Worker.dll"]