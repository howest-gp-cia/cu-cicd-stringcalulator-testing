FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /integration

COPY CICD.IntegrationTests.csproj .
RUN dotnet restore 

COPY . . 
ENTRYPOINT [ "dotnet", "test", "--verbosity=normal" ]

