using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Coteminas_Web_Extranet.Models;
using Coteminas_Web_Extranet.Data;
using Coteminas_Web_Extranet.Controllers;

namespace Coteminas_Web_Extranet
{
    public class DbAccess
    {
        private readonly IConfiguration _config;
        private readonly string conn;
        private readonly string conn2;
        private readonly string esquema = "dbo.";
        public DbAccess(IConfiguration config)
        {
            _config = config;
            conn = _config.GetConnectionString("Default");
            conn2 = _config.GetConnectionString("Prod");
        }

        public async Task<List<ProfilesData>> Autenticar(ProfilesData u)
        {
            using DbContext dbContext = new DbContext(conn);
            using Parameters p = new Parameters();

            p.Add("Usuario", u.IdLog);
            p.Add("Contraseña", u.Clave);

            List<ProfilesData> auth = await dbContext.GetList<ProfilesData>(esquema + "UsuariosAutenticar", parameters: p.Get);

            return auth;
        }

        public async Task<List<ClienteDireccionData>> GetDirecciones(string order)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters p = new Parameters();

            p.Add("ORDERKEY", order);

            List<ClienteDireccionData> list = await dbContext.GetList<ClienteDireccionData>(esquema + "OrderExtension_GetDirecciones", parameters: p.Get);

            return list;
        }

        public async Task<List<TSDocs>> OrderExtensionGetPdfFromTsDocs(string order)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters p = new Parameters();

            p.Add("ORDERKEY", order);

            List<TSDocs> list = await dbContext.GetList<TSDocs>(esquema + "OrderExtensionGetPdfFromTsDocs", parameters: p.Get);

