using Sequential;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

/*
    Student information:

    Pim Breugem 0992420
    Tim van Dijkhuizen 0988700

    INF2F
*/

namespace Concurrent {

    public class ConcurrentServer : SequentialServer {

        private Semaphore clientLock = new Semaphore(1, 1);
        private Semaphore voteLock = new Semaphore(1, 1);
        private Dictionary<string, int> votingDict = new Dictionary<string, int>();

        public ConcurrentServer(Setting settings) : base(settings) {
            string[] commands = settings.votingList.Split(settings.commands_sep);

            foreach(string command in commands) {
                votingDict[command] = 0;
            }
        }

        public override void prepareServer() {
            Console.WriteLine("[Server] is ready to start ...");

            try {
                localEndPoint = new IPEndPoint(ipAddress, settings.serverPortNumber);
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(settings.serverListeningQueue);
                
                while (true) {
                    Console.WriteLine("Waiting for incoming connections ... ");
                    Socket connection = listener.Accept();

                    clientLock.WaitOne();
                    numOfClients++;
                    clientLock.Release();

                    // Handle connection on seperate thread
                    new Thread(() => {
                        try {
                            handleClient(connection);
                        } catch(Exception e) {
                            Console.Out.WriteLine("[Server] Client is not handled correct: {0}", e.Message);
                        }
                    }).Start();
                }
            } catch (Exception e) {
                Console.Out.WriteLine("[Server] Preparation: {0}", e.Message);
            }
        }

        public override string processMessage(string msg) {

            string replyMsg = Message.confirmed;
            Thread.Sleep(settings.serverProcessingTime);

            try {
                switch (msg) {
                    case Message.terminate:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("[Server] received from the client -> {0} ", msg);
                        Console.ResetColor();
                        Console.WriteLine("[Server] END : number of clients communicated -> {0} ", numOfClients);

                        voteLock.WaitOne();

                        //Print command winner
                        Console.WriteLine("[Server] Results:");
                        int maxValue = 0;
                        foreach (KeyValuePair<string, int> commandAndCount in votingDict)
                        {
                            Console.WriteLine("[Server] Key = {0}, Value = {1}", commandAndCount.Key, commandAndCount.Value.ToString());
                            if(maxValue < commandAndCount.Value) { maxValue = commandAndCount.Value; }
                        }
                        //Excecute command

                        foreach (KeyValuePair<string, int> commandAndCount in votingDict)
                        {
                            if(commandAndCount.Value == maxValue)
                            {
                                //Linux commands are not working on windows.
                                Console.WriteLine("[Server] excecuting: {0}", commandAndCount.Key);
                            }
                        }

                        votingDict.Clear();
                        voteLock.Release();

                        clientLock.WaitOne();
                        numOfClients = 0;
                        clientLock.Release();

                        break;
                    default:
                        replyMsg = Message.confirmed;

                        // Track clients and votes
                        voteLock.WaitOne();

                        string command = msg.Split(settings.command_msg_sep)[1];
                        votingDict.TryGetValue(command, out int votes);
                        votingDict[command] = ++votes;

                        voteLock.Release();

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("[Server] received from the client -> {0} ", msg);
                        Console.ResetColor();

                        break;
                }
            } catch (Exception e) {   
                Console.Out.WriteLine("[Server] Process Message {0}", e.Message);    
            }

            return replyMsg;
        }

    }

}