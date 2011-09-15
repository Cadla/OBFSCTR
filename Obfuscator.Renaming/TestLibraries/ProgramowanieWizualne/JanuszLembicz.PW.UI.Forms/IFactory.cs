#region

using JanuszLembicz.PW.DAO;

#endregion

namespace JanuszLembicz.PW
{
    public interface IFactory
    {
        IProducerDAO GetProducerDAO();
        IProductDAO GetProductDAO();
    }
}