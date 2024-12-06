# Usar la imagen oficial de SQL Server
FROM mcr.microsoft.com/mssql/server:2019-latest

# Variables de entorno
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=MyP@ssw0rd!1234

# Cambiar temporalmente al usuario root para operaciones con permisos
USER root

# Crear un directorio para los scripts y ajustar permisos
RUN mkdir -p /usr/src/sqlscripts && \
    chmod 755 /usr/src/sqlscripts

# Copiar el script SQL al contenedor
COPY init-database.sql /usr/src/sqlscripts/init-database.sql

# Instalar herramientas necesarias
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl apt-transport-https gnupg && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev && \
    ln -s /opt/mssql-tools/bin/* /usr/local/bin/ && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Volver al usuario predeterminado de SQL Server
USER 10001

# Arrancar SQL Server y ejecutar el script SQL durante la construcción
RUN (/opt/mssql/bin/sqlservr --accept-eula &) && \
    sleep 20 && \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "MyP@ssw0rd!1234" -i /usr/src/sqlscripts/init-database.sql && \
    pkill sqlservr

# Exponer el puerto estándar
EXPOSE 1433

# Comando por defecto
CMD ["/opt/mssql/bin/sqlservr"]
