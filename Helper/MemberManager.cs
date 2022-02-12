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

            Types.MemberInfo leader = new Types.MemberInfo();
            leader.name = Config.GetName();
            leader.prof = Config.GetProf();
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

        public static void Buff(dynamic buffs)
        {

            foreach (ServerMember member in members)
            {
                Types.MemberInfo info = member.GetMemberInfo();
                Console.WriteLine("{0} {1}", info.name, info.prof);
                if (info.prof == "WC" || info.prof == "PP")
                {
                    Console.WriteLine("Buffing");
                    member.Buff(buffs);
                    break;
                }
            }
        }

        public static void DC()
        {
            List<string> list = new List<string>() {"all"};
            foreach (ServerMember member in members)
            {
                Types.MemberInfo info = member.GetMemberInfo();
                if (info.prof == "DB" || info.prof == "SWS")
                {
                    Console.WriteLine("Dancing");
                    member.Buff(list);
                }
            }
        }
    }
}
