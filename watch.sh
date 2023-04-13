watch(){
    nodemon -w ./src/$1 -e "*" --exec "docker compose up $1 -d --build"
}

docker compose up $1 & watch remix & watch ytdlservice