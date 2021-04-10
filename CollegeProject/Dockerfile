FROM mcr.microsoft.com/dotnet/aspnet:2.1 AS build
WORKDIR /app
EXPOSE 80

COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o output

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:2.1
WORKDIR /app
COPY --from=build /app/output .
ENTRYPOINT ["dotnet", "CollegeProject.dll"]