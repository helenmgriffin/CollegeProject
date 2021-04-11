#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build
WORKDIR /src
COPY ["CollegeProject/CollegeProject.csproj", "CollegeProject/"]
RUN dotnet restore "CollegeProject/CollegeProject.csproj"
COPY . .
WORKDIR "/src/CollegeProject"
RUN dotnet build "CollegeProject.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CollegeProject.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CollegeProject.dll"]