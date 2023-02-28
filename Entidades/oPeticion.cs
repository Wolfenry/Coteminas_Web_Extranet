using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coteminas_Web_Extranet.Entidades
{
    public class oPeticion
    {      
        public Int64 IdPeticion { get; set; }
        public string IdApp { get; set; }
        public string Cuenta { get; set; }
        public string Contraseña { get; set; }
        public string Token { get; set; }
        public string UrlRetornoPublica { get; set; }
        public string UrlRetornoPrivada { get; set; }
        public string TipoAutenticacion { get; set; }
    }
}
