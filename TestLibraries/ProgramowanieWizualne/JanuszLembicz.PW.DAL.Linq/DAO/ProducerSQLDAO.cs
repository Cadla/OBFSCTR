#region

using System.Collections.Generic;
using System.Linq;
using JanuszLembicz.PW.BO;

#endregion

namespace JanuszLembicz.PW.DAO
{
    public class ProducerSQLDAO : BaseDAO, IProducerDAO
    {
        #region Implementation of IProducerDAO

        public IList<IProducer> GetAll()
        {
            using(ORMDataContext context = GetDataContext())
            {
                var producers = from p in context.Producers select p;

                return producers.OfType<IProducer>().ToList();
            }
        }

        #endregion
    }
}