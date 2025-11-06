namespace SkyPathWS.Repositories
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
