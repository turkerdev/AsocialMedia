FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
RUN apt-get update
RUN apt-get install xz-utils

ADD https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz /tmp/ffmpeg.tar.xz
RUN tar -xvf /tmp/ffmpeg.tar.xz -C /tmp
RUN cp /tmp/ffmpeg*/ffmpeg /bin/ffmpeg
RUN cp /tmp/ffmpeg*/ffprobe /bin/ffprobe
RUN rm -rf /tmp/ffmpeg*

ADD https://github.com/yt-dlp/yt-dlp/releases/download/2022.09.01/yt-dlp_linux /bin/ytdlp
RUN chmod +x /bin/ytdlp
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
