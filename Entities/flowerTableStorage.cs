using Microsoft.Azure.Cosmos.Table;

namespace AzureFunVSCode1.Entities
{
    public class FlowerTableStorage : TableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public bool IsActive { get; set; }
    }
}