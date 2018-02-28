using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.RoomConnection
{
    public enum MovingIntoRoomAnswerType { OK, ROOM_FULL, FAIL }

    [ProtoContract]
    public class MovingIntoRoomAnswer
    {
        [ProtoMember(1)]
        public MovingIntoRoomAnswerType Response { get; set; }

        protected MovingIntoRoomAnswer() { }

        public MovingIntoRoomAnswer(MovingIntoRoomAnswerType response)
        {
            Response = response;
        }
    }
}
