using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Timers;

namespace TwitchBotTest
{
    class ChatBot
    {

        private static string viewerListFilePath = Resource.ProjectFolder + "\\FullViewerList.txt";

        public static List<Viewer> currentViewers = new List<Viewer>();
        private static List<Viewer> fullViewerList = new List<Viewer>();
        public static List<Viewer> adventurers = new List<Viewer>();
        public static List<Viewer> healing = new List<Viewer>();


        // 0 - Healing
        // 1 - dps
        // 2 - defense
        // 3 - gil rate
        // 4 - exp rate
        public static float[] faythBlessings = new float[5] { 1, 1, 1, 1, 1 };
        public static Timer[] faythTimers = new Timer[5];

        private static IRCClient irc = new IRCClient("irc.chat.twitch.tv", 6667, "redxiiibot", "oauth:cvyhcq9tg5c6a8hsi8dx3j2h4616kg");

        private static List<Timer> allTimers = new List<Timer>();

        public static Item[] itemDB =
        {
            new Potion(0, "Potion", 50, 10, new string[] { "potion", "pot" }, 50, 99),
            new Potion(1, "Hi-Potion", 250, 50, new string[] { "hipotion", "hipot", "hi-potion", "hi-pot" }, 100, 99),
            new Potion(2, "X-Potion", 500, 100, new string[] { "xpotion", "xpot", "x-potion", "x-pot" }, 300, 99),
            new Weapon(3, "Sword", 5, 750, 50, new string[] { "sword", "swrd" }, 1),
            new Armor(4, "Plate Armor", 10, 1050, 50, new string[] { "plate", "plate armor", "platearmor" }, 1)
        };

        private static Job[] jobDB =
        {
            new Job("Warrior", new char[] { 's', 'c', 'e', 'd', 'a', 'z' }, new Ability[] { }),
            new Job("Thief", new char[] { 'd', 's', 'd', 'd', 'c', 'z' }, new Ability[] { }),
            new Job("Red Mage", new char[] { 'c', 'c', 'b', 'b', 'c', 'b' }, new Ability[] { })
        };

        public static Dictionary<char, float> curveChars = new Dictionary<char, float>()
        {
            {'a', 2.5f },
            {'b', 1.75f },
            {'c', 1.25f },
            {'d', 0.75f },
            {'e', 0.5f }
        };


        public static List<Item> shoplist = new List<Item> {
            itemDB[0],
            itemDB[1],
            itemDB[2],
            itemDB[3],
            itemDB[4]
        };

        public static Enemy[] enemyDB =
        {
            new Enemy("Lizard", new char[] { 'd', 'd', 'c' }, 0, 5, new int[] { 14, 24 }, 50),
            new Enemy("Rarab", new char[] { 'c', 'd', 'd' }, 5, 0,new int[] { 14, 24 }, 50),
            new Enemy("Mandragora", new char[] { 'd', 'c', 'd' }, 0, 7.5f, new int[] { 14, 24 }, 50)
            
        };

        public static Viewer GetUserFromList(List<Viewer> list, string userName)
        {
            return list.Find(x => x.Name == userName);
        }

        public static int ConvertToNumber(string integer)
        {
            List<int> digits = new List<int>();

            for (int i = 0; i < integer.Length; i++)
            {
                digits.Add(integer[i] - 48);
            }


            int multiplier = 1;
            int result = 0;

            for (int i = digits.Count - 1; i >= 0; i--)
            {
                result += digits[i] * multiplier;
                multiplier *= 10;
            }

            return result;
        }

        public static string GetUserName(string message)
        {
            string user = "";
            int index = message.IndexOf(':')+1;

            if (message.Contains('!'))
            {
                while (message[index] != '!')
                {
                    user += message[index];
                    index++;
                }
            }
            
            return user;
        }

        public bool IsNewViewer(string username)
        {
            Viewer temp = new Viewer(username);
            if (fullViewerList.Contains(temp))
            {
                return false;
            } else
            {
                return true;
            }
            
        }

