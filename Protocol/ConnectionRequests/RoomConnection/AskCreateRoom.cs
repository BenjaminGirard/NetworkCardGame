using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.ConnectionRequests.RoomConnection
{
    [ProtoContract]
    public class AskCreateRoom
    {
        public AskCreateRoom() { }
    }
}
