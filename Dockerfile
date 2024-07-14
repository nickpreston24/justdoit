FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
#EXPOSE 3000
#ENV ASPNETCORE_URLS=http://*:3000

COPY . /app
RUN dotnet restore

ENTRYPOINT [ "dotnet", "watch", "run" ]

