using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;


namespace AZSearchService
{
    class Program
    {
        static void Main(string[] args)
        {
            var searchServiceName = "az203clinicaltrialsearch";
            var queryApiKey = "D3F0E3FBFA375F68B705B2ED865EEFDD";
            var indexName = "azureblob-index";
            var indexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(queryApiKey));

            var searchParameters = new SearchParameters
            {
                Select = new[] { "*" }
            };
            
            var searchTerm = "NCT00000102.txt";

            var searchResult = indexClient.Documents.Search(searchTerm, searchParameters);

            foreach (var item in searchResult.Results)
                foreach (var val in item.Document )
                    Console.WriteLine($"Key= {val.Key} = {val.Value}");


            Console.WriteLine("Press any key to terminate ...");
            Console.ReadKey();
        }
    }
}
