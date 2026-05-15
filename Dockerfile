FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SponsorshipApproval.Api.csproj ./

RUN dotnet nuget locals all --clear
RUN dotnet restore SponsorshipApproval.Api.csproj --force --no-cache

COPY . ./

RUN dotnet publish SponsorshipApproval.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "SponsorshipApproval.Api.dll"]