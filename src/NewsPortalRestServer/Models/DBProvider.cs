using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.Models
{
    public class DBProvider
    {
        private NpgsqlConnection conn;
        public DBProvider()
        {

        }

        public void Connect(string server, string port, string user, string pass, string database)
        {           
            conn = new NpgsqlConnection("Server="+server+";Port="+port+";User Id="+user+";Password="+pass+";Database="+database+";");
            conn.Open();
        }

        public List<Dictionary<string, object>> Select(string q)
        {
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();
            NpgsqlDataReader dr;
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(q, conn);
                dr = command.ExecuteReader();
            }
            catch
            {
                conn.Close();
                return res;
            }   

            while (dr.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int i = 0; i < dr.FieldCount; i++)
                    row.Add(dr.GetName(i), dr[i]);
                res.Add(row);
            }
            conn.Close();
            return res;
        }

        public int Insert(string table, Dictionary<string, string> data)
        {
            string q = "INSERT INTO " + table + " (";

            string keys = "", values = "";
            foreach (var item in data)
            {
                keys += item.Key + ",";
                values += "'" + item.Value + "',";
            }

            keys = keys.Substring(0, keys.Length - 1);
            values = values.Substring(0, values.Length - 1);

            q = q + keys + ") VALUES (" + values + "); SELECT currval('"+ table + "_id_seq');";

            int insertID;
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(q, conn);
                NpgsqlDataReader dr = command.ExecuteReader();
                dr.Read();
                insertID = Convert.ToInt32(dr[0]);
            }
            catch
            {
                return 0;
            }
            finally
            {
                conn.Close();
            }

            return insertID;
        }

        public string Update(string table, string where, Dictionary<string, string> data)
        {
            string values = "";
            foreach (var col in data)            
                values += col.Key+"='"+col.Value+"',";
            values = values.Substring(0, values.Length - 1);

            string q = "UPDATE " + table + " SET " + values + " WHERE " + where;

            try
            {
                NpgsqlCommand command = new NpgsqlCommand(q, conn);
                NpgsqlDataReader dr = command.ExecuteReader();
            }
            catch (NpgsqlException ex)
            {
                return ex.ToString();
            }
            finally
            {
                conn.Close();
            }
            return "ok";
        }

        public string Delete(string table, string where)
        {
            string q = "DELETE FROM " + table + " WHERE " + where;
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(q, conn);
                NpgsqlDataReader dr = command.ExecuteReader();
            }
            catch(NpgsqlException ex)
            {
                return ex.ToString();
            }
            finally
            {
                conn.Close();
            }
            return "ok";                       
        }
    }
}
