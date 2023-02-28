using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Coteminas_Web_Extranet.Data
{
    public static class IJSExtensions
    {
        public static ValueTask<object> GuardarComo(this IJSRuntime js, string nombre, byte[] archivo)
        {
            return js.InvokeAsync<object>("saveAsFile", nombre, Convert.ToBase64String(archivo));
        }
    }
}
