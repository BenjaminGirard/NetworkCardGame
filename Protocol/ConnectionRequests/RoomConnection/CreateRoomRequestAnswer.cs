using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.RoomConnection
{
    [ProtoContract]
    public class CreateRoomRequestAnswer
    {
        [ProtoMember(1)]
        public Boolean IsRoomCreated { get; set; }

        protected CreateRoomRequestAnswer() { }

        public CreateRoomRequestAnswer(Boolean isRoomCreated)
        {
            IsRoomCreated = isRoomCreated;
        }
    }
}
