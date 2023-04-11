FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /app
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0-preview
WORKDIR /app
RUN apt update
RUN apt install -y ffmpeg
ADD https://github.com/yt-dlp/yt-dlp/releases/download/2023.02.17/yt-dlp_linux /bin/ytdlp
RUN chmod +x /bin/ytdlp
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet", "ytdlservice.dll" ]