namespace SkyPathWS.ORM.Repositories
{
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
