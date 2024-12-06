# MyBlog
[![Azure Static Web Apps CI/CD](https://github.com/chioroglo/myBlog/actions/workflows/slave_azure-static-web-apps.yml/badge.svg)](https://github.com/chioroglo/myBlog/actions/workflows/slave_azure-static-web-apps.yml)<br/>
[![api](https://github.com/chioroglo/myBlog/actions/workflows/slave_api-myblog-chioroglo.yml/badge.svg)](https://github.com/chioroglo/myBlog/actions/workflows/slave_api-myblog-chioroglo.yml)<br/>
[![Coverage Status Backend](https://coveralls.io/repos/github/chioroglo/myBlog/badge.svg?branch=slave)](https://coveralls.io/github/chioroglo/myBlog?branch=slave)<br/>
# Used Technologies:

Microsoft SQL Server
ASP.NET Core 8 WEB API  
Entity Framework 8  
React(TypeScript) (Material UI,Formik,react-router-dom,Axios,React Redux)


# Contribution guide
1. Create new branch (git branch "name")
2. Switch to this branch (git checkout "name")
3. Make changes
4. Push branch to remote origin (git push)
5. Make pull request using UI GitHub Interface from your branch to origin/slave

# Getting Started
1. Install Docker, then run Docker. To launch the solution, you need to have on your local machine instances of: 
- MS SQL Server to bring up a database
- Redis to support in-memory distributed caching

These instances may be installed via **docker-compose.yml** file. To install - go to the terminal in .NET application
folder ("webApi") and type in 
```shell
docker-compose up -d
```

2. Install an IDE for development, set up environment to develop .NET Applications (Install .NET 8 SDK)
3. Then, install Dotnet Entity Framework 8 Core Tools. This is necessary to manage database migrations.
```shell
dotnet tool install --global dotnet-ef
```
While you've installed CLI (Command Line Interface) for Entity Framework, type in following commands
```shell
# To view the migartions list
    dotnet ef migrations list -s WebApi
# To apply migrations to your brand new database
    dotnet ef database update -s WebApi
# INFO: To add new migartion, type in:
    dotnet ef migrations add "MIGRATION_NAME" -s WebApi -p Data
```
That should do well, after you lanch WebApi, it'll be populated with initial mock values. Launch the API!

4. Set up React application.<br/>
4.1 Install NODE.JS LTS version https://nodejs.org/en<br/>
4.2 Open react section of this project in console ("React/my-blog/")<br/>
4.3 Install all dependencies.<br/>
```shell
    npm install
```
4.4 Run the application
```shell
    npm start
```
