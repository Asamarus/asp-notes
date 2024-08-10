# syntax=docker/dockerfile:1

# Stage 1: Build the React frontend
FROM node:20-alpine AS frontend-build

# Copy the source code
COPY . /source

# Set the working directory
WORKDIR /source/react-app

# Install dependencies, build the React app, copy generated files and remove the node_modules directory
RUN npm install && npm run build && npm run deploy && rm -rf node_modules

################################################################################

# Stage 2: Build the .NET backend
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Copy the entire source folder from the frontend-build stage
COPY --from=frontend-build /source /source

# Set the working directory
WORKDIR /source/AspNotes.Web

# Ensure that db folder is created
RUN mkdir db

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Build the application.
# Leverage a cache mount to /root/.nuget/packages so that subsequent builds don't have to re-download packages.
# If TARGETARCH is "amd64", replace it with "x64" - "x64" is .NET's canonical name for this and "amd64" doesn't
#   work in .NET 6.0.
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -c Release -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Install EF Core CLI tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Restore the tools
RUN dotnet tool restore

# Ensure that db folder has the correct permissions
RUN mkdir -p /app/db && chmod -R 777 /app/db

# Run database migrations with verbose logging
RUN dotnet ef database update --verbose

################################################################################

# Stage 3: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Set the working directory
WORKDIR /app

# Copy the .NET application from the build stage
COPY --from=build /app .

# Create a volume for the SQLite database
VOLUME /app/db

# Ensure the volume has the correct permissions
RUN chmod -R 777 /app/db

# Set environment variables for the SQLite database
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "AspNotes.Web.dll"]