Aula configuração do banco de dados com Docker:

Imagem baixada: docker pull mcr.microsoft.com/mssql/server:2019-latest

host: localhost
nome_usuario: sa //
senha: @Sql2019

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=@Sql2019" -p 1433:1433 --name sqlserver -h sql1 -d mcr.microsoft.com/mssql/server:2019-latest


Comandos para criação das migrations:
1) Instalar a ferramenta do migrations => dotnet tool install --global dotnet-ef

2) Comandos:
dotnet ef migrations add {NOME_DA_TABELA}
