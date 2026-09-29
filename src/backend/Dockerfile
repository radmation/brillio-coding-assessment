FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

CMD ["dotnet", "watch", "run", "--non-interactive", "--urls", "http://0.0.0.0:5000"]
