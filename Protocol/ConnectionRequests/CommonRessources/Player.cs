using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.CommonRessources
{
    [ProtoContract]
    public class Player
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public string Username { get; set; }

        protected Player() { }

        public Player(string id, string username) {
            Id = id;
            Username = username;
        }
    }
}
