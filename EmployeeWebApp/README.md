# Installation 
1. dotnet add package Microsoft.EntityFrameworkCore.SqlServer
2. Download Microsoft Azure sql-server docker image and mount volume to data and log folders:  docker run -e 'ACCEPT_EULA=1' -e 'MSSQL_SA_PASSWORD=Password_123' -e 'MSSQL_PID=Developer' -e 'MSSQL_USER=sa'  -p 1433:1433 -v ./data:/var/opt/mssql/data -v ./log:/var/opt/mssql/log --name sql -d mcr.microsoft.com/azure-sql-edge
   - Dockerhub link for image: https://hub.docker.com/_/microsoft-azure-sql-edge 
3. Download Azure Data Studio: https://learn.microsoft.com/en-us/azure-data-studio/download-azure-data-studio?view=sql-server-ver16&tabs=win-install%2Cwin-user-install%2Credhat-install%2Cwindows-uninstall%2Credhat-uninstall