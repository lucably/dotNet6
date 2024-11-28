Aula configuração do banco de dados com Docker:

Imagem baixada: docker pull mcr.microsoft.com/mssql/server:2019-latest

host: localhost  nome_usuario: sa  senha: @Sql2019  Host:localhost  BancoDeDados/Esquema: Products

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=@Sql2019" -p 1433:1433 --name sqlserver -h sql1 -d mcr.microsoft.com/mssql/server:2019-latest

