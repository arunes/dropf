using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dropf.Codes
{
    public class FtpItem
    {
        public static FtpItem Create(string name, string host)
        {
            FtpItem ftpItem = new FtpItem();
            ftpItem.Name = name;
            ftpItem.Host = host;

            return ftpItem;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _host;
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }
    }
}
