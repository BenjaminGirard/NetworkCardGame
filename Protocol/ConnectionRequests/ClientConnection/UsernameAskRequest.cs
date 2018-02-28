using ProtoBuf;
using System;

namespace Protocol.ConnectionRequests.ClientConnection
{
    [ProtoContract]
    public class UsernameAskRequest
        {
            /// <summary>
            /// Parameterless constructor required for protobuf
            /// </summary>
            public UsernameAskRequest() { }

            // add parametered constructor if needed
        }
}
