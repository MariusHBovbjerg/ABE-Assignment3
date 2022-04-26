FROM mcr.microsoft.com/dotnet/runtime:5.0 as base

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY . /src
WORKDIR /src
RUN ls
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelBookingService.dll"]