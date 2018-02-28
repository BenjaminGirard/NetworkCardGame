using server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sever
{
    class Core
    {
        NetworkManager Net;
        RoomsEntitiesManager Rooms;
        RequestHandler RequestHandler;
        public Core(int port)
        {
            Net = new NetworkManager(port);
            Rooms = new RoomsEntitiesManager(Net);
            RequestHandler = new RequestHandler(Net, Rooms);
        }

        public void Run()
        {
            Net.StartListening();

            while (true)
            {
                lock (Net.Clients)
                {
                    Monitor.Wait(Net.Clients);
                    for (int i = Net.Clients.Count - 1; i >= 0; i--)
                    {
                        if (Net.Clients[i].HandleDisconnection(Net, Rooms) == true)
                            continue;
                        while (Net.Clients.Count > i && Net.Clients[i].IsAnyRequestLeft())
                            RequestHandler.HandleClientRequest(Net.Clients[i], Net.Clients[i].PollRequest());
                    }
                }
            }
        }
    }
}
