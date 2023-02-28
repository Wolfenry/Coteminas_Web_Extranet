using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coteminas_Web_Extranet.Entidades;
using Newtonsoft.Json;


namespace Coteminas_Web_Extranet.Negocio
{
    public class SSO
    {
        public async static Task<List<oAtributo>> ObtenerAtributos(oPeticion peticion)
        {
            string bodyJson = JsonConvert.SerializeObject(peticion);

            string url = await oConfig.ObtenerVariable("urlApiSSO") + @"obtener_atributos";

            string respuesta = await RestAPIManager.Post(url, bodyJson);

            List<oAtributo> listaPermisos = JsonConvert.DeserializeObject<List<oAtributo>>(respuesta);

            return listaPermisos;
        }

        public async static Task<List<oPermiso>> ObtenerPermisos(oPeticion peticion)
        {            
            string bodyJson = JsonConvert.SerializeObject(peticion);

            string url = await oConfig.ObtenerVariable("urlApiSSO") + @"obtener_permisos"; 

            string respuesta = await RestAPIManager.Post(url, bodyJson);
            
            List<oPermiso> listaPermisos = JsonConvert.DeserializeObject<List<oPermiso>>(respuesta);

            return listaPermisos;

        }



        public static bool ValidarPermiso(List<oPermiso> ListaPermisos, string Permiso)
        {
            var Consulta = ListaPermisos.Where(x => x.Permiso == Permiso);

            if (Consulta.Count() > 0)
                return true;
            else
                return false;
        }




        public async static Task<bool> ValidarToken(oPeticion peticion)
        {
            string bodyJson = JsonConvert.SerializeObject(peticion);

            string url = await oConfig.ObtenerVariable("urlApiSSO") + @"validar_token";

            string respuesta = await RestAPIManager.Post(url, bodyJson);

            bool TokenValido = JsonConvert.DeserializeObject<bool>(respuesta);

            return TokenValido;
        }





        public async static Task<oPeticion> ObtenerCuentaPorToken(oPeticion peticion)
        {
            string bodyJson = JsonConvert.SerializeObject(peticion);

            string url = await oConfig.ObtenerVariable("urlApiSSO") + @"obtener_cuenta_por_token";

            string respuesta = await RestAPIManager.Post(url, bodyJson);

            peticion = Newtonsoft.Json.JsonConvert.DeserializeObject<oPeticion>(respuesta);

            return peticion;
        }


    }
}
