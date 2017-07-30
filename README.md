# Net Core With Docker Continuous Integration

This example shows how to:
- setup work environment for .NET Core,
- create simple WebApi project with database usage,
- setup databases without needed to install anything more than docker,
- setup Continuous Integration/Delivery pipeline to make sure that your code runns properly,
- create test environment,
- create prod environment.

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
