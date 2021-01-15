using System;
using System.Threading;
using Sequential;

namespace Concurrent {

    public class ConcurrentClient : SimpleClient {

        public Thread workerThread;

        public ConcurrentClient(int id, Setting settings) : base(id, settings) {
            // todo [Assignment]: implement required code
        }

        public void run() {
            // todo [Assignment]: implement required code
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
                Console.Out.WriteLine("\n[ClientSimulator] Sequential simulator is going to start with {0}", settings.experimentNumberOfClients);
                
                for (int i = 0; i < settings.experimentNumberOfClients; i++) {
                    new Thread(() => {
                        clients[i].prepareClient();
                        clients[i].communicate();
                    }).Start();
                }

                Console.Out.WriteLine("\n[ClientSimulator] All clients finished with their communications ... ");
                Thread.Sleep(settings.delayForTermination);

                SimpleClient endClient = new SimpleClient(-1, settings);

                endClient.prepareClient();
                endClient.communicate();
            } catch (Exception e) {
                Console.Out.WriteLine("[Concurrent Simulator] {0}", e.Message);
            }
        }

    }

}
