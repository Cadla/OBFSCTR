#region

using JanuszLembicz.PW.Properties;

#endregion

namespace JanuszLembicz.PW.DAO
{
    public class BaseDAO
    {
        public ORMDataContext GetDataContext()
        {
            return new ORMDataContext(Settings.Default.JanuszLembicz_PW_DBConnectionString);
        }
    }
}