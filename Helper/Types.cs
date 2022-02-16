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

        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public struct StatColor
        {
            public int R;
            public int G;
            public int B;

            public void Set(int R, int G, int B)
            {
                this.R = R;
                this.G = G;
                this.B = B;
            }
        }

        public struct Stat
        {
            public int total;
            public int current;
        }

        public struct Stats
        {
            public Stat cp;
            public Stat hp;
            public Stat mp;
            public int pet;
        }
    }
}
