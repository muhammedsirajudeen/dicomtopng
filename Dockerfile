# Use .NET SDK as the base image (includes runtime and build tools)
FROM mcr.microsoft.com/dotnet/sdk:8.0.406

# Set the working directory inside the container
WORKDIR /app

# Copy everything from the host to the container
COPY *.csproj ./

# RUN dotnet nuget add source http://nuget.org/api/v2 -n nuget.org
# ARG DNS=8.8.8.8

# RUN dotnet restore

# Restore dependencies
# RUN dotnet restore
COPY . .
# Build and publish the application

# Set the working directory to the published output

# Expose the port your app runs on
EXPOSE 5169

# Set the entry point to run the application
CMD ["/bin/sh", "-c", "dotnet restore && dotnet run"]
