FROM mcr.microsoft.com/dotnet/aspnet:2.1 AS build-env
WORKDIR /app
EXPOSE 80

COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o output

# Runtime image
FROM FROM mcr.microsoft.com/dotnet/aspnet:2.1
WORKDIR /app
COPY --from=build-env /app/output .
ENTRYPOINT ["dotnet", "CollegeProject.dll"]