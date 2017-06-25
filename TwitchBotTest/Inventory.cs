using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBotTest
{
    class Inventory
    {
        private List<Item> itemList;
        private int size = 30;

        public int Count
        {
            get { return itemList.Count; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public List<Item> List
        {
            get { return itemList; }
        }

        public Inventory()
        {
            itemList = new List<Item>();
        }

        public void AddItem(Item i)
        {
            itemList.Add(i);
        }

        /*  ----DEPRECATED?----
        public void AddItem(Item i, int numItems)
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
                if (IsInInventory(i.ID))
                {
                    // Item is in the inventory already.  We need to make sure the item count is not at max.  If its at max just make a new item.
                    List<Item> items = GetItemsByID(i.ID);
                    for (int j = 0; j < items.Count; j++)
                    {
                        if (items[j].Count < tempItem.StackSize)
                        {

                            // i.stacksize - items[j].count = amount to refill
                            int fillAmount = tempItem.StackSize - items[j].Count;

                            // If the fill amount is larger than the item count being added, just add the new items count.
                            // else fill the stack and continue pulling items
                            if (fillAmount > numItems)
                            {
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
                                    if (itemList.Count == size)
                                    {
                                        break;
                                    }
                                    Item temp = tempItem.Copy();
                                    temp.Count = temp.StackSize;
                                    itemList.Add(temp);
                                    numItems -= temp.StackSize;
                                }

                                //if there are any items left make a new partial item stack
                                if (numItems != 0)
                                {
                                    if (itemList.Count == size)
                                    {
                                        break;
                                    }
                                    Item temp = tempItem.Copy();
                                    temp.Count = numItems;
                                    itemList.Add(temp);
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
                            itemList.Add(temp);
                        }
                        else
                        {
                            for (int k = 0; k < loopCounter; k++)
                            {
                                if (itemList.Count == size)
                                {
                                    break;
                                }
                                Item temp = tempItem.Copy();
                                temp.Count = temp.StackSize;
                                itemList.Add(temp);
                                numItems -= temp.StackSize;
                            }

                            if (numItems > 0)
                            {
                                Item temp = tempItem.Copy();
                                temp.Count = numItems;
                                itemList.Add(temp);
                            }
                        }
                    }
                }
                else
                {
                    //divide the rest into stacks and add them to the inventory
                    for (int j = 0; j < loopCounter; j++)
                    {
                        if (itemList.Count == size)
                        {
                            break;
                        }
                        Item temp = tempItem.Copy();
                        temp.Count = temp.StackSize;
                        itemList.Add(temp);
                        numItems -= temp.StackSize;
                    }
                    //if there are any items left make a new partial item stack
                    if (numItems != 0)
                    {
                        if (itemList.Count != size)
                        {
                            Item temp = tempItem.Copy();
                            temp.Count = numItems;
                            itemList.Add(temp);
                        }
                    }
                }
            }
            else if (i is Weapon)
            {
                Weapon tempItem = (Weapon)i;
                if (IsInInventory(i.ID))
                {
                    // Item is in the inventory already.  We need to make sure the item count is not at max.  If its at max just make a new item.
                    List<Item> items = GetItemsByID(i.ID);
                    for (int j = 0; j < items.Count; j++)
                    {
                        if (items[j].Count < tempItem.StackSize)
                        {

                            // i.stacksize - items[j].count = amount to refill
                            int fillAmount = tempItem.StackSize - items[j].Count;

                            // If the fill amount is larger than the item count being added, just add the new items count.
                            // else fill the stack and continue pulling items
                            if (fillAmount > numItems)
                            {
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
                                    if (itemList.Count == size)
                                    {
                                        break;
                                    }
                                    Item temp = tempItem.Copy();
                                    temp.Count = temp.StackSize;
                                    itemList.Add(temp);
                                    numItems -= temp.StackSize;
                                }

                                //if there are any items left make a new partial item stack
                                if (numItems > 0)
                                {
                                    if (itemList.Count == size)
                                    {
                                        break;
                                    }
                                    Item temp = tempItem.Copy();
                                    temp.Count = numItems;
                                    itemList.Add(temp);
                                }
                                break;
                            }
                        }
                    }
                    if (numItems > 0 )
                    {
                        if (i.StackSize > numItems)
                        {
                            Item temp = tempItem.Copy();
                            temp.Count = numItems;
                            itemList.Add(temp);
                        } else
                        {
                            for (int k = 0; k < loopCounter; k++)
                            {
                                if (itemList.Count == size)
                                {
                                    break;
                                }
                                Item temp = tempItem.Copy();
                                temp.Count = temp.StackSize;
                                itemList.Add(temp);
                                numItems -= temp.StackSize;
                            }
                            
                            if (numItems > 0)
                            {
                                Item temp = tempItem.Copy();
                                temp.Count = numItems;
                                itemList.Add(temp);
                            }
                        }
                    }
                }
                else
                {
                    //divide the rest into stacks and add them to the inventory
                    for (int j = 0; j < loopCounter; j++)
                    {
                        if (itemList.Count == size)
                        {
                            break;
                        }
                        Item temp = tempItem.Copy();
                        temp.Count = temp.StackSize;
                        itemList.Add(temp);
                        numItems -= temp.StackSize;
                    }
                    //if there are any items left make a new partial item stack
                    if (numItems > 0)
                    {
                        if (itemList.Count != size)
                        {
                            Item temp = tempItem.Copy();
                            temp.Count = numItems;
                            itemList.Add(temp);
                        }
                    }
                }
            }
            else if (i is Armor)
            {
                Armor tempItem = (Armor)i;
                if (IsInInventory(i.ID))
                {
                    // Item is in the inventory already.  We need to make sure the item count is not at max.  If its at max just make a new item.
                    List<Item> items = GetItemsByID(i.ID);
                    for (int j = 0; j < items.Count; j++)
                    {
                        if (items[j].Count < tempItem.StackSize)
                        {

                            // i.stacksize - items[j].count = amount to refill
                            int fillAmount = tempItem.StackSize - items[j].Count;

                            // If the fill amount is larger than the item count being added, just add the new items count.
                            // else fill the stack and continue pulling items
                            if (fillAmount > numItems)
                            {
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
                                    if (itemList.Count == size)
                                    {
                                        break;
                                    }
                                    Item temp = tempItem.Copy();
                                    temp.Count = temp.StackSize;
                                    itemList.Add(temp);
                                    numItems -= temp.StackSize;
                                }

                                //if there are any items left make a new partial item stack
                                if (numItems != 0)
                                {
                                    if (itemList.Count == size)
                                    {
                                        break;
                                    }
                                    Item temp = tempItem.Copy();
                                    temp.Count = numItems;
                                    itemList.Add(temp);
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
                            itemList.Add(temp);
                        }
                        else
                        {
                            for (int k = 0; k < loopCounter; k++)
                            {
                                if (itemList.Count == size)
                                {
                                    break;
                                }
                                Item temp = tempItem.Copy();
                                temp.Count = temp.StackSize;
                                itemList.Add(temp);
                                numItems -= temp.StackSize;
                            }

                            if (numItems > 0)
                            {
                                Item temp = tempItem.Copy();
                                temp.Count = numItems;
                                itemList.Add(temp);
                            }
                        }
                    }
                }
                else
                {
                    //divide the rest into stacks and add them to the inventory
                    for (int j = 0; j < loopCounter; j++)
                    {
                        if (itemList.Count == size)
                        {
                            break;
                        }
                        Item temp = tempItem.Copy();
                        temp.Count = temp.StackSize;
                        itemList.Add(temp);
                        numItems -= temp.StackSize;
                    }
                    //if there are any items left make a new partial item stack
                    if (numItems > 0)
                    {
                        if (itemList.Count != size)
                        {
                            Item temp = tempItem.Copy();
                            temp.Count = numItems;
                            itemList.Add(temp);
                        }
                    }
                }
            }
        }
        */

        // Item one and two are both partial stacks.
        public List<Item> CombineItems(Item one, Item two)
        {
            List<Item> result = new List<Item>();
            int stackSize = one.StackSize;
            // They HAVE to be the same item otherwise you are turning potions into hi-potions and x-potions into potions.
            // "Humankind cannot gain anything without first giving something in return. To obtain, something of equal value must be lost." - Alphonse Elric
            if (one.ID == two.ID) {

                if (one.Count + two.Count > stackSize)
                {
                    int countOverflow = (one.Count + two.Count) - stackSize;
                    Item temp = ChatBot.itemDB[one.ID];
                    temp.Count = stackSize;
                    result.Add(temp);
                    temp = ChatBot.itemDB[one.ID];
                    temp.Count = countOverflow;
                    result.Add(temp);
                }
            }
            return result;
        }

        public void RemoveItem(Item i)
        {
            itemList.Remove(i);
        }

        //Problem with this function is what happens when there are multiple items with the same id in one inventory?
        //Should this return a list?
        public List<Item> GetItemsByID(int id)
        {
            List<Item> tempList = new List<Item>();
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].ID == id)
                {
                    tempList.Add(itemList[i]);
                }
            }
            return tempList;
        }

        public Item GetItemByID(int id)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].ID == id)
                {
                    return itemList[i];
                }
            }
            return null;
        }

        public Item GetItemByIndex(int index)
        {
            return itemList[index];
        }

        public bool IsInInventory(int id)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].ID == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
