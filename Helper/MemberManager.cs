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

    }
}
