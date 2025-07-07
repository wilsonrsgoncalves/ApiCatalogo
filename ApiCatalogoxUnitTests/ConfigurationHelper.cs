using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoxUnitTests
{
    public static class ConfigurationHelper
    {
        public static IConfiguration LoadConfiguration(string environment = "Test")
        {
            var currentDir = Directory.GetCurrentDirectory();

            
            var projectDir = Directory.GetParent(currentDir)?.Parent?.Parent?.FullName;

            if (projectDir == null)
                throw new DirectoryNotFoundException("Não foi possível localizar o diretório do projeto.");

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(projectDir)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables(); 

            return configBuilder.Build();
        }

        public static string GetConnectionString(string name = "DefaultConnection", string environment = "Test")
        {
            var config = LoadConfiguration(environment);
            var connStr = config.GetConnectionString(name);

            if (string.IsNullOrEmpty(connStr))
                throw new Exception($"ConnectionString '{name}' não encontrada no appsettings.{environment}.json.");

            return connStr;
        }
    }
}

