using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunVSCode1
{
    public static class FlowerBlobListener
    {
        //[FunctionName("FlowerBlobListener")]
        public static void Run(
            [BlobTrigger("flower/{name}", Connection = "AzureWebJobsStorage")]string myBlob, string name,
            [SendGrid(ApiKey ="")]
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
