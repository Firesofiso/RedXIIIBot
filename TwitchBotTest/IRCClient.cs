using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using TwitchLib;
using TwitchLib.Models.API;

namespace TwitchBotTest
{
    class IRCClient
    {
        private string userName;
        private string channel;

        private string ip;
        private int port;

        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;


        public IRCClient(string ip, int port, string userName, string password)
        {
            this.userName = userName;
            this.ip = ip;
            this.port = port;

            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine("PASS " + password);
            outputStream.WriteLine("NICK " + userName);
            outputStream.WriteLine("USER " + userName + " 8 * :" + userName);

            outputStream.Flush();
        }

        public void reconnect()
        {
            if (!tcpClient.Connected)
            {
                tcpClient.Connect(ip, port);
            }
        }

        public void getViewerList()
        {
            sendIRCMessage(":" + userName + "!" + userName + "@" + userName + ".tmi.twitch.tv NAMES #" + channel);
        }

        public void joinRoom(string channel)
        {
            this.channel = channel;

            outputStream.WriteLine("JOIN #" + channel);
            outputStream.Flush();
        }

        public void partRoom(string channel)
        {
            outputStream.WriteLine("PART #" + channel);
            outputStream.Flush();
        }

        public void sendIRCMessage(string message)
        {
            Console.WriteLine(message);
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void sendChatMessage(string message)
        {
            sendIRCMessage(":" + userName + "!" + userName + "@" + userName + ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);
        }

        public string readMessage()
        {
            //string message = "";
            string message = inputStream.ReadLine();
            return message;
        }
    }
}
