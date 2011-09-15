#region

using System.Collections.Generic;
using JanuszLembicz.PW.BO;

#endregion

namespace JanuszLembicz.PW.DAO
{
    public class ProductMockDAO : IProductDAO
    {
        #region Implementation of IProductDAO

        public IList<IProduct> GetAll()
        {
            IList<IProduct> result = new List<IProduct>();

            result.Add(new Product
                           {
                               ProductID = 1,
                               ProducerID = 1,
                               Name = "MOCK: ZEN",
                               HasDisplay = true,
                               Interface = IOInterface.USB2,
                               MemoryCapacity = 4096,
                               Warranty = 2
                           });

            result.Add(new Product
                           {
                               ProductID = 2,
                               ProducerID = 2,
                               Name = "MOCK: IPod",
                               HasDisplay = false,
                               Interface = IOInterface.USB1,
                               MemoryCapacity = 512,
                               Warranty = 1
                           });

            result.Add(new Product
                           {
                               ProductID = 3,
                               ProducerID = 2,
                               Name = "MOCK: ZUNE",
                               HasDisplay = true,
                               Interface = IOInterface.USB2,
                               MemoryCapacity = 8192,
                           });

            result.Add(new Product
                           {
                               ProductID = 4,
                               ProducerID = 1,
                               Name = "MOCK: ZEN X-Fi",
                               HasDisplay = true,
                               Interface = IOInterface.USB2,
                               MemoryCapacity = 1024,
                               Warranty = 3
                           });

            result.Add(new Product
                           {
                               ProductID = 5,
                               ProducerID = 1,
                               Name = "MOCK: MuVo",
                               HasDisplay = true,
                               Interface = IOInterface.USB1,
                               MemoryCapacity = 512,
                               Warranty = 2
                           });

            result.Add(new Product
                           {
                               ProductID = 6,
                               ProducerID = 2,
                               Name = "MOCK: IPod Touch",
                               HasDisplay = true,
                               Interface = IOInterface.USB2,
                               MemoryCapacity = 1024,
                               Warranty = 1
                           });

            return result;
        }

        public IList<ProductListElement> GetProductListAll()
        {
            IList<ProductListElement> result = new List<ProductListElement>();

            result.Add(new ProductListElement()
                           {
                               Producer = "MOCK: Creative",
                               Name = "MOCK: ZEN",
                               HasDisplay = true,
                               Interface = IOInterface.USB2,
                               MemoryCapacity = 4096,
                               Warranty = 2
                           });

            result.Add(new ProductListElement()
                           {
                               Producer = "MOCK: Apple",
                               Name = "MOCK: IPod",
                               HasDisplay = false,
                               Interface = IOInterface.USB1,
                               MemoryCapacity = 512,
                               Warranty = 1
                           });

            result.Add(new ProductListElement()
                           {
                               Producer = "MOCK: Microsoft",
                               Name = "MOCK: ZUNE",
                               HasDisplay = true,
                               Interface = IOInterface.USB2,
                               MemoryCapacity = 8192,
                           });
            result.Add(new ProductListElement()
                           {
                               Producer = "MOCK: Creative",
                               Name = "MOCK: ZEN",
                               HasDisplay = true,
                               Interface = IOInterface.USB2,
                               MemoryCapacity = 1024,
                               Warranty = 3
                           });

            result.Add(new ProductListElement()
                           {
                               Producer = "MOCK: Creative",
                               Name = "MOCK: IPod",
                               HasDisplay = true,
                               Interface = IOInterface.USB2,
                               MemoryCapacity = 512,
                               Warranty = 2
                           });

            result.Add(new ProductListElement()
                           {
                               Producer = "MOCK: Apple",
                               Name = "MOCK: ZUNE",
                               HasDisplay = true,
                               Interface = IOInterface.USB2,
                               MemoryCapacity = 1024,
                               Warranty = 1
                           });

            return result;
        }

        public IList<ProductListElement> GetFilteredProductList(ProductFilter filter)
        {
            return GetProductListAll();
        }

        #endregion
    }
}