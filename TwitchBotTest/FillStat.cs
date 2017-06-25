using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBotTest
{
    class FillStat
    {

        public int cur, max;
        public int modifier;

        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        public void Fill(int value)
        {
            if (cur + value >= max)
            {
                cur = max;
            }
            else
            {
                cur += value;
            }
        }

        public FillStat(int value)
        {
            max = value;
            cur = max;
            modifier = 0;
        }
    }
}
