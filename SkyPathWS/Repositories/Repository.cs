namespace SkyPathWS.Repositories
{
    public class Repository
    {
        protected DbHelperOleDb helperOleDb;
        protected ModelCreators modelCreators;

        public Repository()
        {
            helperOleDb = new DbHelperOleDb();
            modelCreators = new ModelCreators();
        }
    }
}
