using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBotTest
{
    class Weapon : Item
    {

        public float attack;

        public Weapon(int id, string name, float atk, int buy, int sell, string[] options, int stackSize) : base(id, name, buy, sell, options, stackSize)
        {
            attack = atk;
        }

        public Weapon Copy()
        {
            return new Weapon(ID, Name, attack, BuyPrice, SellPrice, nameOptions, StackSize);
        }
    }
}
