namespace JanuszLembicz.PW
{
    public class ProductFilter
    {
        public string ProducerName { get; set; }

        public string ProductName { get; set; }

        public int? MemoryCapacityFrom { get; set; }

        public int? MemoryCapacityTo { get; set; }

        public int? WarrantyFrom { get; set; }

        public int? WarrantyTo { get; set; }

        public bool? HasDisplay { get; set; }

        public IOInterface? Interface { get; set; }
    }
}