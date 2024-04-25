FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
#ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_HTTP_PORT=https://+:5001
ENV ASPNETCORE_URLS=http://+:5000
COPY published/ ./

ENTRYPOINT ["dotnet", "EmployeeWebApp.dll"]
