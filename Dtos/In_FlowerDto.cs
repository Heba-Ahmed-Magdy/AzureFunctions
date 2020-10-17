using System;

namespace AzureFunVSCode1.Dtos
{
    public class In_FlowerDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
    }
}