        public static void StartAdventure(Viewer currentViewer)
        {
            // Start the viewers climb
            if (!currentViewer.isAdventuring)
            {
                if (currentViewer.CurrentHealth == currentViewer.MaxHealth)
                {
                
                    currentViewer.isAdventuring = true;
                    adventurers.Add(currentViewer);
                    if (currentViewer.target == null)
                    {
                        //get a random enemy and set it to the players target.
                        Random rng = new Random();
                        if (currentViewer.Level > 1)
                        {
                            currentViewer.target = enemyDB[rng.Next(0, enemyDB.Length)].GrowEnemy(rng.Next(currentViewer.Level - 1, currentViewer.Level + 2));
                        } else
                        {
                            currentViewer.target = enemyDB[rng.Next(0, enemyDB.Length)].GrowEnemy(rng.Next(currentViewer.Level, currentViewer.Level + 2));
                        }
                    }
                    irc.sendChatMessage("/w " + currentViewer.Name + " Good luck Adventuring!");
                } else
                {
                    irc.sendChatMessage("/w " + currentViewer.Name + " Let's heal up first!");
                }
            }
            else
            {
                irc.sendChatMessage("/w " + currentViewer.Name + " You are already on an adventure.");
            }
        }

        private static void GetFullViewerList()
        {
            // Read the viewer list line by line
            // Create a new viewer based on the info in the txt doc
            // put new viewer into the full list
            string[] viewerArray = File.ReadAllLines(viewerListFilePath);
            for (int i = 0; i < viewerArray.Length; i++)
            {
                if (viewerArray[i].Length > 0)
                {
                    char[] delims = new char[] { ' ' };
                    string[] viewerInfo = viewerArray[i].Split(delims);
                    Viewer tempViewer = new Viewer(viewerInfo[0], ConvertToNumber(viewerInfo[1]));
                    tempViewer.Gil = ConvertToNumber(viewerInfo[2]);
                    tempViewer.Exp = ConvertToNumber(viewerInfo[3]);
                    tempViewer.totalSlain = ConvertToNumber(viewerInfo[4]);
                    tempViewer.fayth = ConvertToNumber(viewerInfo[5]);
                    fullViewerList.Add(tempViewer);
                }
            }
        }

        private static void SaveData(Object source, ElapsedEventArgs e)
        {
            // Write all info from FullViewerList into a txt document
            // Formatted like this: USERNAME POINTS \n

            // Update the fullviewerlist with the new stuff in the currentviewerlist
            for (int i = 0; i < currentViewers.Count; i++)
            {
                Viewer foundViewer = fullViewerList.Find(x => x.Name == currentViewers[i].Name);
                if (foundViewer != null) { 
                    foundViewer.Gil = currentViewers[i].Gil;
                } else
                {
                    fullViewerList.Add(currentViewers[i]);
                }

                currentViewers[i].SaveInventory();
            }

            string[] viewerArray = new string[fullViewerList.Count];
            for (int i = 0; i < fullViewerList.Count; i ++)
            {
                viewerArray[i] = fullViewerList[i].Name + " " + fullViewerList[i].Level + " " + fullViewerList[i].Gil + " " + fullViewerList[i].Exp + " " + fullViewerList[i].totalSlain + " " + fullViewerList[i].fayth + "\n";
                
            }
            File.WriteAllLines(viewerListFilePath, viewerArray);
            //Console.WriteLine("DATA SAVED");
        }

        public static Timer CreateTimer(int interval, bool autoReset, bool enabled, ElapsedEventHandler function)
        {

            Timer aTimer;
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(interval);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += function;
            aTimer.AutoReset = autoReset;
            aTimer.Enabled = enabled;
            return aTimer;
        }

        private static void PING(Object source, ElapsedEventArgs e)
        {
            irc.sendIRCMessage("PING");
        }

        private static void GiveGil(Object source, ElapsedEventArgs e)
        {
            for (int i = 0; i < currentViewers.Count; i++)
            {
                currentViewers[i].Gil += 1;
            }
        }

        private static void GiveFayth(Object source, ElapsedEventArgs e)
        {
            for (int i = 0; i < currentViewers.Count; i++)
            {
                currentViewers[i].fayth += 1;
            }
        }

