using NewsPortalRestServer.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Service
{
    public class DBProvider
    {
        private NpgsqlConnection conn;

        private enum QueryType { Insert = 1, Update = 2}
        public DBProvider()
        {

        }

        public void Connect(string server, string port, string user, string pass, string database)
        {
            try
            {
                conn = new NpgsqlConnection("Server=" + server +
                                       ";Port=" + port +
                                       ";User Id=" + user +
                                       ";Password=" + pass +
                                       ";Database=" + database +
                                       ";");
                conn.Open();
            }
            catch
            {
                throw new DBProviderConnectException("Подключение к базе данных не удалось!");
            }
           
        }

        private NpgsqlCommand BuildQuery(QueryType queryType, string table, Dictionary<string, object> data, string where="")
        {
            string q="";
            
            switch (queryType)
            {
                case QueryType.Insert:
                    q = "INSERT INTO " + table;
                    q = q + BuildInsertParamString(data);
                    q = q + " SELECT currval('" + table + "_id_seq');";
                    break;

                case QueryType.Update:
                    q = "UPDATE " + table;
                    q = q + BuildUpdateParamString(data);
                    q = q + " WHERE " + where;              
                    break;                    
            }

            // create command
            NpgsqlCommand cmd = new NpgsqlCommand(q, conn);

            // add params
            foreach (var par in data)
                cmd.Parameters.AddWithValue("@" + par.Key, par.Value);

            return cmd;
        }      

        private Dictionary<string, object> GetModelData(DataModel model)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            Type t = model.GetType();
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            foreach (var prop in properties.Where(field => field.Name != "id" && field.GetValue(model) != null))
                data.Add(prop.Name, prop.GetValue(model));  

            return data;
        }
       
        private string BuildInsertParamString(Dictionary<string, object> data)
        {  
            string s = " (";

            // add keys
            string keys = string.Join(",", data.Keys);
            s = s + keys + ") VALUES ";

            // add markers
            s = s + "(@" + string.Join(",@", data.Keys) + "); ";
           
            return s;
        }

        private string BuildUpdateParamString(Dictionary<string, object> data)
        {
            string s = " SET ";           

            foreach (var key in data.Keys)
                s = s + key + "=" + "@" + key + ", ";
            s = s.Substring(0, s.Length - 2) + " ";

            return s;
        } 

        public List<Dictionary<string, object>> Select(string q)
        {            
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            NpgsqlDataReader dr;
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(q, conn); 
                
                dr = cmd.ExecuteReader();
            }
            catch(Npgsql.PostgresException ex)
            {
                throw ex;
            }
            catch
            {
                throw new DBProviderExecuteException();
            }
            finally
            {
                conn.Clone();
            }             

            while (dr.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int i = 0; i < dr.FieldCount; i++)
                    row.Add(dr.GetName(i), dr[i]);
                res.Add(row);
            }            
            return res;
        }

        public int Insert(string table, DataModel model)
        {
            NpgsqlCommand cmd = BuildQuery(QueryType.Insert, table, GetModelData(model));

            int insertID;
            try
            {                
                NpgsqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                insertID = Convert.ToInt32(dr[0]);
            }
            catch
            {
                throw new DBProviderExecuteException();
            }
            finally
            {
                conn.Close();
            }

            return insertID;
        }

        public void Update(string table, string where, DataModel model)
        {
            NpgsqlCommand cmd =  BuildQuery(QueryType.Update, table, GetModelData(model), where);
            
            try
            {                
                NpgsqlDataReader dr = cmd.ExecuteReader();
            }
            catch
            {
                throw new DBProviderExecuteException();
            }
            finally
            {
                conn.Close();
            }            
        }

        public void Delete(string table, string where)
        {
            string q = "DELETE FROM " + table + " WHERE " + where;
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(q, conn);
                NpgsqlDataReader dr = command.ExecuteReader();
            }
            catch (Npgsql.PostgresException ex)
            {
                throw ex;
            }
            catch
            {
                throw new DBProviderExecuteException();
            }
            finally
            {
                conn.Close();
            }                                  
        }
    }
}
