using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Service
{
    
    public class PostgresConnection
    {
        public static NpgsqlConnection Connection { get; }

        static PostgresConnection()
        {
            string server = "localhost";
            string port = "5432";
            string user = "postgres";
            string pass = "1";
            string database = "ASP";

            Connection = new NpgsqlConnection("Server=" + server +
                                           ";Port=" + port +
                                           ";User Id=" + user +
                                           ";Password=" + pass +
                                           ";Database=" + database +
                                           ";");
            Connection.Open();
        }
    }
}
