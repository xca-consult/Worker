FROM mcr.microsoft.com/dotnet/aspnet:3.1
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
WORKDIR /app
COPY YZ.ITI.Sample.DDD/bin/Release/netcoreapp3.1/publish .
RUN ls .
ENTRYPOINT [ "dotnet", "Worker.dll" ]
