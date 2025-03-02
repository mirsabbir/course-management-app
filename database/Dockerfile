# Use official PostgreSQL image from Docker Hub
FROM postgres:latest

# Set environment variables for the database
ENV POSTGRES_USER=dbuser
ENV POSTGRES_PASSWORD=dbpass
ENV POSTGRES_DB=mydb

# Copy migration SQL files into the correct directory for execution at runtime
COPY init.sql authorization-migration.sql course-management-api-migration.sql /docker-entrypoint-initdb.d/

# Ensure files have correct Unix-style line endings
RUN dos2unix /docker-entrypoint-initdb.d/*.sql || true


# Ensure init scripts are executed in order
RUN chmod +x /docker-entrypoint-initdb.d/*.sql
