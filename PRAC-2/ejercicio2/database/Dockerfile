# Usar la imagen base de MongoDB
FROM mongo:latest

# Instalar el cliente mongo desde los repositorios oficiales
RUN apt-get update && apt-get install -y wget gnupg && \
    wget -qO - https://www.mongodb.org/static/pgp/server-6.0.asc | apt-key add - && \
    echo "deb [ arch=amd64 ] https://repo.mongodb.org/apt/ubuntu focal/mongodb-org/6.0 multiverse" > /etc/apt/sources.list.d/mongodb-org-6.0.list && \
    apt-get update && apt-get install -y mongodb-mongosh && \
    rm -rf /var/lib/apt/lists/*

# Crear una base de datos y una colección con datos iniciales directamente en el comando RUN
RUN mongod --fork --logpath /var/log/mongodb.log && \
    mongosh --eval 'db = connect("localhost:27017/NoSQLInjectionDB"); db.users.insertMany([{ username: "admin", password: "123456" }, { username: "test", password: "test123" }, { username: "javier", password: "securepass" }]);' && \
    mongod --shutdown

# Exponer el puerto de MongoDB
EXPOSE 27017

# Comando por defecto para iniciar MongoDB
CMD ["mongod"]
