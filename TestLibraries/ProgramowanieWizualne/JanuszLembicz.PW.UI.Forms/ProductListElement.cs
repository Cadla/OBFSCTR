namespace JanuszLembicz.PW
{
    public class ProductListElement
    {
        public string Name { get; set; }

        public string Producer { get; set; }

        public int MemoryCapacity { get; set; }

        public int? Warranty { get; set; }

        public bool HasDisplay { get; set; }

        public IOInterface? Interface { get; set; }


        public override bool Equals(object obj)
        {
            ProductListElement product = obj as ProductListElement;
            if(product != null)
            {
                return HasDisplay == product.HasDisplay && Interface == product.Interface &&
                       MemoryCapacity == product.MemoryCapacity && Name == product.Name && Producer == product.Producer &&
                       Warranty == product.Warranty;
            }
            return false;
        }
    }
}