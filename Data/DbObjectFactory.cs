using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Coteminas_Web_Extranet.Data
{
    public static class DbObjectFactory
    {
        public static IDbCommand Command(Source type = Source.SQL)
        {
            IDbCommand command = null;

            switch (type)
            {
                case Source.SQL:
                    command = new SqlCommand();
                    break;

                case Source.Other:
                    command = new OleDbCommand();
                    break;
            }

            return command;
        }

        public static IDbConnection Connection(string connectionString, Source type = Source.SQL)
        {
            IDbConnection connection = null;

            switch (type)
            {
                case Source.SQL:
                    connection = new SqlConnection(connectionString);
                    break;

                case Source.Other:
                    connection = new OleDbConnection(connectionString);
                    break;
            }

            return connection;
        }

        public static IDbDataParameter Parameter(string name, object value, Source type = Source.SQL)
        {
            IDbDataParameter parameter = null;

            switch (type)
            {
                case Source.SQL:
                    parameter = new SqlParameter(name, value);
                    break;

                case Source.Other:
                    parameter = new OleDbParameter(name, value);
                    break;
            }

            return parameter;
        }
    }
}
