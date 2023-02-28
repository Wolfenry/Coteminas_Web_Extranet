using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Coteminas_Web_Extranet.Negocio
{
    public class RestAPIManager
    {

        public async static Task<string> Post(string url, string bodyJson)
        {
            string api_response = "";
            
            await Task.Run(() =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    //request.Timeout = 20000;


                    using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(bodyJson);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();

                    if (respuesta.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader streamReader = new StreamReader(respuesta.GetResponseStream()))
                        {
                            api_response = streamReader.ReadToEnd();
                        }
                    }
                    else
                    {
                        throw new Exception("Error - " + respuesta.StatusCode.ToString() + " - " + respuesta.StatusDescription);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            return api_response;
        }
    }
}
