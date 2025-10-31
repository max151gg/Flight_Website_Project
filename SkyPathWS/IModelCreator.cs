using System.Data;

namespace SkyPathWS
{
    public interface IModelCreator<T>
    {
        T CreateModel(IDataReader dataReader);
    }
}
