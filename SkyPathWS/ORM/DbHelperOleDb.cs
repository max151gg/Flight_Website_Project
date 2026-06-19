using Microsoft.AspNetCore.Mvc.Formatters;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;

namespace SkyPathWS.ORM
{
    public class DbHelperOleDb : IDatabaseHelper
    {
        //ADO.NET technology data object, there are a lot of classes there
        //תפקיד שלו ליצור קשר עם המוסד נתונים
        OleDbConnection oleDbConnetction;


        //שולח פקודות למוסד נתונים ומחזיר תשובות משם
        OleDbCommand dbCommand;


        OleDbTransaction dbTransaction;


        // Builds the connection to the Access database file (App_Data/Database_SkyPath.accdb)
        // using the ACE OLEDB provider. The path is relative so it works on any machine.
        public DbHelperOleDb()
        {
            oleDbConnetction = new OleDbConnection();

            string dbPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "App_Data",
                "Database_SkyPath.accdb"
            );

            oleDbConnetction.ConnectionString =
                $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=True";

            dbCommand = new OleDbCommand();
            dbCommand.Connection = oleDbConnetction;
        }

        public void CloseConnection()
        {
            oleDbConnetction.Close();
        }

        public void Commit()
        {
            dbTransaction.Commit();
        }

        public int Delete(string sql)
        {
            dbCommand.CommandText = sql;
            int records = dbCommand.ExecuteNonQuery();
            dbCommand.Parameters.Clear();
            return records;
        }

        // Runs an INSERT and returns how many rows were added (used to confirm success).
        public int Insert(string sql)
        {
            dbCommand.CommandText = sql;
            int records = dbCommand.ExecuteNonQuery();
            dbCommand.Parameters.Clear();
            return records;
        }

        public void OpenConnection()
        {
            oleDbConnetction.Open();
        }

        public void OpenTransaction()
        {
            dbTransaction = oleDbConnetction.BeginTransaction();
            this.dbCommand.Transaction = dbTransaction;
        }

        public void RollBack()
        {
            dbTransaction?.Rollback();
        }

        // Runs a SELECT and returns a reader to loop through the rows.
        public IDataReader Select(string sql)
        {
            dbCommand.CommandText = sql;
            Console.WriteLine(sql);
            IDataReader reader = dbCommand.ExecuteReader();
            dbCommand.Parameters.Clear();
            return reader;
        }

        public int Update(string sql)
        {
            dbCommand.CommandText = sql;
            int records = dbCommand.ExecuteNonQuery();
            dbCommand.Parameters.Clear();
            return records;
        }
        // Adds a value for a @parameter in the SQL. Using parameters (instead of building
        // the SQL string by hand) keeps the app safe from SQL injection.
        public void AddParameter(string name, object value)
        {
            dbCommand.Parameters.Add(new OleDbParameter(name, value));
        }
        public string GetLastInsertedId()
        {
            string sql = "SELECT @@IDENTITY";
            dbCommand.CommandText = sql;
            object result = dbCommand.ExecuteScalar();
            return result?.ToString();
        }
    }
}
