using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Coteminas_Web_Extranet.Negocio
{
    public class oConfig
    {

        public static async Task<string> ObtenerVariable(string variable)
        {
            string respuesta = null;

            await Task.Run(() =>
            {
                ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

                string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                configurationBuilder.AddJsonFile(path, false);

                IConfiguration root = configurationBuilder.Build();

                respuesta = root.GetSection("SSO").GetSection(variable).Value;

            });

            return respuesta;
        }

    }
}
