﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace NetCoreWithDocker.Storage
{
    public class TasksDbContextFactory : IDesignTimeDbContextFactory<TasksDbContext>
    {
        public TasksDbContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TasksDbContext>();

            if (optionsBuilder.IsConfigured)
                return new TasksDbContext(optionsBuilder.Options);

            //Called by parameterless ctor Usually Migrations
            var environmentName = Environment.GetEnvironmentVariable("EnvironmentName") ?? "Development";

            var connectionString =
                new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables()
                    .Build()
                    .GetConnectionString("TasksDatabase");

            optionsBuilder.UseSqlServer(connectionString);

            return new TasksDbContext(optionsBuilder.Options);
        }

        public TasksDbContext CreateDbContext(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}