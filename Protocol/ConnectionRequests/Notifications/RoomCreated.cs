using ProtoBuf;
using Protocol.ConnectionRequests.CommonRessources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.Notifications
{
    [ProtoContract]
    public class RoomCreated
    {
        [ProtoMember(1)]
        public RoomInfo Room { get; set; }

        protected RoomCreated() { }

        public RoomCreated(RoomInfo room)
        {
            Room = room;
        }
    }
}
