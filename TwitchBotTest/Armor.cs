using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBotTest
{
    class Armor : Item
    {

        public float defense;

        public Armor(int id, string name, float def, int buy, int sell, string[] options, int stackSize) : base(id, name, buy, sell, options, stackSize)
        {
            defense = def;
        }

        public Armor Copy()
        {
            return new Armor(ID, Name, defense, BuyPrice, SellPrice, nameOptions, StackSize);
        }
    }
}
