using NetworkCommsDotNet.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class ClientInfos
    {
        private Connection _instance;
        private int _id;
        private string _username;

        public ClientInfos(Connection instance)
        {
            _instance = instance;
        }
    }
}
