namespace SkyPathWS.ORM.Repositories
{
    // Base class for every repository. It gives each repository two tools:
    // helperOleDb (to run SQL on the database) and modelCreators (to turn rows into objects).
    public class Repository
    {
        protected DbHelperOleDb helperOleDb;
        protected ModelCreators modelCreators;

        public Repository(DbHelperOleDb helperOleDb, ModelCreators modelCreators)
        {
            this.helperOleDb = helperOleDb;
            this.modelCreators = modelCreators;
        }
    }
}
