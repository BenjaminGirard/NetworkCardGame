using NetworkCommsDotNet.Connections;
using server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sever
{
    class ClientInfo
    {
        private Connection instance;
        private string _id;
        private string _username;
        private List<Tuple<string, Object>> requests;
        public string Id { get => _id; set => _id = value; }
        public string Username { get => _username; set => _username = value; }
        public List<Tuple<string, object>> Requests { get => requests; set => requests = value; }
        public Connection Instance { get => instance; set => instance = value; }

        public ClientInfo(Connection instance)
        {
            Instance = instance;
            Id = instance.ToString().Split(' ').Last();
            Username = "";
            Requests = new List<Tuple<string, object>>();
        }

        public Tuple<string, Object> PollRequest()
        {
            Tuple<string, Object> request = Requests[0];
            Requests.RemoveAt(0);
            return request;
        }

        public Boolean IsAnyRequestLeft()
        {
            return Requests.Any();
        }

        public Boolean HandleDisconnection(NetworkManager Net, RoomsEntitiesManager Rooms)
        {
            foreach (Tuple<string, Object> req in Requests)
                if (req.Item1.Equals("Disconnect"))
                {
                    Net.Clients.Remove(this);
                    Rooms.TryRemoveClient(this);
                    return true ;
                }
            return false;
        }
    }
}
