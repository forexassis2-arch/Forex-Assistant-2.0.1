using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Forex_Assistant.BL
{
    public class NewsAPI
    {
        public string GETNews()
        {
            try
            {
                var url = "https://www.google.com";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                string result = "";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                return result;
            }
            catch
            {
                return null;
            }
        }


        public Root GetNewsAPI()
        {
            try
            {
                string _Res = GETNews();

                _Res = _Res.Replace("_callbacks___jm1j2(", "");



                _Res = _Res.Substring(0, _Res.Length - 2);

                Root LastNews = JsonConvert.DeserializeObject<Root>(_Res);

                return LastNews;
            }
            catch
            {
                return null;
            }
        }

        public class News
        {
            public string id { get; set; }
            public string createdAt { get; set; }
            public Source source { get; set; }
            public string urlStoryDukascopy { get; set; }
            public string urlStoryOriginal { get; set; }
            public string title { get; set; }
            public string excerpt { get; set; }
        }

        public class Result
        {
            public List<News> news { get; set; }
        }

        public class Root
        {
            public string error { get; set; }
            public Result result { get; set; }
        }

        public class Source
        {
            public string title { get; set; }
        }
    }
}
