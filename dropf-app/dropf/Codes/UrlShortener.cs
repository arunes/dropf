using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace dropf.Codes
{
    class UrlShortener
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private string _service;
        public string Service
        {
            get { return _service; }
            set { _service = value; }
        }

        public UrlShortener(string url, string service)
        {
            _url = url;
            _service = service;
        }

        public string GetShortUrl()
        {
            string retVal = null;
            if (!_url.StartsWith("http://")) _url = "http://" + _url;
            switch (_service)
            { 
                case "2d1":
                    retVal = Common.DoRequest("http://2d1.in/?u=" + System.Web.HttpUtility.UrlEncode(_url) + "&f=1");
                    break;

                case "isgd":
                    retVal = Common.DoRequest("http://is.gd/create.php?format=simple&url=" + System.Web.HttpUtility.UrlEncode(_url));
                    break;

                case "googl":
                    retVal = GoogleUrlShortner.Shorten(_url);
                    break;
            }


            return retVal;
        }
    }
}
