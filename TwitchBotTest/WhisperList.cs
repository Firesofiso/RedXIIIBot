using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TwitchBotTest
{
    class WhisperItemList
    {
        string playerName;
        List<Item> iList;
        Timer task;
        int i = 0;

        public WhisperItemList(string name, List<Item> list, int interval, bool isIventory)
        {
            playerName = name;
            iList = list;
            task = new Timer();
            task.Interval = interval;
            task.AutoReset = true;
            task.Enabled = false;
            if (isIventory)
            {
                task.Elapsed += delegate { WriteInventoryLine(); };
            } else
            {
                task.Elapsed += delegate { WriteShopLine(); };
            }
        }

        public WhisperItemList(Viewer v, int interval)
        {

        }

        public void WriteInventoryLine()
        {

            string inventory = "";

            if (iList[i].StackSize == 1)
            {
                int count = 1;
                for (int j = 0; j < iList.Count; j++)
                {
                    if (iList[j].ID == iList[i].ID && j != i)
                    {
                        count++;
                    }
                }
                inventory += "[" + iList[i].Name + " x" + iList[i].Count + "]";
                i += count;

            }
            else
            {
                inventory += "[" + iList[i].Name + " x" + iList[i].Count + "]";
                i++;
            }
            
            if (i == iList.Count())
            {
                task.Enabled = false;
                i = 0;
            } else
            {
                inventory += ", ";
            }
        }

        public void WriteShopLine()
        {
            ChatBot.WriteLineToChat("/w " + playerName + " " + iList[i].Name + " - " + iList[i].BuyPrice + "Gil.");
            i++;
            if (i == iList.Count())
            {
                task.Enabled = false;
                i = 0;
            }
        }

        public void Start()
        {
            task.Enabled = true;
        }

        public void End()
        {
            task.Enabled = false;
        }

    }
}
