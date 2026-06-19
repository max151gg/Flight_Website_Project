using System.Data;

namespace SkyPathWS.ORM.CreatorsModels
{
    // Shared shape for all "creators": take one database row (IDataReader)
    // and build one model object of type T (for example Flight, User, or Ticket).
    public interface IModelCreator<T>
    {
        T CreateModel(IDataReader dataReader);
    }
}
