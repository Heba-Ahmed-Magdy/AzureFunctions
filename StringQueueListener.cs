using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace AzureFunVSCode1
{
    public static class StringQueueListener
    {
        [FunctionName("StringQueueListener")]
        public static async Task Run(
            [QueueTrigger("Flower", Connection = "AzureWebJobsStorage")]string myQueueItem,
            [Blob(blobPath: "flower", FileAccess.Write,Connection = "AzureWebJobsStorage")]CloudBlobContainer cloudBlobContainer,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            await cloudBlobContainer.CreateIfNotExistsAsync();
            var blobRef=cloudBlobContainer.GetBlockBlobReference("xx.txt");
            await blobRef.UploadTextAsync(myQueueItem);
            log.LogInformation(myQueueItem);
        }

        /*if my queue contains objects from flower type then your objec will be serialized you can deserialize
         it only through receiving it as flower object and not as string 
         string myQueueItem, =>Flower flower,*/
    }
}
