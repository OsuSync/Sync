using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Sync.Tools
{
    public static class ProgramUpdater
    {
        private static string UPDATE_XML = "http://d.mcbaka.com/update.xml";
        private static Thread updateT = new Thread(UpdateT);

        public static void Update()
        {

        }

        private static void UpdateT()
        {
            WebClient web = new WebClient();
            string path = AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                IO.CurrentIO.Write("Update: Connecting to server....");
                Stream data = web.OpenRead(UPDATE_XML);
                XmlDocument xml = new XmlDocument();
                IO.CurrentIO.Write("Update: Searching files...");
                xml.Load(data);

            }
        }

        private class UpdateXML
        {

        }

    }
}
