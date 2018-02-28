using ProtoBuf;
using Protocol.ConnectionRequests.CommonRessources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.Notifications
{
    public enum ConnectionInfoType { CONNECT, DISCONNECT }

    [ProtoContract]
    public class ClientConnectionRoomEvent
    {
        [ProtoMember(1)]
        public ConnectionInfoType Type { get; set; }

        [ProtoMember(2)]
        public RoomInfo Room { get; set; }
    
        [ProtoMember(3)]
        public Player Player { get; set; }

        protected ClientConnectionRoomEvent() { }

        public ClientConnectionRoomEvent(ConnectionInfoType type, RoomInfo room, Player player)
        {
            Type = type;
            Room = room;
            Player = player;
        }
    }
}