        private static void RetrieveViewerList(Object source, ElapsedEventArgs e)
        {
            // Create a request for the URL. 		
            WebRequest request = WebRequest.Create("https://tmi.twitch.tv/group/user/firesofiso/chatters");
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
        }

        //not sure what to name this function
        private static void AdventurerBattle(Object source, ElapsedEventArgs e)
        {
            //loop through all players adventuring.
            //deal damage to enemy and take damage from the enemy.
            for (int i = 0; i< adventurers.Count; i++)
            {
                adventurers[i].Battle();
            }
        }

        private static void HealViewers(Object source, ElapsedEventArgs e)
        {
            //loop through all players adventuring.
            //deal damage to enemy and take damage from the enemy.
            for (int i = 0; i < healing.Count; i++)
            {
                if (healing[i].CurrentHealth + 1 * faythBlessings[0] < healing[i].MaxHealth)
                {
                    Console.WriteLine(healing[i].Name + " Health: " + healing[i].CurrentHealth);
                    healing[i].CurrentHealth += 1 * faythBlessings[0];
                }
                else
                {
                    healing[i].CurrentHealth = healing[i].MaxHealth;
                    irc.sendChatMessage("/w " + healing[i].Name + " You have reached max health.  Happy Adventuring!");
                    if (healing[i].autoAdventure)
                    {
                        StartAdventure(healing[i]);
                    }
                    healing.Remove(healing[i]);
                    
                }
            }
        }
        
        private static void SetBlessing(int index, int multiplier)
        {
            faythBlessings[index] = multiplier;
            Console.WriteLine("Blessing has run out.");
        }

        public static void WriteLineToChat(string message)
        {
            irc.sendChatMessage(message);
        }

