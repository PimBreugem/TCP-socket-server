using System;
using System.Threading;
using Sequential;

namespace Concurrent {

    public class ConcurrentClient : SimpleClient {

        public Thread workerThread;

        public ConcurrentClient(int id, Setting settings) : base(id, settings) {
            //Assign Thread to worker Thread
            workerThread = new Thread(() => {
                this.prepareClient();
                this.communicate();
            });
            
        }

        public void run() {
            //Run the Thread
            this.workerThread.Start();
        }

    }

    public class ConcurrentClientsSimulator : SequentialClientsSimulator {

        private ConcurrentClient[] clients;

        public ConcurrentClientsSimulator() : base() {
            Console.Out.WriteLine("\n[ClientSimulator] Concurrent simulator is going to start with {0}", settings.experimentNumberOfClients);
            clients = new ConcurrentClient[settings.experimentNumberOfClients];
        }

        public void ConcurrentSimulation() {
            try {
                for (int i = 0; i < settings.experimentNumberOfClients; i++)
                {
                    clients[i] = new ConcurrentClient(i + 1, settings);
                    clients[i].run();
                }
            } catch (Exception e) {
                Console.Out.WriteLine("[Concurrent Simulator] {0}", e.Message);
            }
        }

    }

}
