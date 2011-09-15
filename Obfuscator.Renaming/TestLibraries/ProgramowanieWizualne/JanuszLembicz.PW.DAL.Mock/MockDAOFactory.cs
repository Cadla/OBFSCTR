#region

using JanuszLembicz.PW;
using JanuszLembicz.PW.DAO;

#endregion

namespace JanuszLembicz
{
    public class MockDAOFactory : IFactory
    {
        #region Implementation of IFactory

        public IProducerDAO GetProducerDAO()
        {
            return new ProducerMockDAO();
        }

        public IProductDAO GetProductDAO()
        {
            return new ProductMockDAO();
        }

        #endregion
    }
}