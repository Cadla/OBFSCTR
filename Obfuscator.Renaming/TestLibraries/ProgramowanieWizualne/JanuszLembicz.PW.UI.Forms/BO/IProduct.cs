#region

#endregion

namespace JanuszLembicz.PW.BO
{
    public interface IProduct
    {
        int ProductID { get; set; }

        int ProducerID { get; set; }

        string Name { get; set; }

        int MemoryCapacity { get; set; }

        int? Warranty { get; set; }

        bool HasDisplay { get; set; }

        IOInterface? Interface { get; set; }
    }
}