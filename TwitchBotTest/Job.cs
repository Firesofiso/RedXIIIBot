using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBotTest
{
    class Job
    {

        private string name;

        private char[] statCurves;
        private Ability[] abilities;

        public char[] StatCurve
        {
            get { return statCurves; }
        }

        public Ability[] Abilities
        {
            get { return abilities; }
        }

        public Job(string _name, char[] _statCurves, Ability[] _abilities)
        {
            name = _name;
            statCurves = _statCurves;
            abilities = _abilities;
        }
    }
}
