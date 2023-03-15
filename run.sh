docker build -t aspnet-app -f Dockerfile .
docker run --rm -it -e ASPNETCORE_URLS=http://+:5055 -p 5055:5055 aspnet-app