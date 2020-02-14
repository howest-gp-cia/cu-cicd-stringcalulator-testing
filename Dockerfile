# build stage 
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /app

# restore stage
# copy csproj files and restore packages - separate container layers
# starting from solution is also possible
COPY CICD.Domain/*.csproj ./CICD.Domain/
RUN dotnet restore ./CICD.Domain/*.csproj
COPY CICD.UnitTests/*.csproj ./CICD.UnitTests/
RUN dotnet restore ./CICD.UnitTests/*.csproj
COPY CICD.Mvc/*.csproj ./CICD.Mvc/
RUN dotnet restore ./CICD.Mvc/*.csproj

# copy source code
COPY . . 

# test stage
# run separate: cached layer if tests success
RUN dotnet test CICD.UnitTests/CICD.UnitTests.csproj --verbosity=normal

# build in release mode to folder publish
RUN dotnet publish CICD.Mvc/CICD.Mvc.csproj -c Release -o /publish 

# runtime image stage
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
# put release build-files in runtime image /publish
WORKDIR /publish
COPY --from=build-env /publish .
ENTRYPOINT [ "dotnet","CICD.Mvc.dll" ]
