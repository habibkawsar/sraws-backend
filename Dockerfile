FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY SponsorshipApproval.Api.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5088
EXPOSE 8080
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "SponsorshipApproval.Api.dll"]
