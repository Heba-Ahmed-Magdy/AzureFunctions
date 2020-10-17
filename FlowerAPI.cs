using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Company.Function
{
    public static class FlowerAPI
    {
        public static List<Flower> Flowers = new List<Flower>();

        [FunctionName("CreateFlower")]
        public static async Task<IActionResult> CreateFlower(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "flower")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Create flower method");
            var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            var flowerViewModel = JsonConvert.DeserializeObject<FlowerCreateViewModel>(reqBody);
            var flower = new Flower
            {
                Name = flowerViewModel.Name,
                Description = flowerViewModel.Description,
                Price = flowerViewModel.Price
            };
            Flowers.Add(flower);
            return new OkObjectResult(flower);
        }

        [FunctionName("GetAllFlowers")]
        public static IActionResult GetAllFlowers(
            [HttpTrigger(AuthorizationLevel.Function, "Get", Route = "flower")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get All Flowers method");
            return new OkObjectResult(Flowers);
        }

        [FunctionName("GetFlower")]
        public static IActionResult GetFlower(
            [HttpTrigger(AuthorizationLevel.Function, "Get", Route = "flower/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Get Flower method");
            if (String.IsNullOrEmpty(id))
                return new BadRequestObjectResult("Please send flower Id");

            var flower = Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null)
                return new NotFoundResult();

            return new OkObjectResult(flower);
        }

        [FunctionName("UpdateFlower")]
        public static async Task<IActionResult> UpdateFlower(
           [HttpTrigger(AuthorizationLevel.Function, "Put", Route = "flower/{id}")] HttpRequest req,
           ILogger log, string id)
        {
            log.LogInformation("Update Flower method");
            if (String.IsNullOrEmpty(id))
                return new BadRequestObjectResult("Please send flower Id");

            var flower = Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null)
                return new NotFoundResult();

            var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            var flowerViewModel = JsonConvert.DeserializeObject<FlowerCreateViewModel>(reqBody);
            flower.Name = flowerViewModel.Name;
            flower.Description = flowerViewModel.Description;
            flower.Price = flowerViewModel.Price;

            return new OkObjectResult(flower);
        }

        [FunctionName("DeleteFlower")]
        public static IActionResult DeleteFlower(
           [HttpTrigger(AuthorizationLevel.Function, "Delete", Route = "flower/{id}")] HttpRequest req,
           ILogger log, string id)
        {
            log.LogInformation("Get Flower method");
            if (String.IsNullOrEmpty(id))
                return new BadRequestObjectResult("Please send flower Id");

            var flower = Flowers.FirstOrDefault(f => f.Id == id);
            if (flower == null)
                return new NotFoundResult();
            Flowers.Remove(flower);

            return new OkResult();
        }

    }
}
