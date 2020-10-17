using AzureFunVSCode1.Dtos;
using AzureFunVSCode1.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunVSCode1.Mapping
{
    public static class MappingBetDtosAndEntities
    {
        public static FlowerTableStorage ToFlowerTableStorage(this In_FlowerDto flower)
        {
            return new FlowerTableStorage
            {
                PartitionKey = "Flower",
                RowKey = flower.Id,
                Name = flower.Name,
                Description = flower.Description,
                Price = flower.Price,
                Timestamp = flower.CreationDate,
                IsActive= flower.IsActive
            };
        }
        public static Op_FlowerDto ToOp_FlowerDto(this FlowerTableStorage flower)
        {
            return new Op_FlowerDto
            {
                Name = flower.Name,
                Description = flower.Description,
                Price = flower.Price,
                IsActive=flower.IsActive
            };
        }
    }
}