        static void Main(string[] args)
        {

            irc.sendIRCMessage("CAP REQ :twitch.tv/membership");
            //irc.sendIRCMessage("CAP REQ :twitch.tv/tags");
            irc.sendIRCMessage("CAP REQ :twitch.tv/commands");
            irc.joinRoom("firesofiso");
            GetFullViewerList();
            allTimers.Add(CreateTimer(240000, true, true, PING));
            allTimers.Add(CreateTimer(6000, true, true, SaveData));
            allTimers.Add(CreateTimer(6000, true, true, AdventurerBattle));
            allTimers.Add(CreateTimer(6000, true, true, HealViewers));
            allTimers.Add(CreateTimer(6000, true, true, GiveFayth));

            //Blessings -
            for (int i = 0; i < 5; i++)
            {
                faythTimers[i] = CreateTimer(300000, false, false, delegate { SetBlessing(i, 2); });
            }

            irc.sendChatMessage("/me Now Processing Adventurers!");

            while (true)
            {

                string message = irc.readMessage();
                
                Console.WriteLine(message);
                if (message != null)
                {
                    
                    // This part is just for keeping track of who is in chat currently. Seems to be working okay.
                    if (message.Contains("PRIVMSG") || message.Contains("JOIN"))
                    {
                        Viewer user = new Viewer(GetUserName(message));
                        if (!currentViewers.Contains(user))
                        {
                            if (fullViewerList.Contains(user))
                            {
                                user = fullViewerList.Find(x => x.Name == user.Name);
                                user.RetrieveInventory(itemDB);
                            }
                            currentViewers.Add(user);
                        }
                    }

                    // Playing ping pong with the twitch server.
                    if (message.Contains("PING"))
                    {
                        irc.sendIRCMessage("PONG");
                    }

                    // Remove the viewer from current viewers when they leave the channel.
                    if (message.Contains("PART"))
                    {
                        //find username in currentViewers
                        //remove them from currentViewers
                        currentViewers.Remove(currentViewers.Find(x => x.Name == GetUserName(message))); // - this is very questionable, not really sure what remove is going to do.
                    }


                    Viewer currentViewer = GetUserFromList(currentViewers, GetUserName(message));

                    // -------------- COMMANDS --------------------

                    // CHAT COMMANDS -

                    // !discord - display a link to the discord
                    if (message.Contains("!discord"))
                    {
                        irc.sendChatMessage("[ discord.gg/sJJa23e ]  Join the Community Discord.  Keep up the conversation with other viewers and Firesofiso.  Maybe play some games or watch movies or something.  Who knows!?");
                    }

                    // !red13 - explaination of the bot game.
                    if (message.Contains("!red13"))
                    {
                        irc.sendChatMessage("Hey, I'm RedXIIIBot and I run a game in chat that allows you to battle in dungeons collecting items and richs.  Level up, gain strength, fight stronger things, level up, repeat.  Commands and info at https://goo.gl/ELjJpn.  Whisper me so I can whisper information back to you.");
                    }




                    // WHISPER COMMANDS - 
                    // Will remain chat commands until things are fleshed out.

                    if(message.Contains("!me"))
                    {
                        //Bot should whisper the user with their stats.
                        // Level, health, attack, defense. 

                        irc.sendChatMessage("/w " + currentViewer.Name + " Lvl: " + currentViewer.Level + " :: Exp: " + currentViewer.Exp + " / "  + currentViewer.ExpNeeded  + " :: Gil: " + currentViewer.Gil + " :: AA: " + currentViewer.autoAdventure + " :: Fayth: " + currentViewer.fayth);
                        irc.sendChatMessage("/w " + currentViewer.Name + " Health: " + currentViewer.CurrentHealth + " / " + currentViewer.MaxHealth + " :: DPS: " + currentViewer.DPS + " :: Defense: " + currentViewer.Defense);

                    }

                    // Check to see what enemy you are fighting and what level it is.
                    if(message.Contains("!check"))
                    {
                        if (currentViewer.target != null) {
                            irc.sendChatMessage("/w " + currentViewer.Name + " Enemy: " + currentViewer.target.Name + " Lvl: " + currentViewer.target.lvl);
                        } else
                        {
                            irc.sendChatMessage("/w " + currentViewer.Name + " You have no target.");
                        }
                    }

                    if (message.Contains("!start"))
                    {
                        StartAdventure(currentViewer);
                    }

                    if (message.Contains("!auto"))
                    {
                        currentViewer.autoAdventure = !currentViewer.autoAdventure;
                        irc.sendChatMessage("/w " + currentViewer.Name + " Auto Adventuring - " + currentViewer.autoAdventure);
                    }


                    // !kills - whispers how many total kills the play has gotten.
                    if (message.Contains("!kills"))
                    {
                        irc.sendChatMessage("/w " + currentViewer.Name + " You have killed " + currentViewer.totalSlain + " enemies throughout your adventures.");
                    }

                    // !shop - Red will whisper the user its current list of shop items.
                    //  [thoughts]
                    //      1 - Diff items in different areas
                    //      2 - One big shop containing the essentials - all equipment and stuff is crafted or found on enemies.
                    //  Within this command is the buying command - !shop <itemName> <quantity>
                    //      -This sub-command could potentially be its own command, i.e. !buy
                    //---
                    if (message.Contains("!shop"))
                    {
                        if (!currentViewer.isAdventuring)
                        {
                            string command = message.Substring(message.LastIndexOf('!'));
                            Console.WriteLine(command);

                            string[] commandParts = command.Split(new char[] { ' ' });

                            // is the player asking for an item?
                            if (commandParts.Length > 1)
                            {
                                for (int i = 0; i < shoplist.Count; i++)
                                {

                                    // Is the item in the shop? Search by name.
                                    if (shoplist[i].CompareName(commandParts[1]))
                                    {

                                        // Is the player asking for a certain quantity of items?
                                        if (commandParts.Length == 3)
                                        {
                                            int numItems = ConvertToNumber(commandParts[2]);
                                            // Does the player have enough gil to buy said items?
                                            if (currentViewer.Gil >= shoplist[i].BuyPrice * numItems)
                                            {
                                                // Take players money
                                                // Give them the items.
                                                // Tell them what they got.
                                                
                                                Item temp = shoplist[i];
                                                currentViewer.AddItem(temp, numItems);
                                                /*
                                                if (temp.StackSize > 1)
                                                {
                                                    
                                                } else
                                                {
                                                    if (currentViewer.Bag.Count() + numItems > currentViewer.Bag.MaxSize)
                                                    {
                                                        numItems = currentViewer.Bag.MaxSize - currentViewer.Bag.Count();
                                                    }
                                                    for (int j = 0; j < numItems; j++)
                                                    {
                                                        currentViewer.AddItem(temp);
                                                    }
                                                }
                                                */
                                                currentViewer.Gil -= shoplist[i].BuyPrice * numItems;
                                            }
                                            else
                                            {
                                                // Player doesn't have enough gil to purchase items.
                                                irc.sendChatMessage("/w " + currentViewer.Name + " You don't have enough Gil for that.");
                                            }
                                        }

                                        // Is the player entering more commands than is required for this certain command?
                                        else if (commandParts.Length > 3)
                                        {

                                            // Tell them the correct format
                                            irc.sendChatMessage("/w " + currentViewer.Name + " There are too many parameters in that command.  Format: !shop <itemname> <quantity>.");
                                        }

                                        // Finally the player is only looking for 1 of an item in the shop.
                                        else
                                        {
                                            // Do they have enough gil?
                                            if (currentViewer.Gil >= shoplist[i].BuyPrice)
                                            {

                                                // Take the players money.
                                                // Give them the item they need.
                                                // Tell them what they bought.
                                                currentViewer.Gil -= shoplist[i].BuyPrice;
                                                currentViewer.AddItem(shoplist[i], 1);
                                            }
                                            else
                                            {
                                                irc.sendChatMessage("/w " + currentViewer.Name + " You don't have enough Gil for that.");
                                            }

                                        }
                                    }
                                }
                            }

                            // !shop command handled.
                            else
                            {
                                irc.sendChatMessage("/w " + currentViewer.Name + " Welcome to the Shop!");
                                WhisperItemList shopWhisper = new WhisperItemList(currentViewer.Name, shoplist, 500, false);
                                shopWhisper.Start();
                            }
                        } else
                        {
                            irc.sendChatMessage("/w " + currentViewer.Name + " Shop is not available on adventures.");
                        }
                    }
                    

                    // !bag or !inv command
                    //  - Red will whisper the player a list of the items in their bag.
                    if (message.Contains("!bag"))
                    {
                        if (currentViewer.Bag.Count == 0)
                        {
                            irc.sendChatMessage("/w " + currentViewer.Name + " Your bag is empty.");
                        }
                        else
                        {
                            string invString = "";
                            irc.sendChatMessage("/w " + currentViewer.Name + " " + currentViewer.Bag.Count + "/" + currentViewer.Bag.Size + " Items in your bag: ");
                            for (int i = 0; i < currentViewer.Bag.Count; i++)
                            {
                                

                                Item temp = currentViewer.Bag.GetItemByIndex(i);

                                if (temp.StackSize == 1)
                                {
                                    int count = 1;

                                    for (int j = 0; j < currentViewer.Bag.Count; j++)
                                    {
                                        if (temp.ID == currentViewer.Bag.GetItemByIndex(j).ID && j != i)
                                        {
                                            count++;
                                        }
                                    }
                                    invString += "[" + temp.Name + " x" + temp.Count + "]";
                                    i += count - 1;

                                }
                                else
                                {
                                    invString += "[" + temp.Name + " x" + temp.Count + "]";
                                }

                                if (i < currentViewer.Bag.Count - 1)
                                {
                                    invString += ", ";
                                }
                            }
                            irc.sendChatMessage("/w " + currentViewer.Name + " " + invString);
                        }
                    }
                    

                    // !pray - pray to a certain deity, god, summon, whatever to get a global buff.
                    if (message.Contains("!pray"))
                    {
                        string command = message.Substring(message.LastIndexOf('!'));
                        Console.WriteLine(command);

                        string[] commandParts = command.Split(new char[] { ' ' });

                        if (commandParts.Length == 1)
                        {
                            irc.sendChatMessage("/me Pray to a summon to obtain a global buff for all.  Requires 2700 Fayth.");
                        } else if (commandParts.Length == 2)
                        {
                            if (currentViewer.fayth >= 2500) {
                                if (commandParts[1].ToLower() == "kirin")
                                {
                                    faythBlessings[0] = 2;
                                    
                                    if (!faythTimers[0].Enabled)
                                    {
                                        faythTimers[0].Enabled = true;
                                        faythTimers[0].Interval = 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Kirin, Sanctuary Regeneration has increased for 5 minutes.");
                                    }
                                    else
                                    {
                                        faythTimers[0].Interval += 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Kirin and extended his blessing by 5 minutes.");
                                    }
                                } else if (commandParts[1].ToLower() == "ifrit")
                                {
                                    faythBlessings[1] = 2;

                                    if (!faythTimers[1].Enabled)
                                    {
                                        faythTimers[1].Enabled = true;
                                        faythTimers[1].Interval = 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Ifrit, Battle Damage has increased for 5 minutes.");
                                    }
                                    else
                                    {
                                        faythTimers[1].Interval += 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Ifrit and extended his blessing by 5 minutes.");
                                    }
                                }
                                else if (commandParts[1].ToLower() == "titan")
                                {
                                    faythBlessings[2] = 2;

                                    if (!faythTimers[2].Enabled)
                                    {
                                        faythTimers[2].Enabled = true;
                                        faythTimers[2].Interval = 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Titan, Battle Defense has increased for 5 minutes.");
                                    }
                                    else
                                    {
                                        faythTimers[2].Interval += 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Titan and extended his blessing by 5 minutes.");
                                    }
                                }
                                else if (commandParts[1].ToLower() == "garuda")
                                {
                                    faythBlessings[3] = 2;

                                    if (!faythTimers[3].Enabled)
                                    {
                                        faythTimers[3].Enabled = true;
                                        faythTimers[3].Interval = 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Garuda, Critical Hit Rate has increased for 5 minutes.");
                                    }
                                    else
                                    {
                                        faythTimers[3].Interval += 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Garuda and extended her blessing by 5 minutes.");
                                    }
                                }
                                else if (commandParts[1].ToLower() == "chocobo")
                                {
                                    faythBlessings[4] = 2;

                                    if (!faythTimers[4].Enabled)
                                    {
                                        faythTimers[4].Enabled = true;
                                        faythTimers[4].Interval = 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Fat Chocobo, Gil Dropped has increased for 5 minutes.");
                                    }
                                    else
                                    {
                                        faythTimers[4].Interval += 300000;
                                        irc.sendChatMessage("/me " + currentViewer.Name + " prayed to Fat Chocobo and extended his blessing by 5 minutes.");
                                    }
                                }
                            } else
                            {
                                irc.sendChatMessage("/w " + currentViewer.Name + " You do not believe enough");
                            }
                        }
                    }

                    // GOD COMMANDS - Firesofiso only
                    
                    if (message.Contains("!givegil"))
                    {
                        if (currentViewer.Name == "firesofiso")
                        {
                            string command = message.Substring(message.LastIndexOf('!'));
                            Console.WriteLine(command);

                            string[] commandParts = command.Split(new char[] { ' ' });
                            string viewerName;

                            if (commandParts.Length == 3)
                            {
                                if (commandParts[1][0] == '@')
                                {
                                    viewerName = commandParts[1].Substring(1).ToLower();
                                } else
                                {
                                    viewerName = commandParts[1].ToLower();
                                }
                                GetUserFromList(fullViewerList, viewerName).Gil += ConvertToNumber(commandParts[2]);
                            }
                        }
                    }

                    if (message.Contains("!givefayth"))
                    {
                        if (currentViewer.Name == "firesofiso")
                        {
                            string command = message.Substring(message.LastIndexOf('!'));
                            Console.WriteLine(command);

                            string[] commandParts = command.Split(new char[] { ' ' });

                            if (commandParts.Length == 3)
                            {
                                string viewerName = commandParts[1].Substring(1).ToLower();
                                GetUserFromList(fullViewerList, viewerName).fayth += ConvertToNumber(commandParts[2]);
                            }
                        }
                    }

                    // !job - if the user doesn't have a job selected - display all the available jobs to pick from.
                    //      - I think it would be best to display the job name and description per line/whisper.
                    // !job <jobname> - Selects the job the user wants.


                }

            }
        }
        
    }
}
