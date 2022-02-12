using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    class Types
    {
        public enum Actions
        {
            MemberInfo = 1,
            Assist,
            NextAttack,
            Follow,
            Stand,
            Pick,
            Leave,
            Buff,
        }

        public enum Profs
        {
            Buffer,
            DB,
            SWS,
        }

        public struct MemberInfo
        {
            public string name;
            public string prof;
            public double hp;
        }

        public struct Action
        {
            public string key;
            public string name;
            public int delay;
            public int hp;
            public bool trigger;
        }
    }
}
