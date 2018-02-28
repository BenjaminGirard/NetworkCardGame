using ProtoBuf;
using Protocol.ConnectionRequests.CommonRessources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.ClientConnection
{
    [ProtoContract]
    public class DataFromEstablishedConnection
    {
        [ProtoMember(1)]
        public Player Player { get; set; }

        [ProtoMember(2)]
        public List<RoomInfo> Rooms { get; set; }
        
        protected DataFromEstablishedConnection() { }

        public DataFromEstablishedConnection(Player player, List<RoomInfo> rooms)
        {
            Player = player;
            Rooms = rooms;
        }
    }
}
