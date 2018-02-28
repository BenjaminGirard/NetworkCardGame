using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.CommonRessources
{
    [ProtoContract]
    public class RoomInfo
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public List<Player> Players { get; set; }

        protected RoomInfo() { }

        public RoomInfo(int id, List<Player> players)
        {
            Id = id;
            Players = players;
        }
    }
}
