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

        private static Types.Config config;
        private static Types.Conditions conditions;
        
        private static List<Types.Action> preattack = new List<Types.Action>();
        private static List<Types.Action> preattackfail = new List<Types.Action>();
        private static List<Types.Action> attack = new List<Types.Action>();
        private static List<Types.Action> postattack = new List<Types.Action>();
        private static List<Types.Action> buffs = new List<Types.Action>();
        private static List<Types.Action> actions = new List<Types.Action>();
        private static List<Types.Support> supports = new List<Types.Support>();
        private static List<Types.Action> selfs = new List<Types.Action>();

        private static XmlDocument doc = new XmlDocument();
        static public void Load(string file)
        {
            XmlAttribute attr;
            doc.Load(file);
            if (doc == null)
            {
                return;
            }
            XmlElement root = doc.DocumentElement;
            if (root == null)
            {
                return;
            }

            XmlNodeList configList = root.GetElementsByTagName("config");
            foreach (XmlNode xmlNode in configList)
            {
                config.type = GetString((XmlElement)xmlNode, "type", "None");
                config.name = GetString((XmlElement)xmlNode, "name", "None");
                config.prof = GetString((XmlElement)xmlNode, "prof", "None");
                config.server = GetString((XmlElement)xmlNode, "server", "127.0.0.1");
                config.lang = GetString((XmlElement)xmlNode, "lang", "eng");
                config.party = GetBool((XmlElement)xmlNode, "party", true);
                break;
            }

            XmlNodeList conditionsList = root.GetElementsByTagName("conditions");
            foreach (XmlNode xmlNode in conditionsList)
            {
                conditions.myhp = GetInt((XmlElement)xmlNode, "myhp", 50);
                conditions.partyhp = GetInt((XmlElement)xmlNode, "partyhp", 50);
                conditions.pethp = GetInt((XmlElement)xmlNode, "pethp", 50);
                conditions.healwait = GetInt((XmlElement)xmlNode, "healwait", 15000);
                conditions.maxtime = GetInt((XmlElement)xmlNode, "maxtime", 30000);
                break;
            }

            preattack = ParseActions(root, "preattack");
            preattackfail = ParseActions(root, "preattackfail");
            attack = ParseActions(root, "attack");
            postattack = ParseActions(root, "postattack");
            buffs = ParseActions(root, "buffs");
            actions = ParseActions(root, "actions");
            selfs = ParseActions(root, "selfs");
            supports = ParseSupport(root);
        }

        private static int GetInt(XmlElement xmlNode, string name, int valueDef) 
        {
            if (xmlNode.HasAttribute(name) == true)
            {
                try
                {
                    return Int32.Parse(xmlNode.GetAttribute(name));
                }
                catch (Exception ex)
                {

                }
            }

            return valueDef;
        }

        private static string GetString(XmlElement xmlNode, string name, string valueDef)
        {
            if (xmlNode.HasAttribute(name) == true)
            {
                return xmlNode.GetAttribute(name);
            }

            return valueDef;
        }

        private static bool GetBool(XmlElement xmlNode, string name, bool valueDef)
        {
            if (xmlNode.HasAttribute(name) == true)
            {
                return Boolean.Parse(((XmlElement)xmlNode).GetAttribute(name));
            }

            return valueDef;
        }

        private static List<Types.Action> ParseActions(XmlElement root, string name)
        {
            List<Types.Action> actions = new List<Types.Action>();

            XmlNodeList xmlNodeList = root.GetElementsByTagName(name);
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                XmlNodeList xmlNodeList2 = ((XmlElement)xmlNode).GetElementsByTagName("action");
                foreach (XmlNode actionNode in xmlNodeList2)
                {
                    Types.Action action = new Types.Action();

                    action.key = GetString((XmlElement)actionNode, "key", "");
                    action.name = GetString((XmlElement)actionNode, "name", "");
                    action.delay = GetInt((XmlElement)actionNode, "delay", 100);
                    action.hp = GetInt((XmlElement)actionNode, "hp", -1);
                    action.trigger = GetBool((XmlElement)actionNode, "trigger", false);


                    actions.Add(action);
                }
                break;
            }
            return actions;
        }

        private static List<Types.Support> ParseSupport(XmlElement root)
        {
            List<Types.Support> supports = new List<Types.Support>();
            XmlNodeList xmlNodeList = root.GetElementsByTagName("supports");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                XmlNodeList xmlNodeList2 = ((XmlElement)xmlNode).GetElementsByTagName("support");
                foreach (XmlNode actionNode in xmlNodeList2)
                {
                    Types.Support support = new Types.Support();
                    support.name = GetString((XmlElement)actionNode, "name", "");
                    supports.Add(support);
                }
                break;
            }
            return supports;
        }

        static public Types.Config GetConfig()
        {
            return config;
        }

        static public Types.Action GetAction(string name)
        {
            foreach (Types.Action action in actions)
            {
                if (name == action.name)
                {
                    return action;
                }
            }

            return new Types.Action();
        }

        static public Types.Action GetBuff(string name)
        {
            foreach (Types.Action action in buffs)
            {
                if (name == action.name)
                {
                    return action;
                }
            }

            return new Types.Action();
        }

        public static Types.Conditions GetConditions()
        {
            return conditions;
        }
        public static List<Types.Action> GetPreActions()
        {
            return preattack;
        }

        public static List<Types.Action> GetPreFailActions()
        {
            return preattackfail;
        }

        public static List<Types.Action> GetAttackActions()
        {
            return attack;
        }

        public static List<Types.Action> GetPostActions()
        {
            return postattack;
        }

        public static List<Types.Action> GetSelfs()
        {
            return selfs;
        }
        public static List<Types.Support> GetSupports()
        {
            return supports;
        }

    }
}
