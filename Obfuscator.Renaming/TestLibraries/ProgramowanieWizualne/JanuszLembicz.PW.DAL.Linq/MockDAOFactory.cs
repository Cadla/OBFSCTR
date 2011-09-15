#region

using JanuszLembicz.PW.DAO;

#endregion

namespace JanuszLembicz.PW
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