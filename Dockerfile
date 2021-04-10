#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY ["CollegeProject/CollegeProject.csproj", "CollegeProject/"]
RUN dotnet restore "CollegeProject/CollegeProject.csproj"
COPY . .
WORKDIR "/src/CollegeProject"
RUN dotnet build "CollegeProject.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CollegeProject.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CollegeProject.dll"]