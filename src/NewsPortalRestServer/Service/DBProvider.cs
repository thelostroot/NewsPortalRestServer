using NewsPortalRestServer.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace NewsPortalRestServer.Service
{
    public static class DBProvider
    {
        private static NpgsqlConnection conn;

        private enum QueryType { Insert = 1, Update = 2}

        static DBProvider()
        {
            // to config
            string server = "localhost";
            string port = "5432";
            string user = "postgres";
            string pass = "1";
            string database = "ASP";
            
            try
            {
                conn = new NpgsqlConnection("Server=" + server +
                                       ";Port=" + port +
                                       ";User Id=" + user +
                                       ";Password=" + pass +
                                       ";Database=" + database +
                                       ";");
            }
            catch
            {
                throw new DBProviderConnectException("Подключение к базе данных не удалось!");
            }
           
        }        

        private static NpgsqlCommand BuildQuery(QueryType queryType, string table, Dictionary<string, object> data, string where="")
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

        private static Dictionary<string, object> GetModelData(DataModel model)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            Type t = model.GetType();
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            foreach (var prop in properties.Where(field => field.Name != "id" && field.GetValue(model) != null))
                data.Add(prop.Name, prop.GetValue(model));  

            return data;
        }
       
        private static string BuildInsertParamString(Dictionary<string, object> data)
        {  
            string s = " (";

            // add keys
            string keys = string.Join(",", data.Keys);
            s = s + keys + ") VALUES ";

            // add markers
            s = s + "(@" + string.Join(",@", data.Keys) + "); ";
           
            return s;
        }

        private static string BuildUpdateParamString(Dictionary<string, object> data)
        {
            string s = " SET ";           

            foreach (var key in data.Keys)
                s = s + key + "=" + "@" + key + ", ";
            s = s.Substring(0, s.Length - 2) + " ";

            return s;
        }
        
        private static List<Dictionary<string, object>> SelectFromDB(NpgsqlCommand cmd)
        {
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            NpgsqlDataReader dr;
            try
            {
                conn.Open();               

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int i = 0; i < dr.FieldCount; i++)
                        row.Add(dr.GetName(i), dr[i]);
                    res.Add(row);
                }
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

            return res;
        }

        public static List<Dictionary<string, object>> Select(string q)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(q, conn);
            return SelectFromDB(cmd);
        }

        public static List<Dictionary<string, object>> SelectByFilter(string resource, List<KeyValuePair<string, StringValues>> filters)
        {
            string q = "SELECT * FROM " + resource + " WHERE ";            

            // Add markers
            var pars = new  List<string>();
            int i = 0;
            foreach (var item in filters)
                pars.Add(item.Key + "= @p" + (++i).ToString());
                //pars.Add("@p" + (++i).ToString() + "= @p" + (++i).ToString());

            // Create where string
            q = q + string.Join(" and ", pars);

            // create command
            NpgsqlCommand cmd = new NpgsqlCommand(q, conn);

            // add params
            i = 0;
            foreach (var item in filters)
            {
                //cmd.Parameters.AddWithValue("@p" + (++i).ToString(), item.Key );
                cmd.Parameters.AddWithValue("@p" + (++i).ToString(), item.Value.ToString() );
            }            

            return SelectFromDB(cmd);
        }

        public static int Insert(string table, DataModel model)
        {
            NpgsqlCommand cmd = BuildQuery(QueryType.Insert, table, GetModelData(model));

            int insertID;
            try
            {
                conn.Open();
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

        public static void Update(string table, string where, DataModel model)
        {
            NpgsqlCommand cmd =  BuildQuery(QueryType.Update, table, GetModelData(model), where);
            
            try
            {
                conn.Open();
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

        public static void Delete(string table, string where)
        {
            string q = "DELETE FROM " + table + " WHERE " + where;
            try
            {
                conn.Open();
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
