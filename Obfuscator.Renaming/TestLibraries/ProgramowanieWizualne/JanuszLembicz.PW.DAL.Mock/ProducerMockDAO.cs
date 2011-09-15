#region

using System.Collections.Generic;
using JanuszLembicz.PW.BO;

#endregion

namespace JanuszLembicz.PW.DAO
{
    public class ProducerMockDAO : IProducerDAO
    {
        #region Implementation of IProducerDAO

        public IList<IProducer> GetAll()
        {
            IList<IProducer> result = new List<IProducer>();

            result.Add(new Producer {ProducerID = 1, Name = "Creative"});
            result.Add(new Producer {ProducerID = 2, Name = "Apple"});
            result.Add(new Producer {ProducerID = 3, Name = "Microsoft"});

            return result;
        }

        #endregion
    }
}