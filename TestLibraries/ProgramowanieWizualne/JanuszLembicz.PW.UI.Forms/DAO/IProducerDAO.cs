#region

using System.Collections.Generic;
using JanuszLembicz.PW.BO;

#endregion

namespace JanuszLembicz.PW.DAO
{
    public interface IProducerDAO
    {
        IList<IProducer> GetAll();
    }
}