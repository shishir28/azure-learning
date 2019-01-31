using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace AZStorageSample
{
    public class AppSettings
    {
        public string ConnectionStrings { get; set; }
        public string CosmosDBEndpoint { get; set; }
        public string CosmosDBKey { get; set; }
    }
}
