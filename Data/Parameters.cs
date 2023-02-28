using System;
using System.Collections.Generic;
using System.Data;

namespace Coteminas_Web_Extranet.Data
{
    public class Parameters : IDisposable
    {
        private Source _type;
        private Dictionary<string, object> _collection = new Dictionary<string, object>();

        public Parameters(Source type = Source.SQL)
        {
            _type = type;
        }

        public void Add(string name, object value)
        {
            _collection.Add("@" + name, value);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public void Dispose()
        {
            _collection = null;
        }

        public IDbDataParameter[] Get
        {
            get
            {
                List<IDbDataParameter> parameters = new List<IDbDataParameter>();

                foreach (var item in _collection)
                {
                    parameters.Add(DbObjectFactory.Parameter(item.Key, item.Value, _type));
                }

                return parameters.ToArray();
            }
        }
    }
}
