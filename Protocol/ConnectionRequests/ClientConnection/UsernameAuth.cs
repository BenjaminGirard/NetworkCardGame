using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.ClientConnection
{
    [ProtoContract]
    public class UsernameAuth
    {
        [ProtoMember(1)]
        public string Username { get; private set; }
        protected UsernameAuth() { }

        public UsernameAuth(string _username)
        {
            Username = _username;
        }
    }
}
