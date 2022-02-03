using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    class Types
    {
        public enum Action
        {
            Auth = 1,
            MembersInfo,
            Buff
        }

        public struct MemberInfo
        {
            public string name;
            public string prof;
            public double hp;
        }
    }
}
