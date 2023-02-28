using System;
namespace Coteminas_Web_Extranet.Models
{

    [Serializable]
    public class ClienteDireccionData
    {
        public string ID
        { get; set; }
        public string CODIGO
        { get; set; }
        public string CLIENTE
        { get; set; }
        public string ENTREGA
        { get; set; }
        public string DIRECCION
        { get; set; }
        public string CIUDAD
        { get; set; }
    }
}