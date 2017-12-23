#FROM microsoft/aspnetcore:2.0 AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM microsoft/aspnetcore-build:2.0 AS build
#WORKDIR /src
#COPY *.sln ./
#COPY test/Shay.Web.Tests/Shay.Web.Tests.csproj test/Shay.Web.Tests/
#RUN dotnet restore
#COPY . .
#WORKDIR /src/test/Shay.Web.Tests
#RUN dotnet build -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Shay.Web.Tests.dll"]
#

FROM microsoft/aspnetcore:2.0
#拷贝项目publish文件夹中的所有文件到 docker容器中的publish文件夹中 
COPY . /publish
 
#设置工作目录为 `/publish` 文件夹，即容器启动默认的文件夹
WORKDIR /publish
 
#设置Docker容器对外暴露5000端口
EXPOSE 5000

CMD ["dotnet", "Shay.Web.Tests.dll"]
