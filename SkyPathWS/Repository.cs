namespace SkyPathWS
{
    public class Repository
    {
        protected DbHelperOleDb helperOleDb;
        protected ModelCreators modelCreators;

        public Repository()
        {
            this.helperOleDb = new DbHelperOleDb();
            this.modelCreators = new ModelCreators();
        }
    }
}
