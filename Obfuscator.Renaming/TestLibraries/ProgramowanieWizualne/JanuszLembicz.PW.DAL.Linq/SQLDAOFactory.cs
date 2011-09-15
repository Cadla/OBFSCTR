#region

using JanuszLembicz.PW.DAO;

#endregion

namespace JanuszLembicz.PW
{
    public class SQLDAOFactory : IFactory
    {
        #region Implementation of IFactory

        public IProducerDAO GetProducerDAO()
        {
            return new ProducerSQLDAO();
        }

        public IProductDAO GetProductDAO()
        {
            return new ProductSQLDAO();
        }

        #endregion
    }
}