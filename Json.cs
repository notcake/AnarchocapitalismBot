using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AnarchocapitalismBot
{
    public static class Json
    {
        private static JsonSerializer Serializer = new JsonSerializer();

        public static async Task<T> DeserializeUrl<T>(string url)
        {
            WebRequest httpWebRequest = HttpWebRequest.Create(url);

            using (WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync())
            using (Stream stream = httpWebResponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                return Json.Serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}
