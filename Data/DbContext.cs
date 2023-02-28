using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
//using System.Data.SqlClient;

namespace Coteminas_Web_Extranet.Data
{
    public class DbContext : IDisposable
    {
        private const int TIMEOUT = 500;

        private Source _type;
        private string _connectionString = string.Empty;

        #region Constructors

        /// <summary>
        /// Constructor for external shared IDbConnection use.
        /// </summary>
        /// <param name="type"></param>
        public DbContext(Source type = Source.SQL)
        {
            _type = type;
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="type"></param>
        public DbContext(string connectionString, Source type = Source.SQL)
        {
            _connectionString = connectionString;
            _type = type;
        }

        #endregion

        #region Public Methods

        public async Task<T> GetObject<T>(string commandText, CommandType commandType = CommandType.StoredProcedure, IDbDataParameter[] parameters = null, int timeOut = TIMEOUT)
        {
            DataTable dt = await GetData(commandText, commandType, parameters, timeOut);

            return CastToObject<T>(dt);
        }

        /// <summary>
        /// Method for external shared IDbConnection, close connection is requiered outside this method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="dbConnection"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public T GetObject<T>(ref IDbConnection dbConnection, string commandText, CommandType commandType = CommandType.StoredProcedure, IDbDataParameter[] parameters = null, int timeOut = TIMEOUT)
        {
            DataTable dt = GetData(commandText, commandType, parameters, timeOut, ref dbConnection);

            return CastToObject<T>(dt);
        }

        public async Task<List<T>> GetList<T>(string commandText, CommandType commandType = CommandType.StoredProcedure, IDbDataParameter[] parameters = null, int timeOut = TIMEOUT)
        {
            DataTable dt = await GetData(commandText, commandType, parameters, timeOut);

            return TableToList<T>(dt);
        }

        /// <summary>
        /// Method for external shared IDbConnection, close connection is requiered outside this method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="dbConnection"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public List<T> GetList<T>(ref IDbConnection dbConnection, string commandText, CommandType commandType = CommandType.StoredProcedure, IDbDataParameter[] parameters = null, int timeOut = TIMEOUT)
        {
            DataTable dt = GetData(commandText, commandType, parameters, timeOut, ref dbConnection);

            return TableToList<T>(dt);
        }

        public void Execute(string commandText, CommandType commandType = CommandType.StoredProcedure, IDbDataParameter[] parameters = null, int timeOut = TIMEOUT)
        {
            GetData(commandText, commandType, parameters, timeOut);
        }

        /// <summary>
        /// Method for external shared IDbConnection, close connection is requiered outside this method.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="dbConnection"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="timeOut"></param>
        public void Execute(ref IDbConnection dbConnection, string commandText, CommandType commandType = CommandType.StoredProcedure, IDbDataParameter[] parameters = null, int timeOut = TIMEOUT)
        {
            GetData(commandText, commandType, parameters, timeOut, ref dbConnection);
        }

        /// <summary>
        /// Massive Insert based on Table Structure, only enabled for SqlClient.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        public void SqlBulkCopy(DataTable dt, string tableName)
        {
            using (System.Data.SqlClient.SqlBulkCopy sbc = new System.Data.SqlClient.SqlBulkCopy(_connectionString))
            {
                sbc.DestinationTableName = tableName;
                sbc.WriteToServer(dt);
            }
        }

        public void Dispose()
        {
            //Garbage collector clean first IDisposable objects, where Dispose() method is requiered.
        }

        #endregion

        #region Private Methods

        private T CastToObject<T>(DataTable dt)
        {
            var list = TableToList<T>(dt);

            return (list.Count > 0) ? list[0] : Activator.CreateInstance<T>();
        }

        public async Task<DataTable> GetData(string commandText, CommandType commandType, IDbDataParameter[] parameters, int timeOut)
        {
            DataTable dt = new DataTable();

            using (IDbConnection dbConnection = DbObjectFactory.Connection(_connectionString, _type))
            {
                using (IDbCommand cmd = DbObjectFactory.Command(_type))
                {
                    cmd.Connection = dbConnection;
                    cmd.CommandText = commandText;
                    cmd.CommandType = commandType;
                    cmd.CommandTimeout = timeOut;

                    //if (transaction != null)
                    //{ cmd.Transaction = transaction; }

                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        { cmd.Parameters.Add(p); }
                    }

                    if (dbConnection.State != ConnectionState.Open)
                    { dbConnection.Open(); }

                    using (IDataReader dataReader = cmd.ExecuteReader())
                    {
                        dt.Load(dataReader);
                    }
                }
            }
            await Task.Delay(1);
            return dt;
        }

        public DataTable GetData(string commandText, CommandType commandType, IDbDataParameter[] parameters, int timeOut, ref IDbConnection dbConnection)
        {
            DataTable dt = new DataTable();

            using (IDbCommand cmd = DbObjectFactory.Command(_type))
            {
                cmd.Connection = dbConnection;
                cmd.CommandText = commandText;
                cmd.CommandType = commandType;
                cmd.CommandTimeout = timeOut;

                //if (transaction != null)
                //{ cmd.Transaction = transaction; }

                if (parameters != null)
                {
                    foreach (var p in parameters)
                    { cmd.Parameters.Add(p); }
                }

                using (IDataReader dataReader = cmd.ExecuteReader())
                {
                    dt.Load(dataReader);
                }
            }

            return dt;
        }

        private List<T> TableToList<T>(DataTable dt)
        {
            List<T> list = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T instanceObject = Activator.CreateInstance<T>();
                foreach (DataColumn cl in dt.Columns)
                {
                    PropertyInfo pi = typeof(T).GetProperty(cl.ColumnName);

                    if (pi != null && dr[cl] != DBNull.Value)
                    { pi.SetValue(instanceObject, ChangeType(dr[cl], pi.PropertyType), new object[0]); }
                }
                list.Add(instanceObject);
            }
            return list;
        }

        private static object ChangeType(object objectToConvert, Type typeToConvert)
        {
            if (objectToConvert == null) return null;

            if (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                typeToConvert = Nullable.GetUnderlyingType(typeToConvert);
            }

            return Convert.ChangeType(objectToConvert, typeToConvert);
        }

        /*
        private DataTable ToDataTable<T>(IList<T> list)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dt.Columns.Add(property.Name, property.PropertyType);
            }
            object[] values = new object[properties.Count];
            foreach (T item in list)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }
        */

        #endregion
    }
}
