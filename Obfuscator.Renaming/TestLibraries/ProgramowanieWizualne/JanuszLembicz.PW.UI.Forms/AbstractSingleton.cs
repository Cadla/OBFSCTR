namespace JanuszLembicz.Utils
{
    public abstract class AbstractSingleton<T> where T : class, new()
    {
        private static readonly T _instance;

        public static T GetInstance
        {
            get
            {
                return _instance;
            }
        }

        static AbstractSingleton()
        {
            _instance = new T();
        }
    }
}