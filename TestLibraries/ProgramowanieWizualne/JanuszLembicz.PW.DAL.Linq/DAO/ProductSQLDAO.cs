#region

using System.Collections.Generic;
using System.Linq;
using JanuszLembicz.PW.BO;

#endregion

namespace JanuszLembicz.PW.DAO
{
    public class ProductSQLDAO : BaseDAO, IProductDAO
    {
        #region Implementation of IProductDAO

        public IList<IProduct> GetAll()
        {
            using(ORMDataContext context = GetDataContext())
            {
                var products = from p in context.Products select p;
                return products.OfType<IProduct>().ToList();
            }
        }

        public IList<ProductListElement> GetProductListAll()
        {
            using(ORMDataContext context = GetDataContext())
            {
                var products = from p in context.Products
                               select
                                   new ProductListElement
                                       {
                                           HasDisplay = p.HasDisplay,
                                           Producer = p.Producer.Name,
                                           Interface = p.Interface,
                                           MemoryCapacity = p.MemoryCapacity,
                                           Name = p.Name,
                                           Warranty = p.Warranty
                                       };
                return products.ToList();
            }
        }

        public IList<ProductListElement> GetFilteredProductList(ProductFilter filter)
        {
            using(ORMDataContext context = GetDataContext())
            {
                var products = from p in context.Products
                               where
                                   (!filter.HasDisplay.HasValue || p.HasDisplay == filter.HasDisplay) &&
                                   (!filter.Interface.HasValue || p.IntInterface.Equals((int?)filter.Interface)) &&
                                   (!filter.MemoryCapacityFrom.HasValue || p.MemoryCapacity >= filter.MemoryCapacityFrom) &&
                                   (!filter.MemoryCapacityTo.HasValue || p.MemoryCapacity <= filter.MemoryCapacityTo) &&
                                   (!filter.WarrantyFrom.HasValue || p.Warranty >= filter.WarrantyFrom) &&
                                   (!filter.WarrantyTo.HasValue || p.Warranty <= filter.WarrantyTo) &&
                                   (p.Name.ToLower().Contains(filter.ProductName.ToLower())) &&
                                   (p.Producer.Name.ToLower().Contains(filter.ProducerName.ToLower()))
                               select
                                   new ProductListElement
                                       {
                                           HasDisplay = p.HasDisplay,
                                           Producer = p.Producer.Name,
                                           Interface = p.Interface,
                                           MemoryCapacity = p.MemoryCapacity,
                                           Name = p.Name,
                                           Warranty = p.Warranty
                                       };

                return products.ToList();
            }
        }

        #endregion
    }
}