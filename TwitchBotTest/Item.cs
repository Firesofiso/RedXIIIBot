using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBotTest
{
    class Item
    {

        private int id;
        private string name;
        public string[] nameOptions;

        private int buyPrice, sellPrice;
        private int count, stackSize;

        public int ID
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int BuyPrice
        {
            get { return buyPrice; }
        }

        public int SellPrice
        {
            get { return sellPrice; }
        }

        public int Count
        {
            get { return count; } 
            set { count = value; }
        }

        public int StackSize
        {
            get { return stackSize; }
        }

        public bool CompareName(string name)
        {
            for (int i = 0; i < nameOptions.Length; i++)
            {
                if (name.ToLower() == nameOptions[i])
                {
                    return true;
                }
            }
            return false;
        }

        public Item(int _id, string _name, int _buyPrice, int _sellPrice, string[] _nameOptions, int _stackSize)
        {
            id = _id;
            nameOptions = _nameOptions;
            name = _name;
            buyPrice = _buyPrice;
            sellPrice = _sellPrice;
            count = 1;
            stackSize = _stackSize;
        }

    }
}
