using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    class MemberManager
    {
        private static List<ServerMember> members = new List<ServerMember>();

        public static void Add(ServerMember member)
        {
            members.Add(member);
        }

        public static void Delete(ServerMember member)
        {
            members.Remove(member);
        }

        public static List<Types.MemberInfo> GetMembersInfo()
        {
            List<Types.MemberInfo> result = new List<Types.MemberInfo>();
            Types.Config config = Config.GetConfig();

            Types.MemberInfo leader = new Types.MemberInfo();
            leader.name = config.name;
            leader.prof = config.prof;
            leader.lang = config.lang;
            result.Add(leader);

            foreach (ServerMember member in members)
            {
                Types.MemberInfo info = member.GetMemberInfo();
                if (info.name == "" || info.prof == "")
                {
                    continue;
                }
                result.Add(info);

            }

            return result;
        }

        public static void Buff(Types.Action buff)
        {
            ServerMember member = GetServer(buff.name);
            if (member != null)
            {
                Types.MemberInfo info = member.GetMemberInfo();
                if (info.party == false)
                {
                    Invite(info, member);
                }

                Console.WriteLine("Buffing {0}", buff);
                member.Buff(buff);
                Dismiss(info);
            }
        }

        public static void GroupHeal()
        {
            foreach (ServerMember member in members)
            {
                Types.MemberInfo info = member.GetMemberInfo();
                Console.WriteLine("{0} {1}", info.name, info.prof);
                if (info.prof == "WC" || info.prof == "PP")
                {
                    if (info.party == false)
                    {
                        Invite(info, member);
                    }

                    member.GroupHeal();
                    Dismiss(info);
                    break;
                }
            }
        }

        public static void Support(string name)
        {
            ServerMember member = MemberManager.GetServer(name);
            if (member != null)
            {
                Console.WriteLine("Support {0}", name);
                member.Support();
            }
        }

        private static ServerMember GetServer(Types.MemberInfo member)
        {
            foreach (ServerMember server in members)
            {
                Types.MemberInfo info = server.GetMemberInfo();
                if (info.name == member.name)
                {
                    return server;
                }
            }
            return null;
        }

        private static ServerMember GetServer(string name)
        {
            foreach (ServerMember server in members)
            {
                Types.MemberInfo info = server.GetMemberInfo();
                if (info.name == name)
                {
                    return server;
                }
            }
            return null;
        }

        public static void Invite(Types.MemberInfo memberInfo, ServerMember member = null)
        {
            Console.WriteLine("Invite start");

            if (member == null)
            {
                member = GetServer(memberInfo);
            }

            if (member == null)
            {
                return;
            }

            if (AsteriosManager.OpenWindow() == false)
            {
                return;
            }

            Keyboard.Invite(memberInfo);
            member.Invite();
            Console.WriteLine("Invite stop");
        }

        public static void Dismiss(Types.MemberInfo memberInfo)
        {
            Console.WriteLine("Dismiss start");
            if (memberInfo.party == true)
            {
                return;
            }

            if (AsteriosManager.OpenWindow() == false)
            {
                return;
            }

            Keyboard.Dismiss(memberInfo);
            Console.WriteLine("Dismiss stop");
        }
    }
}
