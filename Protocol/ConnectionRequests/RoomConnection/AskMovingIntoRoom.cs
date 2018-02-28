using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.RoomConnection
{
    [ProtoContract]
    public class AskMovingIntoRoom
    {
        [ProtoMember(1)]
        public int RoomId { get; set; }

        protected AskMovingIntoRoom() { }

        public AskMovingIntoRoom(int id) {
            RoomId = id;
        }
    }
}
