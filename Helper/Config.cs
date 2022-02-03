using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Helper
{
    class Config
    {

        private static string type = "None";
        private static string name = "None";
        private static string prof = "None";
        private static string server = "127.0.0.1";


        private static XmlDocument doc = new XmlDocument();
        static public void Load(string file)
        {
            XmlAttribute attr;
            doc.Load(file);
            if (doc == null)
            {
                return;
            }

            XmlNode config = doc.FirstChild;
            if (config != null)
            {
                type = ((XmlElement)config).GetAttribute("type");
                name = ((XmlElement)config).GetAttribute("name");
                prof = ((XmlElement)config).GetAttribute("prof");
                server = ((XmlElement)config).GetAttribute("server");
            }

        }

        static public string GetType()
        {
            return type;
        }
        static public string GetProf()
        {
            return prof;
        }
        static public string GetName()
        {
            return name;
        }
        static public string GetServer()
        {
            return server;
        }
    }
}
