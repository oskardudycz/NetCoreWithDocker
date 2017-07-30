# Net Core With Docker Continuous Integration

This example shows how to:
- [x] setup work environment for .NET Core,
- [ ] create simple WebApi project with database usage,
- [ ] setup databases without needed to install anything more than docker,
- [ ] setup Continuous Integration/Delivery pipeline to make sure that your code runns properly,
- [ ] create test environment,
- [ ] create prod environment.

## Setup work environment for .NET Core
1. Install [Docker](https://www.docker.com/get-docker) 
2. Install [Visual Studio 2017](https://www.visualstudio.com/pl/thank-you-downloading-visual-studio/?sku=Community&rel=15), [VisualStudioCode](https://code.visualstudio.com/download), [Rider](https://www.jetbrains.com/rider/) or you other favourite .NET IDE

## Create new Web Api project
We will use default Visual Studio template for the WebApi project:
1. From the Visual Studio menu select: `File => New => Project`
2. Select from the `Templates => Visual C# => .Net Core => ASP.NET Core Web Application`
3. Select also .NET Framework Version on the top (suggested 4.7), place where you want to create new project and the name.
4. In the new window select `Web Api` template. Unselect "Enable Docker Support" and leave Authentication settings as they are.
5. Open Package Manager Console and run `dotnet restore` command - this will get all of the needed NuGet packages for your application.
6. Now you have your basic app setup, you can click `F5` to run it.
7. If everything went properly then you should see browser page with `http://localhost:{port}/api/values` and `["value1","value2"]`.

You can check the detailed changes in [pull request](https://github.com/oskardudycz/NetCoreWithDockerCI/pull/2/files)

## Add MSSQL Database to the Web Api project
Most of our applications needs to have the database. We'll use Entity Framework and MSSQL server in this example.
1. From the `Package Manger Console` run `Install-Package Microsoft.EntityFrameworkCore.SqlServer` and `Install-Package Microsoft.EntityFrameworkCore.Tools`. This will add Nuget Packages nessesary for the MSSQL server databasse usage.
2. Create Entity Class (eg. [Task](https://github.com/oskardudycz/NetCoreWithDockerCI/blob/c3b2dc31fb7ae8b834b94cb338b49fd3a8dbe2b5/src/NetCoreWithDocker/NetCoreWithDocker/Storage/Entities/Task.cs)) and DbContext (eg. [TasksDbContext](https://github.com/oskardudycz/NetCoreWithDockerCI/blob/df641d876b094eb64918c3823ede6a14529216e4/src/NetCoreWithDocker/NetCoreWithDocker/Storage/TasksDbContext.cs)).
3. You should get simmilar changes as in this [commit](https://github.com/oskardudycz/NetCoreWithDockerCI/pull/4/commits/c3b2dc31fb7ae8b834b94cb338b49fd3a8dbe2b5).
4. Next step is to provide the connection string to the database. For this example we'll use LocalDB, which is distributed and installed automatically with the Visual Studio 2017 (if you're not using the Visual Studio then you can get it from this [link](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-2016-express-localdb)).
5. We need to provide proper connection string to [appsettings.json](https://github.com/oskardudycz/NetCoreWithDockerCI/blob/df641d876b094eb64918c3823ede6a14529216e4/src/NetCoreWithDocker/NetCoreWithDocker/appsettings.Development.json) and pass it to the Entity Framework configuration in  [Startup.cs](https://github.com/oskardudycz/NetCoreWithDockerCI/blob/df641d876b094eb64918c3823ede6a14529216e4/src/NetCoreWithDocker/NetCoreWithDocker/Startup.cs).
6. Having that configuration we can remove the dummy `ValuesController` and add new [TasksController](https://github.com/oskardudycz/NetCoreWithDockerCI/blob/df641d876b094eb64918c3823ede6a14529216e4/src/NetCoreWithDocker/NetCoreWithDocker/Controllers/TasksController.cs). 
This example contains few important things derived from the best practices like:
* `async` usage - it' much better for the performance perspective to use `async/await` in the api. What's crucial is that you cannot leave the async statement using database not awaited, because it may end up with your db connection not disposed properly,
* returning proper Http Status Codes for the Api endpoints:
    * `OK` - will return `200`,
    * `NotFound`- will return `404` result,
    * `Forbid` - will return `403` status,
    
    Good explanation of http status codes can be found [here](https://ict.ken.be/Data/Sites/1/images/articles/http-status-code-explained.jpg)
* few examples of the new usefull [C# 6 syntax](https://msdn.microsoft.com/en-us/magazine/dn802602.aspx)
7. You should also update your `launchSettings.json` to redirect you by default to the `/tasks/` instead of the `/values/` route.
8. If you run now you're application then you'll get following exception:

    ``
System.Data.SqlClient.SqlException: 'Cannot open database "NetCoreWithDocker" requested by the login. The login failed.
``

    It basically means that it cannot login, because in fact there is no database that Entity Framework can login to. How to setup it? By using build in Migrations mechanism.
9. It's needed to setup the migrations (event the initial one), that will create database with object<=>relation mapping defined in the [TasksDbContext](https://github.com/oskardudycz/NetCoreWithDockerCI/blob/df641d876b094eb64918c3823ede6a14529216e4/src/NetCoreWithDocker/NetCoreWithDocker/Storage/TasksDbContext.cs). To do that open `Package Manager Console` and run: `Add-Migration InitialCreate](). This will automatically freeze the current model definiton in  [Current Model Snapshot` and the define [Initial Migration]().
10. Now to create/update our database it's needed to run `Update-Database` from `Package Manager Console`.
11. You should now be able to run the application. If you did all of the steps properly then you should see browser page with `http://localhost:{port}/api/values` and `[]`. That means that our [TasksController](https://github.com/oskardudycz/NetCoreWithDockerCI/blob/df641d876b094eb64918c3823ede6a14529216e4/src/NetCoreWithDocker/NetCoreWithDocker/Controllers/TasksController.cs) returned empty list of tasks - because our database is currently empty.
12. Entity Framework also provides mechanism to add initial data (it's usefull for the eg. dictionaries or test data setup). Unfortunatelly it's needed to provide some boilerplate code for that. We need to do following steps:

    12.1. Run `Add-Migration InitialSeed` in the `Package Manager Console` - this will generate empty migration.
    
    12.2. Then we need to prepare our [TasksDbContext](https://github.com/oskardudycz/NetCoreWithDockerCI/blob/df641d876b094eb64918c3823ede6a14529216e4/src/NetCoreWithDocker/NetCoreWithDocker/Storage/TasksDbContext.cs) to be also created not only from the depenedency injection. To do that we should create `TasksDbContextFactory`. This class will be used for the injecting ConnectionStrings and other options. It needs to implement `IDbContextFactory<TasksDbContext>` and unfortunatelly mimic some of the Startup Configuration reading functionality. Full implementation can be seen [here]()

    12.3. Now we can use our factory class in our [Initial Seed]() migration. In `Up` method we're defining our insertion, in `Down` method we're cleaning data (it' being run if something went wrong).

    12.4. You need also to mark `appsettings.Development.json`. To do that we need to change the settings in the [NetCoreWithDocker.csproj]() from `Never` to `PreserveNewest`.
    
    Now you should be able to run `Update-Database` from `Package Manager Console` to apply seeding, start application by clicking `F5` and see the results in the browser (eg. `[{"id":1,"description":"Do what you have to do"},{"id":2,"description":"Really urgent task"},{"id":3,"description":"This one has really low priority"}]`)

You can check the detailed changes in [pull request](https://github.com/oskardudycz/NetCoreWithDockerCI/pull/4/files)