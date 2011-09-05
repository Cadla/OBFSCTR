#region

#endregion

namespace JanuszLembicz.PW.BO
{
    public partial class Product : IProduct
    {
        #region Implementation of IProduct

        public IOInterface? Interface
        {
            get
            {
                return (IOInterface?)IntInterface;
            }
            set
            {
                IntInterface = (int)(value ?? 0);
            }
        }

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