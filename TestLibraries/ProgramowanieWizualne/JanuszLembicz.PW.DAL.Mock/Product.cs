namespace JanuszLembicz.PW.BO
{
    public class Product : IProduct
    {
        #region Implementation of IProduct

        public int ProductID { get; set; }
        public int ProducerID { get; set; }
        public string Name { get; set; }

        public int MemoryCapacity { get; set; }
        public int? Warranty { get; set; }

        public bool HasDisplay { get; set; }
        public IOInterface? Interface { get; set; }

        #endregion

        public override bool Equals(object obj)
        {
            IProduct product = obj as IProduct;
            if(product != null)
            {
                return HasDisplay == product.HasDisplay && Interface == product.Interface &&
                       MemoryCapacity == product.MemoryCapacity && Name == product.Name &&
                       ProducerID == product.ProducerID && ProductID == product.ProductID &&
                       Warranty == product.Warranty;
            }
            return false;
        }
    }
}