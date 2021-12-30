using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace dropf.Codes
{
    internal class CheckUpdate
    {
        private bool _newVersionFound;
        public bool NewVersionFound { get { return _newVersionFound; } }

        private string _lastVersion;
        public string LastVersion { get { return _lastVersion; } }

        private string _downloadLink;
        public string DownloadLink { get { return _downloadLink; } }

        private string _whatsNew;
        public string WhatsNew { get { return _whatsNew; } }

        public CheckUpdate()
        {
            XDocument xd = XDocument.Parse(Common.DoRequest("http://dropf.com/check-update?lcid=" + dropf.Properties.Settings.Default.Language));
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Version ver = assembly.GetName().Version;

            var lMajor = Convert.ToInt16(xd.Root.Element("Version").Attribute("Major").Value);
            var lMinor = Convert.ToInt16(xd.Root.Element("Version").Attribute("Minor").Value);
            var lBuild = Convert.ToInt16(xd.Root.Element("Version").Attribute("Build").Value);

            if (lMajor > ver.Major)
                _newVersionFound = true;
            else if (lMajor == ver.Major && lMinor > ver.Minor)
                _newVersionFound = true;
            else if (lMajor == ver.Major && lMinor == ver.Minor && lBuild > ver.Build)
                _newVersionFound = true;

            _lastVersion = lMajor + "." + lMinor + "." + lBuild;
            _downloadLink = xd.Root.Element("Download").Value.Trim();
            _whatsNew = xd.Root.Element("WhatsNew").Value.Trim();
        }
    }
}
