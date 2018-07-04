using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace TechSharpy.Data
{
    public abstract class DataAccess
    {
        public MySql.Data.MySqlClient.MySqlConnection Connection;
        private MySql.Data.MySqlClient.MySqlDataAdapter da;
        private System.Data.DataTable dt;
        private MySqlCommand cmd;
        public void Init()
        {
            try
            {
                Connection = new MySql.Data.MySqlClient.MySqlConnection(GetConnection());
                Connection.Open();
                Connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid connection or unable to open connection", ex.InnerException);
            }

        }
        private string GetConnection()
        {
           // string Connection = ConfigurationManager.AppSettings["ConnectionString"];
            string Connection = "server=localhost;user=root;database=tshris;password=admin312;";
            return Connection;
            //SMRHRT.Services.Security.CryptoProvider cryp = new SMRHRT.Services.Security.CryptoProvider("check your connection");
            //Connection = cryp.DecryptString(Connection);
            //return Connection;

        }
        public System.Data.DataTable GetData(Query DataQuery)
        {
            try
            {
                Connection.Open();
                dt = new System.Data.DataTable();
                cmd = new MySqlCommand();
                cmd.CommandText = DataQuery.toString();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = Connection;
                using (da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                Connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get data", ex.InnerException);
            }
            finally {
                Connection.Close();
                cmd.Dispose();
            }
            return dt;
        }
        public int ExecuteQuery(Query DataQuery)
        {
            int Result;
            string stQuery = DataQuery.toString();
            try
            {
                cmd = new MySqlCommand(stQuery, Connection);
                Connection.Open();
                Result = cmd.ExecuteNonQuery();
                Connection.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Execute this query", ex.InnerException);
            }
            finally {
                Connection.Close();
            }
            return Result;
        }

        public bool ExecuteTQuery(TQuery tQuery) {
            MySqlCommand command = Connection.CreateCommand();
            try
            {
                Connection.Open();
                command.CommandText = tQuery.toString();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create table", ex.InnerException);
            }
            finally {
                Connection.Close();
                command.Dispose();
            }
            return true;
        }

        public int getNextID(string EntityID) {
            Query sQuery = new Query(QueryType._Select).AddField("keyvalue", "s_entitykeys")
                .AddWhere(0, "s_entitykeys", "entityid", FieldType._String, Operator._Equal, EntityID.ToString(), Condition._None);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = this.GetData(sQuery);
            int Nextid = 0;
            if (dt.Rows.Count > 0)
            {
                Nextid = Convert.ToInt32(dt.Rows[0]["keyvalue"]);
                Nextid = Nextid + 1;
                Query iQuery = new Query(QueryType._Update
                     ).AddTable("s_entitykeys")
                     .AddField("keyvalue", "s_entitykeys", FieldType._Number, "", Nextid.ToString())
                     .AddWhere(0, "s_entitykeys", "EntityID", FieldType._String, Operator._Equal, EntityID.ToString());
                this.ExecuteQuery(iQuery);
            }
            else {
                Nextid = Nextid + 1;
                Query iQuery = new Query(QueryType._Insert
                   ).AddTable("s_entitykeys")
                   .AddField("keyvalue", "s_entitykeys", FieldType._Number, "", Nextid.ToString())
                    .AddField("EntityID", "s_entitykeys", FieldType._String, "", EntityID.ToString())
                     .AddField("LastUPD", "s_entitykeys", FieldType._DateTime, "", DateTime.Now.ToString());             
                this.ExecuteQuery(iQuery);
            }
            return Nextid;
           
        }
    }
}

