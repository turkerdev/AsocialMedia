FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY src/ytdlservice/*.csproj .
RUN dotnet restore
COPY src/ytdlservice .
RUN dotnet build -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS development
WORKDIR /app
RUN apt update
RUN apt install -y ffmpeg
ADD https://github.com/yt-dlp/yt-dlp/releases/download/2023.02.17/yt-dlp_linux /bin/ytdlp
RUN chmod +x /bin/ytdlp

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
RUN apt update
RUN apt install -y ffmpeg
ADD https://github.com/yt-dlp/yt-dlp/releases/download/2023.02.17/yt-dlp_linux /bin/ytdlp
RUN chmod +x /bin/ytdlp
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ytdlservice.dll"]