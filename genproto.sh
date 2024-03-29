#!/bin/sh

base_dir=$(pwd)

gen_proto_dotnet() {
  echo "Generating .NET protobuf files for $1"
  cd "$base_dir"/src/"$1" || return
  rm -rf ./Protos
  mkdir -p ./Protos/
  cp -r "$base_dir"/pb/* ./Protos/
  cd "$base_dir" || return
}

gen_proto_ts() {
  echo "Generating TypeScript protobuf files for $1"
  image=$(docker build --target typescript -q .)
  id=$(docker run -d $image)
  container=$(docker ps -af id=$id --format '{{.Names}}')
  docker wait $container
  cd "$base_dir"/src/"$1" || return
  rm -rf ./src/protos
  docker cp $container:/app/dist ./src/protos
  docker rm $container
  cd "$base_dir" || return
}

gen_proto_ts browser
gen_proto_ts gateway
gen_proto_ts frontend
gen_proto_dotnet ytdlservice