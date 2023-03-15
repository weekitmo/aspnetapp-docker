docker build -t aspnet-app -f Dockerfile .
docker run --rm -it -e ASPNETCORE_URLS=http://+:5000 -p 5000:5000 aspnet-app