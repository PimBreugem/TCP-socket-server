﻿using Sequential;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Concurrent {

    public class ConcurrentServer : SequentialServer {

        private Semaphore threadLock;

        public ConcurrentServer(Setting settings) : base(settings) {
            // todo [Assignment]: implement required code
            threadLock = new Semaphore(0, settings.serverListeningQueue);
        }

        public override void prepareServer() {
            Console.WriteLine("[Server] is ready to start ...");

            try {
                localEndPoint = new IPEndPoint(ipAddress, settings.serverPortNumber);
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);
                listener.Listen(settings.serverListeningQueue);
                
                while (true) {
                    Console.WriteLine("Waiting for incoming connections ...");
                    
                    // Wait for connection
                    Socket connection = listener.Accept();

                    threadLock.WaitOne();
                    numOfClients++;

                    // Handle connection on seperate thread
                    new Thread(() => {
                        try {
                            handleClient(connection);
                        } catch(Exception e) {
                            Console.Out.WriteLine("[Server] Client is not handled correct: {0}", e.Message);
                        }
                    }).Start();

                    threadLock.Release();
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

                        break;
                    default:
                        replyMsg = Message.confirmed;

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