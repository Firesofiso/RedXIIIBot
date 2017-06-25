using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBotTest
{
    class Potion : Item
    {

        public int value;

        public Potion(int id, string name, int buy, int sell, string[] options, int heal, int stackSize) : base(id, name, buy, sell, options, stackSize)
        {
            value = heal;
        }

        public Potion Copy()
        {
            return new Potion(ID, Name, BuyPrice, SellPrice, nameOptions, value, StackSize);
        }
    }
}
