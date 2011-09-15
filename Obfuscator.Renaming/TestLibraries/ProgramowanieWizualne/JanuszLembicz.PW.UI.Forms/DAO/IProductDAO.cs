#region

using System.Collections.Generic;
using JanuszLembicz.PW.BO;

#endregion

namespace JanuszLembicz.PW.DAO
{
    public interface IProductDAO
    {
        IList<IProduct> GetAll();

        IList<ProductListElement> GetProductListAll();

        IList<ProductListElement> GetFilteredProductList(ProductFilter filter);
    }
}