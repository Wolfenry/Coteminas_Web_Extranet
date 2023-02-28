using System.Collections.Generic;

namespace Coteminas_Web_Extranet.Entidades
{
    public class oSession
    {
        public string Error { get; set; }
        public List<oPermiso> Permisos { get; set; }
        public bool Comercial { get; set; }
        public bool Logistico { get; set; }
        public string Username { get; set; }
        public string RPTE { get; set; }
        public List<oAtributo> Atributos { get; set; }
    }
}
