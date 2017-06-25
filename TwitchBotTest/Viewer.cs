using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;

namespace TwitchBotTest
{
    class Viewer : IEquatable<Viewer>
    {
        private string name;
        private int gil, exp, expToLvl, lvl;

        // A currency to bestow global buffs upon the chat.
        public int fayth;

        // I'm thinking the viewer will have an Inventory object to hold and keep track of all their items.
        // the question then becomes how do users use an item?
        private Inventory bag;
        private float[] health = new float[2];
        private float dps, defense;
        public int crit;
        private Job charJob;

        //Adventure stuff
        public bool isAdventuring;
        public bool autoAdventure;
        public Enemy target;

        public int slainEnemies;
        public int totalSlain;
        public int adventureGil;

        public WhisperItemList timer;

        //Stats

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

        //Other shit

        public Job Job
        {
            get { return charJob; }
            set { charJob = value; }
        }

        public Inventory Bag
        {
            get { return bag; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Level
        {
            get { return lvl; }
        }

        public int Gil
        {
            get { return gil; }
            set { gil = value; }
        }

        public int Exp
        {
            get { return exp; }
            set { exp = value; }
        }

        public int ExpNeeded
        {
            get { return expToLvl; }
        }

        public int Tnl()
        {
            return expToLvl - exp;
        }

        public Viewer(string name)
        {
            bag = new Inventory();
            lvl = 1;
            this.name = name;
            this.gil = 0;
            expToLvl = 500;
            health[1] = 30;
            health[0] = health[1];
            dps = 3;
            defense = 5;
            slainEnemies = 0;
            totalSlain = 0;
            crit = 2;
            isAdventuring = false;
            autoAdventure = false;
            //timer = new WhisperItemList(name, bag.List, 3000);
        }

        public Viewer(string name, int level)
        {
            bag = new Inventory();
            lvl = level;
            this.name = name;
            this.gil = 0;
            expToLvl = 500;
            for (int i = 1; i < level; i++) {
                if (level < 10)
                {
                    expToLvl += 100;
                } else if (level <= 20 &&  level > 10) {
                    expToLvl += 150;
                } else if (level <= 30 && level > 20)
                {
                    expToLvl += 200;
                } else if (level <= 40 && level > 30)
                {
                    expToLvl += 250;
                } else if (level <= 50 && level > 40)
                {
                    expToLvl += 350;
                } else if (level <= 60 && level > 50)
                {
                    expToLvl += 500;
                } else if (level > 60)
                {
                    expToLvl += 600;
                }
                
            }

            health[1] = 20 + lvl * 10;
            health[0] = health[1];
            dps = 2 + lvl;
            defense = 5 + ((lvl - 1));
            crit = 2;

            slainEnemies = 0;
            totalSlain = 0;

            isAdventuring = false;
            autoAdventure = false;
            //timer = new WhisperItemList(name, bag.List, 3000);
        }

        public void LevelUp()
        {
            expToLvl += expToLvl / 4;
            lvl++;
            exp = 0;
            health[1] += 10;
            dps += 1;
            defense += 1;
        }

        public void LevelUp(int numTimes)
        {
            for (int i = 0; i < numTimes; i++)
            {
                LevelUp();
            }
        }

        public void AddItem(Item i, int numItems)
        {
            int recievedCount = numItems;
            if (!bag.IsInInventory(i.ID) && bag.Count == bag.Size)
            {
                ChatBot.WriteLineToChat("/w " + name + " Inventory is full.");
            }
            else
            {

                // Create a list of the incoming items?
                // say you are adding 500 potions each at a stack of 99
                // Make 5 full stacks of potions and a partial stack of 5 potions.
                // Then add the full stacks to the inventory (if inventory size allows) and look for a partial stack in the inventory to combine the partial stacks

                //single items will be changed into multiple in stacks.
                i.Count = 1;
                int loopCounter = numItems / i.StackSize;
                if (numItems % i.StackSize != 0)
                {
                    loopCounter = (i.Count / i.StackSize) + 1;
                }


                if (i is Potion)
                {
                    Potion tempItem = (Potion)i;
                    if (bag.IsInInventory(i.ID))
                    {
                        // Item is in the inventory already.  We need to make sure the item count is not at max.  If its at max just make a new item.
                        List<Item> items = bag.GetItemsByID(i.ID);
                        for (int j = 0; j < items.Count; j++)
                        {
                            // i.stacksize - items[j].count = amount to refill
                            
                            if (items[j].Count < tempItem.StackSize)
                            {
                                int fillAmount = tempItem.StackSize - items[j].Count;
                                // If the fill amount is larger than the item count being added, just add the new items count.
                                // else fill the stack and continue pulling items
                                if (fillAmount > numItems)
                                {
                                    // Adding items to a stack so it doesn't add to bagsize
                                    items[j].Count += numItems;
                                    break;
                                }
                                else
                                {
                                    //Fill the partial item stack
                                    items[j].Count = tempItem.StackSize;
                                    numItems -= fillAmount;

                                    //divide the rest into stacks and add them to the inventory
                                    for (int k = 0; k < loopCounter; k++)
                                    {
                                        if (bag.Count == bag.Size)
                                        {
                                            break;
                                        }
                                        Item temp = tempItem.Copy();
                                        temp.Count = temp.StackSize;
                                        bag.AddItem(temp);
                                        numItems -= temp.StackSize;
                                    }

                                    //if there are any items left make a new partial item stack
                                    if (numItems != 0)
                                    {
                                        if (bag.Count == bag.Size)
                                        {
                                            break;
                                        }
                                        Item temp = tempItem.Copy();
                                        temp.Count = numItems;
                                        bag.AddItem(temp);
                                    }
                                    break;
                                }
                            }
                        }
                        if (numItems > 0)
                        {
                            if (i.StackSize > numItems)
                            {
                                Item temp = tempItem.Copy();
                                temp.Count = numItems;
                                bag.AddItem(temp);
                            }
                            else
                            {
                                for (int k = 0; k < loopCounter; k++)
                                {
                                    if (bag.Count == bag.Size)
                                    {
                                        break;
                                    }
                                    Item temp = tempItem.Copy();
                                    temp.Count = temp.StackSize;
                                    bag.AddItem(temp);
                                    numItems -= temp.StackSize;
                                }

                                if (numItems > 0)
                                {
                                    Item temp = tempItem.Copy();
                                    temp.Count = numItems;
                                    bag.AddItem(temp);
                                }
                            }
                        }
                    }

                    //IF the item is not in the inventory
                    else
                    {
                        //divide the rest into stacks and add them to the inventory
                        if (numItems >= tempItem.StackSize)
                        {
                            for (int j = 0; j < loopCounter; j++)
                            {
                                if (bag.Count == bag.Size)
                                {
                                    break;
                                }
                                Item temp = tempItem.Copy();
                                temp.Count = temp.StackSize;
                                bag.AddItem(temp);
                                numItems -= temp.StackSize;
                            }
                        }
                        //if there are any items left make a new partial item stack
                        if (numItems != 0)
                        {
                            if (bag.Count != bag.Size)
                            {
                                Item temp = tempItem.Copy();
                                temp.Count = numItems;
                                bag.AddItem(temp);
                            }
                        } 
                    }
                }
                else if (i is Weapon)
                {
                    Weapon tempItem = (Weapon)i;
                    if (bag.IsInInventory(i.ID))
                    {
                        // Item is in the inventory already.  We need to make sure the item count is not at max.  If its at max just make a new item.
                        List<Item> items = bag.GetItemsByID(i.ID);
                        for (int j = 0; j < items.Count; j++)
                        {
                            // i.stacksize - items[j].count = amount to refill

                            if (items[j].Count < tempItem.StackSize)
                            {
                                int fillAmount = tempItem.StackSize - items[j].Count;
                                // If the fill amount is larger than the item count being added, just add the new items count.
                                // else fill the stack and continue pulling items
                                if (fillAmount > numItems)
                                {
                                    // Adding items to a stack so it doesn't add to bagsize
                                    items[j].Count += numItems;
                                    break;
                                }
                                else
                                {
                                    //Fill the partial item stack
                                    items[j].Count = tempItem.StackSize;
                                    numItems -= fillAmount;

                                    //divide the rest into stacks and add them to the inventory
                                    for (int k = 0; k < loopCounter; k++)
                                    {
                                        if (bag.Count == bag.Size)
                                        {
                                            break;
                                        }
                                        Item temp = tempItem.Copy();
                                        temp.Count = temp.StackSize;
                                        bag.AddItem(temp);
                                        numItems -= temp.StackSize;
                                    }

                                    //if there are any items left make a new partial item stack
                                    if (numItems != 0)
                                    {
                                        if (bag.Count == bag.Size)
                                        {
                                            break;
                                        }
                                        Item temp = tempItem.Copy();
                                        temp.Count = numItems;
                                        bag.AddItem(temp);
                                    }
                                    break;
                                }
                            }
                        }
                        if (numItems > 0)
                        {
                            if (i.StackSize > numItems)
                            {
                                Item temp = tempItem.Copy();
                                temp.Count = numItems;
                                bag.AddItem(temp);
                            }
                            else
                            {
                                for (int k = 0; k < loopCounter; k++)
                                {
                                    if (bag.Count == bag.Size)
                                    {
                                        break;
                                    }
                                    Item temp = tempItem.Copy();
                                    temp.Count = temp.StackSize;
                                    bag.AddItem(temp);
                                    numItems -= temp.StackSize;
                                }

                                if (numItems > 0)
                                {
                                    Item temp = tempItem.Copy();
                                    temp.Count = numItems;
                                    bag.AddItem(temp);
                                }
                            }
                        }
                    }

                    //IF the item is not in the inventory
                    else
                    {
                        //divide the rest into stacks and add them to the inventory
                        if (numItems >= tempItem.StackSize)
                        {
                            for (int j = 0; j < loopCounter; j++)
                            {
                                if (bag.Count == bag.Size)
                                {
                                    break;
                                }
                                Item temp = tempItem.Copy();
                                temp.Count = temp.StackSize;
                                bag.AddItem(temp);
                                numItems -= temp.StackSize;
                            }
                        }
                        //if there are any items left make a new partial item stack
                        if (numItems != 0)
                        {
                            if (bag.Count != bag.Size)
                            {
                                Item temp = tempItem.Copy();
                                temp.Count = numItems;
                                bag.AddItem(temp);
                            }
                        }
                    }
                }
                else if (i is Armor)
                    {
                        Armor tempItem = (Armor)i;
                        if (bag.IsInInventory(i.ID))
                        {
                            // Item is in the inventory already.  We need to make sure the item count is not at max.  If its at max just make a new item.
                            List<Item> items = bag.GetItemsByID(i.ID);
                            for (int j = 0; j < items.Count; j++)
                            {
                                // i.stacksize - items[j].count = amount to refill

                                if (items[j].Count < tempItem.StackSize)
                                {
                                    int fillAmount = tempItem.StackSize - items[j].Count;
                                    // If the fill amount is larger than the item count being added, just add the new items count.
                                    // else fill the stack and continue pulling items
                                    if (fillAmount > numItems)
                                    {
                                        // Adding items to a stack so it doesn't add to bagsize
                                        items[j].Count += numItems;
                                        break;
                                    }
                                    else
                                    {
                                        //Fill the partial item stack
                                        items[j].Count = tempItem.StackSize;
                                        numItems -= fillAmount;

                                        //divide the rest into stacks and add them to the inventory
                                        for (int k = 0; k < loopCounter; k++)
                                        {
                                            if (bag.Count == bag.Size)
                                            {
                                                break;
                                            }
                                            Item temp = tempItem.Copy();
                                            temp.Count = temp.StackSize;
                                            bag.AddItem(temp);
                                            numItems -= temp.StackSize;
                                        }

                                        //if there are any items left make a new partial item stack
                                        if (numItems != 0)
                                        {
                                            if (bag.Count == bag.Size)
                                            {
                                                break;
                                            }
                                            Item temp = tempItem.Copy();
                                            temp.Count = numItems;
                                            bag.AddItem(temp);
                                        }
                                        break;
                                    }
                                }
                            }
                            if (numItems > 0)
                            {
                                if (i.StackSize > numItems)
                                {
                                    Item temp = tempItem.Copy();
                                    temp.Count = numItems;
                                    bag.AddItem(temp);
                                }
                                else
                                {
                                    for (int k = 0; k < loopCounter; k++)
                                    {
                                        if (bag.Count == bag.Size)
                                        {
                                            break;
                                        }
                                        Item temp = tempItem.Copy();
                                        temp.Count = temp.StackSize;
                                        bag.AddItem(temp);
                                        numItems -= temp.StackSize;
                                    }

                                    if (numItems > 0)
                                    {
                                        Item temp = tempItem.Copy();
                                        temp.Count = numItems;
                                        bag.AddItem(temp);
                                    }
                                }
                            }
                        }

                        //IF the item is not in the inventory
                        else
                        {
                            //divide the rest into stacks and add them to the inventory
                            if (numItems >= tempItem.StackSize)
                            {
                                for (int j = 0; j < loopCounter; j++)
                                {
                                    if (bag.Count == bag.Size)
                                    {
                                        break;
                                    }
                                    Item temp = tempItem.Copy();
                                    temp.Count = temp.StackSize;
                                    bag.AddItem(temp);
                                    numItems -= temp.StackSize;
                                }
                            }
                            //if there are any items left make a new partial item stack
                            if (numItems != 0)
                            {
                                if (bag.Count != bag.Size)
                                {
                                    Item temp = tempItem.Copy();
                                    temp.Count = numItems;
                                    bag.AddItem(temp);
                                }
                            }
                        }
                    }
                if (recievedCount > 1)
                {
                    ChatBot.WriteLineToChat("/w " + name + " Received " + recievedCount + " " + i.Name + "s");
                }
                else
                {
                    ChatBot.WriteLineToChat("/w " + name + " Received " + recievedCount + " " + i.Name);
                }
                if (i is Weapon)
                {
                    Weapon temp = (Weapon)i;
                    dps += temp.attack * numItems;
                }
                else if (i is Armor)
                {
                    Armor temp = (Armor)i;
                    defense += temp.defense * numItems;
                }
            }
        }

        public void RemoveItem(Item i)
        {
            bag.RemoveItem(i);
            if (i is Weapon)
            {
                Weapon temp = (Weapon)i;
                dps -= temp.attack;
            }
            else if (i is Armor)
            {
                Armor temp = (Armor)i;
                defense -= temp.defense;
            }
        }

        public bool Equals(Viewer other)
        {
            if (other.Name == name)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public void SaveInventory()
        {


            string[] itemArray = new string[bag.Count + 1];

            for (int i = 0; i < bag.Count; i++)
            {
                // add each item to the string
                itemArray[i] = bag.GetItemByIndex(i).ID + " " + bag.GetItemByIndex(i).Count + "\n";
            }

            File.WriteAllLines(Resource.ProjectFolder + "\\Inventories\\" + name + ".txt", itemArray);
        }

        public void RetrieveInventory(Item[] itemDatabase)
        {
            string[] invStringArray = File.ReadAllLines(Resource.ProjectFolder + "\\Inventories\\" + name + ".txt");
            


            for (int i = 0; i < invStringArray.Length; i++)
            {
                if (invStringArray[i].Length > 0)
                {
                    string[] itemInfo = invStringArray[i].Split(new char[] { ' ' });

                    int itemID = Convert.ToInt32(itemInfo[0]);
                    int itemCount = Convert.ToInt32(itemInfo[1]);

                    Item currItem = itemDatabase[itemID];

                    if (currItem is Potion)
                    {
                        Potion temp = (Potion)currItem;
                        temp.Count = itemCount;
                        bag.AddItem(temp.Copy());
                    } else if (currItem is Weapon)
                    {
                        Weapon temp = (Weapon)currItem;
                        temp.Count = itemCount;
                        bag.AddItem(temp.Copy());
                    } else if (currItem is Armor)
                    {
                        Armor temp = (Armor)currItem;
                        temp.Count = itemCount;
                        bag.AddItem(temp.Copy());
                    }
                    
                    if (itemDatabase[itemID] is Weapon)
                    {
                        Weapon temp = itemDatabase[itemID] as Weapon;
                        dps += temp.attack;
                    } else if (itemDatabase[itemID] is Armor)
                    {
                        Armor temp = itemDatabase[itemID] as Armor;
                        defense += temp.defense;
                    }
                }
            }
        }


        public void UsePotion(Potion p)
        {
            ChatBot.WriteLineToChat("/w " + name + " You drank a " + p.Name + ".  HP: " + health[0] + " / " + health[1]);
            if (p.Count == 1)
            {
                bag.RemoveItem(p);
                ChatBot.WriteLineToChat("/w " + name + " You have 0 " + p.Name + " left.");
            } else
            {
                p.Count--;
                ChatBot.WriteLineToChat("/w " + name + " " + p.Count + " " + p.Name + "'s left.");
            }
            health[0] += p.value;
        }

        public void Battle()
        {
            if (target != null)
            {

                //Player stuff
                Random rng = new Random();
                float dmgMitigation = 65 / (65 + (defense * ChatBot.faythBlessings[2]));

                float rngCrit = rng.Next(0, 101);
                //Console.WriteLine("target rng-crit: " + rngCrit);
                bool doesCrit = rngCrit < crit;


                float damageDealt, critMultiplier;
                if (doesCrit)
                {
                    critMultiplier = 2.5f;
                }
                else
                {
                    critMultiplier = 1;
                }
                damageDealt = (float)Math.Round(((target.DPS * ChatBot.faythBlessings[1]) * critMultiplier) * dmgMitigation, 1, MidpointRounding.AwayFromZero);

                //player dies
                if (health[0] - damageDealt <= 0)
                {
                    //Call a function from chatBot to display the message of how many rats you killed.
                    target = null;
                    health[0] = 0;
                    ChatBot.WriteLineToChat("/w " + name + " You got " + adventureGil + " gil from killing " + slainEnemies + " enemies on your adventure.");
                    isAdventuring = false;
                    ChatBot.adventurers.Remove(this);
                    ChatBot.healing.Add(this);
                    slainEnemies = 0;
                    adventureGil = 0;
                }
                else
                {

                    health[0] -= damageDealt;

                    //Potions
                    if ((health[0] / health[1]) <= 0.25f)
                    {
                        if (bag.IsInInventory(0))
                        {
                            UsePotion((Potion)bag.GetItemByID(0));
                        }
                        else if (bag.IsInInventory(1))
                        {
                            //use the potion.
                            UsePotion((Potion)bag.GetItemByID(1));
                        }
                        else if (bag.IsInInventory(2))
                        {
                            //use the potion.
                            UsePotion((Potion)bag.GetItemByID(2));
                        }
                    }
                    //Console.WriteLine(name + " Defense: " + defense);
                    //Console.WriteLine(target.Name + " DPS: " + target.DPS);
                    /*
                    if (!doesCrit)
                    {
                        Console.WriteLine(target.Name + " dealt " + Math.Round(((target.DPS * critMultiplier) * dmgMitigation), 1, MidpointRounding.AwayFromZero) + " damage.");
                    } else
                    {
                        Console.WriteLine(target.Name + " dealt " + Math.Round(((target.DPS * critMultiplier) * dmgMitigation), 1, MidpointRounding.AwayFromZero) + " damage!!!");
                    }
                    Console.WriteLine(name + " Health: " + health[0]);
                    */
                }
                
                //Target stuff
                if (target != null)
                {
                    dmgMitigation = 65 / (65 + target.Defense);
                    rngCrit = rng.Next(0, 101);
                    //Console.WriteLine("player rng-crit: " + rngCrit);
                    doesCrit = rngCrit < (target.crit * ChatBot.faythBlessings[3]);
                    if (doesCrit)
                    {
                        critMultiplier = 2.5f;
                    } else
                    {
                        critMultiplier = 1;
                    }
                    damageDealt = (float)Math.Round((dps * critMultiplier) * dmgMitigation, 1, MidpointRounding.AwayFromZero);
                    if (target.CurrentHealth - damageDealt <= 0)
                    {
                        Console.WriteLine("Enemy Killed.");
                        // the target dies
                        // get a new target
                        slainEnemies++;
                        totalSlain++;
                        gil += (int)(target.GilDropped * ChatBot.faythBlessings[4]);
                        adventureGil += (int)(target.GilDropped * ChatBot.faythBlessings[4]);
                        if (exp + (target.Exp * ChatBot.faythBlessings[4]) >= expToLvl)
                        {
                            LevelUp();
                            ChatBot.WriteLineToChat("/w " + name + " You are now level " + lvl + ".");
                        } else
                        {
                            exp += (int)(target.Exp * ChatBot.faythBlessings[4]);
                        }
                        if (lvl > 1)
                        {
                            target = ChatBot.enemyDB[rng.Next(0, ChatBot.enemyDB.Length)].GrowEnemy(rng.Next(lvl - 1, lvl + 2));
                        }
                        else
                        {
                            target = ChatBot.enemyDB[rng.Next(0, ChatBot.enemyDB.Length)].GrowEnemy(rng.Next(lvl, lvl + 2));
                        }

                        

                    }
                    else
                    {
                        //critchance

                        target.CurrentHealth -= damageDealt;
                        /*
                        if (!doesCrit)
                        {
                            Console.WriteLine(name + " dealt " + Math.Round((dps * critMultiplier) * dmgMitigation, 1, MidpointRounding.AwayFromZero) + " damage.");
                        } else
                        {
                            Console.WriteLine(name + " dealt " + Math.Round((dps * critMultiplier) * dmgMitigation, 1, MidpointRounding.AwayFromZero) + " damage!!!");
                        }
                        */
                        Console.WriteLine("Target CH: " + target.CurrentHealth);
                        
                    }
                }
            }
        }
    }
}
