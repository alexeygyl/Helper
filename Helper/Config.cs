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

        private static string assist = "None";
        private static string nextAttack = "None";
        private static string leave = "None";
        private static string pick = "None";
        private static string stand = "None";
        private static string follow = "None";

        private static List<Types.Action> preattack = new List<Types.Action>();
        private static List<Types.Action> preattackfail = new List<Types.Action>();
        private static List<Types.Action> attack = new List<Types.Action>();
        private static List<Types.Action> postattack = new List<Types.Action>();
        private static List<Types.Action> buffs = new List<Types.Action>();
        private static List<Types.Action> actions = new List<Types.Action>();

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


            XmlNodeList xmlNodeList = root.GetElementsByTagName("config");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                type = ((XmlElement)xmlNode).GetAttribute("type");
                name = ((XmlElement)xmlNode).GetAttribute("name");
                prof = ((XmlElement)xmlNode).GetAttribute("prof");
                server = ((XmlElement)xmlNode).GetAttribute("server");
                break;
            }


            /*
            xmlNodeList = root.GetElementsByTagName("actions");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                assist = ((XmlElement)xmlNode).GetAttribute("assist");
                nextAttack = ((XmlElement)xmlNode).GetAttribute("nextAttack");
                leave = ((XmlElement)xmlNode).GetAttribute("leave");
                follow = ((XmlElement)xmlNode).GetAttribute("follow");
                pick = ((XmlElement)xmlNode).GetAttribute("pick");
                stand = ((XmlElement)xmlNode).GetAttribute("stand");
                start = ((XmlElement)xmlNode).GetAttribute("start");
                stop = ((XmlElement)xmlNode).GetAttribute("stop");
                break;
            }
            */
            preattack = ParseActions(root, "preattack");
            preattackfail = ParseActions(root, "preattackfail");
            attack = ParseActions(root, "attack");
            postattack = ParseActions(root, "postattack");
            buffs = ParseActions(root, "buffs");
            actions = ParseActions(root, "actions");
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
                    action.delay = 100;
                    action.name = "";
                    action.hp = -1;
                    action.trigger = false;

                    try
                    {
                        action.key = ((XmlElement)actionNode).GetAttribute("key");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                    try
                    {
                        action.name = ((XmlElement)actionNode).GetAttribute("name");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    try
                    {
                        action.delay = Int32.Parse(((XmlElement)actionNode).GetAttribute("delay"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    try
                    {
                        action.hp = Int32.Parse(((XmlElement)actionNode).GetAttribute("hp"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    try
                    {
                        action.trigger = Boolean.Parse(((XmlElement)actionNode).GetAttribute("trigger"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    actions.Add(action);
                }
                break;
            }
            return actions;
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


        static public Types.Action GetAction(string name)
        {
            Types.Action action1 = new Types.Action();
            foreach (Types.Action action in actions)
            {
                if (name == action.name)
                {
                    return action;
                }
            }

            return action1;
        }

        static public Types.Action GetBuff(string name)
        {
            Types.Action action1 = new Types.Action();
            foreach (Types.Action action in buffs)
            {
                if (name == action.name)
                {
                    return action;
                }
            }

            return action1;
        }
    }
}
