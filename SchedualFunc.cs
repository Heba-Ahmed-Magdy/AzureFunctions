using System;
using System.Threading.Tasks;
using AzureFunVSCode1.Entities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunVSCode1
{
    public static class SchedualFunc
    {
        [FunctionName("SchedualFunc")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Table("Flower",Connection = "AzureWebJobsStorage")]CloudTable flowerTable,
            [Queue("Flower",Connection = "AzureWebJobsStorage")] IAsyncCollector<string> stringQueue,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var deleted = 0;
            var query = new TableQuery<FlowerTableStorage>();
            var segment = await flowerTable.ExecuteQuerySegmentedAsync(query,null);
            foreach(var flower in segment)
            {
                if (!flower.IsActive) 
                {
                    await flowerTable.ExecuteAsync(TableOperation.Delete(flower));
                  deleted++;
                }
            }
            await stringQueue.AddAsync($"No. of deleted flowers is {deleted}");
            log.LogInformation($"No. of deleted flowers is {deleted}");
        }
    }
}