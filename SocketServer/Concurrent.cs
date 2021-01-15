using Sequential;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Concurrent
{
    public class ConcurrentServer : SequentialServer
    {
        // todo [Assignment]: implement required attributes specific for concurrent server

        public ConcurrentServer(Setting settings) : base(settings)
        {
            // todo [Assignment]: implement required code
        }
        public override void prepareServer()
        {
            // todo [Assignment]: implement required code
        }
        public override string processMessage(String msg)
        {
            // todo [Assignment]: implement required code
            return "";
        }
    }
}