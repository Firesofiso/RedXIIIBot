using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBotTest
{
    class Enemy
    {

        /*
         * Level?
         * Health / DPS / Defense
         * Name
         * 
         * Loot - an array of items.  Maybe a second array with corresponding droprates?
         *
         * 
         * 
         */

        private string name;
        public int lvl;
        private float[] health = new float[2];
        private float dps, defense;
        public float crit, evasion;
        public char[] statCurves;

        private int[] gilDropped;
        private int expRewarded;

        public string Name
        {
            get { return name; }
        }

        public float MaxHealth
        {
            get { return health[1]; }
            set { health[1] = value; }
        }

        public float CurrentHealth
        {
            get { return health[0]; }
            set { health[0] = value; }
        }

        public float DPS
        {
            get { return dps; }
            set { dps = value; }
        }

        public float Defense
        {
            get { return defense; }
            set { defense = value; }
        }

        public int GilDropped
        {
            get {
                Random rnd = new Random();
                return rnd.Next(gilDropped[0], gilDropped[1]);
            }
        }
        
        public int Exp
        {
            get { return expRewarded; }
        }

        public Enemy(string _name, char[] _statCurves, float critChance, float eva, int[] gil, int exp)
        {
            name = _name;
            lvl = 1;
            statCurves = _statCurves;
            dps = ChatBot.curveChars[statCurves[0]];
            defense = ChatBot.curveChars[statCurves[1]];
            health[1] = 15 + ChatBot.curveChars[statCurves[2]] * 5;
            health[0] = health[1];
            crit = critChance;
            evasion = eva;
            gilDropped = gil;
            expRewarded = exp;
        }

        public Enemy GrowEnemy(int level)
        {
            Enemy newBorn = new Enemy(name, statCurves, crit, evasion, gilDropped, expRewarded);
            Console.WriteLine("Name: " + newBorn.name + " Level: " + newBorn.lvl + " Health: " + newBorn.health[0] + "/" + newBorn.health[1] + " DPS: " + newBorn.dps + " DEF: " + newBorn.defense);
            newBorn.lvl = level;
            for (int i = 1; i < level; i++)
            {
                newBorn.dps += ChatBot.curveChars[statCurves[0]];
                newBorn.defense += ChatBot.curveChars[statCurves[1]];
                newBorn.health[1] += ChatBot.curveChars[statCurves[2]] * 5;
                newBorn.health[0] = newBorn.health[1];
            }
            InflatGilExp(level);
            Console.WriteLine("Name: " + newBorn.name + " Level: " + newBorn.lvl + " Health: " + newBorn.health[0] + "/" + newBorn.health[1] + " DPS: " + newBorn.dps + " DEF: " + newBorn.defense);
            return newBorn;
        }

        public void InflatGilExp(int l)
        {
            if (l > 10 && l <= 20)
            {
                gilDropped[0] += 10;
                gilDropped[1] += 10;
                expRewarded += 25;
            } else if(l > 20 && l <= 30)
            {
                gilDropped[0] += 15;
                gilDropped[1] += 15;
                expRewarded += 50;
            } else if (l > 30 && l <= 40)
            {
                gilDropped[0] += 20;
                gilDropped[1] += 20;
                expRewarded += 100;
            } else if (l > 40 && l <= 50)
            {
                gilDropped[0] += 30;
                gilDropped[1] += 30;
                expRewarded += 200;
            } else if (l > 50 && l <=  60)
            {
                gilDropped[0] += 50;
                gilDropped[1] += 50;
                expRewarded += 300;
            } else if (l > 60)
            {
                gilDropped[0] += 80;
                gilDropped[1] += 80;
                expRewarded += 350;
            }
        }

    }
}
