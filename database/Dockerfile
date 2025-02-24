# Use official PostgreSQL image from Docker Hub
FROM postgres:latest

# Set environment variables for the database
ENV POSTGRES_USER=your_username
ENV POSTGRES_PASSWORD=your_password
ENV POSTGRES_DB=your_database

# Copy migration SQL files into the correct directory for execution at runtime
COPY init.sql authorization-migration.sql course-management-api-migration.sql /docker-entrypoint-initdb.d/

# Ensure files have correct Unix-style line endings
RUN dos2unix /docker-entrypoint-initdb.d/*.sql || true


# Ensure init scripts are executed in order
RUN chmod +x /docker-entrypoint-initdb.d/*.sql
