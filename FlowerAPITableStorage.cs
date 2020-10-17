using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunVSCode1.Entities;
using AzureFunVSCode1.Dtos;
using AzureFunVSCode1.Mapping;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Cosmos.Table;

namespace AzureFunVSCode1
{
    public static class FlowerAPITableStorage
    {
        [FunctionName("CreateFlowerTableStorage")]
        public static async Task<IActionResult> CreateFlowerTableStorage(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "flowerTS")] HttpRequest req,
           [Table("Flower",Connection ="AzureWebJobsStorage")] IAsyncCollector<FlowerTableStorage> flowerTable,
           ILogger log)
        {
            log.LogInformation("Create TableStorage flower method");
            var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            var flowerViewModel = JsonConvert.DeserializeObject<FlowerCreateViewModel>(reqBody);
            var flower = new In_FlowerDto
            {
                Name = flowerViewModel.Name,
                Description = flowerViewModel.Description,
                Price = flowerViewModel.Price,
                IsActive= flowerViewModel.IsActive
            };
            await flowerTable.AddAsync(flower.ToFlowerTableStorage());
            return new OkObjectResult(flowerViewModel);
        }

        [FunctionName("GetAllFlowersTableStorage")]
        public static async Task<IActionResult> GetAllFlowersTableStorage(
            [HttpTrigger(AuthorizationLevel.Function, "Get", Route = "flowerTS")] HttpRequest req,
            [Table("Flower",Connection ="AzureWebJobsStorage")] CloudTable flowerTable,
            ILogger log)
        {
            log.LogInformation("Get All Flowers method");
            var query = new TableQuery<FlowerTableStorage>();
            var segment = await flowerTable.ExecuteQuerySegmentedAsync(query,null);
            return new OkObjectResult(segment.Select(x=>x.ToOp_FlowerDto()));
        }

        [FunctionName("GetFlowerTableStorage")]
        public static IActionResult GetFlowerTableStorage(
            [HttpTrigger(AuthorizationLevel.Function, "Get", Route = "flowerTS/{id}")] HttpRequest req,
            [Table("Flower", "Flower", "{id}",Connection = "AzureWebJobsStorage")]FlowerTableStorage flowerTableStorage,
            ILogger log, string id)
        {
            log.LogInformation("Get Flower method");
            if (flowerTableStorage == null)
                return new NotFoundResult();

            return new OkObjectResult(flowerTableStorage);
        }

        [FunctionName("UpdateFlowerTableStorage")]
        public static async Task<IActionResult> UpdateFlowerTableStorage(
           [HttpTrigger(AuthorizationLevel.Function, "Put", Route = "flowerTS/{id}")] HttpRequest req,
           [Table("Flower",Connection = "AzureWebJobsStorage")]CloudTable flowerTable,
           ILogger log, string id)
        {
            log.LogInformation("Update Flower method");

            if (String.IsNullOrEmpty(id))
                return new BadRequestObjectResult("Please send flower Id");

            log.LogInformation("Try to get flower from tableStorage");
            var findOperation = TableOperation.Retrieve<FlowerTableStorage>("Flower",id);
            var findResult = await flowerTable.ExecuteAsync(findOperation);
            if (findResult.Result == null)
                return new NotFoundResult();

            log.LogInformation("Try to get data from req body");
            var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            var flowerViewModel = JsonConvert.DeserializeObject<FlowerCreateViewModel>(reqBody);

            var updatedFlower = (FlowerTableStorage)findResult.Result;
            updatedFlower.Name = String.IsNullOrEmpty(flowerViewModel.Name)? updatedFlower.Name: flowerViewModel.Name;
            updatedFlower.Description = String.IsNullOrEmpty(flowerViewModel.Description) ? updatedFlower.Description : flowerViewModel.Description;
            updatedFlower.Price = flowerViewModel.Price==0 ? updatedFlower.Price : flowerViewModel.Price;
            updatedFlower.IsActive =  flowerViewModel.IsActive;

            var updateOperation = TableOperation.Replace(updatedFlower);
            await flowerTable.ExecuteAsync(updateOperation);

            return new OkObjectResult(updatedFlower.ToOp_FlowerDto());
        }

        [FunctionName("DeleteFlowerTableStorage")]
        public static async Task<IActionResult> DeleteFlowerTableStorage(
           [HttpTrigger(AuthorizationLevel.Function, "Delete", Route = "flowerTS/{id}")] HttpRequest req,
           [Table("Flower",Connection ="AzureWebJobsStorage")] CloudTable flowerTable,
           ILogger log, string id)
        {
            log.LogInformation("Get Flower method");
            var deleteOperation = TableOperation.Delete(
                   new FlowerTableStorage { PartitionKey="Flower", RowKey=id, ETag="*"});

            try
            {
               await flowerTable.ExecuteAsync(deleteOperation);
            }
            catch(Exception ex) /*when()*/
            {
                return new NotFoundResult();
            }

            return new OkResult();
        }

    }
}