            return list;
        }

        public async Task<List<Stats>> GetEstadisticas()
        {
            using DbContext dbContext = new DbContext(conn);

            List<Stats> list = await dbContext.GetList<Stats>(esquema + "GetStats");

            return list;
        }

        public async Task<List<StatsOBS>> GetEstadisticasOBS()
        {
            using DbContext dbContext = new DbContext(conn);

            List<StatsOBS> list = await dbContext.GetList<StatsOBS>(esquema + "GetStatsOBS");

            return list;
        }

        public async Task<List<StatsVOL>> GetEstadisticasVOL(DateTime desde, DateTime hasta,string est)
        {
            using DbContext dbContext = new DbContext(conn);
            using Parameters p = new Parameters();

            p.Add("DESDE", desde);
            p.Add("HASTA", hasta);
            p.Add("ESTADO", est);

            List<StatsVOL> list = await dbContext.GetList<StatsVOL>(esquema + "GetStatsVOL", parameters: p.Get);
            return list;
        }

        public async Task<List<OrderExtensionStatic>> GetOES(bool activos, int year)
        {
            using DbContext dbContext = new DbContext(conn);
            using Parameters p = new Parameters();

            p.Add("activos", activos);
            p.Add("year", year);

            List<OrderExtensionStatic> list = await dbContext.GetList<OrderExtensionStatic>(esquema + "GetOES", parameters: p.Get);

            if (list.Count == 0)
            {
                return null;
            }
            else
            {
                return list;
            }

        }

        public async Task<List<OrderExtensionStatic>> GetRangoFechaOES(DateTime desde, DateTime hasta)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters p = new Parameters();

            p.Add("desde", desde);
            p.Add("hasta", hasta);

            List<OrderExtensionStatic> list = await dbContext.GetList<OrderExtensionStatic>(esquema + "GetRangoFechaOES", parameters: p.Get);

            return list;
        }

        public async Task LiberarComercial(string order, string estado, string user)
        {
            using DbContext dbContext = new DbContext(conn2);//TESTEO
            using Parameters p = new Parameters();

            p.Add("ORDERKEY", order);
            p.Add("ESPC", estado);

            dbContext.Execute("LiberacionComercial", parameters: p.Get);

            LogController ctrlLog = new LogController(conn2);
            string descr = (estado == "N") ? "Liberacion" : "Anulacion de Liberacion";
            await ctrlLog.Save(new LogData { Item = order, Descr = descr + " Comercial", CR = DateTime.Now, CR_User = user, TS = DateTime.Now, TS_User = user });
        }


        public async Task LiberarLogistica(string order, string estado, string user)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters p = new Parameters();

            p.Add("ORDERKEY", order);
            p.Add("ESPL", estado);

            dbContext.Execute("LiberacionLogistica", parameters: p.Get);

            LogController ctrlLog = new LogController(conn2);
            string descr = (estado == "N") ? "Liberacion" : "Anulacion de Liberacion";
            await ctrlLog.Save(new LogData { Item = order, Descr = descr + " Logistica", CR = DateTime.Now, CR_User = user, TS = DateTime.Now, TS_User = user });
        }

        public async Task<List<LogData>> Select(LogData obj)
        {
            using DbContext dbContext = new DbContext(conn2);
            Parameters prm = BuildParametersLog(obj);

            List<LogData> list = await dbContext.GetList<LogData>(esquema + "Log_Select", parameters: prm.Get);

            return list;
        }
        public async Task OrderExtensionUpdateDireccion(string order, string storer, string user)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters p = new Parameters();

            p.Add("ORDERKEY", order);
            p.Add("STORERKEY", storer);
            p.Add("TS_USER", user);

            dbContext.Execute(esquema + "OrderExtension_UpdateDirecciones", parameters: p.Get);

            LogController log = new LogController(conn2);
            await log.Save(new LogData { Item = order, Descr = "Cambio en Direccion del Remito", CR = DateTime.Now, CR_User = user, TS = DateTime.Now, TS_User = user });
        }

        public async Task<ScalarData> OrderExtensionReservar(string order, string user)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters p = new Parameters();

            var x = await Reservar(order);

            LogController log = new LogController(conn2);
            await log.Save(new LogData { Item = order, Descr = "Reserva manual de Stock", CR = DateTime.Now, CR_User = user, TS = DateTime.Now, TS_User = user });

            return x;
        }

        public async Task OrderExtensionLiberar(string order, string user)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters prm = new Parameters();

            prm.Add("ORDERKEY", order);

            dbContext.Execute(esquema + "OrderExtension_Liberar", parameters: prm.Get);

            //sync static table
            SyncOrderExtensionStatic(order);

            LogController log = new LogController(conn2);
            await log.Save(new LogData { Item = order, Descr = "Liberacion manual de Stock", CR = DateTime.Now, CR_User = user, TS = DateTime.Now, TS_User = user });
        }

        public async Task OrderExtensionAnular(string order, string user)
        {
            using DbContext dbContext = new DbContext(conn2);//TESTEO ver
            using Parameters prm = new Parameters();

            prm.Add("ORDERKEY", order);

            dbContext.Execute(esquema + "OrderExtension_Anular", parameters: prm.Get);

            //sync static table
            SyncOrderExtensionStatic(order);

            LogController log = new LogController(conn2);
            await log.Save(new LogData { Item = order, Descr = "Anulacion de Remito", CR = DateTime.Now, CR_User = user, TS = DateTime.Now, TS_User = user });
        }

        public async Task<OrderExtensionStatic> SaveOES(OrderExtensionStatic oes)
        {
            using DbContext dbContext = new DbContext(conn);
            using Parameters p = BuildParameters(oes);

            List<OrderExtensionStatic> list = await dbContext.GetList<OrderExtensionStatic>(esquema + "OrderExtension_Save", parameters: p.Get);
            //sync static table
            SyncOrderExtensionStatic(oes.NOPED);

            return list.Count > 0 ? list.First() : null;
        }

        public async Task<List<OrderDetailData>> GetODD(string o)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters p = new Parameters();

            p.Add("ORDERKEY", o);

            List<OrderDetailData> list = await dbContext.GetList<OrderDetailData>(esquema + "OrderDetail_Select", parameters: p.Get);

            return list;
        }

        public async Task<List<ProfilesData>> GetProfile(string u)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters p = new Parameters();

            p.Add("IdLog", u);

            List<ProfilesData> auth = await dbContext.GetList<ProfilesData>(esquema + "Perfiles_Get", parameters: p.Get);

            return auth;
        }

        public async Task<List<SkuStockData>> GetSSD()
        {
            using DbContext dbContext = new DbContext(conn2);

            List<SkuStockData> list = await dbContext.GetList<SkuStockData>(esquema + "SkuStock_Select");

            return list;
        }

        public async Task<List<SkuStockData>> GetSOS(string sku)
        {
            using DbContext dbContext = new DbContext(conn2);
            using Parameters p = new Parameters();

            p.Add("SKU", sku);

            List<SkuStockData> list = await dbContext.GetList<SkuStockData>(esquema + "SkuOrder_Select", parameters: p.Get);

            return list;
        }

        public async Task<List<RDResumeData>> GetRD()
        {
            using DbContext dbContext = new DbContext(conn2);

            List<RDResumeData> list = await dbContext.GetList<RDResumeData>(esquema + "RDResume_Select");

            return list;
        }

        public async Task<ColumnasOES> GetColumnasOES(string s)
        {
            using DbContext dbContext = new DbContext(conn);
            using Parameters p = new Parameters();

            p.Add("U", s);

            List<ColumnasOES> list = await dbContext.GetList<ColumnasOES>(esquema + "GetColumnasOES", parameters: p.Get);

            return list[0];
        }

        public void AutoSync()
        {
            using DbContext dbContext = new DbContext(conn2);

            dbContext.Execute("wmwhse1.sp_auto_reserva_pedidos");

            //sync static table
            SyncOrderExtensionStatic();
        }

        public async Task<ScalarData> Reservar(string order)
        {
            using DbContext dbContext = new DbContext(conn2);
            Parameters prm = new Parameters();
            prm.Add("ORDERKEY", order);

            List<ScalarData> dataToReturn = await dbContext.GetList<ScalarData>(esquema + "OrderExtension_Reservar", parameters: prm.Get);

            //sync static table
            SyncOrderExtensionStatic(order);

            return dataToReturn.First();
        }

        public void SyncOrderExtensionStatic(string orderkey = "")
        {
            using DbContext dbContext = new DbContext(conn2);

            Parameters prm = new Parameters();

            if (!string.IsNullOrEmpty(orderkey))
            { prm.Add("NOPED", orderkey); }

            try
            {
                dbContext.Execute("sp_sync_OrderExtensionStatic", parameters: prm.Get);
            }
            catch
            {

            }
        }

        public async Task<DataTable> GetStockHabilitado()
        {
            using DbContext dbContext = new DbContext(conn2);

            return await dbContext.GetData("sp_getOrderDetails_Habilitado", CommandType.StoredProcedure, null, 0);
        }

        public async Task<string> SetColumnasOES(ColumnasOES coes, string user)
        {
            using DbContext dbContext = new DbContext(conn);
            using Parameters p = new Parameters();

            p.Add("U", user);
            p.Add("TDOC", coes.TDOC);
            p.Add("LETRA", coes.LETRA);
            p.Add("SUC", coes.SUC);
            p.Add("NOPED", coes.NOPED);
            p.Add("NOFACT", coes.NOFACT);
            p.Add("NOOC", coes.NOOC);
            p.Add("DEST", coes.DEST);
            p.Add("ESPC", coes.ESPC);
            p.Add("ESPL", coes.ESPL);
            p.Add("NOEST", coes.NOEST);
            p.Add("EST", coes.EST);
            p.Add("BLT", coes.BLT);
            p.Add("UNI", coes.UNI);
            p.Add("VOL", coes.VOL);
            p.Add("CLI", coes.CLI);
            p.Add("DIRE", coes.DIRE);
            p.Add("CIU", coes.CIU);
            p.Add("LOC", coes.LOC);
            p.Add("CP", coes.CP);
            p.Add("FECDOC", coes.FECDOC);
            p.Add("FECING", coes.FECING);
            p.Add("FECDISP", coes.FECDISP);
            p.Add("FECDIFER", coes.FECDIFER);
            p.Add("FECLIBCOM", coes.FECLIBCOM);
            p.Add("FECLIBLOG", coes.FECLIBLOG);
            p.Add("FECINICOMP", coes.FECINICOMP);
            p.Add("FECPREVENT", coes.FECPREVENT);
            p.Add("FECCONFMOB", coes.FECCONFMOB);
            p.Add("FECDIFERMOB", coes.FECDIFERMOB);
            p.Add("FECCONFENT", coes.FECCONFENT);
            p.Add("FECENT", coes.FECENT);
            p.Add("FECRECH", coes.FECRECH);
            p.Add("OBS", coes.OBS);
            p.Add("RPTE", coes.RPTE);
            p.Add("PRIOR", coes.PRIOR);
            p.Add("CR_User", coes.CR_User);
            p.Add("TS_User", coes.TS_User);
            p.Add("C_Valor", coes.C_Valor);
            p.Add("C_Rendido", coes.C_Rendido);
            p.Add("C_Stock", coes.C_STOCK);

            DataTable list = await dbContext.GetData(esquema + "SetColumnasOES", CommandType.StoredProcedure, parameters: p.Get, 0);

            return list.Rows[0][0].ToString();
        }

        private Parameters BuildParameters(OrderExtensionStatic obj)
        {
            Parameters parameters = new Parameters();

            foreach (var prop in obj.GetType().GetProperties())
            {
                string name = prop.Name;
                object value = prop.GetValue(obj, null);

                if (name.StartsWith("C_")) continue;
                if (name.StartsWith("_")) continue;

                if (value != null && value.GetType() == Type.GetType("System.DateTime") && (DateTime)value == DateTime.MinValue)
                { parameters.Add(prop.Name, null); }
                else
                { parameters.Add(prop.Name, value); }
            }

            return parameters;
        }

        private Parameters BuildParametersLog(LogData obj)
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


    }
}