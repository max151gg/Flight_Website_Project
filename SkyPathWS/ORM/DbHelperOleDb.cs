using Microsoft.AspNetCore.Mvc.Formatters;
using System.Data;
using System.Data.OleDb;

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


        public DbHelperOleDb()
        {
            oleDbConnetction = new OleDbConnection();
            //הסבר לאיזה מוסד נתונים ליצור קשר על ידי מחרוזת התחברות
            oleDbConnetction.ConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\maxka\source\repos\Flight_Website_Project\SkyPathWS\App_Data\Database_SkyPath.accdb;Persist Security Info=True";
            //this.oleDbConnetction.ConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Directory.GetCurrentDirectory()}\App_Data\Database_SkyPath.accdb;Persist Security Info=True";
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
            int records= dbCommand.ExecuteNonQuery();
            dbCommand.Parameters.Clear();
            return records;
        }

        public int Insert(string sql)
        {
            dbCommand.CommandText = sql;
            int records= dbCommand.ExecuteNonQuery();
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
        }

        public void RollBack()
        {
            dbTransaction?.Rollback();
        }

        public IDataReader Select(string sql)
        {
            dbCommand.CommandText = sql;
            Console.WriteLine(sql);
            IDataReader reader= dbCommand.ExecuteReader();
            dbCommand.Parameters.Clear();
            return reader;
        }

        public int Update(string sql)
        {
            dbCommand.CommandText = sql;
            int records= dbCommand.ExecuteNonQuery();
            dbCommand.Parameters.Clear();
            return records;
        }
        public void AddParameter(string name, object value)
        {
            dbCommand.Parameters.Add(new OleDbParameter(name, value));
        }
    }
}
