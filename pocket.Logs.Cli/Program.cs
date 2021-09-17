using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace pocket.Logs.Cli
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            var serviceProvider = services.BuildServiceProvider();
            
            var dataFolder = "Data";
            Directory.CreateDirectory(dataFolder);
            
            var limit = (uint)10000;
            var offset = (uint)198000;
            uint received = limit;

            do
            {
                var client = new LogClient(serviceProvider.GetRequiredService<IHttpClientFactory>());
                try
                {
                    var query = await client.GetLogs(limit: limit, offset: offset);
                    var firstId = query.Logs.Select(l => l.Id).Max();

                    var json = JsonSerializer.Serialize(query,
                        new JsonSerializerOptions
                            { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                    var file = Path.Join(dataFolder, $"Log_{limit}_{firstId}.json");
                    Console.Write("Writing to {0}... ", file);
                    await File.WriteAllTextAsync(file, json);
                    Console.WriteLine("Finished.");

                    offset += limit;
                    received = (uint)query.Logs.Length;
                }
                catch (HttpRequestException)
                {
                    continue;
                }
            } while (received == limit);
        }
    }
}