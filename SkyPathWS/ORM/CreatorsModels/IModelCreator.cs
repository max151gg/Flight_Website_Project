using System.Data;

namespace SkyPathWS.ORM.CreatorsModels
{
    public interface IModelCreator<T>
    {
        T CreateModel(IDataReader dataReader);
    }
}
