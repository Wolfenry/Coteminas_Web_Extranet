using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Coteminas_Web_Extranet.Models;
using System.Threading.Tasks;
using Coteminas_Web_Extranet.Data;

namespace Coteminas_Web_Extranet.Controllers
{
    public class LogController
    {
       readonly DbContext dbContext;
        private readonly string esquema = "dbo.";

        public LogController(string connstring)
        {
            dbContext = new DbContext(connstring);
        }

        #region PUBLIC METHODS

        public async Task<List<LogData>> Select(LogData obj, bool or = false, bool like = true)
        {
            Parameters prm = BuildParameters(obj);
            prm.Add("OR", or);
            prm.Add("LIKE", like);

            return await dbContext.GetList<LogData>(esquema + "Log_Select", parameters: prm.Get);
        }

        public async  Task<LogData> Save(LogData obj)
        {
            Parameters prm = BuildParameters(obj);

            var result = await dbContext.GetList<LogData>(esquema + "Log_Save", parameters: prm.Get);

            return result.Count > 0 ? result.First() : null;
        }

        public void Delete(LogData obj)
        {
            Parameters prm = BuildParameters(obj);

            dbContext.Execute(esquema + "Log_Delete", parameters: prm.Get);
        }

        #endregion

        #region PRIVATE METHODS  

        private Parameters BuildParameters(LogData obj)
        {
            Parameters parameters = new Parameters();

            foreach (var prop in obj.GetType().GetProperties())
            {
                string name = prop.Name;
                object value = prop.GetValue(obj, null);

                if (value != null && value.GetType() == Type.GetType("System.DateTime") && (DateTime)value == DateTime.MinValue)
                { parameters.Add(prop.Name, null); }
                else
                { parameters.Add(prop.Name, value); }
            }

            return parameters;
        }

        #endregion
    }
}
