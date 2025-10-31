namespace SkyPathWS
{
    public interface IRepository<T>
    {
        bool Create();
        bool Update();
        bool Delete();
        List<T> GetALL();
        T GetById(string id);
    }
}
