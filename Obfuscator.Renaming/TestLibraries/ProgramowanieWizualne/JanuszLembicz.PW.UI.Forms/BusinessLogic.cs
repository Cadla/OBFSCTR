#region

using System;
using System.Collections.Generic;
using System.Linq;
using JanuszLembicz.PW.BO;
using JanuszLembicz.PW.DAO;
using JanuszLembicz.Utils;

#endregion

namespace JanuszLembicz.PW
{
    public class BusinessLogic
    {
        private readonly IFactory _factory;

        public BusinessLogic()
        {
            _factory = ObjectFactory.GetInstance.CreateInstance<IFactory>();
        }


        public IList<ProductListElement> GetAllProducts()
        {
            IProductDAO productDAO = _factory.GetProductDAO();
            return productDAO.GetProductListAll();
        }

        public IList<IProducer> GetAllProducers()
        {
            IProducerDAO producerDAO = _factory.GetProducerDAO();
            return producerDAO.GetAll();
        }

        public IList<ProductListElement> GetFilteredProducts(ProductFilter filter)
        {
            IProductDAO productDAO = _factory.GetProductDAO();
            return productDAO.GetFilteredProductList(filter);
        }

        public IList<ProductListElement> GetSmallestProducts(int n)
        {
            IProductDAO productDAO = _factory.GetProductDAO();
            return GetSmallestProducts(productDAO.GetProductListAll(), n);
        }

        private IList<ProductListElement> GetSmallestProducts(IList<ProductListElement> products, int n)
        {
            IList<ProductListElement> result = new List<ProductListElement>();
            n = Math.Min(n, products.Count);
            if(n > 0)
            {
                products = products.OrderBy(product => product.MemoryCapacity).ToList();
                result =
                    products.TakeWhile(product => product.MemoryCapacity <= products[n - 1].MemoryCapacity).ToList();
            }
            return result;
        }
    }
}