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
            MemberInfo = 1,
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
