using Microsoft.AspNetCore.Mvc.Formatters;
using System.Data;
using System.Data.OleDb;

namespace SkyPathWS
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
            this.oleDbConnetction = new OleDbConnection();
            //הסבר לאיזה מוסד נתונים ליצור קשר על ידי מחרוזת התחברות
            this.oleDbConnetction.ConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\maxka\source\repos\Flight_Website_Project\SkyPathWS\App_Data\Database_SkyPath.accdb;Persist Security Info=True";
            //this.oleDbConnetction.ConnectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Directory.GetCurrentDirectory()}\App_Data\Database_SkyPath.accdb;Persist Security Info=True";
            this.dbCommand = new OleDbCommand();
            this.dbCommand.Connection = this.oleDbConnetction;
        }
        public void CloseConnection()
        {
            this.oleDbConnetction.Close();
        }

        public void Commit()
        {
            this.dbTransaction.Commit();
        }

        public int Delete(string sql)
        {
            this.dbCommand.CommandText = sql;
            return this.dbCommand.ExecuteNonQuery();
        }

        public int Insert(string sql)
        {
            this.dbCommand.CommandText = sql;
            return this.dbCommand.ExecuteNonQuery();
        }

        public void OpenConnection()
        {
            this.oleDbConnetction.Open();
        }

        public void OpenTransaction()
        {
            this.dbTransaction = this.oleDbConnetction.BeginTransaction();
        }

        public void RollBack()
        {
            this.dbTransaction?.Rollback();
        }

        public IDataReader Select(string sql)
        {
            this.dbCommand.CommandText = sql;
            return this.dbCommand.ExecuteReader();
        }

        public int Update(string sql)
        {
            this.dbCommand.CommandText = sql;
            return this.dbCommand.ExecuteNonQuery();
        }
    }
}
